var mainContainerName = 'list-ann-main-container';
var subTableClassName = 'sub-table';
var subTableHeaderCellClassName = 'sub-table-header-cell';
var mainTableClassName = 'main-table';
var subTableRowClassName = 'sub-table-row';
var subTableRowSelectedClassName = 'sub-table-row-selected';
var weekTimetableTableClassName = 'week-timetable-table';
var weekTimetableCellClassName = 'week-timetable-cell';
var weekTimetableCellHighlightClassName = 'week-timetable-cell-highlight';
var weekTimetableCellDisabledClassName = 'week-timetable-cell-disabled';
var weekTimetableHeaderCellClassName = 'week-timetable-header-cell';
var announcementListNavBarId = 'list-ann-nav-bar';
var announcementListSearchBarId = 'list-ann-search-bar';
var navigationButtonClassName = 'nav-button';
var dayTimetableTableClassName = 'day-timetable-table';
var dayTimetableCellClassName = 'day-timetable-cell';
var dayTimetableHeaderCellClassName = 'day-timetable-header-cell';
var dayTimetableCellDisabledClassName = 'day-timetable-cell-disabled';
var dayTimetableHeaderCellHighlightClassName = 'day-timetable-header-cell-highlight';
var usersReservationsListTableClassName = 'main-table';
var usersReservationsListTableCellClassName = 'day-timetable-header-cell';


function drawAnnouncementsList(announcements) {
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
			case 0:	// House type
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
		var displayTimetableContent = 'Timetable off';
		if (announcement.timetable != 0) {
			displayTimetableContent = '<a href="#" onClick="getReservations(' + announcement.id + ',\'' + todayDate + '\');">Display timetable</a>';
		}
		$('#sub-table-right').append('<tr>' +
			'<td class="' + subTableHeaderCellClassName + '"><a href="/Announcement/EditAnnouncement/?newModel=false" onClick="editAnnouncement(' + index + ');">Edit announcement</a></td>' +
			'<td class="' + subTableHeaderCellClassName + '">' + displayTimetableContent + '</td>' +
			'</tr>');
		index++;

	}
}

