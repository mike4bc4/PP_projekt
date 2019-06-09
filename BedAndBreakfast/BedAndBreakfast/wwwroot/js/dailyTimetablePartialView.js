
/**
 * Allows to acquire reservations for specified week or schedule item.
 * On success context will contain object {reservations, announcement, scheduleItems}.
 */
function getReservations(context, requestSynchronizer) {
    $.ajax({
        url: "/Announcement/GetReservations",
        data: { announcementID: context.announcementID, date: context.date.toISOString() },
        dataType: "json",
        method: "post",
        success: function (response) {
            context.getReservationsResponse = response;
            requestSynchronizer.generator.next();
        }
    });
}

function dailyTimetableInit(announcementID) {
    var context = {};
    context.date = new Date();
    context.announcementID = announcementID;
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () { getReservations(context, requestSynchronizer) },
        function () {
            fillDailyTimetable(context.getReservationsResponse.reservations);
        },
    ];
    requestSynchronizer.run();
}

function fillDailyTimetable(reservations) {
    /**
     * Returns array of table cells that should contain date.
     */
    function getDateContainers() {
        var containers = [];
        for (var i = 0; i < 7; i++) {
            containers.push(document.getElementById("timetable-day-" + i));
        }
        return containers;
    }
    /**
     * Returns array of table cells that should contain amount of reservations
     * and details button.
     */
    function getReservationContainers() {
        var containers = [];
        for (var i = 0; i < 7; i++) {
            containers.push("timetable-day-" + i + "-reservations");
        }
        return containers;
    }

    /**
     * Returns array of date strings where 4th element is provided middle date.
     */
    function getDateStrings(middleDate) {
        middleDate.setHours(0, 0, 0, 0);
        var firstDayOfSequence = new Date(middleDate.getTime() - (24 * 60 * 60 * 1000 * 3));
        var dateSequence = [];
        for (var i = 0; i < 7; i++) {
            dateSequence.push(new Date(firstDayOfSequence.getTime() + 24 * 60 * 60 * 1000 * i));
        }
        var dateStrings = [];
        for (var date of dateSequence) {
            dateStrings.push(date.toLocaleDateString("en-US"));
        }
        return dateStrings;
    }
    
}