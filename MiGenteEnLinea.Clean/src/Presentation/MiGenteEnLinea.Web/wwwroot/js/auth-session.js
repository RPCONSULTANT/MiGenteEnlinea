(function () {
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
})();
