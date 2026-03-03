(function () {
  function applyMaskingRules(root) {
    const scope = root || document;
    const phoneInputs = scope.querySelectorAll(
      'input[type="tel"], input[data-mask="phone"], input[id*="telefono" i]',
    );
    phoneInputs.forEach((input) => {
      input.addEventListener("input", () => {
        if (window.formatPhoneInput) {
          window.formatPhoneInput(input);
        }
      });
    });

    const cedulaInputs = scope.querySelectorAll(
      'input[data-mask="cedula"], input[id*="cedula" i], input[id*="identificacion" i]',
    );
    cedulaInputs.forEach((input) => {
      input.addEventListener("input", () => {
        if (window.formatCedulaInput) {
          window.formatCedulaInput(input);
        }
      });
    });
  }

  function focusFirstInvalid(form) {
    const invalid = form.querySelector(".is-invalid, :invalid");
    if (invalid && typeof invalid.focus === "function") {
      invalid.focus();
    }
  }

  function wireFormValidation() {
    document.querySelectorAll("form").forEach((form) => {
      if (form.dataset.mgeWired === "1") return;
      form.dataset.mgeWired = "1";

      form.addEventListener("submit", (event) => {
        if (!form.checkValidity()) {
          event.preventDefault();
          event.stopPropagation();
          form.classList.add("was-validated");
          focusFirstInvalid(form);
        }
      });
    });
  }

  document.addEventListener("DOMContentLoaded", () => {
    applyMaskingRules(document);
    wireFormValidation();
  });

  window.applyFormSystemRules = applyMaskingRules;
})();
