var ShowConversationViewContainerId = 'show-conversations-view-container';

function currentCButtonHandler() {
    var context = {};
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { getConversations(context, requestSynchronizer) },
        function () { drawConversationsList(context.conversationList) },
    ];
    requestSynchronizer.run();
}

function hiddenCButtonHandler() {

}

function showMessagesButtonHandler(conversationID) {
    var context = {};
    context.userNames = [];
    context.conversationID = conversationID;
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function(){getCurrentUserName(context, requestSynchronizer)},
        function(){getMessages(context, requestSynchronizer)},
        function(){},
    ]
    requestSynchronizer.run();
}

function hideConversationButtonHandler(conversationID) {

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

function getMessages(context, synchronizer) {
    $.ajax({
        url: '/Message/GetMessages',
        method: 'post',
        dataType: 'json',
        data: { conversationID: context.conversationID },
        success: function (response) {
            if (response == null) {
                setMessage(3);
            }
            else {
                context.messages = response;
                synchronizer.generator.next();
            }
        }
    });
}

function drawMessagesList(messages, currentUserName){
    var container = $('#' + ShowConversationViewContainerId);
    container.empty();
    if(messages == 0){
        container.append('There are no messages for this conversation.');
        return;
    }
    for(var message of messages){
        if(message.senderUserName == currentUserName){
            // My message
        }
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
        html += '<td style=""><button onclick="showMessagesButtonHandler(' + conversation.conversationID + ');">Show messages</button></td>';
        html += '<td style=""><button onclick="hideConversationButtonHandler(' + conversation.conversationID + ');">Hide</button></td></tr>';
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