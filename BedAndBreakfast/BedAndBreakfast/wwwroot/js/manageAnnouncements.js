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
			handlePartialViewInitialLoad(null);
		},
	];
	requestSynchronizer.run();
}

function handleAnnouncementEditButton(announcementID) {
    /**
     * Handles get announcement request (server action).
     */
	function getAnnouncementRequest(context, requestSynchronizer) {
		$.ajax({
			url: "/Announcement/GetAnnouncement",
			data: { announcementID: context.announcementID },
			dataType: "json",
			method: "post",
			success: function (response) {
				context.getAnnouncementRequestResponse = response;
				requestSynchronizer.generator.next();
			}
		});
	}

    /**
     * Converts given integer value to hour string which
     * is correct value for input field.
     */
	function parseIntToHour(integer) {
		if (integer == 24) {
			return "00:00";
		}
		if (integer < 10) {
			return "0" + integer + ":00";
		}
		if (integer >= 10) {
			return integer + ":00";
		}
	}

    /**
     * Acquires fields from edit announcement partial view and
     * fills them with data provided as parameter.
     */
	function drawExistingAnnouncementData(announcement) {
		// Get input fields and containers.
		var typeDropDownList = document.getElementById("announcement-type-drop-down-list");
		var subtypeDropDownList;
		var houseSharedPartDropDownList = null;
		switch (announcement.type) {
			case 0:
				subtypeDropDownList = document.getElementById("house-subtype-drop-down-list");
				houseSharedPartDropDownList = document.getElementById("house-shared-part-drop-down-list");
				break;
			case 1:
				subtypeDropDownList = document.getElementById("entertainment-subtype-drop-down-list");
				break;
			case 2:
				subtypeDropDownList = document.getElementById("food-subtype-drop-down-list");
				break;
		}
		var address = [
			document.getElementById("country-input-field"),
			document.getElementById("region-input-field"),
			document.getElementById("city-input-field"),
			document.getElementById("street-input-field"),
			document.getElementById("street-number-input-field"),
		];
		var dateRange = [
			document.getElementById("from-date-input-field"),
			document.getElementById("to-date-input-field"),
		];
		var description = document.getElementById("description-text-area");
		var contactsContainer = document.getElementById("contacts-container");
		var paymentsContainer = document.getElementById("payments-container");
		var timetableDropDownList = document.getElementById("timetable-drop-down-list");
		var perDayTimetableContainer = document.getElementById("per-day-timetable-container");
		var perHourTimetableContainer = document.getElementById("per-hour-timetable-container");
		// Fill fields with data.
		typeDropDownList.value = announcement.type;
		handleTypeVisibility();
		subtypeDropDownList.value = announcement.subtype;
		if (houseSharedPartDropDownList != null) {
			houseSharedPartDropDownList.value = announcement.sharedPart;
		}
		address[0].value = announcement.country;
		address[1].value = announcement.region;
		address[2].value = announcement.city;
		address[3].value = announcement.street;
		address[4].value = announcement.streetNumber;
		var from = new Date(announcement.from);
		from.setHours(12, 0, 0, 0);
		var to = new Date(announcement.to);
		to.setHours(12, 0, 0, 0)
		dateRange[0].value = from.toISOString().substring(0, 10);
		dateRange[1].value = to.toISOString().substring(0, 10);
		description.value = announcement.description;
		handleTextareaValidation();
		for (var i = 0; i < announcement.contacts.length; i++) {
			var lastContactElement = contactsContainer.children[contactsContainer.children.length - 1];
			var selectDropDownList = lastContactElement.getElementsByTagName("select")[0];
			var inputField = lastContactElement.getElementsByTagName("input")[0];
			selectDropDownList.value = announcement.contacts[i].type;
			inputField.value = announcement.contacts[i].value;
			addInput(inputField, 5);
			handleContactInfoValidation();
		}
		for (var i = 0; i < announcement.payments.length; i++) {
			var lastPaymentElement = paymentsContainer.children[paymentsContainer.children.length - 1];
			var selectDropDownList = lastPaymentElement.getElementsByTagName("select")[0];
			var inputField = lastPaymentElement.getElementsByTagName("input")[0];
			selectDropDownList.value = announcement.payments[i].type;
			inputField.value = announcement.payments[i].value;
			addInput(inputField, 5);
			handlePaymentInfoValidation();
		}
		timetableDropDownList.value = announcement.timetable;
		handleTimetableVisibility();
		switch (announcement.timetable) {
			case 1:
				var inputField = perDayTimetableContainer.getElementsByTagName("input")[0];
				inputField.value = announcement.perDayReservations;
				break;
			case 2:
				for (var i = 0; i < announcement.scheduleItems.length; i++) {
					var lastScheduleItem = perHourTimetableContainer.children[perHourTimetableContainer.children.length - 1];
					var lastScheduleItemInputs = lastScheduleItem.getElementsByTagName("input");
					lastScheduleItemInputs[0].value = parseIntToHour(announcement.scheduleItems[i].from);
					lastScheduleItemInputs[1].value = parseIntToHour(announcement.scheduleItems[i].to);
					lastScheduleItemInputs[2].value = announcement.scheduleItems[i].maxReservations;
					addTimeTableInput(lastScheduleItemInputs[0], 12);
					handleTimetableValidation();
				}
				break;
		}
		drawAnnouncementImagesPreview(announcement);
	}

	function drawAnnouncementImagesPreview(announcement) {
		// If there is no images related to this announcement return.
		if (announcement.images.length == 0) {
			return;
		}
		var imagesPreviewTable = document.getElementById("images-preview-table");
		var imagesPreviewTableRow = imagesPreviewTable.getElementsByTagName("tr")[0];
		var imagePreviewPrototype = document.getElementById("image-preview-prototype");

		// Show item preview table (hidden by default).
		imagesPreviewTable.hidden = false;
		// Clear table row drawing.
		imagesPreviewTableRow.innerHTML = "";
		// Store prototype back.
		imagesPreviewTableRow.appendChild(imagePreviewPrototype);

		// Add prototype clones.
		for (var i = 0; i < announcement.images.length; i++) {
			// Setup clone.
			var newNode = imagePreviewPrototype.cloneNode(true);
			newNode.removeAttribute("id");
			newNode.hidden = false;
			// Fill clone data.
			newNode.getElementsByTagName("label")[0].innerText = announcement.images[i].imageName;
			newNode.getElementsByTagName("img")[0].setAttribute("src", "data:image/png;base64," + announcement.images[i].imageByteArray);
			imagesPreviewTableRow.appendChild(newNode);
		}

		// Change amount of available image inputs/
		var imageInputsContainer = document.getElementById("images-container");
		if (5 - announcement.images.length < 1) {
			// Hide images input container if no images can be added.
			imageInputsContainer.hidden = true;
		}
		imageInputsContainer.children[0].getElementsByTagName("input")[0].setAttribute("onchange", "addInput(this, " + (5 - announcement.images.length) + ",'images-container');");

	}

	var container = document.getElementById("manage-announcements-view-container");
	var context = {};
	context.announcementID = announcementID;
	var requestSynchronizer = new RequestSynchronizer();
	requestSynchronizer.requestQueue = [
		function () { getAnnouncementRequest(context, requestSynchronizer) },
		function () { editAnnouncement(context, requestSynchronizer) },
		function () {
			if (context.editAnnouncementResponse == null) {
				// Failed to acquire edit announcement view.
				setGlobalMessage(-1);
				return;
			}
			if (context.getAnnouncementRequestResponse == null) {
				// Failed to acquire announcement view model.
				setGlobalMessage(0);
				return;
			}
			// Load create announcement view.
			container.innerHTML = context.editAnnouncementResponse;
			handlePartialViewInitialLoad(announcementID);
			drawExistingAnnouncementData(context.getAnnouncementRequestResponse);
		},
	];
	requestSynchronizer.run();
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
		buttonsContainer.innerHTML = "<button class='button-10 text-segue-14' onclick='handleRemoveSelectedButton(" + JSON.stringify(selectedAnnouncementsIDs) + ");'>Remove selected</button>\
        <button class='button-10 text-segue-14' onclick='handleChangeStatusButton("+ JSON.stringify(selectedAnnouncementsIDs) + ");'>Change status</button>";
	}
}

