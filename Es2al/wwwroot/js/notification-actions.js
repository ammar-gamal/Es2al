function markAsRead(notificationId) {
    connection.invoke("MarkNotificationAsRead", notificationId);
}
connection.on("changeNotificationPlace", (data) => {
    const notificationElement = document.getElementById(`notification-${data.notificationId}`);
    if (notificationElement) {
        notificationElement.style.transition = 'all 0.5s ease';
        notificationElement.style.opacity = '0.5';

        setTimeout(() => {
            moveNotificationToRead(notificationElement);
        }, 250);
    }
});


function moveNotificationToRead(notificationElement) {
    const unreadContainer = document.getElementById('unread-notifications');
    let readContainer = document.getElementById('read-notifications');

    if (!readContainer) {
        createReadNotificationsSection();
        readContainer = document.getElementById('read-notifications');
    }

    const markReadBtn = notificationElement.querySelector('.mark-read-btn');
    if (markReadBtn) {
        markReadBtn.remove();
    }

    notificationElement.setAttribute('data-section', 'read');
    notificationElement.style.borderLeftColor = '#198754';
    notificationElement.style.background = '#ffffff';

    const notificationDate = getNotificationDate(notificationElement);

    insertNotificationWithBinarySearch(readContainer, notificationElement, notificationDate);

    notificationElement.style.opacity = '1';

    if (unreadContainer && unreadContainer.children.length === 0) {
        const unreadSection = unreadContainer.closest('.notification-section');
        if (unreadSection) {
            unreadSection.remove();
        }
    }

    updateBadgeCounts();
}
function createReadNotificationsSection() {
    const container = document.getElementById('notifications-container');
    const readSection = document.createElement('div');
    readSection.className = 'notification-section mb-4';
    readSection.setAttribute('data-section', 'read');

    readSection.innerHTML = `
                <div class="section-header d-flex justify-content-between align-items-center mb-3 p-3 rounded-top bg-light border">
                    <div class="section-title">
                        <h5 class="mb-0 text-success">
                            <i class="fas fa-check-circle me-2"></i>
                            Read Notifications
                            <span class="badge bg-success ms-2">0</span>
                        </h5>
                    </div>
                    <button type="button" class="btn btn-outline-danger btn-sm delete-all-btn" data-bs-toggle="modal" data-bs-target="#deleteAllModal">
                        <i class="fas fa-trash me-1"></i>
                        Clear All
                    </button>
                </div>
                <div class="notifications-list" id="read-notifications"></div>
            `;

    container.appendChild(readSection);
}

function updateBadgeCounts() {
    const unreadContainer = document.getElementById('unread-notifications');
    const unreadBadge = document.querySelector('[data-section="unread"] .badge');
    if (unreadContainer && unreadBadge) {
        unreadBadge.textContent = unreadContainer.children.length;
    }

    const readContainer = document.getElementById('read-notifications');
    const readBadge = document.querySelector('[data-section="read"] .badge');
    if (readContainer && readBadge) {
        readBadge.textContent = readContainer.children.length;
    }
}

function addNewNotification(notification) {
    try {
        const isOnNotificationsPage = window.location.pathname.includes('/notifications');
        if (!isOnNotificationsPage) {
            return;
        }

        const emptyState = document.querySelector('.empty-state');

        if (emptyState) {
            emptyState.remove();
            createNotificationsContainer();
        }
        let unreadSection = document.querySelector('[data-section="unread"]');
        let unreadContainer = document.getElementById('unread-notifications');

        if (!unreadSection || !unreadContainer) {
            createUnreadNotificationsSection();
            unreadSection = document.querySelector('[data-section="unread"]');
            unreadContainer = document.getElementById('unread-notifications');
        }

        const notificationElement = createNotificationElement(notification);

        unreadContainer.insertBefore(notificationElement, unreadContainer.firstChild);

        animateNewNotification(notificationElement);

        updateUnreadBadgeCount();


    } catch (error) {
        console.error("Error adding new notification:", error);
    }
}


