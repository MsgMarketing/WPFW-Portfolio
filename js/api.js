const WEATHER_API_URL = "https://api.open-meteo.com/v1/forecast?latitude=52.0705&longitude=4.3007&current=temperature_2m,weather_code&timezone=Europe%2FAmsterdam";

// 1. DOM-render: Toont de laadstatus
function renderLoadingState(container) {
  container.replaceChildren();
  const loadingP = document.createElement("p");
  loadingP.textContent = "Actuele gegevens ophalen...";
  loadingP.classList.add("status-loading");
  container.append(loadingP);
}

// 2. DOM-render: Toont de foutstatus (bijv. geen internet of 404/500)
function renderErrorState(container, message) {
  container.replaceChildren();
  const errorP = document.createElement("p");
  errorP.textContent = `Fout bij laden: ${message}`;
  errorP.classList.add("error-msg");
  container.append(errorP);
}

// 3. DOM-render: Toont de succesvolle data
function renderWeatherSuccess(container, data) {
  container.replaceChildren();

  const temp = data.current.temperature_2m;
  const unit = data.current_units.temperature_2m;

  const card = document.createElement("div");
  card.classList.add("card");

  const title = document.createElement("h3");
  title.textContent = "Actuele temperatuur De Haagse Hogeschool";

  const tempDisplay = document.createElement("p");
  tempDisplay.textContent = `${temp} ${unit}`;
  tempDisplay.style.fontWeight = "bold";

  card.append(title, tempDisplay);
  container.append(card);
}

// 4. Data-ophalen via async/await met strikte foutafhandeling
async function loadWeatherData() {
  const container = document.getElementById("weather-container");
  if (!container) return;

  // Status 1: Laden tonen
  renderLoadingState(container);

  try {
    const response = await fetch(WEATHER_API_URL);

    // Controleer HTTP-foutcodes (404, 500, etc.)
    if (!response.ok) {
      throw new Error(`Serverstatus ${response.status}`);
    }

    const data = await response.json();

    // Status 2: Succes tonen
    renderWeatherSuccess(container, data);
  } catch (error) {
    // Status 3: Fout tonen
    renderErrorState(container, "Kan actuele weergegevens niet ophalen.");
  }
}

// Start direct bij inladen van de pagina
loadWeatherData();