/**
 * Loads timetable partial view and starts its initialization.
 */
function handleTimetableButton(announcementID, timetableOption) {

    /**
     * Calls controller action request to acquire proper timetable partial view.
     */
	function loadTimetablePartialView(context, requestSynchronizer) {
		$.ajax({
			url: "/Announcement/LoadTimetable",
			data: { timetableOption: context.timetableOption },
			dataType: "html",
			method: "post",
			success: function (response) {
				context.timetablePartialView = response;
				requestSynchronizer.generator.next();
			}
		});
	}

    /**
     * Starts proper partial view initialization, based on timetable option.
     */
	function startPartialViewInit() {
		if (timetableOption == 1) {
			dailyTimetableInit(announcementID);
		}
		else if (timetableOption == 2) {
			hourlyTimetableInit(announcementID);
		}
	}

	if (timetableOption == 0) {
		// Timetable is off for this announcement, display simple message.
		setGlobalMessage(5);
		return;
	}
	else {
		var partialViewContainer = document.getElementById("manage-announcements-view-container");
		var context = {};
		context.timetableOption = timetableOption;
		requestSynchronizer = new RequestSynchronizer();
		requestSynchronizer.requestQueue = [
			function () { loadTimetablePartialView(context, requestSynchronizer) },
			function () {
				partialViewContainer.innerHTML = context.timetablePartialView;
				startPartialViewInit();
			},
		];
		requestSynchronizer.run();
	}
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
	container.innerHTML = "<table class='table-02 text-segue-16' id='announcements-container'>\
    <tr class='table-box-row-01 text-segue-14'>\
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
		container.innerHTML += "<tr class='table-box-row-02'>\
        <td class='table-box-cell-07'><input onclick='handleSelectAnnouncementCheckbox();' type='checkbox' id='announcement-selected-"+ announcement.announcementID + "'/></td>\
        <td><a class='a-link-01' href='/Announcement/Announcement?announcementID=" + announcement.announcementID + "'>"+ announcement.announcementID + "</a></td>\
        <td>"+ announcementTypeToString(announcement.type) + "</td>\
        <td>"+ announcementSubtypeToString(announcement.type, announcement.subtype) + "</td>\
        <td>"+ announcementSharedPartToString(announcement.type, announcement.sharedPart) + "</td>\
        <td>"+ (new Date(announcement.from)).toLocaleDateString() + "</td>\
        <td>"+ (new Date(announcement.to)).toLocaleDateString() + "</td>\
        <td>"+ announcement.country + " " + announcement.region + " " + announcement.city + " \
            "+ announcement.street + " " + announcement.streetNumber + "</td>\
        <td>"+ announcementActiveToString(announcement.isActive) + "</td>\
        <td><button class='button-05 text-segue-14' onclick='handleAnnouncementEditButton("+ announcement.announcementID + ");'>Edit</button></td>\
        <td><button class='button-05 text-segue-14' onclick='handleTimetableButton("+ announcement.announcementID + "," + announcement.timetable + ");'>Timetable</button></td>\
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
			container.innerText = "For one or more announcement status has not been changed because of invalid active time range."
			break;
		case 4:
			container.innerText = "Announcement successfully created.";
			break;
		case 5:
			container.innerText = "This announcement does not have timetable. Use edit button to add it.";
			break;
		case 6:
			container.innerText = "An error occurred while acquiring reservations data form database.";
			break;
		case 7:
			container.innerText = "An error occurred while updating reservations amount."
			break;
		case 8:
			container.innerText = "Reservations amount successfully updated";
			break;
		case 9:
			container.innerText = "Announcement successfully updated.";
			break;
		case 10:
			container.innerText = "An error occurred while saving your announcement.";
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