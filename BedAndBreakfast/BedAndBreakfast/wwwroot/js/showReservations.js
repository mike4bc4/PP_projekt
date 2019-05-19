var showReservationsViewContainerId = 'show-res-view-container';

function getReservations() {
    $.ajax({
        url: '/Announcement/UserReservations',
        dataType: 'json',
        method: 'post',
        success: function (response) {
            if (response == null) {
                setMessage(0);
            }
            else {
                drawReservations(response);
            }
        }
    });
}

function drawReservations(reservations) {
    var container = $('#' + showReservationsViewContainerId);
    if (reservations == null) {
        return;
    }
    container.empty();
    if (reservations.length == 0) {
        container.append('<p>You have no reservations yet.</p>');
        return;
    }
    var index = 0;
    for (var reservation of reservations) {
        var reservationDate = new Date(reservation.date);
        var todayDate = new Date();
        todayDate.setHours(0, 0, 0, 0);
        container.append('<div id="reservation-' + index + '" style="border: 1px solid black; margin: 3px;"></div>')
        $('#reservation-' + index).append(getAnnouncementTypes()[reservation.announcementType] + ' ');
        switch (reservation.announcementType) {
            case 0:     // House
                $('#reservation-' + index).append(getHouseSubtypes()[reservation.announcementSubtype] + ' ');
                $('#reservation-' + index).append(getHouseSharedParts()[reservation.houseSharedPart] + ' ');
                break;
            case 1:     // Entertainment
                $('#reservation-' + index).append(getEntertainmentSubtypes()[reservation.announcementSubtype] + ' ');
                break;
            case 2:     // Food
                $('#reservation-' + index).append(getFoodSubtypes()[reservation.announcementSubtype] + ' ');
                break;
        }
        $('#reservation-' + index).append('Address: ' + reservation.country + ' ' + reservation.region + ' ' +
            reservation.city + ' ' + reservation.street + ' ' + reservation.streetNumber + ' ');
        $('#reservation-' + index).append(reservationDate.toLocaleDateString('en-US') + ' ');
        if (reservation.scheduleItem != null) {
            var from = reservation.scheduleItem.from.toString() + ':00';
            var to;
            if (reservation.scheduleItem.to != 24) {
                to = reservation.scheduleItem.to.toString() + ':00';
            }
            else {
                to = '23:59';
            }
            $('#reservation-' + index).append('From: ' + from +
                ' To: ' + to + ' ');
        }
        $('#reservation-' + index).append('Reservations: ' + reservation.amount + ' ');

        if (reservationDate > todayDate) {
            $('#reservation-' + index).append('<button onclick="requestRemoval(' + reservation.announcementID +
                ',\'' + reservationDate.toLocaleDateString('en-US') +
                '\',' + reservation.scheduleItemID + ');">Request removal</button>');
        }
        index++;
    }
}

function requestRemoval(announcementID, date, scheduleItemID) {

    setMessage(1);
}

var messageTimeout;
function setMessage(messageCode) {
    var messageTag = document.getElementById('message-container');
    switch (messageCode) {
        case 0:
            messageTag.innerText = 'An error occurred while browsing reservations data.';
            break;
        case 1:
            messageTag.innerText = 'Your request has been send to announcement owner.';
            break;
        default:
            messageTag.innerText = '';
            break;
    }
    // Message will be visible for 5 seconds.
    if (messageTimeout != null) {
        window.clearTimeout(messageTimeout);
    }
    messageTimeout = window.setTimeout(function () {
        messageTag.innerText = '';
    }, 5000);
}