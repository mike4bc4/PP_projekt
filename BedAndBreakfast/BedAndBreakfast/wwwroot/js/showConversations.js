var ShowConversationViewContainerId = 'show-conversations-view-container';
var MaxConversationMessageLength = 1024;

function handleCurrentCButton() {
    var context = {};
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { getConversations(context, requestSynchronizer) },
        function () { drawConversationsList(context.conversationList) },
    ];
    requestSynchronizer.run();
}

function handleHiddenCButton() {

}

function handleShowMessagesButton(conversationID) {
    var context = {};
    context.userNames = [];
    context.conversationID = conversationID;
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { getCurrentUserName(context, requestSynchronizer) },
        function () { getMessages(context, requestSynchronizer) },
        function () {
            if (context.messages == null) {
                setMessage(3);
                return;
            }
            drawMessagesList(context.messages, context.userNames[0]);
            drawMessageCreator(conversationID, context.userNames[0])
        },
    ]
    requestSynchronizer.run();
}

function handleHideConversationButton(conversationID) {

}

function handleSendMessageButton(conversationID, senderUserName, announcementID, scheduleItemsIDs) {
    var textarea = document.getElementById('message-textarea');
    var error = document.getElementById('message-textarea-error');
    var text = textarea.value;
    if (text.length > MaxConversationMessageLength) {
        error.innerText = 'Message is too long.';
        textarea.setAttribute('data-valid', 'false');
        return;
    }
    if (text.length == 0) {
        error.innerText = 'Message cannot be empty.';
        textarea.setAttribute('data-valid', 'false');
        return;
    }
    error.innerText = '';

    var messageContext = {};
    messageContext.conversationID = conversationID;
    messageContext.userNames = [senderUserName];
    messageContext.announcementID = announcementID;
    messageContext.content = text;
    messageContext.scheduleItemsIDs = scheduleItemsIDs;
    messageContext.dateSend = new Date();

    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { addMessage(messageContext, requestSynchronizer) },
        function () {
            if (messageContext.messageResponse != null) {
                setMessage(4);
            }
            else {
                setMessage(5);
            }
        }
    ];
    requestSynchronizer.run();

}

function handleMessageTextarea() {
    var textarea = document.getElementById('message-textarea');
    var counter = document.getElementById('message-textarea-counter');
    var error = document.getElementById('message-textarea-error');
    var text = textarea.value;
    counter.innerText = 'Characters: ' + text.length + '/' + MaxConversationMessageLength;
    if (text.length > MaxConversationMessageLength) {
        error.innerText = 'Message is too long.';
        textarea.setAttribute('data-valid', 'false');
    }
    else {
        error.innerHTML = '';
    }
}

function getConversations(context, synchronizer) {
    $.ajax({
        url: '/Message/GetConversations',
        dataType: 'json',
        method: 'post',
        success: function (response) {
            if (response == null) {
                setMessage(2);
            }
            else {
                context.conversationList = response;
                synchronizer.generator.next();
            }
        }
    });
}

function drawMessageCreator(conversationID, senderUserName, announcementID, scheduleItemsIDs) {
    var container = $('#' + ShowConversationViewContainerId);
    var html = '<div id="message-creator-container"></div>';
    container.append(html);
    container = $('#message-creator-container');
    html = '<textarea oninput="handleMessageTextarea();" id="message-textarea" style="resize: none;" rows="6" cols="100" data-valid="false"></textarea>';
    html += '<span id="message-textarea-counter">Characters: 0/' + MaxConversationMessageLength + '</span>';
    html += '<span id="message-textarea-error"></span>';
    html += '<br /><button onclick="handleSendMessageButton(' + conversationID + ',\'' + senderUserName + '\',' + announcementID + ');">Send</button>';
    container.append(html);
}

function drawMessagesList(messages, currentUserName) {
    var container = $('#' + ShowConversationViewContainerId);
    container.empty();
    if (messages == 0) {
        container.append('There are no messages for this conversation.');
        return;
    }
    var index = 0;
    for (var message of messages) {
        var tempMessageStyle = 'width: 600px; margin: 5px; border: 1px solid black;';
        if (message.senderUserName == currentUserName) {
            tempMessageStyle = 'width: 600px; margin: 5px; margin-left: 50px; border: 1px solid black;';
        }
        var dateSend = new Date(message.dateSend);
        html = '<div id="message-' + index + '" style="' + tempMessageStyle + '"><table border="1" style="width: 600px;"><tr><td>';
        html += '<p>' + message.senderFirstName + ' ' + message.senderLastName +
            ' (' + message.senderUserName + ') ' + dateSend.toLocaleDateString('en-US') + ' '
            + dateSend.toLocaleTimeString('en-US') + ' Announcement: ' + message.announcementID + '</p></td></tr>';
        html += '<tr><td><p>' + message.content + '</p></td></tr></table></div>';
        container.append(html);
    }
}

function drawConversationsList(conversations) {
    var tempCellStyle = 'border: 1px solid black; padding: 3px;';     // Development option
    var container = $('#' + ShowConversationViewContainerId);
    container.empty();
    if (conversations == 0) {
        container.append('There are no conversations yet');
        return;
    }
    container.append('<table id="conversations-table" style="border-collapse:collapse;"></table>');
    var html = '<tr><td style="' + tempCellStyle + '">Conversation title</td>';
    html += '<td style="' + tempCellStyle + '">Started date</td>';
    html += '<td style="">&nbsp</td>';
    html += '<td style="">&nbsp</td></tr>';
    $('#conversations-table').append(html);
    for (var conversation of conversations) {
        var dateStarted = new Date(conversation.dateStarted);
        html = '<tr><td style="' + tempCellStyle + '">' + conversation.title + '</td>';
        html += '<td style="' + tempCellStyle + '">' + dateStarted.toLocaleDateString() + ' ' + dateStarted.toLocaleTimeString() + '</td>';
        html += '<td style=""><button onclick="handleShowMessagesButton(' + conversation.conversationID + ');">Show messages</button></td>';
        html += '<td style=""><button onclick="handleHideConversationButton(' + conversation.conversationID + ');">Hide</button></td></tr>';
        $('#conversations-table').append(html);
    }
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
        case 2:
            messageTag.innerText = 'An error occurred while acquiring user data.';
            break;
        case 3:
            messageTag.innerText = 'An error occurred while acquiring conversation messages.';
            break;
        case 4:
            messageTag.innerText = 'Your message has been send.';
            break;
        case 5:
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