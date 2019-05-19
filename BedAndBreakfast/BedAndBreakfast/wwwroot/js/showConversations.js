var ShowConversationViewContainerId = 'show-conversations-view-container';

function getConversations() {
    $.ajax({
        url: '/Message/GetConversations',
        dataType: 'json',
        method: 'post',
        success: function (response) {
            drawConversationsList(response);
        }
    });
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
    html += '<td style="' + tempCellStyle + '">Started date</td></tr>';
    $('#conversations-table').append(html);
    for (var conversation of conversations) {
        var dateStarted = new Date(conversation.dateStarted);
        html = '<tr><td style="' + tempCellStyle + '">' + conversation.title + '</td>'
        html += '<td style="' + tempCellStyle + '">' + dateStarted.toLocaleDateString() + ' ' + dateStarted.toLocaleTimeString() + '</td></tr>'
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