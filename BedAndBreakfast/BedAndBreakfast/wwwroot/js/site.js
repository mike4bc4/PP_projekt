// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


/**
 * Client side model which represents announcement view model data.
 * */
class Model {
	constructor() {
		this.type;
		this.subtype;
		this.sharedPart;
		this.country;
		this.region;
		this.city;
		this.street;
		this.streetNumber;
		this.from;
		this.to;
		this.description;
		this.contactMethods = {};
		this.paymentMethods = {};
	}
}

/**
 * Creates new model and stores in session if previous model does not exists.
 * */
function createNewModel() {
	var model = getModelFromSession();
	if (!model) {
		sessionStorage.setItem("model", JSON.stringify(new Model()));
	}
}

/**
 * Replaces model stored in session with null value.
 * */
function deleteCurrentModel() {
	sessionStorage.setItem("model", null);
}

/**
 * Return model from session.
 * */
function getModelFromSession() {
	return JSON.parse(sessionStorage.getItem("model"));
}

/**
 * Updates model stored in session with new object provided.
 * @param {any} model
 */
function updateModelInSession(model) {
	sessionStorage.setItem("model", JSON.stringify(model));
}

/**
 * Replaces HTML code in tag specified by id,
 * @param {any} itemID
 * @param {any} html
 */
function injectHtml(itemID, html) {
	document.getElementById(itemID).innerHTML = html;
}

/**
 * Returns value of HTML tag.
 * @param {any} elementID
 */
function getElementValue(elementID) {
	return $('#' + elementID).val();
}

/**
 * Executes post request of specified partial view from hosting group.
 * If response is received content of tag with 'content' id is replaced.
 * Also modifies received views with proper forms if they are related
 * to dynamic lists of contacts or payments.
 * @param {any} partialViewName
 */
function setPartialView(partialViewName) {
	updateValidStatus();
	$.ajax({
		method: 'post',
		url: '/Hosting/GetPartialViewWithData',
		data: {
			partialViewName: partialViewName,
			viewModel: getModelFromSession()
		},
		dataType: 'html',
		success: function (response) {
			injectHtml('content', response);
			if (partialViewName == contactPartialViewName) {
				retrieveContacts(contactItemIndex, 'contacts', getContactMethods());
			}
			else if (partialViewName == paymentPartialViewName) {
				retrievePayments(paymentItemIndex, 'payments', getPaymentMethods());
			}
		}
	});
}

/**
 * Makes correct flag invalid for each view. 
 * */
function resetValidViews() {
	validViews[typePartialViewName] = false;
	validViews[subtypePartialViewName] = false;
	validViews[timePlacePartialViewName] = false;
	validViews[descriptionPartialViewName] = false;
	validViews[contactPartialViewName] = false;
	validViews[paymentPartialViewName] = false;
}

/**
 * Calls save announcement method from controller and resets model
 * if announcement was successfully created.
 * */
function saveAnnouncement() {
	var model = getModelFromSession();
	$.ajax({
		method: 'post',
		url: '/Hosting/SaveAnnouncement',
		data: {
			viewModel: getModelFromSession()
		},
		dataType: 'json',
		success: function (response) {
			if (response.announcementCorrect) {
				// Announcement has been saved in database.
				// So create new view model.
				updateModelInSession(new Model());
				// Mark all views as invalid
				resetValidViews();
				// Display views as saved.
				markHeadersAsSaved();
				// Show proper response
				injectHtml('content', response.page);
			}
			else {
				injectHtml('content', response.page);
			}
		}
	});
}

function markHeadersAsSaved() {
	var typeHeader = document.getElementById(typePartialViewName);
	var subtypeHeader = document.getElementById(subtypePartialViewName);
	var timeplaceHeader = document.getElementById(timePlacePartialViewName);
	var descriptionHeader = document.getElementById(descriptionPartialViewName);
	var contactsHeader = document.getElementById(contactPartialViewName);
	var paymentsHeader = document.getElementById(paymentPartialViewName);

	typeHeader.setAttribute('style', 'background-color:yellow;');
	subtypeHeader.setAttribute('style', 'background-color:yellow;');
	timeplaceHeader.setAttribute('style', 'background-color:yellow;');
	descriptionHeader.setAttribute('style', 'background-color:yellow;');
	contactsHeader.setAttribute('style', 'background-color:yellow;');
	paymentsHeader.setAttribute('style', 'background-color:yellow;');
}

function updateValidStatus() {
	// Update valid status for all headers.
	updateValidStatusForView(typePartialViewName);
	updateValidStatusForView(subtypePartialViewName);
	updateValidStatusForView(timePlacePartialViewName);
	updateValidStatusForView(descriptionPartialViewName);
	updateValidStatusForView(contactPartialViewName);
	updateValidStatusForView(paymentPartialViewName);
}

function updateValidStatusForView(partialViewName) {
	var viewHeader = document.getElementById(partialViewName);
	if (validViews[partialViewName]) {
		viewHeader.setAttribute('style', 'background-color:green;');
	}
	else {
		viewHeader.setAttribute('style', 'background-color:red;');
	}
}

function setType(type) {
	var model = getModelFromSession();

	// Update subtype and shared part status if main type changed.
	if (model.type != type) {
		validViews[subtypePartialViewName] = false;
		model.subtype = null;
		model.sharedPart = null;
	}

	validViews[typePartialViewName] = true;

	model.type = type;
	updateModelInSession(model);
	updateValidStatus(typePartialViewName);
	setPartialView(typePartialViewName);
}

