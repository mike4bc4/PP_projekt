// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var announcementSearchFieldId = 'ann-browser-qry-in-fld';

function setAnnouncementSearchField(){
    var query = sessionStorage.getItem('announcementSearchQuery');
    if(query != null){
        document.getElementById(announcementSearchFieldId).value = query;
    }
}