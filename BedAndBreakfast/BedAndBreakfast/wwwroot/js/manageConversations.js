
function getTopUsers(context, requestSynchronizer) {
    $.ajax({
        url: '/Administration/GetTopUsers',
        method: 'post',
        dataType: 'json',
        success: function (response) {
            context.topUsers = response,
                requestSynchronizer.generator.next();
        }
    });
}

function findUsersByQuery(context, requestSynchronizer) {
    $.ajax({
        url: '/Administration/FindUsersByQuery',
        method: 'post',
        dataType: 'json',
        data: { query: context.query },
        success: function (response) {
            context.usersFound = response;
            requestSynchronizer.generator.next();
        }
    });
}

function handleUserClicked(user) {

}

function handleReceiverClicked(receiverItemID) {

}

function handleSearchClicked() {
    var inputField = document.getElementById('search-query-input-field');
    var query = inputField.value;
    var context = {};
    context.query = query;
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { findUsersByQuery(context, requestSynchronizer) },
        function () {
            if (context.usersFound.length == 0) {
                setGlobalMessage(0);
                return;
            }
            drawUsers(context.usersFound);
        },
    ]
    requestSynchronizer.run();
}


function drawReceiver(user) {
    var container = $('#receivers-list-container');
    container.append('<div id="receiver-' + user.userName + '" onclick="handleReceiverClicked(this.id);">' +
        user.userName + ' ' + user.firstName + ' ' + user.lastName + ' ' + lockedString + '</div>');
}

function drawUsers(users) {
    var container = $('#users-list-container');
    container.empty();
    for (var user of users) {
        var lockedString = '';
        if (user.isLocked == true) {
            lockedString == 'Locked';
        }
        container.append('<div id="user-' + user.userName + '" style="border: 1px solid black;\
            margin: 3px; padding: 3px;" data-selected="false" \
            onclick="handleUserClicked({userName: \'' +
            user.userName + '\', firstName: \'' + user.firstName + '\',\'' + user.lastName +
            '\',\'' + user.isLocked + '\' });">' + user.userName + ' ' + user.firstName +
            ' ' + user.lastName + ' ' + lockedString + '</div>');
    }
}

var globalMessageTimeout;
function setGlobalMessage(messageIndex) {
    var container = document.getElementById('global-message-container');
    switch (messageIndex) {
        case 0:
            container.innerText = 'There are no users matching query.';
            break;
        default:
            break;
    }
    if (globalMessageTimeout != null) {
        clearTimeout(globalMessageTimeout);
    }
    globalMessageTimeout = setTimeout(function () {
        context.innerText = '';
    }, 5000);
}