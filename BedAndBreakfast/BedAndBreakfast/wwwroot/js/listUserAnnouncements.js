function drawAnnouncementsList(announcements) {
	var today = new Date();
	today.setHours(0, 0, 0, 0);

	// Clear container content.
	var container = document.getElementById('usrAnnPageContainer');
	container.innerHTML = '';
	// Create table body.
	$('#usrAnnPageContainer').append('<table border="1">' +
		'<thead id="usrAnnTabHeader"></thead><tbody id="usrAnnTabBody"></tbody></table>');
	// Set table header.
	$('#usrAnnTabHeader').append('<tr><th>Type</th><th>Subtype</th><th>Additional option</th>' +
		'<th>From</th><th>To</th><th>Active</th><th></th><th></th></tr>');
	// Set table content.
	for (var i = 0; i < announcements.length; i++) {
		$('#usrAnnTabBody').append('<tr id="usrAnnRow' + i + '"></tr>');

		$('#usrAnnRow' + i).append('<td>' + announcements[i].type + '</td>' +
			'<td>' + announcements[i].subtype + '</td>' +
			'<td>' + announcements[i].sharedPart + '</td>' +
			'<td>' + announcements[i].from.substr(0, 10) + '</td>' +
			'<td>' + announcements[i].to.substr(0, 10) + '</td>');
		if (!announcements[i].isActive || announcements[i].from > today || announcements[i].to < today) {
			$('#usrAnnRow' + i).append('<td>No</td>');
		}
		else {
			$('#usrAnnRow' + i).append('<td>Yes</td>');
		}
		$('#usrAnnRow' + i).append('<td>RemoveButtonPlaceholder</td><td>EditButtonPlaceholder</td>');
	}
}

