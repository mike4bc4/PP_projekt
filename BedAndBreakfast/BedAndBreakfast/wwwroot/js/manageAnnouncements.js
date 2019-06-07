function getCurrentUserAnnouncements(context, requestSynchronizer) {
    $.ajax({
        url: "/Announcement/GetCurrentUserAnnouncements",
        dataType: "json",
        method: "post",
        success: function (response) {
            context.userAnnouncements = response;
            requestSynchronizer.generator.next();
        }
    });
}

function changeAnnouncementsStatus(context, requestSynchronizer) {
    $.ajax({
        url: "/Announcement/ChangeAnnouncementsStatus",
        dataType: "json",
        method: "post",
        data: { announcementIDs: context.announcementIDs },
        success: function (response) {
            context.changeAnnouncementsStatusResponse = response;
            requestSynchronizer.generator.next();
        }
    });
}

function removeAnnouncements(context, requestSynchronizer) {
    $.ajax({
        url: "/Announcement/RemoveAnnouncements",
        dataType: "json",
        method: "post",
        data: { announcementIDs: context.announcementIDs },
        success: function (response) {
            context.removeAnnouncementsResponse = response;
            requestSynchronizer.generator.next();
        }
    });
}

function editAnnouncement(context, requestSynchronizer) {
    $.ajax({
        url: "/Announcement/EditAnnouncement",
        dataType: "html",
        method: "post",
        success: function (response) {
            context.editAnnouncementResponse = response;
            requestSynchronizer.generator.next();
        }
    });
}

function handleMyAnnouncementsButton() {
    var context = {};
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { getCurrentUserAnnouncements(context, requestSynchronizer); },
        function () {
            if (context.userAnnouncements == null) {
                setGlobalMessage(0);
                return;
            }
            drawUserAnnouncements(context.userAnnouncements);
            return;
        },
    ]
    requestSynchronizer.run();
}

function handleCreateAnnouncementButton() {
    var container = document.getElementById("manage-announcements-view-container");
    var context = {};
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { editAnnouncement(context, requestSynchronizer); },
        function () {
            container.innerHTML = context.editAnnouncementResponse;
            handlePartialViewInitialLoad("Create");
        },
    ];
    requestSynchronizer.run();
}

function handleAnnouncementEditButton(announcementID) {

}

function handleRemoveSelectedButton(selectedAnnouncementsIDs) {
    var context = {};
    context.announcementIDs = selectedAnnouncementsIDs;
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { removeAnnouncements(context, requestSynchronizer); },
        function () {
            if (context.removeAnnouncementsResponse != 0) {
                setGlobalMessage(2);
            }
            else {
                setGlobalMessage(-1);
            }
            // Redraw announcements to update data.
            handleMyAnnouncementsButton();
            hideAdditionalButtons();
            return;
        },
    ];
    requestSynchronizer.run();
}

function handleChangeStatusButton(selectedAnnouncementsIDs) {
    var context = {};
    context.announcementIDs = selectedAnnouncementsIDs;
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { changeAnnouncementsStatus(context, requestSynchronizer); },
        function () {
            if (context.changeAnnouncementsStatusResponse.result != 0) {
                setGlobalMessage(1);
                if (context.changeAnnouncementsStatusResponse.error == true) {
                    setGlobalMessage(3);
                }
            }
            else {
                setGlobalMessage(-1);
            }
            // Redraw announcements to update data.
            handleMyAnnouncementsButton();
            hideAdditionalButtons();
            return;
        },
    ];
    requestSynchronizer.run();
}

function handleSelectAnnouncementCheckbox() {
    var inputFields = document.getElementsByTagName("input");
    var checkboxes = [];
    // Get only checkboxes related to announcement selection
    for (var i = 0; i < inputFields.length; i++) {
        var itemIDArray = inputFields[i].getAttribute("id").split("-");
        if (inputFields[i].getAttribute("type") == "checkbox" && itemIDArray[0] == "announcement" && itemIDArray[1] == "selected") {
            checkboxes.push(inputFields[i]);
        }
    }
    var selectedAnnouncementsIDs = [];
    for (var checkbox of checkboxes) {
        var itemIDArray = checkbox.getAttribute("id").split("-");
        if (checkbox.checked == true) {
            selectedAnnouncementsIDs.push(itemIDArray[2]);
        }
    }
    var buttonsContainer = document.getElementById("additional-options");
    if (selectedAnnouncementsIDs.length == 0) {
        buttonsContainer.innerHTML = "";
        return 0;
    }
    else {
        buttonsContainer.innerHTML = "<button onclick='handleRemoveSelectedButton(" + JSON.stringify(selectedAnnouncementsIDs) + ");'>Remove selected</button>\
        <button onclick='handleChangeStatusButton("+ JSON.stringify(selectedAnnouncementsIDs) + ");'>Change status</button>";
    }
}

