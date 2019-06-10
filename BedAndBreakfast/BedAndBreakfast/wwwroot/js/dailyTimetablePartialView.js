
var timetableContext = {};

/**
 * Handles get reservations request.
 */
function getReservationsRequest(context, requestSynchronizer) {
    $.ajax({
        url: "/Announcement/GetReservations",
        data: { announcementID: context.announcementID, date: context.date.toISOString() },
        dataType: "json",
        method: "post",
        success: function (response) {
            context.getReservationsRequestResponse = response;
            requestSynchronizer.generator.next();
        }
    });
}

/**
 * Handles get users reservations request.
 */
function getUsersReservationsRequest(context, requestSynchronizer) {
    $.ajax({
        url: "/Announcement/GetUsersReservations",
        dataType: "json",
        method: "post",
        data: {
            announcementID: context.announcementID,
            date: context.date.toISOString(),
            scheduleItem: context.scheduleItem,
        },
        success: function (response) {
            context.getUsersReservationsRequestResponse = response;
            requestSynchronizer.generator.next();
        }
    });
}

/**
 * Handles update reservation request.
 */
function updateReservationsRequest(context, requestSynchronizer) {
    $.ajax({
        url: "/Announcement/UpdateReservations",
        data: {
            announcementID: context.announcementID,
            userName: context.userName,
            date: context.date.toISOString(),
            newReservationsAmount: context.newReservationsAmount,
            scheduleItem: context.scheduleItem,
        },
        dataType: "json",
        method: "post",
        success: function (response) {
            context.updateReservationsRequestResponse = response;
            requestSynchronizer.generator.next();
        }
    });
}

function dailyTimetableInit(announcementID) {
    timetableContext.announcementID = announcementID;
    timetableContext.middleDate = new Date();
    reloadDailyTimetable();
    document.getElementById("details-items-container").hidden = true;
}

/**
 * Loads reservations data (reservations,
 * maxReservations, middleDate, timetable) into timetable context.
 */
function reloadDailyTimetable() {
    var context = {};
    context.announcementID = timetableContext.announcementID;
    context.date = timetableContext.middleDate;
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { getReservationsRequest(context, requestSynchronizer) },
        function () {
            // If error occurs.
            if (context.getReservationsRequestResponse == null) {
                setGlobalMessage(6);
                return;
            }
            // Provide context data.
            timetableContext.reservations = context.getReservationsRequestResponse.reservations;
            timetableContext.maxReservations = context.getReservationsRequestResponse.announcement.maxReservations;
            timetableContext.timetable = context.getReservationsRequestResponse.announcement.timetable;
            timetableContext.from = new Date(context.getReservationsRequestResponse.announcement.from);
            timetableContext.to = new Date(context.getReservationsRequestResponse.announcement.to);
            timetableContext.from.setHours(0, 0, 0, 0);
            timetableContext.to.setHours(0, 0, 0, 0);
            drawDailyTimetableContent();
        },
    ];
    requestSynchronizer.run();
}

function handlePreviousWeekButton() {
    var currentMiddleDate = timetableContext.middleDate;
    var previousMiddleDate = new Date(currentMiddleDate.getTime() - 24 * 60 * 60 * 1000 * 7);
    timetableContext.middleDate = previousMiddleDate;
    reloadDailyTimetable();
}

function handleNexWeekButton() {
    var currentMiddleDate = timetableContext.middleDate;
    var nextMiddleDate = new Date(currentMiddleDate.getTime() + 24 * 60 * 60 * 1000 * 7);
    timetableContext.middleDate = nextMiddleDate;
    reloadDailyTimetable();
}