function drawTimetable(reservations, announcement, scheduleItems, date) {
	// Remove navigation bars.
	document.getElementById(announcementListNavBarId).innerHTML = '';
	document.getElementById(announcementListSearchBarId).innerHTML = '';
	// Draw timetable.
	var middleDate = new Date(date);
	middleDate.setHours(0, 0, 0, 0);
	var today = new Date();
	today.setHours(0, 0, 0, 0);
	var from = new Date(announcement.from);
	from.setHours(0, 0, 0, 0);
	var to = new Date(announcement.to);
	to.setHours(0, 0, 0, 0);

	switch (announcement.timetable) {
		case 1:	// Per day timetable
			var d1 = middleDate.getTime() - 1000 * 60 * 60 * 24 * 7;
			var d2 = middleDate.getTime() + 1000 * 60 * 60 * 24 * 7;
			var previousWeekMiddleDate = new Date();
			var nextWeekMiddleDate = new Date();
			previousWeekMiddleDate.setTime(d1);
			nextWeekMiddleDate.setTime(d2);
			var previousTableTag = document.getElementById('week-timetable-table');
			if (previousTableTag == null) {
				document.getElementById(mainContainerName).innerHTML = '<table class="' + weekTimetableTableClassName + '" id="week-timetable-table"></table>';
			}

			document.getElementById('week-timetable-table').innerHTML = '<tr id="week-table-date-row"></tr><tr id="week-table-content-row"></tr>'
			$('#week-table-date-row').append('<td rowspan="2"><button onclick="getReservations(' + announcement.id + ',\'' + previousWeekMiddleDate.toLocaleDateString('en-US') + '\');" class="' + navigationButtonClassName + '">left arrow</button></td>');

			for (var index = 0; index < 7; index++) {
				var timetableDay = new Date();
				var d3 = middleDate.getTime() - 1000 * 60 * 60 * 24 * 3 + 1000 * 60 * 60 * 24 * index;
				timetableDay.setTime(d3);
				var headerCellClass = weekTimetableHeaderCellClassName;
				var rowCellClass = weekTimetableCellClassName;
				if (timetableDay.getTime() == today.getTime()) {	// Highlight current day.
					headerCellClass = weekTimetableCellHighlightClassName;
				}
				if (timetableDay < from || timetableDay > to) {		// Mark days out of announcement active time range.
					rowCellClass = weekTimetableCellDisabledClassName;
				}

				$('#week-table-date-row').append('<td class="' + headerCellClass + '">' + timetableDay.toLocaleDateString('en-US') + '</td>');
				$('#week-table-content-row').append('<td onClick="getUsersReservations(' + announcement.id + ',\'' +
					timetableDay.toLocaleDateString('en-US') + '\',' + null + ');" class="' + rowCellClass + '" >Reservations ' + reservations[index] + '/' + announcement.maxReservations + '</td>');
			}
			$('#week-table-date-row').append('<td rowspan="2"><button onclick="getReservations(' + announcement.id + ',\'' + nextWeekMiddleDate.toLocaleDateString('en-US') + '\');" class="' + navigationButtonClassName + '">right arrow</button></td>');
			break;
		case 2:	// Per hour timetable
			var d1 = middleDate.getTime() - 1000 * 60 * 60 * 24 * 1;
			var d2 = middleDate.getTime() + 1000 * 60 * 60 * 24 * 1;
			var previousDayMiddleDate = new Date();
			var nextDayMiddleDate = new Date();
			previousDayMiddleDate.setTime(d1);
			nextDayMiddleDate.setTime(d2);

			var headerCellClass = dayTimetableHeaderCellClassName;
			var scheduleItemCellClass = dayTimetableCellClassName;
			if (middleDate.getTime() == today.getTime()) {
				headerCellClass = dayTimetableHeaderCellHighlightClassName;
			}
			if (middleDate < from || middleDate > to) {
				scheduleItemCellClass = dayTimetableCellDisabledClassName;
			}
			var previousTableTag = document.getElementById('day-timetable-table');
			if (previousTableTag == null) {
				document.getElementById(mainContainerName).innerHTML = '<table class="' + dayTimetableTableClassName + '" id="day-timetable-table"></table>';
			}
			//document.getElementById(mainContainerName).innerHTML = '<table class="' + dayTimetableTableClassName + '" id="day-timetable-table"></table>';
			document.getElementById('day-timetable-table').innerHTML = '<tr id="day-timetable-header"></tr>';
			$('#day-timetable-header').append('<td rowspan="' + (scheduleItems.length + 1) + '">' +
				'<button onclick="getReservations(' + announcement.id + ',\'' + previousDayMiddleDate.toLocaleDateString('en-US') + '\');" class="' + navigationButtonClassName + '">left arrow</button></td>' +
				'<td class="' + headerCellClass + '">' + middleDate.toLocaleDateString('en-US') + '</td>' +
				'<td rowspan="' + (scheduleItems.length + 1) + '">' +
				'<button onclick="getReservations(' + announcement.id + ',\'' + nextDayMiddleDate.toLocaleDateString('en-US') + '\');" class="' + navigationButtonClassName + '">right arrow</button></td>');

			// Add schedule items with reservations.
			var index = 0;
			for (var item of scheduleItems) {
				$('#day-timetable-table').append('<tr onclick="getUsersReservations(' + announcement.id + ',\'' + middleDate.toLocaleDateString('en-US') + '\',' +
					'{from: ' + item.from + ', to: ' + item.to + ', maxReservations: ' + item.maxReservations + '}' + ');" class="' + dayTimetableCellClassName + '">' +
					'<td class="' + scheduleItemCellClass + '">' + (item.from.toString() + ':00-') + (item.to.toString() + ':00') +
					' Reservations ' + reservations[index] + '/' + item.maxReservations + '</td></tr>');
				index++;
			}
			break;
	}
}

