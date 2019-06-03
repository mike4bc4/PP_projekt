
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

function getAllUsers(context, requestSynchronizer) {
    $.ajax({
        url: "/Administration/GetAllUsers",
        dataType: "json",
        method: "post",
        success: function (response) {
            context.usersFound = response;
            requestSynchronizer.generator.next();
        }
    });
}

function handleSendClicked() {
    var textarea = document.getElementById("message-textarea");
    var titleInputField = document.getElementById("title-input-field");
    var readOnlyCheckbox = document.getElementById("readonly-checkbox");
    var text = textarea.value;
    var title = titleInputField.value;
    if (text.length == 0) {
        setGlobalMessage(1);
        return;
    }
    if (title.length == 0) {
        setGlobalMessage(2);
        return;
    }

    var context = {};
    context.userNames = [];
    context.title = title;
    context.dateStarted = new Date();
    context.announcementID = null;
    context.scheduleItemsIDs = null;
    context.readOnly = readOnlyCheckbox.checked;
    context.content = text;
    context.dateSend = new Date();

    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { getCurrentUserName(context, requestSynchronizer) },
        function () {
            var receiversUserNames = getReceivers();
            if (receiversUserNames.length == 0) {
                getAllUsers(context, requestSynchronizer);
            }
            else {
                context.userNames = context.userNames.concat(receiversUserNames);
                // One does not simply skip generator function from generator function :)
                setTimeout(function(){
                    requestSynchronizer.generator.next();
                },0);          
            }
        },
        function () {
            if (context.hasOwnProperty("usersFound") == true) {
                // Receivers list was empty and all possible users were acquired.
                for (var user of context.usersFound) {
                    context.userNames.push(user.userName);
                }
            }
            createConversation(context, requestSynchronizer);
        },
        function () { addMessage(context, requestSynchronizer); },
        function () {
            if (context.messageResponse == null) {
                setGlobalMessage(3);
                return;
            }
            setGlobalMessage(4);
            // Reset message creator
            titleInputField.value = '';
            readOnlyCheckbox.checked = true;
            textarea.value = '';

        },
    ];
    requestSynchronizer.run();
}

function getReceivers() {
    var container = document.getElementById("receivers-list-container");
    var receivers = container.childNodes;
    var receiversUserNames = [];
    if (receivers.length == 0) {
        return receiversUserNames;
    }
    for (var item of receivers) {
        // Add user name to receiver user names array (skip "receiver-" part).
        receiversUserNames.push(item.id.substring(9));
    }
    return receiversUserNames;
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
        case 1:
            container.innerText = "Message cannot be empty.";
            break;
        case 2:
            container.innerText = "Title cannot be empty.";
            break;
        case 3:
            container.innerText = "An error occurred while sending message."
            break;
        case 4:
            container.innerText = "Message created successfully."
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