function setSubtype() {
	var model = getModelFromSession();
	// reset previous values
	model.subtype = null;
	model.sharedPart = null;
	// load new values
	switch (model.type) {
		case "House":
			// Validation
			var subtype = getElementValue('houseSubtype');
			var sharedPart = getElementValue('houseSharedPart');
			if (subtype && sharedPart) {
				model.subtype = subtype;
				model.sharedPart = sharedPart;
				validViews[subtypePartialViewName] = true;
			}
			else {
				validViews[subtypePartialViewName] = false;
			}
			break;
		case "Entertainment":
			// Validation
			var subtype = getElementValue('entertainmentType');
			if (subtype) {
				model.subtype = subtype;
				validViews[subtypePartialViewName] = true;
			}
			else {
				validViews[subtypePartialViewName] = false;
			}
			break;
		case "Food":
			// Validation
			var subtype = getElementValue('foodType');
			if (subtype) {
				model.subtype = subtype;
				validViews[subtypePartialViewName] = true;
			}
			else {
				validViews[subtypePartialViewName] = false;
			}
			break;
		default:
			break;
	}
	// save in session
	updateModelInSession(model);
	updateValidStatusForView(subtypePartialViewName);
}

function setTimePlace() {
	var model = getModelFromSession();

	var country = getElementValue('country');
	var region = getElementValue('region');
	var city = getElementValue('city');
	var street = getElementValue('street');
	var streetNumber = getElementValue('streetNumber');
	var from = getElementValue('from').split('-');
	var fromDate = new Date(from[0], from[1] - 1, from[2]);
	var to = getElementValue('to').split('-');
	var toDate = new Date(to[0], to[1] - 1, to[2]);
	var today = new Date();

	// Always save data - even if wrong - to keep form updated.
	model.country = country
	model.region = region
	model.city = city
	model.street = street
	model.streetNumber = streetNumber
	model.from = fromDate
	model.to = toDate

	if (country && region && city && street && streetNumber && fromDate && toDate
		&& fromDate >= today && fromDate < toDate
	) {
		validViews[timePlacePartialViewName] = true;
	}
	else {
		
		validViews[timePlacePartialViewName] = false;
	}
	// save in session
	updateModelInSession(model);
	updateValidStatusForView(timePlacePartialViewName);
}

function setDescription() {
	var model = getModelFromSession();
	var description = getElementValue('descriptionField');

	model.description = description;

	if (description) {
		validViews[descriptionPartialViewName] = true;
	}
	else {
		validViews[descriptionPartialViewName] = false;
	}
	updateModelInSession(model);
	// Reload view
	setPartialView(descriptionPartialViewName);

}

function removeItem(container, index, itemNumber) {
	var node = document.getElementById(container + itemNumber);
	while (node.firstChild) {
		node.removeChild(node.firstChild);
	}
	node.remove();
	index.items[itemNumber] = null;
}

function addItem(index, container, dropDownListContent, selectedValue, inputValue) {
	var contactMethods = dropDownListContent;
	var html = '<div id="'+ container + index.value + '">';
	html += '<select id="value' + container + index.value + '">';
	for (var i = 0; i < contactMethods.length; i++) {
		if (selectedValue) {
			if (contactMethods[i] == selectedValue) {
				html += '<option selected>' + contactMethods[i] + '</option>';
			}
			html += '<option>' + contactMethods[i] + '</option>';
		}
		else {
			html += '<option>' + contactMethods[i] + '</option>';
		}
	}
	html += '</select>';
	html += '<input type="text" id="key' + container + index.value + '" value="' + inputValue + '"/>';
	html += '<a href="#" onclick="removeItem(\'' + container + '\',' + index.name + ',' + index.value + ')">Remove</a>';
	html += '</div>';
	$('#' + container).append(html);

	var key = $('#key' + container + index.value);
	var value = $('#value' + container + index.value);
	index.items[index.value] = { key: key, value: value };

	index.value++;
}

function retrieveContacts(index, container, dropDownListContent) {
	var model = getModelFromSession();
	// Reset index
	index.value = 0;
	index.items = [];
	Object.entries(model.contactMethods).forEach(([key, value]) => {
		addItem(index, container, dropDownListContent, value, key);
	});
}
function retrievePayments(index, container, dropDownListContent) {
	var model = getModelFromSession();
	// Reset index
	index.value = 0;
	index.items = [];
	Object.entries(model.paymentMethods).forEach(([key, value]) => {
		addItem(index, container, dropDownListContent, value, key);
	});
}


function saveContacts(index) {
	var model = getModelFromSession();
	// Reset previous contact methods
	model.contactMethods = {};
	for (var i = 0; i < index.items.length; i++) {
		if (index.items[i] != null) {
			// Dictionary key does not support dot notation so it's necessary to replace it with unicode character (which is dot anyway -.-)
			model.contactMethods[index.items[i].key.val().replace(/\./g, "\u2024")] = index.items[i].value.val();
		}
	}
	// Validate
	if (Object.keys(model.contactMethods).length > 0) {
		validViews[contactPartialViewName] = true;
	}
	else {
		validViews[contactPartialViewName] = false;
	}
	updateModelInSession(model);
	updateValidStatusForView(contactPartialViewName);
}


function savePayments(index) {
	var model = getModelFromSession();
	// Reset previous contact methods
	model.paymentMethods = {};
	for (var i = 0; i < index.items.length; i++) {
		if (index.items[i] != null) {
			model.paymentMethods[index.items[i].key.val().replace(/\./g, "\u2024")] = index.items[i].value.val();
		}
	}
	// Validate

	for (var key in model.paymentMethods) {
		if (key && model.paymentMethods[key]) {
			validViews[paymentPartialViewName] = true;
		}
		else {
			validViews[paymentPartialViewName] = false;
		}
		break;
	}

	updateModelInSession(model);
	updateValidStatusForView(paymentPartialViewName);
}