function drawUsersReservationsList(reservationsPerUser, announcementID, date, scheduleItem) {
	// Clear previous table if present.
	var usersReservationsTableTag = document.getElementById('res-per-usr-lst');
	if (usersReservationsTableTag != null) {
		usersReservationsTableTag.remove();
	}
	if (reservationsPerUser.length == 0) {
		$('#' + mainContainerName).append('<div id="res-per-usr-lst">There are no reservations yet!</div>');
		return;
	}
	// Draw new table.
	$('#' + mainContainerName).append('<table id="res-per-usr-lst" class="' + usersReservationsListTableClassName + '"></table>');
	$('#res-per-usr-lst').append('<tr>' +
		'<td class="' + usersReservationsListTableCellClassName + '">User name</td>' +
		'<td class="' + usersReservationsListTableCellClassName + '">First name</td>' +
		'<td class="' + usersReservationsListTableCellClassName + '">Last name</td>' +
		'<td class="' + usersReservationsListTableCellClassName + '">Reservations</td>' +
		'</tr>');
	var scheduleItemAsString = 'null';
	if (scheduleItem != null) {
		scheduleItemAsString = '{from:' + scheduleItem.from + ', to:' + scheduleItem.to + ', maxReservations:' + scheduleItem.maxReservations + '}';
	}

	var index = 0;
	for (var item of reservationsPerUser) {
		$('#res-per-usr-lst').append('<tr>' +
			'<td class="' + usersReservationsListTableCellClassName + '">' + item.userData.userName + '</td>' +
			'<td class="' + usersReservationsListTableCellClassName + '">' + item.userData.firstName + '</td>' +
			'<td class="' + usersReservationsListTableCellClassName + '">' + item.userData.lastName + '</td>' +
			'<td class="' + usersReservationsListTableCellClassName + '">' +
			'<input id="res-per-usr-lst-in-fld-' + index + '" type="text" value="' + item.reservations + '"  size="5" maxlength="5" />' +
			'<button ' +
			' onclick="updateReservations(' + announcementID + ',\'' + item.userData.userName + '\',\'' + date + '\',\'res-per-usr-lst-in-fld-' + index + '\',' + scheduleItemAsString + ');"' +
			'>Update</button>' +
			'</td>' +
			'</tr>');
		index++;
	}
}

function getReservations(announcementID, date) {
	$.ajax({
		url: '/Announcement/GetReservations',
		data: { announcementID: announcementID, date: date },
		dataType: 'json',
		method: 'post',
		success: function (response) {
			// Response will be null if announcement with specified id cannot be found (database error).
			if (response != null) {
				drawTimetable(response.reservations, response.announcement, response.scheduleItems, date);
			}
			else {
				setAnnouncementManagementMessage(5);
			}
		}
	});
}

function getUsersReservations(announcementID, date, scheduleItem) {
	$.ajax({
		url: '/Announcement/GetUsersReservations',
		data: { announcementID, date, scheduleItem },
		dataType: 'json',
		method: 'post',
		success: function (response) {
			if (response != null) {
				drawUsersReservationsList(response.reservationsPerUser, announcementID, date, scheduleItem);
			}
			else {
				setAnnouncementManagementMessage(5);
			}
		}
	});
}

function updateReservations(announcementID, userName, date, inputTagID, scheduleItem) {
	var newReservationsAmount = parseInt(document.getElementById(inputTagID).value);
	$.ajax({
		url: '/Announcement/UpdateReservations',
		data: { announcementID, userName, date, newReservationsAmount, scheduleItem },
		dataType: 'json',
		method: 'post',
		success: function (response) {
			if (response != null) {
				getReservations(announcementID, date);
				//getUsersReservations(announcementID, date, scheduleItem);
			}
			else {
				setAnnouncementManagementMessage(6);
			}

		}
	});
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

var messageTimeout;
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
		case 5:
			messageTag.innerText = 'An error occurred while browsing announcement data.'
			break;
		case 6:
			messageTag.innerText = 'An error occurred while updating reservation amount.'
			break;
		case 7:
			messageTag.innerText = 'Reservations updated successfully.';
			break;
		default:
			messageTag.innerText = "";
			break;
	}
	// Message will be visible for 5 seconds.
    if (messageTimeout != null) {
        window.clearTimeout(messageTimeout);
    }
    messageTimeout = window.setTimeout(function () {
        document.getElementById('announcementManagementMessage').innerText = '';
    }, 5000);
}




