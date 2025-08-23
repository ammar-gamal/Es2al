const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notification")
    .withAutomaticReconnect()
    .build();
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected")
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
}
connection.onclose(async () => {

    await start();
});

start();
connection.on("setNotificationCount", (data) => {
    updateNotificationBadge(data.count);
});
connection.on("notifyUser", (data) => {
    updateNotificationBadge(data.count);
    const isOnNotificationsPage = window.location.pathname.includes('/notifications');
    if (data.notification && isOnNotificationsPage) {
        addNewNotification(data.notification);
    }
});
function updateNotificationBadge(count) {
    const notificationBadge = document.getElementById('notifications-count');

    if (count > 0) {
        if (!notificationBadge) {
            const notificationLink = document.getElementById('notification-link');
            if (notificationLink) {
                const badge = document.createElement('span');
                badge.id = 'notifications-count';
                badge.className = 'badge rounded-pill bg-danger position-absolute top-0 start-0 translate-middle';
                badge.setAttribute('data-count', count);
                badge.textContent = count;
                notificationLink.appendChild(badge);
            }
        } else {
            notificationBadge.setAttribute('data-count', count);
            notificationBadge.textContent = count;
            notificationBadge.style.display = 'inline-block';
            notificationBadge.classList.remove('d-none');
        }
    } else {
        if (notificationBadge) {
            notificationBadge.style.display = 'none';
            notificationBadge.classList.add('d-none');
            notificationBadge.setAttribute('data-count', '0');
        }
    }
}