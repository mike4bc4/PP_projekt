var mainContainerName = 'list-ann-main-container';
var subTableClassName = 'sub-table';
var subTableHeaderCellClassName = 'sub-table-header-cell';
var mainTableClassName = 'main-table';
var subTableRowClassName = 'sub-table-row';
var subTableRowSelectedClassName = 'sub-table-row-selected';
var weekTimetableTableClassName = 'week-timetable-table';
var weekTimetableTableCellClassName = 'week-timetable-cell';
var weekTimetableTableCellHighlightClassName = 'week-timetable-cell-highlight';
var weekTimetableTableCellDisabledClassName = 'week-timetable-cell-disabled';
var weekTimetableTableHeaderCellClassName = 'week-timetable-header-cell';
var announcementListNavBarId = 'list-ann-nav-bar';
var announcementListSearchBar = 'list-ann-search-bar';
var navigationButtonClassName = 'nav-button';



function drawAnnouncementsList(announcements) {

	// Clear container content.
	//container.innerHTML = '';
	// Create table body.

	document.getElementById(mainContainerName).innerHTML = '<table id="main-table" class="' + mainTableClassName + '"></table>';
	document.getElementById('main-table').innerHTML = '<tr><td id="sub-table-left-container"></td><td id="sub-table-right-container"></td></tr>';
	document.getElementById('sub-table-left-container').innerHTML = '<table class="' + subTableClassName + '" id="sub-table-left"></table>';
	document.getElementById('sub-table-right-container').innerHTML = '<table class="' + subTableClassName + '" id="sub-table-right"></table>';
	$('#sub-table-left').append('<tr>' +
		'<td class="' + subTableHeaderCellClassName + '">Type</td>' +
		'<td class="' + subTableHeaderCellClassName + '">Subtype</td>' +
		'<td class="' + subTableHeaderCellClassName + '">Additional options</td>' +
		'<td class="' + subTableHeaderCellClassName + '">From</td>' +
		'<td class="' + subTableHeaderCellClassName + '">To</td>' +
		'<td class="' + subTableHeaderCellClassName + '">Active</td>' +
		'</tr>');
	$('#sub-table-right').append('<tr>' +
		'<td class="' + subTableHeaderCellClassName + '">&nbsp</td>' +
		'<td class="' + subTableHeaderCellClassName + '">&nbsp</td>' +
		'</tr>')
	var index = 0;
	for (var announcement of announcements) {
		var announcementSubtype = '';
		var announcementSharedPart = '';
		switch (announcement.type) {
			case 0:	// House typ
				announcementSubtype = getHouseSubtypes()[announcement.type];
				announcementSharedPart = getHouseSharedPart()[announcement.type];
				break;
			case 1:	// Entertainment type
				announcementSubtype = getEntertainmentSubtypes()[announcement.type];
				break;
			case 2:	// Food type
				announcementSubtype = getFoodSubtypes()[announcement.type];
				break;
		}
		var active = '';
		if (!announcement.isActive || isOutOfDateRange(announcement)) {
			active = 'No';
		}
		else {
			active = 'Yes';
		}
		var rowClass;
		if (clickedAnnouncementRowIndex(index) != null) {
			rowClass = subTableRowSelectedClassName;
		}
		else {
			rowClass = subTableRowClassName;
		}

		$('#sub-table-left').append('<tr class="' + rowClass + '" onClick="toggleAnnouncementSelection(' + index + '); redraw();">' +
			'<td class="' + subTableHeaderCellClassName + '">' + getTypes()[announcement.type] + '</td>' +
			'<td class="' + subTableHeaderCellClassName + '">' + announcementSubtype + '</td>' +
			'<td class="' + subTableHeaderCellClassName + '">' + announcementSharedPart + '</td>' +
			'<td class="' + subTableHeaderCellClassName + '">' + announcement.from.split('T')[0] + '</td>' +
			'<td class="' + subTableHeaderCellClassName + '">' + announcement.to.split('T')[0] + '</td>' +
			'<td class="' + subTableHeaderCellClassName + '">' + active + '</td>' +
			'</tr>');
		var todayDate = new Date();
		todayDate.setHours(0, 0, 0, 0);
		todayDate = todayDate.toLocaleDateString('en-US');
		$('#sub-table-right').append('<tr>' +
			'<td class="' + subTableHeaderCellClassName + '"><a href="/Announcement/EditAnnouncement/?newModel=false" onClick="editAnnouncement(' + index + ');">Edit announcement</a></td>' +
			'<td class="' + subTableHeaderCellClassName + '"><a href="#" onClick="getReservations(' + announcement.id + ',\'' + todayDate + '\');">Display timetable</a></td>' +
			'</tr>');
		index++;

	}
}

