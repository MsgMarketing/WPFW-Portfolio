// 1. Schakelt de status van een enkel detail-paneel om
function toggleBlogDetails(button, detailsElement) {
  const isExpanded = button.getAttribute("aria-expanded") === "true";

  if (isExpanded) {
    // Klap in
    detailsElement.hidden = true;
    button.setAttribute("aria-expanded", "false");
    button.textContent = "Lees meer";
  } else {
    // Klap uit
    detailsElement.hidden = false;
    button.setAttribute("aria-expanded", "true");
    button.textContent = "Lees minder";
  }
}

// 2. Initialiseert event listeners voor alle uitklapknoppen
function initBlogAccordions() {
  const toggleButtons = document.querySelectorAll(".toggle-btn");

  toggleButtons.forEach((button) => {
    button.addEventListener("click", () => {
      // Zoek het gekoppelde paneel via het aria-controls attribuut
      const targetId = button.getAttribute("aria-controls");
      const detailsElement = document.getElementById(targetId);

      if (detailsElement) {
        toggleBlogDetails(button, detailsElement);
      }
    });
  });
}

// Start initialisatie
initBlogAccordions();