function handleTimetableButton(announcementID) {

}

function hideAdditionalButtons() {
    var container = document.getElementById("additional-options");
    container.innerHTML = "";
}

function drawUserAnnouncements(userAnnouncements) {
    var container = document.getElementById("manage-announcements-view-container");
    if (userAnnouncements == 0) {
        container.innerHTML = "<p>You have no announcements yet. Would you like to become a host?</p>";
        container.innerHTML += "<button onclick='handleCreateAnnouncementButton();'>Become a host</button>";
        return;
    }
    container.innerHTML = "<table id='announcements-container' border='1'>\
    <tr>\
        <td></td>\
        <td>ID</td>\
        <td>Type</td>\
        <td>Subtype</td>\
        <td>Additional</td>\
        <td>From</td>\
        <td>To</td>\
        <td>Address</td>\
        <td>Status</td>\
        <td></td>\
        <td></td>\
    </tr>\
    </table>";
    container = document.getElementById("announcements-container");
    for (var announcement of userAnnouncements) {
        container.innerHTML += "<tr>\
        <td><input onclick='handleSelectAnnouncementCheckbox();' type='checkbox' id='announcement-selected-"+ announcement.announcementID + "'/></td>\
        <td>"+ announcement.announcementID + "</td>\
        <td>"+ announcementTypeToString(announcement.type) + "</td>\
        <td>"+ announcementSubtypeToString(announcement.type, announcement.subtype) + "</td>\
        <td>"+ announcementSharedPartToString(announcement.type, announcement.sharedPart) + "</td>\
        <td>"+ (new Date(announcement.from)).toLocaleDateString() + "</td>\
        <td>"+ (new Date(announcement.to)).toLocaleDateString() + "</td>\
        <td>"+ announcement.country + " " + announcement.region + " " + announcement.city + " \
            "+ announcement.street + " " + announcement.streetNumber + "</td>\
        <td>"+ announcementActiveToString(announcement.isActive) + "</td>\
        <td><button onclick='handleAnnouncementEditButton("+ announcement.announcementID + ");'>Edit</button></td>\
        <td><button onclick='handleTimetableButton("+ announcement.announcementID + ");'>Timetable</button></td>\
        </tr>";
    }
}

function announcementActiveToString(isActive) {
    var statusString = "Active";
    if (isActive == false) {
        statusString = "Inactive";
    }
    return statusString;
}

function announcementTypeToString(announcementType) {
    return getAnnouncementTypes()[announcementType];
}

function announcementSubtypeToString(announcementType, announcementSubtype) {
    var subtype = "";
    switch (announcementType) {
        case 0:
            subtype = getHouseSubtypes()[announcementSubtype];
            break;
        case 1:
            subtype = getEntertainmentSubtypes()[announcementSubtype];
            break;
        case 2:
            subtype = getFoodSubtypes()[announcementSubtype];
            break;
        default:
            break;
    }
    return subtype;
}

function announcementSharedPartToString(announcementType, sharedPart) {
    if (announcementType == 0) {
        return getSharedParts()[sharedPart];
    }
    return "";
}

var globalMessageTimeout;
function setGlobalMessage(messageCode) {
    var container = document.getElementById("message-container");
    switch (messageCode) {
        case -1:
            container.innerText = "Unidentified error occurred.";
            break;
        case 0:
            container.innerText = "An error occurred while retrieving your announcements from database.";
            break;
        case 1:
            container.innerText = "Announcements status successfully changed.";
            break;
        case 2:
            container.innerText = "Announcements successfully removed.";
            break;
        case 3:
            container.innerHTML = "For one or more announcement status has not been changed because of invalid active time range."
            break;
        default:
            break;
    }
    if (globalMessageTimeout != null) {
        clearTimeout(globalMessageTimeout);
    }
    globalMessageTimeout = setTimeout(function () {
        container.innerText = "";
    }, 5000);
}