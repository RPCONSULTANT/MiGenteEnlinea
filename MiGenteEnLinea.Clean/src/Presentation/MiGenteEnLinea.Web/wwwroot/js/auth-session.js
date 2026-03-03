(function () {
  function logDebug(event, payload) {
    const debugEnabled = localStorage.getItem("mge:debug") === "1";
    if (!debugEnabled) return;
    console.log(`[SESSION][${event}]`, payload || {});
  }

  function parseJwtClaims(token) {
    if (!token || typeof token !== "string" || token.split(".").length < 2) return null;
    try {
      const payload = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
      const decoded = atob(payload);
      return JSON.parse(decoded);
    } catch {
      return null;
    }
  }

  function getCurrentUserId() {
    const storedUserId = localStorage.getItem("userId") || sessionStorage.getItem("userId");
    if (storedUserId) return storedUserId;

    const token = localStorage.getItem("accessToken") || sessionStorage.getItem("accessToken");
    const claims = parseJwtClaims(token);
    const claimUserId = claims?.nameid || claims?.sub || claims?.userId || claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
    if (claimUserId) {
      localStorage.setItem("userId", claimUserId);
      return claimUserId;
    }

    return null;
  }

  function clearRedirectFlags() {
    try {
      const keys = [];
      for (let i = 0; i < sessionStorage.length; i++) {
        const key = sessionStorage.key(i);
        if (key && key.startsWith("mge:redirecting:")) {
          keys.push(key);
        }
      }
      keys.forEach((key) => sessionStorage.removeItem(key));
      logDebug("REDIRECT_FLAGS_CLEARED", { count: keys.length });
    } catch (error) {
      console.warn("clearRedirectFlags warning:", error);
    }
  }

  function persistAuthSession(authResponse) {
    const user = authResponse?.user || {};
    const accessToken = authResponse?.accessToken || "";
    const refreshToken = authResponse?.refreshToken || "";
    const roles = Array.isArray(user.roles) ? user.roles : [];
    const tipo = user.tipo != null ? String(user.tipo).toLowerCase() : "";
    const planId = user.planId != null ? String(user.planId) : "0";
    const vencimientoPlan = user.vencimientoPlan || "";

    localStorage.setItem("accessToken", accessToken);
    localStorage.setItem("refreshToken", refreshToken);
    localStorage.setItem("token", accessToken);
    localStorage.setItem("userId", String(user.userId || ""));
    localStorage.setItem("email", user.email || "");
    localStorage.setItem("nombreCompleto", user.nombreCompleto || "");
    localStorage.setItem("tipo", tipo);
    localStorage.setItem("planId", planId);
    localStorage.setItem("vencimientoPlan", vencimientoPlan);
    localStorage.setItem("roles", JSON.stringify(roles));
    localStorage.setItem("userInfo", JSON.stringify(user));

    if (authResponse?.accessTokenExpires) {
      localStorage.setItem("accessTokenExpires", String(authResponse.accessTokenExpires));
    }
    if (authResponse?.refreshTokenExpires) {
      localStorage.setItem("refreshTokenExpires", String(authResponse.refreshTokenExpires));
    }

    clearRedirectFlags();
    logDebug("SESSION_PERSISTED", {
      userId: user.userId,
      tipo,
      planId,
      hasVencimiento: Boolean(vencimientoPlan)
    });
  }

  function hydrateSessionFromUserInfoIfNeeded() {
    const hasCanonical = Boolean(localStorage.getItem("userId")) && Boolean(localStorage.getItem("accessToken"));
    if (hasCanonical) return;

    const userInfoRaw = localStorage.getItem("userInfo");
    if (!userInfoRaw) return;

    try {
      const user = JSON.parse(userInfoRaw);
      if (!user || typeof user !== "object") return;

      if (!localStorage.getItem("userId") && user.userId) {
        localStorage.setItem("userId", String(user.userId));
      }
      if (!localStorage.getItem("email") && user.email) {
        localStorage.setItem("email", String(user.email));
      }
      if (!localStorage.getItem("nombreCompleto") && user.nombreCompleto) {
        localStorage.setItem("nombreCompleto", String(user.nombreCompleto));
      }
      if (!localStorage.getItem("tipo") && user.tipo != null) {
        localStorage.setItem("tipo", String(user.tipo).toLowerCase());
      }
      if (!localStorage.getItem("planId")) {
        localStorage.setItem("planId", String(user.planId ?? "0"));
      }
      if (!localStorage.getItem("vencimientoPlan") && user.vencimientoPlan) {
        localStorage.setItem("vencimientoPlan", String(user.vencimientoPlan));
      }
      if (!localStorage.getItem("roles") && Array.isArray(user.roles)) {
        localStorage.setItem("roles", JSON.stringify(user.roles));
      }

      const tokenUserId = getCurrentUserId();
      if (tokenUserId && !localStorage.getItem("userId")) {
        localStorage.setItem("userId", tokenUserId);
      }

      logDebug("SESSION_HYDRATED", {
        userId: localStorage.getItem("userId"),
        tipo: localStorage.getItem("tipo"),
        planId: localStorage.getItem("planId")
      });
    } catch (error) {
      console.warn("hydrateSessionFromUserInfoIfNeeded warning:", error);
    }
  }

  async function clearClientSession() {
    try {
      localStorage.clear();
    } catch (error) {
      console.warn("clearClientSession localStorage error:", error);
    }

    try {
      sessionStorage.clear();
    } catch (error) {
      console.warn("clearClientSession sessionStorage error:", error);
    }

    try {
      const cookies = document.cookie ? document.cookie.split(";") : [];
      for (const cookie of cookies) {
        const eqPos = cookie.indexOf("=");
        const cookieName = eqPos > -1 ? cookie.substring(0, eqPos).trim() : cookie.trim();
        if (!cookieName) continue;
        document.cookie = `${cookieName}=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/`;
      }
    } catch (error) {
      console.warn("clearClientSession cookie cleanup error:", error);
    }

    if (typeof caches !== "undefined" && caches.keys) {
      try {
        const cacheKeys = await caches.keys();
        await Promise.all(cacheKeys.map((key) => caches.delete(key)));
      } catch (error) {
        console.warn("clearClientSession cache cleanup error:", error);
      }
    }

    if (navigator.serviceWorker?.getRegistrations) {
      try {
        const registrations = await navigator.serviceWorker.getRegistrations();
        await Promise.all(registrations.map((registration) => registration.unregister()));
      } catch (error) {
        console.warn("clearClientSession service worker cleanup error:", error);
      }
    }
  }

  async function revokeRefreshToken() {
    const refreshToken = localStorage.getItem("refreshToken") || sessionStorage.getItem("refreshToken");
    if (!refreshToken) {
      return;
    }

    const revokePath = window.API_ENDPOINTS?.AUTH?.REVOKE;
    if (!revokePath || typeof window.buildApiUrl !== "function") {
      return;
    }

    try {
      await fetch(window.buildApiUrl(revokePath), {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          refreshToken,
          reason: "User logout"
        })
      });
    } catch (error) {
      console.warn("performLogout revoke token warning:", error);
    }
  }

  async function performLogout(options) {
    const resolved = {
      redirectTo: "/Auth/Login?logout=true",
      ...options
    };

    try {
      await revokeRefreshToken();
    } finally {
      await clearClientSession();
      if (resolved.redirectTo) {
        window.location.replace(resolved.redirectTo);
      }
    }
  }

  window.clearClientSession = clearClientSession;
  window.performLogout = performLogout;
  window.persistAuthSession = persistAuthSession;
  window.hydrateSessionFromUserInfoIfNeeded = hydrateSessionFromUserInfoIfNeeded;
  window.clearRedirectFlags = clearRedirectFlags;
  window.getCurrentUserId = getCurrentUserId;
})();
