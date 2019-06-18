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
    var container = $('#' + showReservationsViewContainerId);
    if (reservations == null) {
        return;
    }
    container.empty();
    if (reservations.length == 0) {
        container.append('<p>You have no reservations yet.</p>');
        return;
    }
    container.append('<table id="user-reservation-table" class="table-02 text-segue-16"></table>');
	$('#user-reservation-table').append('<tr class="table-box-row-01 text-segue-14">' +
        '<td>Announcement ID</td>' +
        '<td>Type</td>' +
        '<td>Subtype</td>' +
        '<td>Additional</td>' +
        '<td>Address</td>' +
        '<td>Date</td>' +
        '<td>Time</td>' +
        '<td>Amount of reservations</td>' +
        '<td>&nbsp</td>' +
        '</tr>')

    for (var reservation of reservations) {
        var reservationDate = new Date(reservation.date);
        var todayDate = new Date();
        todayDate.setHours(0, 0, 0, 0);
		var html = '<tr class="table-box-row-02"><td class="table-box-cell-07"><a class="a-link-01" href="/Announcement/Announcement?announcementID=' + reservation.announcementID+'" >ID: ' + reservation.announcementID + '</a></td>';
		html += '<td>' + getAnnouncementTypes()[reservation.announcementType] + '</td>';
        switch (reservation.announcementType) {
            case 0:     // House
                html += '<td>' + getHouseSubtypes()[reservation.announcementSubtype] + '</td>';
                html += '<td>' + getHouseSharedParts()[reservation.houseSharedPart] + '</td>';
                break;
            case 1:     // Entertainment
                html += '<td>' + getEntertainmentSubtypes()[reservation.announcementSubtype] + '</td>';
                html += '<td>&nbsp</td>';
                break;
            case 2:     // Food
                html += '<td>' + getFoodSubtypes()[reservation.announcementSubtype] + '</td>';
                html += '<td>&nbsp</td>';
                break;
        }
        html += '<td>' + reservation.country + ' ' + reservation.region + ' ' +
            reservation.city + ' ' + reservation.street + ' ' + reservation.streetNumber + '</td>';
        html += '<td>' + reservationDate.toLocaleDateString('en-US') + '</td>';
        if (reservation.scheduleItem != null) {
            var from = reservation.scheduleItem.from.toString() + ':00';
            var to;
            if (reservation.scheduleItem.to != 24) {
                to = reservation.scheduleItem.to.toString() + ':00';
            }
            else {
                to = '23:59';
            }
            html += '<td>' + from + '-' + to + '</td>';
        }
        else {
            html += '<td>&nbsp</td>';
        }
        html += '<td>' + reservation.amount + '</td>';
		if (reservationDate > todayDate) {
			html += '<td><button class="button-05 text-segue-14" onclick="requestRemoval(' + reservation.announcementID +
				',\'' + reservationDate.toLocaleDateString('en-US') +
				'\',' + reservation.scheduleItemID + ');">Request removal</button></td></tr>';
		}
		else {
			html += "<td>&nbsp</td>";
		}
        $('#user-reservation-table').append(html);
    }
}

function requestRemoval(announcementID, date, scheduleItemID) {
    var context = {};
    context.announcementID = announcementID;
    context.dateStarted = new Date();
    context.userNames = [];
    context.title = 'Reservation removal request';
    context.readOnly = false;
    context.scheduleItemsIDs = [scheduleItemID];
    context.scheduleItemID = scheduleItemID;
    context.dateSend = new Date();

    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { getCurrentUserName(context, requestSynchronizer) },
        function () { getAnnouncementOwnerUserName(context, requestSynchronizer) },
        function () {
            // If for specified reservation there is no schedule item
            // just skip this request.
            if (scheduleItemID != null) {
                getScheduleItem(context, requestSynchronizer)
            }
            else {
                context.scheduleItem = null;
                requestSynchronizer.generator.next();
            }
        },
        function () { createConversation(context, requestSynchronizer) },
        function () {
            var reservationDate = new Date(date);
            context.content = MessageContentCreator.CreateRequestRemovalContent(context.announcementID, reservationDate, context.scheduleItem);
            addMessage(context, requestSynchronizer);
        },
        function () {
            if (context.messageResponse != null) {
                // Message sending successful.
                setMessage(1);
            }
            else {
                setMessage(2);
            }
        },
    ];
    requestSynchronizer.run();
}



var messageTimeout;
function setMessage(messageCode) {
    var messageTag = document.getElementById('message-container');
    switch (messageCode) {
        case 0:
            messageTag.innerText = 'An error occurred while browsing reservations data.';
            break;
        case 1:
            messageTag.innerText = 'Your request has been send to announcement owner. If you wish to add something to basic message check your conversation.';
            break;
        case 2:
            messageTag.innerText = 'An error occurred while sending message.';
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