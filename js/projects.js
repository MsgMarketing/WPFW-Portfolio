// 1. Data-bron: gestructureerde array van projecten
const projects = [
  {
    id: 1,
    title: "Multilayer Webapplicatie",
    description: "Een complete drielagen-architectuur met frontend, backend API en database-integratie.",
    category: "Fullstack"
  },
  {
    id: 2,
    title: "Database Management Systeem",
    description: "Relationeel datamodel ontworpen en gerealiseerd met focus op data-integriteit en performance.",
    category: "Database"
  },
  {
    id: 3,
    title: "RESTful Web API in ASP.NET Core",
    description: "Ontwerp en implementatie van modulaire endpoints met gestandaardiseerde JSON-responses.",
    category: "Backend"
  },
  {
    id: 4,
    title: "Interactief Portfolio met DOM-manipulatie",
    description: "Client-side applicatie waarbij inhoud dynamisch via JavaScript wordt gerenderd en gefilterd.",
    category: "Fullstack"
  }
];

// 2. Bouwt één projectkaartje (<article class="card">)
function createProjectCard(project) {
  const article = document.createElement("article");
  article.classList.add("card");

  const title = document.createElement("h3");
  title.textContent = project.title;

  const description = document.createElement("p");
  description.textContent = project.description;

  article.append(title, description);
  return article;
}

// 3. Tekent de meegegeven lijst in het HTML-bakje
function renderProjects(projectList) {
  const container = document.getElementById("projects-container");
  if (!container) return;

  container.replaceChildren();

  projectList.forEach((project) => {
    const card = createProjectCard(project);
    container.append(card);
  });
}

// 4. Filtert de data op basis van de gekozen categorie
function filterProjects(category) {
  if (category === "all") {
    return projects;
  }
  return projects.filter((project) => project.category === category);
}

// 5. Werkt de visuele (.active) en toegankelijke (aria-pressed) status van knoppen bij
function updateActiveButton(activeBtn) {
  const buttons = document.querySelectorAll(".filter-btn");
  buttons.forEach((btn) => {
    btn.classList.remove("active");
    btn.setAttribute("aria-pressed", "false");
  });

  activeBtn.classList.add("active");
  activeBtn.setAttribute("aria-pressed", "true");
}

// 6. Handelt het klikken op de knoppen af
function setupFilterEvents() {
  const filterToolbar = document.querySelector(".filter-container");
  if (!filterToolbar) return;

  filterToolbar.addEventListener("click", (event) => {
    const clickedBtn = event.target;
    if (!clickedBtn.classList.contains("filter-btn")) return;

    // Wissel de actieve styling en ARIA-status om
    updateActiveButton(clickedBtn);

    // Filter de data en render opnieuw
    const selectedCategory = clickedBtn.dataset.category;
    const filteredProjects = filterProjects(selectedCategory);
    renderProjects(filteredProjects);
  });
}

// 7. Start alles op
renderProjects(projects);
setupFilterEvents();