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
        function () { isUserInAdminRole(context, requestSynchronizer)},
        function () {
            if (context.messages == null) {
                setMessage(3);
                return;
            }
            drawMessagesList(context.messages, context.userNames[0]);
            if (conversationReadOnly == false || context.isUserInAdminRole == true) {
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
                // Reset message creator textarea.
                textarea.value = '';
                // Perform validation to reset character counter.
                handleMessageTextarea();
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
	html = '<textarea spellcheck="false" class="textarea-style-01 text-segue-16" oninput="handleMessageTextarea();" id="message-textarea" style="resize: none;" rows="6" \
    cols="100" data-valid="false"></textarea>';
	html += '<span class="counter-style-01 text-segue-16" id="message-textarea-counter">Characters: 0/' + MaxConversationMessageLength + '</span>';
	html += '<span class="counter-style-01 text-segue-16" id="message-textarea-error"></span>';
    html += '<br /><button class="button-01" onclick="handleSendMessageButton(' + conversationID + ',\'' + senderUserName + '\');">Send</button>';
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
		
		var userFirstNameString = message.senderFirstName == null ? "" : message.senderFirstName;
		var userLastNameString = message.senderLastName == null ? "" : message.senderLastName;
		var className = message.senderUserName == currentUserName ? 'message-style-01 text-segue-16' : 'message-style-02 text-segue-16';

        var dateSend = new Date(message.dateSend);
        html = '<div id="message-' + index + '" class="' + className + '"><table><tr><td>';
		html += '<p class="message-header-style-01 text-segue-14">' + userFirstNameString + ' ' + userLastNameString +
            ' (' + message.senderUserName + ') ' + dateSend.toLocaleDateString('en-US') + ' '
            + dateSend.toLocaleTimeString('en-US') + '</p></td></tr>';
		html += '<tr><td><p class="message-content-style-01">' + message.content + '</p></td></tr></table></div>';
        container.append(html);
    }
}

function drawConversationsList(conversations, hidden = false) {

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
	container.append("<p class='indent-02 text-segue-18' >" + headerString + "<p><hr />");
    container.append('<table class="text-segue-16 table-02" id="conversations-table"></table>');
    var html = '';
	html += '<tr class="text-segue-14 table-box-row-01 "><td class="table-td-style-01">Conversation title</td>';
	html += '<td class="table-td-style-01">Announcement info</td>';
	html += '<td class="table-td-style-01">Started date</td>';
	html += '<td class="table-td-style-01">&nbsp</td>';
    html += '<td>&nbsp</td></tr>';
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

		html = '<tr class="table-box-row-02"><td class="table-box-cell-07">' + conversation.title + '</td>';
		if (conversation.announcementID != null) {
			html += '<td><a class="a-link-01" href="/Announcement/Announcement?announcementID=' + conversation.announcementID + '" >' + conversation.announcementID + scheduleItemsString + '</a></td>'
		}
		else {
			html += '<td>None</td>'
		}
		html += '<td>' + dateStarted.toLocaleDateString() + ' ' + dateStarted.toLocaleTimeString() + '</td>';
		html += '<td class="table-box-cell-12"><button class="button-05 text-segue-14" onclick="handleShowMessagesButton(' + conversation.conversationID + ',' + conversation.readOnly + ');">Show</button></td>';
        if (hidden == false) {
			html += '<td class="table-box-cell-12"><button class="button-05 text-segue-14" onclick="handleHideRevertConversationButton(' + conversation.conversationID + ', true);">Hide</button></td></tr>';
        }
        else {
			html += '<td class="table-box-cell-12"><button class="button-05 text-segue-14" onclick="handleHideRevertConversationButton(' + conversation.conversationID + ', false);">Revert</button></td></tr>';
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