var mainViewContainerId = 'browse-ann-view-container';
var announcementTimetableContainerId = 'ann-timetable-container';
var reservationsContainerId = 'reservations-container';
var makeReservationButtonContainerId = 'make-reservation-button-container';

function drawAnnouncementsList() {
    saveQueryInSession();
    setAnnouncementSearchField();
    var announcements = getAnnouncements();
    var viewContainer = document.getElementById(mainViewContainerId);
    var html = '';
    var announcementIndex = 0;
    for (var announcement of announcements) {
        html += '<div onclick="displayAnnouncementDetails(' + announcementIndex + ');" class="ann-row" id="ann-' + announcementIndex + '">';
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

function displayAnnouncementDetails(announcementIndex) {
    var announcement = getAnnouncements()[announcementIndex];
    $.ajax({
        url: '/Announcement/GetAnnouncementOwnerInfo',
        data: { announcementID: announcement.id },
        dataType: 'json',
        method: 'post',
        success: function (response) {
            if (response == null) {
                setMessage(0);
            }
            else {
                drawAnnouncement(announcementIndex, response);
            }
        }
    });
}

function drawAnnouncement(announcementIndex, ownerData) {
    var todayDate = new Date();
    todayDate.setHours(0, 0, 0, 0);
    var announcement = getAnnouncements()[announcementIndex];
    document.getElementById(mainViewContainerId).innerHTML = '<table border="1" id="selected-announcement-table"></table>';
    var sharedPart = '';
    var subtype = '';
    if (announcement.sharedPart != null) {
        sharedPart = getHouseSharedParts()[announcement.sharedPart];
    }
    switch (announcement.type) {
        case 0:     // House type
            subtype = getHouseSubtypes()[announcement.subtype];
            break;
        case 1:     // Entertainment type
            subtype = getEntertainmentSubtypes()[announcement.subtype];
            break;
        case 2:     // Food type
            subtype = getFoodSubtypes()[announcement.subtype];
            break;
    }

    $('#selected-announcement-table').append('<tr><td><h4>Details</h4></td></tr>' +
        '<tr><td><p>' + getAnnouncementTypes()[announcement.type] + '</p>' +
        '<p>' + subtype + '</p>' +
        '<p>' + sharedPart + '</p></td></tr>' +
        '<tr><td><h4>Address</h4></td></tr>' +
        '<tr><td><p>' + announcement.country + ' ' + announcement.region + ' ' + announcement.city +
        ' ' + announcement.street + ' ' + announcement.streetNumber + '</p></td></tr>' +
        '<tr><td><h4>Description</h4></td></tr>' +
        '<tr><td><p>' + announcement.description + '</p></td></tr>');
    $('#selected-announcement-table').append('<tr><td><h4>Owner</h4></td></tr>' +
        '<tr><td><p>' + ownerData.firstName + ' ' + ownerData.lastName + ' ' + ownerData.userName + '</p></td></tr>');
    $('#selected-announcement-table').append('<tr><td><h4>Additional contact info</h4></td></tr>');
    var html = '<tr><td>';
    Object.entries(announcement.contactMethods).forEach(([key, value]) => {
        html += '<p>' + getContactMethods()[value] + ' ' + key + '</p>';
    });
    html += '</tr></td>';
    $('#selected-announcement-table').append(html + '<tr><td><h4>Payment methods</h4></td></tr>');
    var html = '<tr><td>';
    Object.entries(announcement.paymentMethods).forEach(([key, value]) => {
        html += '<p>' + getPaymentMethods()[value] + ' ' + key + '</p>';
    });
    html += '</tr></td></table>';
    $('#selected-announcement-table').append(html);
    $('#' + mainViewContainerId).append('<div id="' + announcementTimetableContainerId + '"></div>');
    $('#' + mainViewContainerId).append('<div id="' + reservationsContainerId + '"></div>');
    $('#' + mainViewContainerId).append('<div id="' + makeReservationButtonContainerId + '"></div>');
    getReservations(announcement.id, todayDate.toLocaleDateString('en-US'));

}

function getReservations(announcementID, date) {
    $.ajax({
        url: '/Announcement/GetReservations',
        data: { announcementID: announcementID, date: date },
        dataType: 'json',
        method: 'post',
        success: function (response) {
            // Response will be null if announcement with specified id cannot be found (database error).
            if (response != null) {
                drawTimetable(response.reservations, response.announcement, response.scheduleItems, date);
            }
            else {
                setAnnouncementManagementMessage(5);
            }
        }
    });
}

function drawTimetable(reservations, announcement, scheduleItems, date) {
    // Draw timetable.
    var middleDate = new Date(date);
    middleDate.setHours(0, 0, 0, 0);
    var today = new Date();
    today.setHours(0, 0, 0, 0);
    var from = new Date(announcement.from);
    from.setHours(0, 0, 0, 0);
    var to = new Date(announcement.to);
    to.setHours(0, 0, 0, 0);

    switch (announcement.timetable) {
        case 1:	// Per day timetable
            var d1 = middleDate.getTime() - 1000 * 60 * 60 * 24 * 7;
            var d2 = middleDate.getTime() + 1000 * 60 * 60 * 24 * 7;
            var previousWeekMiddleDate = new Date();
            var nextWeekMiddleDate = new Date();
            previousWeekMiddleDate.setTime(d1);
            nextWeekMiddleDate.setTime(d2);

            document.getElementById('ann-timetable-container').innerHTML = '<table border="1" id="week-timetable-table"></table>';
            document.getElementById('week-timetable-table').innerHTML = '<tr id="week-table-date-row"></tr><tr id="week-table-content-row"></tr>'
            $('#week-table-date-row').append('<td rowspan="2"><button onclick="getReservations(' + announcement.id + ',\'' + previousWeekMiddleDate.toLocaleDateString('en-US') + '\');" >left arrow</button></td>');

            for (var index = 0; index < 7; index++) {
                var timetableDay = new Date();
                var d3 = middleDate.getTime() - 1000 * 60 * 60 * 24 * 3 + 1000 * 60 * 60 * 24 * index;
                timetableDay.setTime(d3);
                var cellTag = '<td onClick="addReservationItem(\'' + timetableDay.toLocaleDateString('en-US') + '\',' + reservations[index] + ',' + announcement.maxReservations + ',' + null + ');" class="clickable" >';
                if (timetableDay.getTime() < today.getTime() || timetableDay.getTime() > to.getTime()) {
                    cellTag = '<td bgcolor="lightgray">';  // Make cell not-clickable. 
                }
                $('#week-table-date-row').append('<td>' + timetableDay.toLocaleDateString('en-US') + '</td>');
                $('#week-table-content-row').append(cellTag + 'Reservations ' + reservations[index] + '/' + announcement.maxReservations + '</td>');
            }
            $('#week-table-date-row').append('<td rowspan="2"><button onclick="getReservations(' + announcement.id + ',\'' + nextWeekMiddleDate.toLocaleDateString('en-US') + '\');" >right arrow</button></td>');
            break;
        case 2:	// Per hour timetable
            var d1 = middleDate.getTime() - 1000 * 60 * 60 * 24 * 1;
            var d2 = middleDate.getTime() + 1000 * 60 * 60 * 24 * 1;
            var previousDayMiddleDate = new Date();
            var nextDayMiddleDate = new Date();
            previousDayMiddleDate.setTime(d1);
            nextDayMiddleDate.setTime(d2);

            document.getElementById('ann-timetable-container').innerHTML = '<table border="1" id="day-timetable-table"></table>';
            document.getElementById('day-timetable-table').innerHTML = '<tr id="day-timetable-header"></tr>';
            $('#day-timetable-header').append('<td rowspan="' + (scheduleItems.length + 1) + '">' +
                '<button onclick="getReservations(' + announcement.id + ',\'' + previousDayMiddleDate.toLocaleDateString('en-US') + '\');" >left arrow</button></td>' +
                '<td>' + middleDate.toLocaleDateString('en-US') + '</td>' +
                '<td rowspan="' + (scheduleItems.length + 1) + '">' +
                '<button onclick="getReservations(' + announcement.id + ',\'' + nextDayMiddleDate.toLocaleDateString('en-US') + '\');" >right arrow</button></td>');

            var index = 0;
            for (var item of scheduleItems) {
                var rowTag = '<tr onClick="addReservationItem(\'' + middleDate.toLocaleDateString('en-US') + '\',' + reservations[index] + ',' + item.maxReservations + ',{from: ' + item.from + ', to:' + item.to + ', maxReservations:' + item.maxReservations + '});" class="clickable" >';
                if (middleDate.getTime() < today.getTime() || middleDate.getTime() > to.getTime()) {
                    rowTag = '<tr bgcolor="lightgray">';  // Make row not-clickable. 
                }
                $('#day-timetable-table').append(rowTag +
                    '<td>' + (item.from.toString() + ':00-') + (item.to.toString() + ':00') +
                    ' Reservations ' + reservations[index] + '/' + item.maxReservations + '</td></tr>');
                index++;
            }
            break;
    }
}

function addReservationItem(date, currentReservations, maxReservations, scheduleItem) {
    if (currentReservations >= maxReservations) {
        setMessage(1);
        return;
    }
    var container = $('#' + reservationsContainerId);
    var reservations = getSessionReservations();
    if (reservations == null) {
        reservations = [];
        setSessionReservations(reservations);
    }
    var existingReservation = getSessionReservation(date, scheduleItem);
    if (existingReservation == null) {
        var reservationIndex = reservations.length;
        var reservationDate = new Date(date);
        reservationDate.setHours(0, 0, 0, 0);
        container.append('<div style="border: 1px solid black; width:450px; margin: 3px;" id="reservation-' + reservationIndex + '"></div>');
        $('#reservation-' + reservationIndex).append('Date: ' + reservationDate.toLocaleDateString('en-US') + ' ');
        if (scheduleItem != null) {
            var to;
            if (parseInt(scheduleItem.to) != 24) {
                to = scheduleItem.to.toString() + ':00';
            }
            else {
                to = '23:59';
            }
            $('#reservation-' + reservationIndex).append(scheduleItem.from.toString() + ':00-' + to + ' ');
        }
        $('#reservation-' + reservationIndex).append('Reservations: <input onchange="validateReservation(' + reservationIndex + ',' + currentReservations + ',' + maxReservations + ');" id="reservation-input-fld-' + reservationIndex + '" value="1" type="text"  size="5" maxlength="5" />');
        $('#reservation-' + reservationIndex).append('<button onclick="removeReservation(' + reservationIndex + ');">Remove</button>');
        $('#reservation-' + reservationIndex).append('<span id="reservation-msg-' + reservationIndex + '"></span>');
        addReservationToSession({ date: date, reservations: parseInt(document.getElementById('reservation-input-fld-' + reservationIndex).value), scheduleItem: scheduleItem, reservationIndex: reservationIndex, isValid: true });
        addMakeReservationButton();
    }
    else {
        var currentReservationsNumber = parseInt(document.getElementById('reservation-input-fld-' + existingReservation.reservationIndex).value);
        if (currentReservations + currentReservationsNumber + 1 <= maxReservations) {
            document.getElementById('reservation-input-fld-' + existingReservation.reservationIndex).value = (currentReservationsNumber + 1);
            reservations[existingReservation.reservationIndex].reservations = (currentReservationsNumber + 1);
            setSessionReservations(reservations);
        }
        else {
            setMessage(1);
        }
        addMakeReservationButton();
    }
}

function makeReservations() {
    var reservations = getSessionReservations();
    var reservationsEmpty = true;
    var reservationsValid = true;
    for (var item of reservations) {
        if (item != null) {
            reservationsEmpty = false;
            break;
        }
    }
    if (reservationsEmpty == true) {
        setMessage(2);
        return;
    }
    for (var item of reservations) {
        if (item != null && item.isValid == false) {
            reservationsValid = false;
            break;
        }
    }
    if (reservationsValid == false) {
        setMessage(3);
        return;
    }
    // List of reservations valid and not empty.
    // Parse before sending.
    var simpleReservations = [];
    for (var item of reservations) {
        if (item != null) {
            if (item.scheduleItem != null) {
                simpleReservations.push({
                    date: item.date,
                    reservations: item.reservations,
                    from: item.scheduleItem.from,
                    to: item.scheduleItem.to,
                    maxReservations: item.scheduleItem.maxReservations
                });
            }
            else {
                simpleReservations.push({
                    date: item.date,
                    reservations: item.reservations,
                    from: null,
                    to: null,
                    maxReservations: null
                });
            }
        }
    }
    $.ajax({
        url: '/Announcement/MakeReservations',
        data: {reservations: simpleReservations},
        dataType: 'json',
        method: 'post',
        success: function(response){

        } 
    });

}

function validateReservation(reservationIndex, currentReservations, maxReservations) {
    var reservations = getSessionReservations();
    var currentReservationsNumber = parseInt(document.getElementById('reservation-input-fld-' + reservationIndex).value);
    reservations[reservationIndex].reservations = currentReservationsNumber;
    if (currentReservationsNumber <= 0) {
        removeReservation(reservationIndex);
        return;
    }
    if (currentReservationsNumber + currentReservations <= maxReservations) {
        document.getElementById('reservation-msg-' + reservationIndex).innerText = '';
        reservations[reservationIndex].isValid = true;
    }
    else {
        document.getElementById('reservation-msg-' + reservationIndex).innerText = 'Value exceeds maximum number of reservations for this event.';
        reservations[reservationIndex].isValid = false;
    }
    setSessionReservations(reservations);
}

function removeReservation(reservationIndex) {
    document.getElementById('reservation-' + reservationIndex).remove();
    var reservations = getSessionReservations();
    reservations[reservationIndex] = null;
    setSessionReservations(reservations);
    var reservationsEmpty = true;
    for (var item of reservations) {
        if (item != null) {
            reservationsEmpty = false;
            break;
        }
    }
    if (reservationsEmpty == true) {
        removeMakeReservationButton();
    }
}

function addReservationToSession(reservation) {
    var reservations = getSessionReservations();
    reservations.push(reservation);
    setSessionReservations(reservations);
}

function addMakeReservationButton() {
    var button = document.getElementById(makeReservationButtonContainerId).innerHTML;
    if (button == '') {
        document.getElementById(makeReservationButtonContainerId).innerHTML = '<button onclick="makeReservations();">Make reservation</button>';
    }
}

function removeMakeReservationButton() {
    document.getElementById(makeReservationButtonContainerId).innerHTML = '';
}

function getSessionReservation(date, scheduleItem) {
    var reservations = getSessionReservations();
    if (scheduleItem != null) {
        for (var item of reservations) {
            if (item != null &&
                item.date == date &&
                item.scheduleItem.from == scheduleItem.from &&
                item.scheduleItem.to == scheduleItem.to &&
                item.scheduleItem.maxReservations == scheduleItem.maxReservations) {
                return item;
            }
        }
    }
    else {
        for (var item of reservations) {
            if (item != null &&
                item.date == date) {
                return item;
            }
        }
    }
    return null;
}

function getSessionReservations() {
    return JSON.parse(sessionStorage.getItem('reservations'));
}
function setSessionReservations(reservations) {
    return sessionStorage.setItem('reservations', JSON.stringify(reservations));
}

function saveQueryInSession() {
    var query = getQuery();
    if (query == null) {
        query = '';
    }
    sessionStorage.setItem('announcementSearchQuery', query);
}

var messageTimeout;

function setMessage(messageCode) {
    var messageTag = document.getElementById('message-container');
    switch (messageCode) {
        case 0:
            messageTag.innerText = 'An error occurred while browsing announcement data.';
            break;
        case 1:
            messageTag.innerText = 'Cannot make reservation for specified time.';
            break;
        case 2:
            messageTag.innerText = 'Reservations list cannot be empty.';
            break;
        case 3:
            messageTag.innerText = 'Some of reservations seems to be invalid, please correct those.';
        default:
            messageTag.innerText = '';
            break;
    }
    // Message will be visible for 5 seconds.
    if (messageTimeout != null) {
        window.clearTimeout(messageTimeout);
    }
    messageTimeout = window.setTimeout(function () {
        document.getElementById('message-container').innerText = '';
    }, 5000);
}