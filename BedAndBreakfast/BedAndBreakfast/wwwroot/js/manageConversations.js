
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

function handleUserClicked(user) {

}

function handleReceiverClicked(receiverItemID) {

}

function handleSearchClicked(){
    
}

function drawReceiver(user) {
    var container = $('#receivers-list-container');
    container.append('<div id="receiver-' + user.userName + '" onclick="handleReceiverClicked(this.id);">' +
        user.userName + ' ' + user.firstName + ' ' + user.lastName + ' ' + lockedString + '</div>');
}

function drawUsers(users) {
    var container = $('#users-list-container');
    container.empty();
    for (var user of topUsers) {
        var lockedString = '';
        if (user.isLocked == true) {
            lockedString == 'Locked';
        }
        container.append('<div id="user-' + user.userName + '" data-selected="false" \
            onclick="handleUserClicked({userName: \'' +
            user.userName + '\', firstName: \'' + user.firstName + '\',\'' + user.lastName +
            '\',\'' + user.isLocked + '\' });">' + user.userName + ' ' + user.firstName +
            ' ' + user.lastName + ' ' + lockedString + '</div>');
    }
}