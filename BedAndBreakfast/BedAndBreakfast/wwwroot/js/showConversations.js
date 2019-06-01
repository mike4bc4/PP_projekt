var ShowConversationViewContainerId = 'show-conversations-view-container';
var MaxConversationMessageLength = 1024;
var MessageCreatorContainerId = 'message-creator-container';

function handleCurrentHiddenConversationButton(hidden = false) {
    var context = {};
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { getConversations(context, requestSynchronizer) },
        function () {
            drawConversationsList(context.conversationList, hidden);
            // Clear message creator on conversation menu reload.
            $('#' + MessageCreatorContainerId).empty();
        },
    ];
    requestSynchronizer.run();
}

function handleShowMessagesButton(conversationID, conversationReadOnly) {
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
            if (conversationReadOnly == false) {
                // Message creator may only be drawn while conversation is not read only.
                drawMessageCreator(conversationID, context.userNames[0])
            }
        },
    ]
    requestSynchronizer.run();
}

function handleHideRevertConversationButton(conversationID, hide) {
    var context = {};
    context.conversationID = conversationID;
    context.userNames = [];
    context.hide = hide;
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { getCurrentUserName(context, requestSynchronizer) },
        function () { hideRevertConversation(context, requestSynchronizer) },
        function () {
            if (context.hideRevertConversation == null) {
                setMessage(6);
                return;
            }
            // Conversation successfully hidden.
            // Redraw not hidden conversations and send proper message.
            handleCurrentHiddenConversationButton(!hide);
            if (hide == true) {
                setMessage(7);
            }
            else {
                setMessage(8);
            }
        }
    ]
    requestSynchronizer.run();
}

function hideRevertConversation(context, requestSynchronizer) {
    $.ajax({
        url: '/Message/HideRevertConversation',
        dataType: 'json',
        method: 'post',
        data: { userName: context.userNames[0], conversationID: context.conversationID, hide: context.hide },
        success: function (response) {
            context.hideRevertConversation = response;
            requestSynchronizer.generator.next();
        }
    });
}

function handleSendMessageButton(conversationID, senderUserName) {
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
    messageContext.content = text;
    messageContext.dateSend = new Date();

    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { addMessage(messageContext, requestSynchronizer) },
        function () {
            if (messageContext.messageResponse != null) {
                // Message has been send correctly.
                // Redraw interface to show updated conversations and display message.
                handleShowMessagesButton(conversationID);
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

function drawMessageCreator(conversationID, senderUserName) {
    var container = $('#' + MessageCreatorContainerId);
    container.empty();
    html = '<textarea oninput="handleMessageTextarea();" id="message-textarea" style="resize: none;" rows="6" \
    cols="100" data-valid="false"></textarea>';
    html += '<span id="message-textarea-counter">Characters: 0/' + MaxConversationMessageLength + '</span>';
    html += '<span id="message-textarea-error"></span>';
    html += '<br /><button onclick="handleSendMessageButton(' + conversationID + ',\'' + senderUserName + '\');">Send</button>';
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
        var tempMessageStyle = 'width: 600px; margin: 5px; margin-left: 50px; border: 1px solid black;';
        // Use different style for messages from currently logged in user.
        if (message.senderUserName == currentUserName) {
            tempMessageStyle = 'width: 600px; margin: 5px; border: 1px solid black;';
        }
        var dateSend = new Date(message.dateSend);
        html = '<div id="message-' + index + '" style="' + tempMessageStyle + '"><table border="1" \
        style="width: 600px;"><tr><td>';
        html += '<p>' + message.senderFirstName + ' ' + message.senderLastName +
            ' (' + message.senderUserName + ') ' + dateSend.toLocaleDateString('en-US') + ' '
            + dateSend.toLocaleTimeString('en-US') + '</p></td></tr>';
        html += '<tr><td><p style="white-space: pre-wrap">' + message.content + '</p></td></tr></table></div>';
        container.append(html);
    }
}

function drawConversationsList(conversations, hidden = false) {
    var tempCellStyle = 'border: 1px solid black; padding: 3px;';     // Development option
    var container = $('#' + ShowConversationViewContainerId);
    var headerString = 'Current conversations';
    container.empty();

    if (hidden == true) {
        headerString = 'Hidden conversations';
    }

    if (conversations == 0) {
        container.append('There are no conversations yet');
        return;
    }
    container.append('<table id="conversations-table" style="border-collapse:collapse;"></table>');
    var html = '<tr><td style="' + tempCellStyle + '" colspan="3">' + headerString + '</td><td>&nbsp</td><td>&nbsp</td></tr>';
    html += '<tr><td style="' + tempCellStyle + '">Conversation title</td>';
    html += '<td style="' + tempCellStyle + '">Announcement info</td>';
    html += '<td style="' + tempCellStyle + '">Started date</td>';
    html += '<td style="">&nbsp</td>';
    html += '<td style="">&nbsp</td></tr>';
    $('#conversations-table').append(html);
    for (var conversation of conversations) {
        if (conversation.isHidden == !hidden) {
            continue;
        }

        var dateStarted = new Date(conversation.dateStarted);

        var scheduleItemsString = '';
        if (conversation.scheduleItems.length != 0) {
            scheduleItemsString += ', Time: | ';
            for (var scheduleItem of conversation.scheduleItems) {
                scheduleItemsString += scheduleItem.from.toString() + ':00 - ';
                if (scheduleItem.to != 24) {
                    scheduleItemsString += scheduleItem.to.toString() + ':00';
                }
                else {
                    scheduleItemsString += '23:59';
                }
                scheduleItemsString += ' | ';
            }
        }

        html = '<tr><td style="' + tempCellStyle + '">' + conversation.title + '</td>';
        html += '<td style="' + tempCellStyle + '">ID: ' + conversation.announcementID + scheduleItemsString + '</td>'
        html += '<td style="' + tempCellStyle + '">' + dateStarted.toLocaleDateString() + ' ' + dateStarted.toLocaleTimeString() + '</td>';
        html += '<td style=""><button onclick="handleShowMessagesButton(' + conversation.conversationID + ',' + conversation.readOnly + ');">Show messages</button></td>';
        if (hidden == false) {
            html += '<td style=""><button onclick="handleHideRevertConversationButton(' + conversation.conversationID + ', true);">Hide</button></td></tr>';
        }
        else {
            html += '<td style=""><button onclick="handleHideRevertConversationButton(' + conversation.conversationID + ', false);">Revert</button></td></tr>';
        }
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
        case 6:
            messageTag.innerText = 'An error occurred while hiding conversation.';
            break;
        case 7:
            messageTag.innerText = 'Conversation has been moved to hidden conversations folder.';
            break;
        case 8:
            messageTag.innerText = 'Conversation has been moved to current conversations folder.';
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