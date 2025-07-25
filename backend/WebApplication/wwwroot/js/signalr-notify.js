const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

// Shared function to show a toast
function showToast(message, type = "info") {
    const toastId = `toast-${Date.now()}`;
    const iconMap = {
        info: "bi-info-circle",
        success: "bi-check-circle",
        warning: "bi-exclamation-triangle",
        danger: "bi-x-circle"
    };

    const icon = iconMap[type] || "bi-bell";

    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center text-bg-${type} border-0 mb-2 shadow" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="bi ${icon} me-2"></i>${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>`;

    const container = document.querySelector('.toast-container');
    container.insertAdjacentHTML('beforeend', toastHtml);

    const toastEl = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastEl, { delay: 5000 });
    toast.show();

    toastEl.addEventListener('hidden.bs.toast', () => toastEl.remove());
}

// Handle different SignalR messages
connection.on("ProductUpdated", function (product) {
    showToast(`Product "${product.productName}" updated.`, "success");
});

connection.on("ProductCreated", function (product) {
    showToast(`New product "${product.productName}" created.`, "info");
});

connection.on("OrderPlaced", function (order) {
    showToast(`New order #${order.id} placed.`, "primary");
});

connection.on("LowStockAlert", function (product) {
    showToast(`Low stock alert: "${product.productName}"`, "danger");
});

connection.start().catch(err => console.error(err));
