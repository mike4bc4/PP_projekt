// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var announcementSearchFieldId = 'ann-browser-qry-in-fld';

function setAnnouncementSearchField(){
    var query = sessionStorage.getItem('announcementSearchQuery');
    if(query != null){
        //document.getElementById(announcementSearchFieldId).value = query;
    }
}

class RequestSynchronizer {
    constructor() {
        this.generator;
        this.requestQueue = [];
    }
    run() {
        var requestQueue = this.requestQueue;
        function* runRequestsGenerator() {
            for (var i = 0; i < requestQueue.length; i++) {
                yield requestQueue[i]();
            }
        }
        this.generator = runRequestsGenerator();
        this.generator.next();
    }
}

function getCurrentUserName(context, requestSynchronizer) {
    $.ajax({
        url: '/Account/GetCurrentUserName',
        dataType: 'json',
        method: 'post',
        success: function (response) {
            if (context.userNames == null) {
                context.userNames = [];
            }
            context.userNames.push(response);
            requestSynchronizer.generator.next();
        }
    });
}

function getAnnouncementOwnerUserName(context, requestSynchronizer) {
    $.ajax({
        url: '/Announcement/GetAnnouncementOwnerUserName',
        data: { announcementID: context.announcementID },
        dataType: 'json',
        method: 'post',
        success: function (response) {
            if (context.userNames == null) {
                context.userNames = [];
            }
            context.userNames.push(response);
            requestSynchronizer.generator.next();
        }
    });
}

function getScheduleItem(context, requestSynchronizer){
    $.ajax({
        url: '/Announcement/GetScheduleItem',
        dataType: 'json',
        method: 'post',
        data: {scheduleItemID: context.scheduleItemID},
        success: function(response){
            context.scheduleItem = response; 
            requestSynchronizer.generator.next();
        }
    });
}

function isUserInAdminRole(context, requestSynchronizer){
    $.ajax({
        url: "/Validation/IsUserInAdminRole",
        data: "json",
        method: "post",
        success: function(response){
            context.isUserInAdminRole = response;
            requestSynchronizer.generator.next(); 
        }
    });
}