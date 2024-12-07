function markAsRead(notificationId) {
    $.ajax({
        url: 'notifications/mark-read/' + notificationId,
        method: 'POST',
        success: function () {
            const notificationsCountElement = $('#notifications-count');
            let notificationCount = (notificationsCountElement.data('count')) - 1;
            notificationsCountElement.data('count', notificationCount);
            if (notificationCount > 0) {
                notificationsCountElement.text(notificationCount);
            } else {
                notificationsCountElement.hide();
            }

            const notificationItem = $('#' + notificationId);
            if (notificationItem.length) {

                const unreadedNotificationsList = $('#unreaded-notifications');
                let readedNotificationsList = $('#readed-notifications');

                if (!readedNotificationsList.length) {
                    const readNotificationsContainer = $('.notification-wrapper');
                    const readSection = $(`
                            <div class="mb-5">
                                <div class="d-flex justify-content-between align-items-center mb-3">
                                    <h5 class="text-success">Readed Notifications</h5>
                                    <a href="notifications/delete-all-readed"  class="btn btn-sm btn-danger" onclick="return confirm('Are you sure you want to delete all read notifications?');">
                                        Delete All
                                    </a>
                                </div>
                                <div id="notifications-container">
                                    <ul id="readed-notifications" class="list-group shadow-sm"></ul>
                                </div>
                            </div>`
                    );
                    readNotificationsContainer.append(readSection);
                    readedNotificationsList = $('#readed-notifications');
                }
                notificationItem.find('.btn-outline-primary').remove();

                readedNotificationsList.append(notificationItem);

                if (unreadedNotificationsList.children().length === 0) {
                    unreadedNotificationsList.closest('.mb-5').remove();
                }
            }
        },
        error: function () {
            alert('Failed to mark the notification as read.');
        }
    });
}