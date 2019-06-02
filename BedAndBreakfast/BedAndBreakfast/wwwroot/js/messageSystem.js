
class MessageContentCreator {
    static CreateNewReservationContent(simplifiedReservations) {
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
        content += '\n\nThis message was generated automatically.';
        return content;
    }
    static CreateRequestRemovalContent(announcementID, date, scheduleItem) {
        var content = 'Hello, I am writing to ask you about removing my reservation(s) for: \n';
        content += '\tAnnouncement: ' + announcementID + ' on ' + date.toLocaleDateString('en-US');
        if (scheduleItem != null) {
            content += ' from ' + scheduleItem.from.toString() + ':00 to ';
            if (scheduleItem.to != 24) {
                content += scheduleItem.to.toString() + ':00';
            }
            else {
                content += '23:59';
            }
        }
        content += '\n\nThis message was generated automatically.';
        return content;
    }
    static CreateAskAboutAnnouncementContent(announcementID) {
        var content = 'Hello, I would like to ask you a question about your announcement (' + announcementID + ').';
        content += '\n\nThis message was generated automatically.';
        return content;
    }
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



