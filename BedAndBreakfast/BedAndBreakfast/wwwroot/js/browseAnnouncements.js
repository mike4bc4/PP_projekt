function drawAnnouncementsList() {
    var announcements = getAnnouncements();
    var viewContainer = getViewContainer();
    var html = '';
    var announcementIndex = 0;
    for (var announcement of announcements) {
        html += '<div onclick="" class="ann-row" id="ann-' + announcementIndex + '">';
        html += '<div class="ann-row-subtitle">Announcement type</div>';
        html += '<div>' + getAnnouncementTypes()[announcement.type] + '</div>';
        switch (announcement.type) {
            case 0: // House announcement
                html += '<div>' + getHouseSubtypes()[announcement.subtype] + '</div>';
                break;
            case 1: // Entertainment announcement
                html += '<div>' + getEntertainmentSubtypes()[announcement.subtype] + '</div>';
                break;
            case 2: // Food announcement
                html += '<div>' + getFoodSubtypes()[announcement.subtype] + '</div>';
                break;
        }
        if (announcement.type == 0) {
            html += '<div>' + getHouseSharedParts()[announcement.sharedPart] + '</div>';
        }
        html += '<div class="ann-row-subtitle">Where event takes place</div>';
        html += '<div>' + announcement.country + ' ' + announcement.region
            + ' ' + announcement.city + ' ' + announcement.street + ' ' + announcement.streetNumber + '</div>';
        html += '</div>';
        announcementIndex++;
    }
    viewContainer.innerHTML = html;
}


function getViewContainer() {
    return document.getElementById('browse-ann-view-container');
}