
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

function handleSendClicked(){
    
}

function handleUserClicked(user) {
    var userClickedBox = document.getElementById("user-" + user.userName);
    var selected = userClickedBox.getAttribute("data-selected");
    if (selected == "true") {
        // Do not perform any action if user is already selected.
        return;
    }

    // Change style attribute to display as selected.
    userClickedBox.setAttribute("style", "background-color: gray;");
    userClickedBox.setAttribute("data-selected", "true");
    drawReceiver(user);
}

function handleReceiverClicked(userName) {
    // Delete clicked receiver box.
    var receiver = document.getElementById("receiver-" + userName);
    receiver.remove();
    // Recover user from users tab.
    var user = document.getElementById("user-" + userName);
    // Change item style to mark as not selected.
    user.setAttribute("style", "background-color: white;");
    user.setAttribute("data-selected", "false");
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
    var lockedString = '';
    if (user.isLocked == true) {
        lockedString == 'Locked';
    }
    container.append('<div id="receiver-' + user.userName + '" class="user-select-box" onclick="handleReceiverClicked(\'' + user.userName + '\');">' +
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
        container.append("<div id='user-" + user.userName + "' onclick='handleUserClicked(" + JSON.stringify(user) + ");' \
        class='user-select-box' data-selected='false'>"+ user.userName + " " + user.firstName + " " + user.lastName +
            " " + lockedString + "</div>");
        var receiver = document.getElementById("receiver-" + user.userName);
        if (receiver != null) {
            // Drawn user already in receivers box.
            // Mark it as clicked.
            var addedUser = document.getElementById("user-" + user.userName);
            addedUser.setAttribute("style", "background-color:gray;");
            addedUser.setAttribute("data-selected", "true");
        }
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
        container.innerText = '';
    }, 5000);
}