function drawTimetable(reservations, announcement, scheduleItems, date) {
	// Parse short string date to date format.
	var dateArray = date.split('/');
	var currentDate = new Date();
	currentDate.setMonth(dateArray[0] - 1);
	currentDate.setDate(dateArray[1]);
	currentDate.setFullYear(dateArray[2]);
	currentDate.setHours(0, 0, 0, 0);
	// Setup date time display options.
	var dateOptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
	// Remove listing navigation bar.
	document.getElementById(announcementListNavBarId).innerHTML = '';
	// Remove listing search and sor bar.
	document.getElementById(announcementListSearchBar).innerHTML = '';

	switch (announcement.timetable) {
		case 1:	// Per day timetable
			// Insert tables.
			document.getElementById(mainContainerName).innerHTML = '<table class="' + weekTimetableTableClassName + '" id="week-timetable-table"></table>';
			document.getElementById('week-timetable-table').innerHTML = '<tr id="week-table-date-row"></tr><tr id="week-table-content-row"></tr>'
			// Add left navigation button.
			$('#week-table-date-row').append('<td rowspan="2"><button class="' + navigationButtonClassName + '">left arrow</button></td>');
			// Fill table body.
			for (var index = 0; index < 7; index++) {
				var res = '0';
				var dt = new Date();
				dt.setDate(currentDate.getDate() - 3 + index);
				dt.setHours(0, 0, 0, 0);
				if (reservations[index] != null) {
					res = reservations[index];
				}
				var from = new Date(announcement.from);
				from.setHours(0, 0, 0, 0);
				var to = new Date(announcement.to);
				to.setHours(0, 0, 0, 0);

				if (dt < from || dt > to) {
					$('#week-table-date-row').append('<td class="' + weekTimetableTableCellDisabledClassName + '">' + dt.toLocaleDateString('en-US', dateOptions) + '</td>');
					$('#week-table-content-row').append('<td class="' + weekTimetableTableCellDisabledClassName + '">&nbsp</td>');
				}
				else {
					$('#week-table-date-row').append('<td class="' + weekTimetableTableHeaderCellClassName + '">' + dt.toLocaleDateString('en-US', dateOptions) + '</td>');
					$('#week-table-content-row').append('<td class="' + weekTimetableTableCellClassName + '" onClick="">Reservations ' + res + '/' + announcement.maxReservations + '</td>');
				}
			}
			// Add right navigation button.
			$('#week-table-date-row').append('<td rowspan="2"><button class="' + navigationButtonClassName + '">right arrow</button></td>');
			break;
		case 2:	// Per hour timetable
			break;
	}

}

function getReservations(announcementID, date) {
	$.ajax({
		url: '/Announcement/GetReservations',
		data: { announcementID: announcementID, date: date },
		dataType: 'json',
		method: 'post',
		success: function (response) {
			drawTimetable(response.reservations, response.announcement, response.scheduleItems, date);
		}
	})
}

function getAnnouncements() {
	return JSON.parse(sessionStorage.getItem('userAnnouncements'));

}

function updateSessionAnnouncements(announcements) {
	sessionStorage.setItem('userAnnouncements', JSON.stringify(announcements));
}

function isOutOfDateRange(announcement) {
	var today = new Date();
	today.setHours(0, 0, 0, 0);
	var fromAsDate = new Date(announcement.from.replace(/T/g, ' '));
	var toAsDate = new Date(announcement.to.replace(/T/g, ' '));
	return (fromAsDate > today || toAsDate < today);
}

function getAnnouncementSelectedList() {
	var announcementSelectedList = JSON.parse(sessionStorage.getItem('announcementSelectedList'));
	if (!announcementSelectedList) {
		announcementSelectedList = [];
		sessionStorage.setItem('announcementSelectedList', JSON.stringify(announcementSelectedList));
	}
	return announcementSelectedList;
}

function clearAnnouncementSelectedList() {
	sessionStorage.setItem('announcementSelectedList', null);
}


function toggleAnnouncementSelection(announcementIndex) {
	var announcementSelectedList = getAnnouncementSelectedList();
	var selectedIndex = clickedAnnouncementRowIndex(announcementIndex);

	if (selectedIndex == null) {
		// Clicked announcement index does not exist in selected list.
		announcementSelectedList.push(announcementIndex);
	}
	else {
		// Remove selected item record.
		announcementSelectedList.splice(selectedIndex, 1);
	}
	// Save changes
	sessionStorage.setItem('announcementSelectedList', JSON.stringify(announcementSelectedList));
}

