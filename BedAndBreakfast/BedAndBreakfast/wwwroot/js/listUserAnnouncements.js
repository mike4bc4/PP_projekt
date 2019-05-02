function drawAnnouncementsList(announcements) {

	// Clear container content.
	var container = document.getElementById('usrAnnPageContainer');
	container.innerHTML = '';
	// Create table body.
	$('#usrAnnPageContainer').append('<table class="' + tableClass + '">' +
		'<thead id="usrAnnTabHeader"></thead><tbody id="usrAnnTabBody"></tbody></table>');

	// Create edit buttons table
	$('#usrAnnPageContainer').append('<table class="' + editButtonsTableClass + '"><thead><th>&nbsp</th></thead>' +
		'<tbody id="usrAnnEditButtonsBody"></tbody></table>');

	// Set table header.
	$('#usrAnnTabHeader').append('<tr><th class="' + cellClass + '">Type</th><th class="' + cellClass + '">Subtype</th><th class="' + cellClass + '">Additional option</th>' +
		'<th class="' + cellClass + '">From</th><th class="' + cellClass + '">To</th><th class="' + cellClass + '">Active</th></tr>');


	// Set table content.
	for (var i = 0; i < announcements.length; i++) {
		if (announcements[i] != null) {
			// Select row class - based on selection array.
			var rowClass;
			if (clickedAnnouncementRowIndex(i) != null) {
				rowClass = rowSelectedClass;
			}
			else {
				rowClass = rowDefaultClass;
			}

			$('#usrAnnTabBody').append('<tr id="usrAnnRow' + i + '" class="' + rowClass + '" onClick="toggleAnnouncementSelection(' + i + '); redraw();"></tr>');

			var subtypeName = '';
			var sharedPartName = '';
			switch (announcements[i].type) {
				case 0:
					subtypeName = Object.keys(getHouseSubtypes())
						.find(key => getHouseSubtypes()[key] === announcements[i].subtype);
					sharedPartName = Object.keys(getHouseSharedPart())
						.find(key => getHouseSharedPart()[key] === announcements[i].s);
					break;
				case 1:
					subtypeName = Object.keys(getEntertainmentSubtypes())
						.find(key => getEntertainmentSubtypes()[key] === announcements[i].subtype);
					break;
				case 2:
					subtypeName = Object.keys(getFoodSubtypes())
						.find(key => getFoodSubtypes()[key] === announcements[i].subtype);
					break;
			}


			$('#usrAnnRow' + i).append(
				'<td class="' + cellClass + '">' + Object.keys(getTypes()).find(key => getTypes()[key] === announcements[i].type) + '</td>' +
				'<td class="' + cellClass + '">' + subtypeName + '</td>' +
				'<td class="' + cellClass + '">' + sharedPartName + '</td>' +
				'<td class="' + cellClass + '">' + announcements[i].from.substr(0, 10) + '</td>' +
				'<td class="' + cellClass + '">' + announcements[i].to.substr(0, 10) + '</td>');

			// Date is retrieved as string so it has to be parsed into date time.
			// In retrieved date string T character replaces space between date and time.

			if (!announcements[i].isActive || isOutOfDateRange(announcements[i])) {
				$('#usrAnnRow' + i).append('<td class="' + cellClass + '">No</td>');
			}
			else {
				$('#usrAnnRow' + i).append('<td class="' + cellClass + '">Yes</td>');
			}

			$('#usrAnnEditButtonsBody').append('<tr><td class="' + cellClass + '"><a href="/Hosting/EditAnnouncement/?newModel=false" onClick="editAnnouncement(' + i + ');">Edit announcement</a></td></tr>');
		}
	}
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
	var announcmentSelectedList = JSON.parse(sessionStorage.getItem('announcmentSelectedList'));
	if (!announcmentSelectedList) {
		announcmentSelectedList = [];
		sessionStorage.setItem('announcmentSelectedList', JSON.stringify(announcmentSelectedList));
	}
	return announcmentSelectedList;
}

function clearAnnouncementSelectedList() {
	sessionStorage.setItem('announcmentSelectedList', null);
}


function toggleAnnouncementSelection(announcementIndex) {
	var announcmentSelectedList = getAnnouncementSelectedList();
	var selectedIndex = clickedAnnouncementRowIndex(announcementIndex);

	if (selectedIndex == null) {
		// Clicked announcement index does not exist in selected list.
		announcmentSelectedList.push(announcementIndex);
	}
	else {
		// Remove selected item record.
		announcmentSelectedList.splice(selectedIndex, 1);
	}
	// Save changes
	sessionStorage.setItem('announcmentSelectedList', JSON.stringify(announcmentSelectedList));
}

/**
 * Finds array index of provided announcement index.
 * @param {any} announcementIndex
 */
function clickedAnnouncementRowIndex(announcementIndex) {
	var announcmentSelectedList = getAnnouncementSelectedList();
	var selectedIndex = null;

	// Find record with clicked announcement index.
	for (var i = 0; i < announcmentSelectedList.length; i++) {
		if (announcmentSelectedList[i] == announcementIndex) {
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
		url: '/Hosting/ChangeAnnouncementsStatus',
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
		url: '/Hosting/ChangeAnnouncementsStatus',
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
		url: '/Hosting/ChangeAnnouncementsStatus',
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




