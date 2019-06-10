
function hourlyTimetableInit(announcementID) {

    timetableContext.announcementID = announcementID;
    timetableContext.middleDate = new Date();
    timetableContext.middleDate.setHours(0, 0, 0, 0);
    reloadHourlyTimetable();
    document.getElementById("details-items-container").hidden = true;
}

function handlePreviousDayButton() {
    var previousMiddleDate = new Date(timetableContext.middleDate.getTime() - 24 * 60 * 60 * 1000);
    timetableContext.middleDate = previousMiddleDate;
    reloadHourlyTimetable();
}

function handleNextDayButton() {
    var nextMiddleDate = new Date(timetableContext.middleDate.getTime() + 24 * 60 * 60 * 1000);
    timetableContext.middleDate = nextMiddleDate;
    reloadHourlyTimetable();
}

function reloadHourlyTimetable() {
    // Call request.
    var context = {};
    context.announcementID = timetableContext.announcementID;
    context.date = timetableContext.middleDate;
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { getReservationsRequest(context, requestSynchronizer) },
        function () {
            if (context.getReservationsRequestResponse == null) {
                setGlobalMessage(6);
                return;
            }
            timetableContext.reservations = context.getReservationsRequestResponse.reservations;
            timetableContext.scheduleItems = context.getReservationsRequestResponse.scheduleItems;
            timetableContext.timetable = context.getReservationsRequestResponse.announcement.timetable;
            timetableContext.from = new Date(context.getReservationsRequestResponse.announcement.from);
            timetableContext.to = new Date(context.getReservationsRequestResponse.announcement.to);
            timetableContext.from.setHours(0, 0, 0, 0);
            timetableContext.to.setHours(0, 0, 0, 0);
            draw();
            return;
        },
    ];
    requestSynchronizer.run();

    function draw() {
        // Draw day date.
        var timetableDayContainer = document.getElementById("hourly-timetable-day-container");
        timetableDayContainer.innerText = timetableContext.middleDate.toLocaleDateString("en-US");
        // Mark day date if outside announcement active time range.
        if (timetableContext.middleDate.getTime() < timetableContext.from.getTime() ||
            timetableContext.middleDate.getTime() > timetableContext.to.getTime()) {
            timetableDayContainer.setAttribute("style", "background-color:lightgray;");
        }
        else {
            timetableDayContainer.removeAttribute("style");
        }

        var scheduleItemsContainer = document.getElementById("hourly-timetable-schedule-item-container");
        var scheduleItemPrototype = document.getElementById("hourly-timetable-schedule-item-prototype");
        // Clear schedule items container before drawing.
        scheduleItemsContainer.innerHTML = "";
        // Keep prototype.
        scheduleItemsContainer.appendChild(scheduleItemPrototype);
        var scheduleItems = timetableContext.scheduleItems;
        for (var i = 0; i < scheduleItems.length; i++) {
            // Create prototype clone.
            var newNode = scheduleItemPrototype.cloneNode(true);
            newNode.removeAttribute("id");
            newNode.hidden = false;
            // Prepare reservations string.
            var reservationsString = "0/" + scheduleItems[i].maxReservations;
            if (timetableContext.reservations[i] != null) {
                reservationsString = timetableContext.reservations[i] + "/" + scheduleItems[i].maxReservations;
            }
            // Prepare to date string.
            var toDateString = scheduleItems[i].to + ":00";
            if (scheduleItems[i].to == 24) {
                toDateString = "23:59";
            }
            // Fill new node with data.
            newNode.getElementsByClassName("hourly-timetable-schedule-item-from")[0].innerText = scheduleItems[i].from + ":00";
            newNode.getElementsByClassName("hourly-timetable-schedule-item-to")[0].innerText = toDateString;
            newNode.getElementsByClassName("hourly-timetable-schedule-item-reservations")[0].innerText = reservationsString;
            newNode.getElementsByTagName("button")[0].setAttribute("onclick", "handleDetailsButton(" + i + ");");
            // Append new node.
            scheduleItemsContainer.appendChild(newNode);
        }
    }
}