function createNotificationsContainer() {
    const notificationsPage = document.querySelector('.notifications-page');
    const container = notificationsPage.querySelector('.container');

    const notificationsContainer = document.createElement('div');
    notificationsContainer.className = 'notifications-container';
    notificationsContainer.id = 'notifications-container';

    container.appendChild(notificationsContainer);
}
function createUnreadNotificationsSection() {
    const container = document.getElementById('notifications-container');

    const unreadSection = document.createElement('div');
    unreadSection.className = 'notification-section mb-4';
    unreadSection.setAttribute('data-section', 'unread');

    unreadSection.innerHTML = `
                <div class="section-header d-flex justify-content-between align-items-center mb-3 p-3 rounded-top bg-light border">
                    <div class="section-title">
                        <h5 class="mb-0 text-primary">
                            <i class="fas fa-exclamation-circle me-2"></i>
                            Unread Notifications
                            <span class="badge bg-primary ms-2">0</span>
                        </h5>
                    </div>
                </div>
                <div class="notifications-list" id="unread-notifications"></div>
            `;

    container.insertBefore(unreadSection, container.firstChild);
}
function formatNotificationDate(dateString) {
    try {
        const date = new Date(dateString);

        if (isNaN(date.getTime())) {
            return 'Invalid date';
        }

        const options = {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: 'numeric',
            minute: '2-digit',
            hour12: true
        };

        return date.toLocaleDateString('en-US', options);
    } catch (error) {
        console.error('Error formatting date:', error);
        return 'Invalid date';
    }
}
function createNotificationElement(notification) {
    const notificationDiv = document.createElement('div');
    notificationDiv.className = 'notification-item border rounded mb-2 p-3 bg-white shadow-sm';
    notificationDiv.id = `notification-${notification.id}`;
    notificationDiv.setAttribute('data-notification-id', notification.id);
    notificationDiv.setAttribute('data-section', 'unread');
    if (notification.date) {
        const dateObj = new Date(notification.date);
        notificationDiv.setAttribute('data-date', dateObj.toISOString());
    }
    const notificationDate = notification.date ? formatNotificationDate(notification.date) : 'Just now';

    notificationDiv.innerHTML = `
                <div class="notification-content">
                    <div class="notification-meta mb-2">
                        <small class="text-muted">
                            <i class="fas fa-clock me-1"></i>
                            ${notificationDate}
                        </small>
                    </div>
                    <div class="notification-text mb-2">
                        <p class="mb-0">${escapeHtml(notification.text)}</p>
                    </div>
                    ${notification.relatedUrl ? `
                        <div class="notification-link">
                            <a href="${escapeHtml(notification.relatedUrl)}"
                               class="btn btn-link btn-sm p-0 text-decoration-none">
                                <i class="fas fa-external-link-alt me-1"></i>
                                ${escapeHtml(notification.anchorText || 'View Details')}
                            </a>
                        </div>
                    ` : ''}
                </div>
                <div class="notification-actions mt-2 d-flex justify-content-end">
                    <button type="button"
                            class="btn btn-outline-primary btn-sm mark-read-btn"
                            onclick="markAsRead(${notification.id})">
                        <i class="fas fa-check me-1"></i>
                        Mark as Read
                    </button>
                </div>
            `;

    return notificationDiv;
}
function animateNewNotification(element) {
    element.style.opacity = '0';
    element.style.transform = 'translateY(-20px)';
    element.style.transition = 'all 0.5s ease';

    setTimeout(() => {
        element.style.opacity = '1';
        element.style.transform = 'translateY(0)';
    }, 50);
}

function updateUnreadBadgeCount() {
    const unreadContainer = document.getElementById('unread-notifications');
    const unreadBadge = document.querySelector('[data-section="unread"] .badge');

    if (unreadContainer && unreadBadge) {
        const count = unreadContainer.children.length;
        unreadBadge.textContent = count;
    }
}

function escapeHtml(text) {
    if (!text) return '';
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, function (m) { return map[m]; });
}

function insertNotificationWithBinarySearch(container, newElement, newDate) {
    const existingNotifications = container.children;
    const length = existingNotifications.length;

    if (length === 0) {
        container.appendChild(newElement);
        return;
    }
    let left = 0;
    let right = length - 1;
    let mid = 0;
    while (left <= right) {  
        mid = Math.floor((left + right) / 2);
        const midDate = getNotificationDate(existingNotifications[mid]);
        if (newDate > midDate) {
            right = mid - 1;
        } else {
            left = mid + 1;
        }
    }

    if (getNotificationDate(existingNotifications[mid]) < newDate) 
        container.insertBefore(newElement, existingNotifications[mid]);
    else
        existingNotifications[mid].parentNode.insertBefore(newElement, existingNotifications[mid].nextSibling);

}
function getNotificationDate(notificationElement) {
    const dateAttr = notificationElement.getAttribute('data-date');
    if (dateAttr) {
        return new Date(dateAttr);
    }

    const dateElement = notificationElement.querySelector('.text-muted');
    if (!dateElement) {
        console.warn('No date element found, using current date');
        return new Date();
    }

    const dateText = dateElement.textContent;
    try {
        const cleanDateText = dateText.replace(/^\s*.*?\s+/, '').trim();
        const parsedDate = new Date(cleanDateText);

        if (isNaN(parsedDate.getTime())) {
            console.warn('Invalid date parsed:', cleanDateText);
            return new Date();
        }

        return parsedDate;
    } catch (error) {
        console.error('Error parsing date:', error);
        return new Date();
    }
}