/**
 * Finds array index of provided announcement index.
 * @param {any} announcementIndex
 */
function clickedAnnouncementRowIndex(announcementIndex) {
	var announcementSelectedList = getAnnouncementSelectedList();
	var selectedIndex = null;

	// Find record with clicked announcement index.
	for (var i = 0; i < announcementSelectedList.length; i++) {
		if (announcementSelectedList[i] == announcementIndex) {
			selectedIndex = i;
			break;
		}
	}

	return selectedIndex;
}


function activateSelectedAnnouncements() {
	var announcementSelectedList = getAnnouncementSelectedList();
	var announcements = getAnnouncements();
	var selectedAnnouncementsIDs = [];
	var messageCode = 1;

	if (announcementSelectedList.length == 0) {
		setAnnouncementManagementMessage(2);
		return;
	}

	for (var i = 0; i < announcementSelectedList.length; i++) {
		if (isOutOfDateRange(announcements[announcementSelectedList[i]])) {
			// Is inactive because of time and cannot be activated without time change;
			messageCode = 0;
		}
		else {
			selectedAnnouncementsIDs.push(announcements[announcementSelectedList[i]].id);
			announcements[announcementSelectedList[i]].isActive = true;
		}
	}
	setAnnouncementManagementMessage(messageCode);

	// Perform update in database.
	$.ajax({
		url: '/Announcement/ChangeAnnouncementsStatus',
		method: 'post',
		data: { announcementsIDs: selectedAnnouncementsIDs, areActive: true },
		dataType: 'json'
	})

	// Update local announcements list for proper display.
	updateSessionAnnouncements(announcements);

}


function deactivateSelectedAnnouncements() {
	var announcementSelectedList = getAnnouncementSelectedList();
	var selectedAnnouncementsIDs = [];
	var announcements = getAnnouncements();

	if (announcementSelectedList.length == 0) {
		setAnnouncementManagementMessage(2);
		return;
	}

	for (var i = 0; i < announcementSelectedList.length; i++) {
		selectedAnnouncementsIDs.push(announcements[announcementSelectedList[i]].id);
		announcements[announcementSelectedList[i]].isActive = false;
	}
	setAnnouncementManagementMessage(3);

	// Perform update in database.
	$.ajax({
		url: '/Announcement/ChangeAnnouncementsStatus',
		method: 'post',
		data: { announcementsIDs: selectedAnnouncementsIDs, areActive: false },
		dataType: 'json'
	})

	// Update local announcements list for proper display.
	updateSessionAnnouncements(announcements);
}


function removeSelectedAnnouncements() {
	var announcementSelectedList = getAnnouncementSelectedList();
	var selectedAnnouncementsIDs = [];
	var announcements = getAnnouncements();

	if (announcementSelectedList.length == 0) {
		setAnnouncementManagementMessage(2);
		return;
	}

	for (var i = 0; i < announcementSelectedList.length; i++) {
		selectedAnnouncementsIDs.push(announcements[announcementSelectedList[i]].id);
		announcements[announcementSelectedList[i]] = null;
	}

	// All selected rows has been removed, reset selection list.
	clearAnnouncementSelectedList();

	setAnnouncementManagementMessage(4);

	// Perform update in database.
	$.ajax({
		url: '/Announcement/ChangeAnnouncementsStatus',
		method: 'post',
		data: { announcementsIDs: selectedAnnouncementsIDs },
		dataType: 'json'
	})

	// Update local announcements list for proper display.
	updateSessionAnnouncements(announcements);

}


function editAnnouncement(announcementIndex) {
	announcement = getAnnouncements()[announcementIndex];
	sessionStorage.setItem('model', JSON.stringify(announcement));
}

function setAnnouncementManagementMessage(messageCode) {
	var messageTag = document.getElementById('announcementManagementMessage');

	switch (messageCode) {
		case 0:
			messageTag.innerText = 'Some announcements cannot be activated because of improper activation date range.';
			break;
		case 1:
			messageTag.innerText = 'Selected announcements have been marked as active.';
			break;
		case 2:
			messageTag.innerText = 'Please select at least one announcement.';
			break;
		case 3:
			messageTag.innerText = 'Selected announcements have been marked as inactive.';
			break;
		case 4:
			messageTag.innerText = 'Selected announcements have been removed.';
			break;
		default:
			messageTag.innerText = "";
			break;
	}
}




