// 1. Pure validatiefuncties (elk heeft één taak: controleren en true/false teruggeven)
function isValidName(name) {
  return name.trim().length >= 2;
}

function isValidEmail(email) {
  // Regex: controleert op formaat tekst@tekst.domein
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email.trim());
}

function isValidMessage(message) {
  return message.trim().length >= 15;
}

// 2. DOM-hulpfunctie: zet de visuele en toegankelijke foutstatus per veld
function setFieldError(inputElement, errorElement, errorMessage) {
  if (errorMessage) {
    // Veld is ongeldig
    inputElement.setAttribute("aria-invalid", "true");
    errorElement.textContent = errorMessage;
  } else {
    // Veld is geldig: herstel de status
    inputElement.setAttribute("aria-invalid", "false");
    errorElement.textContent = "";
  }
}

// 3. Hoofdfunctie die alle velden valideert
function validateForm(form) {
  let isFormValid = true;

  // Veldelementen ophalen
  const nameInput = form.querySelector("#name");
  const emailInput = form.querySelector("#email");
  const messageInput = form.querySelector("#message");

  // Foutmelding span-elementen ophalen
  const nameError = form.querySelector("#name-error");
  const emailError = form.querySelector("#email-error");
  const messageError = form.querySelector("#message-error");

  // Validatie Veld 1: Naam
  if (!isValidName(nameInput.value)) {
    setFieldError(nameInput, nameError, "Vul een geldige naam in van minimaal 2 tekens.");
    isFormValid = false;
  } else {
    setFieldError(nameInput, nameError, "");
  }

  // Validatie Veld 2: E-mail
  if (!isValidEmail(emailInput.value)) {
    setFieldError(emailInput, emailError, "Vul een geldig e-mailadres in (bijv. naam@domein.nl).");
    isFormValid = false;
  } else {
    setFieldError(emailInput, emailError, "");
  }

  // Validatie Veld 3: Bericht
  if (!isValidMessage(messageInput.value)) {
    setFieldError(messageInput, messageError, "Het bericht moet minimaal 15 tekens lang zijn.");
    isFormValid = false;
  } else {
    setFieldError(messageInput, messageError, "");
  }

  return isFormValid;
}

// 4. Submit event listener koppelen
function setupFormValidation() {
  const form = document.getElementById("contact-form");
  const successBox = document.getElementById("form-success");

  if (!form || !successBox) return;

  form.addEventListener("submit", (event) => {
    // Voorkom standaard pagina-refresh / HTTP POST request
    event.preventDefault();

    // Verberg eerdere succesmelding
    successBox.hidden = true;
    successBox.textContent = "";

    const isValid = validateForm(form);

    if (isValid) {
      // Alles klopt: toon toegankelijke bevestiging
      successBox.textContent = "Bedankt! Je bericht is succesvol verzonden.";
      successBox.hidden = false;

      // Maak formuliervelden leeg
      form.reset();

      // Reset ARIA-invalid attributen
      form.querySelectorAll("input, textarea").forEach((field) => {
        field.removeAttribute("aria-invalid");
      });
    }
  });
}

// Start validatieluisteraar
setupFormValidation();