const stockTableBody = document.querySelector("#stock-table tbody");
const notificationsList = document.getElementById("notifications");

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleString('en-GB'); // dd/MM/yyyy, HH:mm:ss
}

async function loadStockPrices() {
    try {
        const response = await fetch('http://localhost:5230/api/stocks/latest');
        const data = await response.json();

        const tableBody = document.querySelector('#stockTableBody');
        tableBody.innerHTML = '';

        Object.entries(data).forEach(([symbol, stock]) => {
            const row = document.createElement('tr');

            const symbolCell = document.createElement('td');
            symbolCell.textContent = symbol;

            const priceCell = document.createElement('td');
            priceCell.textContent = stock.c.toFixed(2);

            const timestampCell = document.createElement('td');
            const date = formatDate(stock.t * 1000);
            timestampCell.textContent = date.toLocaleString();

            row.appendChild(symbolCell);
            row.appendChild(priceCell);
            row.appendChild(timestampCell);

            tableBody.appendChild(row);
        });
    } catch (error) {
        console.error('Failed to load stock prices:', error);
    }
}

document.getElementById("alert-form").addEventListener("submit", async (e) => {
    e.preventDefault();

    const symbol = document.getElementById("symbol").value;
    const thresholdInput = document.getElementById("threshold").value.trim().replace(",", ".");
    const threshold = parseFloat(thresholdInput)
    const direction = document.getElementById("direction").value;

    if (isNaN(threshold)) {
        alert("Please enter a valid price threshold.");
        return;
    }

    const response = await fetch("http://localhost:5230/api/AlertRules", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ stockSymbol: symbol, priceThreshold: threshold, direction })
    });

    if (response.ok) {
        alert("Alert created successfully.");
    } else {
        const error = await response.json();
        alert("Error creating alert: " + error.message);
        console.error(error);
    }
});

//Connect to SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5230/notificationHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveNotification", (message) => {
    alert(`📢 Alert: ${message}`);
    const li = document.createElement("li");
    li.textContent = message;
    notificationsList.prepend(li);
});

connection.start()
    .then(() => console.log("SignalR connected."))
    .catch(err => console.error("SignalR error:", err));

// Poll every 25 seconds
loadStockPrices();
setInterval(loadStockPrices, 25000);