var reservationsCountErrorSpanTimeout;
function handleReservationsUpdate(button) {
    /**
     * Sets message of field next to update button.
     * Message will be cleared after 5s.
     */
    function setErrorSpanMessage(message) {
        errorSpan.innerText = message;
        if (reservationsCountErrorSpanTimeout != null) {
            clearTimeout(reservationsCountErrorSpanTimeout);
        }
        reservationsCountErrorSpanTimeout = setTimeout(function () {
            errorSpan.innerText = "";
        }, 5000);
    }

    var detailItemContainer = button.parentNode.parentNode;
    var errorSpan = detailItemContainer.getElementsByTagName("span")[0];
    var inputField = detailItemContainer.getElementsByTagName("input")[0];
    var reservationsCount = parseInt(inputField.value);

    // Perform reservations count validation.
    if (isNaN(reservationsCount) || reservationsCount < 0) {
        // Set message and return.
        setErrorSpanMessage("Reservations count is invalid.");
        return;
    }

    // Handle update request.
    var context = {};
    context.announcementID = timetableContext.announcementID;
    context.userName = detailItemContainer.getElementsByClassName("details-item-username")[0].innerText;
    context.date = timetableContext.updatedReservationsDate;
    context.newReservationsAmount = reservationsCount;
    context.scheduleItem = null;
    // For announcement with schedule items get saved schedule item info.
    if (timetableContext.timetable == 2) {
        context.scheduleItem = timetableContext.scheduleItem;
    }

    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { updateReservationsRequest(context, requestSynchronizer) },
        function () {
            // Handle errors.
            if (context.updateReservationsRequestResponse == null) {
                setGlobalMessage(7);
                return;
            }
            if (context.updateReservationsRequestResponse == 0) {
                setErrorSpanMessage("Reservations amount remains the same.");
                return;
            }
            // Initialize reload.
            setGlobalMessage(8);
            // Chose timetable to reload.
            if (timetableContext.timetable == 1) {
                reloadDailyTimetable();
            }
            else if (timetableContext.timetable == 2) {
                reloadHourlyTimetable();
            }
            handleDetailsButton(timetableContext.dayNumber);
            return;
        }
    ];
    requestSynchronizer.run();
}

/**
 * Allows to display reservations done by users for specified
 * day or schedule item.
 */
function handleDetailsButton(itemNumber) {
    timetableContext.dayNumber = itemNumber;
    var timetableOption = timetableContext.timetable;
    switch (timetableOption) {
        case 1:     // Does not have schedule items.
            var context = {};
            context.announcementID = timetableContext.announcementID;
            context.date = new Date(document.getElementById("timetable-day-date-" + itemNumber).innerText);
            context.scheduleItem = null;
            var requestSynchronizer = new RequestSynchronizer();
            requestSynchronizer.requestQueue = [
                function () { getUsersReservationsRequest(context, requestSynchronizer) },
                function () {
                    // If error occurs.
                    if (context.getUsersReservationsRequestResponse == null) {
                        setGlobalMessage(6);
                        return;
                    }
                    timetableContext.usersReservations = context.getUsersReservationsRequestResponse;
                    timetableContext.updatedReservationsDate = context.date;
                    drawDetails();
                },
            ];
            requestSynchronizer.run();
            break;
        case 2:     // Does have schedule items.
            var context = {};
            context.announcementID = timetableContext.announcementID;
            context.date = timetableContext.middleDate;
            // For announcement with schedule items item itemNumber represents schedule item number.
            context.scheduleItem = timetableContext.scheduleItems[itemNumber];
            timetableContext.scheduleItem = context.scheduleItem;
            var requestSynchronizer = new RequestSynchronizer();
            requestSynchronizer.requestQueue = [
                function () { getUsersReservationsRequest(context, requestSynchronizer) },
                function () {
                    // If error occurs.
                    if (context.getUsersReservationsRequestResponse == null) {
                        setGlobalMessage(6);
                        return;
                    }
                    timetableContext.usersReservations = context.getUsersReservationsRequestResponse;
                    timetableContext.updatedReservationsDate = context.date;
                    drawDetails();
                },
            ];
            requestSynchronizer.run();
            break;
    }
}

