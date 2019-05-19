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
                drawMakeReservationsResponse(response);
            }
        }
    });
}

function drawMakeReservationsResponse(reservations) {
    var tempCellStyle = 'border: 1px solid black; padding: 3px;'; // Development option.
    var container = $('#' + showReservationsViewContainerId);
    if (reservations == null) {
        return;
    }
    container.empty();
    if (reservations.length == 0) {
        container.append('<p>You have no reservations yet.</p>');
        return;
    }
    container.append('<table id="user-reservation-table" style="border-collapse:collapse;"></table>');
    $('#user-reservation-table').append('<tr>' +
        '<td style="' + tempCellStyle + '">Announcement ID</td>' +
        '<td style="' + tempCellStyle + '">Type</td>' +
        '<td style="' + tempCellStyle + '">Subtype</td>' +
        '<td style="' + tempCellStyle + '">Additional</td>' +
        '<td style="' + tempCellStyle + '">Address</td>' +
        '<td style="' + tempCellStyle + '">Date</td>' +
        '<td style="' + tempCellStyle + '">Time</td>' +
        '<td style="' + tempCellStyle + '">Amount of reservations</td>' +
        '<td style="">&nbsp</td>' +
        '</tr>')
    var index = 0;
    for (var reservation of reservations) {
        var reservationDate = new Date(reservation.date);
        var todayDate = new Date();
        todayDate.setHours(0, 0, 0, 0);
        var html = '<tr><td style="' + tempCellStyle + '">' + reservation.announcementID + '</td>';
        html += '<td style="' + tempCellStyle + '">' + getAnnouncementTypes()[reservation.announcementType] + '</td>';
        switch (reservation.announcementType) {
            case 0:     // House
                html += '<td style="' + tempCellStyle + '">' + getHouseSubtypes()[reservation.announcementSubtype] + '</td>';
                html += '<td style="' + tempCellStyle + '">' + getHouseSharedParts()[reservation.houseSharedPart] + '</td>';
                break;
            case 1:     // Entertainment
                html += '<td style="' + tempCellStyle + '">' + getEntertainmentSubtypes()[reservation.announcementSubtype] + '</td>';
                html += '<td style="' + tempCellStyle + '">&nbsp</td>';
                break;
            case 2:     // Food
                html += '<td style="' + tempCellStyle + '">' + getFoodSubtypes()[reservation.announcementSubtype] + '</td>';
                html += '<td style="' + tempCellStyle + '">&nbsp</td>';
                break;
        }
        html += '<td style="' + tempCellStyle + '">' + reservation.country + ' ' + reservation.region + ' ' +
            reservation.city + ' ' + reservation.street + ' ' + reservation.streetNumber + '</td>';
        html += '<td style="' + tempCellStyle + '">' + reservationDate.toLocaleDateString('en-US') + '</td>';
        if (reservation.scheduleItem != null) {
            var from = reservation.scheduleItem.from.toString() + ':00';
            var to;
            if (reservation.scheduleItem.to != 24) {
                to = reservation.scheduleItem.to.toString() + ':00';
            }
            else {
                to = '23:59';
            }
            html += '<td style="' + tempCellStyle + '">' + from + '-' + to + '</td>';
        }
        else {
            html += '<td style="' + tempCellStyle + '">&nbsp</td>';
        }
        html += '<td style="' + tempCellStyle + '">' + reservation.amount + '</td>';
        if (reservationDate > todayDate) {
            html += '<td><button onclick="requestRemoval(' + reservation.announcementID +
                ',\'' + reservationDate.toLocaleDateString('en-US') +
                '\',' + reservation.scheduleItemID + ');">Request removal</button></td></tr>';
        }
        $('#user-reservation-table').append(html);
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