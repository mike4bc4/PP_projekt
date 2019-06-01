
class MessageContentCreator {
    static createNewReservationContent(simplifiedReservations) {
        var content = 'Hello, I have just made reservation(s) for: \n';
        for (var reservation of simplifiedReservations) {
            content += '\tAnnouncement ' + reservation.announcementID + ' on ' + reservation.date;
            if (reservation.from != null && reservation.to != null) {
                content += ' from ' + reservation.from.toString() + ':00 to ';
                if (reservation.to != 24) {
                    content += reservation.to.toString() + ':00';
                }
                else {
                    content += '23:59';
                }
            }
            content += ' for ' + reservation.reservations + ' place(s).\n';
        }
        return content;
    }
}

function getCurrentUserName(context, requestSynchronizer) {
    $.ajax({
        url: '/Account/GetCurrentUserName',
        dataType: 'json',
        method: 'post',
        success: function (response) {
            if (context.userNames == null) {
                context.userNames = [];
            }
            context.userNames.push(response);
            requestSynchronizer.generator.next();
        }
    });
}

function getAnnouncementOwnerUserName(context, requestSynchronizer) {
    $.ajax({
        url: '/Announcement/GetAnnouncementOwnerUserName',
        data: { announcementID: context.announcementID },
        dataType: 'json',
        method: 'post',
        success: function (response) {
            if (context.userNames == null) {
                context.userNames = [];
            }
            context.userNames.push(response);
            requestSynchronizer.generator.next();
        }
    });
}

function createConversation(context, requestSynchronizer) {
    $.ajax({
        url: '/Message/CreateConversation',
        data: {
            title: context.title,
            userNames: context.userNames,
            dateStarted: context.dateStarted.toISOString(),
            announcementID: context.announcementID,
            scheduleItemsIDs: context.scheduleItemsIDs,
            readOnly: context.readOnly
        },
        dataType: 'json',
        method: 'post',
        success: function (response) {
            context.conversationID = response;
            requestSynchronizer.generator.next();
        }
    });
}

function addMessage(context, requestSynchronizer) {
    $.ajax({
        url: '/Message/AddMessage',
        data: {
            conversationID: context.conversationID,
            content: context.content,
            dateSend: context.dateSend.toISOString(),
            senderUserName: context.userNames[0],
        },
        dataType: 'json',
        method: 'post',
        success: function (response) {
            context.messageResponse = response;
            requestSynchronizer.generator.next();
        }
    });


}

function getMessages(context, synchronizer) {
    $.ajax({
        url: '/Message/GetMessages',
        method: 'post',
        dataType: 'json',
        data: { conversationID: context.conversationID },
        success: function (response) {
            context.messages = response;
            synchronizer.generator.next();
        }
    });
}