/**
 * Fills details table with elements basing on acquired
 * users reservations data.
 */
function drawDetails() {
    var detailsMessageContainer = document.getElementById("details-message-container");
    var detailsItemsContainer = document.getElementById("details-items-container");
    var usersReservations = timetableContext.usersReservations;
    // Draw simple massage if users reservations is empty.
    if (usersReservations.length == 0) {
        detailsMessageContainer.innerText = "There are no reservations for selected item.";
        // Hide details items table if visible.
        detailsItemsContainer.hidden = true;
        return;
    }
    var prototype = document.getElementById("details-item-prototype");
    // Clear container before drawing.
    detailsItemsContainer.innerHTML = "";
    // Clear message.
    detailsMessageContainer.innerText = "";
    // Keep prototype.
    detailsItemsContainer.appendChild(prototype);

    // Add prototype clones
    for (var i = 0; i < usersReservations.length; i++) {
        // Setup prototype clone.
        var newNode = prototype.cloneNode(true);
        newNode.hidden = false;
        newNode.removeAttribute("id");
        // Fill new node with provided info.
        newNode.getElementsByClassName("details-item-username")[0].innerText = usersReservations[i].userData.userName;
        newNode.getElementsByClassName("details-item-first-name")[0].innerText = usersReservations[i].userData.firstName;
        newNode.getElementsByClassName("details-item-last-name")[0].innerText = usersReservations[i].userData.lastName;
        newNode.getElementsByTagName("input")[0].value = usersReservations[i].reservations;
        newNode.getElementsByTagName("button")[0].setAttribute("onclick", "handleReservationsUpdate(this);")
        // Add new node to container.
        detailsItemsContainer.appendChild(newNode);
    }
    // Make items container visible.
    detailsItemsContainer.hidden = false;
}

/**
 * Fills date containers and reservation containers inside 
 * timetable.
 */
function drawDailyTimetableContent() {
    var maxReservations = timetableContext.maxReservations;
    var middleDate = timetableContext.middleDate;
    var reservations = timetableContext.reservations;
    // Get date containers.
    var timetableDayDateContainers = [];
    for (var i = 0; i < 7; i++) {
        timetableDayDateContainers.push(document.getElementById("timetable-day-date-" + i));
    }
    // Get dates for containers.
    var timetableWeekDates = [];
    middleDate.setHours(0, 0, 0, 0);
    // First week day should be three days before middle date.
    timetableWeekDates[0] = new Date(middleDate.getTime() - 24 * 60 * 60 * 1000 * 3);
    for (var i = 1; i < 7; i++) {
        timetableWeekDates.push(new Date(timetableWeekDates[0].getTime() + 24 * 60 * 60 * 1000 * i));
    }
    // Fill date containers.
    for (var i = 0; i < timetableDayDateContainers.length; i++) {
        // Mark date if it is not in announcement active time range.
        if (timetableWeekDates[i].getTime() < timetableContext.from.getTime() ||
            timetableWeekDates[i].getTime() > timetableContext.to.getTime()) {
            timetableDayDateContainers[i].setAttribute("style", "background-color:lightgray;");
        }
        else {
            timetableDayDateContainers[i].removeAttribute("style");
        }
        timetableDayDateContainers[i].innerText = timetableWeekDates[i].toLocaleDateString("en-US");
    }
    // Get reservations containers.
    var reservationsContainers = [];
    for (var i = 0; i < 7; i++) {
        reservationsContainers.push(document.getElementById("timetable-reservations-" + i));
    }
    // Fill reservations containers.
    for (var i = 0; i < reservationsContainers.length; i++) {
        var reservationsString = "0/" + maxReservations;
        if (reservations[i] != null) {
            reservationsString = reservations[i] + "/" + maxReservations;
        }
        reservationsContainers[i].innerText = reservationsString;
    }
}