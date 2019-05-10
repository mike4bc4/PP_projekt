/**
 * Client side model which represents announcement view model data.
 * */
class ViewModel {
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
        this.timetable;
        this.maxReservations;
        this.scheduleItems = [];
    }
}

/**
 * Creates new model and stores in session if previous model does not exists.
 * */
function createNewModel() {
    var model = getModelFromSession();
    if (!model) {
        sessionStorage.setItem("model", JSON.stringify(new ViewModel()));
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
        url: '/Announcement/GetPartialViewWithData',
        data: {
            partialViewName: partialViewName,
            viewModel: getModelFromSession()
        },
        dataType: 'html',
        success: function (response) {
            injectHtml('content', response);
            if (partialViewName == contactPartialViewName) {
                retrieveContacts(contactItemIndex, 'contacts', getContactMethods());
            } else if (partialViewName == paymentPartialViewName) {
                retrievePayments(paymentItemIndex, 'payments', getPaymentMethods());
            }
            else if (partialViewName == timePlacePartialViewName) {
                retrieveTimetableOptions();
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
 * Makes correct flag valid for each view.
 * */
function setValidViews() {
    validViews[typePartialViewName] = true;
    validViews[subtypePartialViewName] = true;
    validViews[timePlacePartialViewName] = true;
    validViews[descriptionPartialViewName] = true;
    validViews[contactPartialViewName] = true;
    validViews[paymentPartialViewName] = true;
}

/**
 * Calls save announcement method from controller and resets model
 * if announcement was successfully created.
 * */
function saveAnnouncement() {
    var model = getModelFromSession();
    $.ajax({
        method: 'post',
        url: '/Announcement/SaveAnnouncement',
        data: {
            viewModel: getModelFromSession()
        },
        dataType: 'json',
        success: function (response) {
            if (response.announcementCorrect) {
                // Announcement has been saved in database.
                // So create new view model.
                updateModelInSession(new ViewModel());
                // Mark all views as invalid
                resetValidViews();
                // Display views as saved.
                markHeadersAsSaved();
                // Show proper response.
                injectHtml('content', response.page);
                // Change layout tag without page reload.
                var becomeHostTag = document.getElementById('becomeHostTag');
                if (becomeHostTag) {
                    becomeHostTag.setAttribute('href', '/Announcement/ListUserAnnouncements');
                    becomeHostTag.innerText = 'Manage my announcements';
                }
            } else {
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

    typeHeader.setAttribute('class', savedHeaderClassName);
    subtypeHeader.setAttribute('class', savedHeaderClassName);
    timeplaceHeader.setAttribute('class', savedHeaderClassName);
    descriptionHeader.setAttribute('class', savedHeaderClassName);
    contactsHeader.setAttribute('class', savedHeaderClassName);
    paymentsHeader.setAttribute('class', savedHeaderClassName);
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
        viewHeader.setAttribute('class', validHeaderClassName);
    } else {
        viewHeader.setAttribute('class', invalidHeaderClassName);
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
    // No validation is needed as selected values are predefined
    // and empty value cannot be provided.
    switch (model.type) {
        case 0:
            // House
            model.subtype = parseInt(getElementValue('houseSubtype'), 10);
            model.sharedPart = parseInt(getElementValue('houseSharedPart'), 10);
            validViews[subtypePartialViewName] = true;
            break;
        case 1:
            // Entertainment
            model.subtype = parseInt(getElementValue('entertainmentType'), 10);
            validViews[subtypePartialViewName] = true;
            break;
        case 2:
            // Food
            model.subtype = parseInt(getElementValue('foodType'), 10);
            validViews[subtypePartialViewName] = true;
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
    fromDate.setHours(0, 0, 0, 0);
    var to = getElementValue('to').split('-');
    var toDate = new Date(to[0], to[1] - 1, to[2]);
    toDate.setHours(0, 0, 0, 0);
    var today = new Date();
    today.setHours(0, 0, 0, 0);
    var timetable = parseInt(getElementValue('timetableOptionSelect'));

    var perDayReservationsValid;
    var scheduleItemsValid = true;
    var scheduleItems = getScheduleItems();

    // Always save data - even if wrong - to keep form updated.
    model.country = country
    model.region = region
    model.city = city
    model.street = street
    model.streetNumber = streetNumber
    model.from = fromDate
    model.to = toDate
    model.timetable = timetable;

    switch (timetable) {
        case 0:     // Timetable is off
            model.maxReservations = null;
            model.scheduleItems = null;
            break;
        case 1:     // Timetable per day
            perDayReservationsValid = getPerDayMaxReservations().maxReservationsValid;
            model.maxReservations = getPerDayMaxReservations().maxReservations;
            model.scheduleItems = null;
            break;
        case 2:     // Timetable per hour
            model.maxReservations = null;
            var scheduleItems = [];
            // Parse schedule items to plain data without validation.
            for (var item of getScheduleItems()) {
                if (item != null) {
                    scheduleItems.push({ from: item.from, to: item.to, maxReservations: item.maxReservations });
                }
            }
            model.scheduleItems = scheduleItems;
            break;
    }



    validViews[timePlacePartialViewName] = true;

    // Validation
    if (scheduleItems == null || scheduleItems.length == 0) {
        scheduleItemsValid = false;
    }
    else {
        for (var item of getScheduleItems()) {
            if (item == null || item.maxReservationsValid == false || item.timeValid == false) {
                scheduleItemsValid = false;
                break;
            }
        }
    }

    if (!country || !region || !city || !street || !streetNumber || !fromDate || !toDate || fromDate < today || fromDate > toDate) {
        validViews[timePlacePartialViewName] = false;
    }
    if (country.length > maxInputLength || region.length > maxInputLength || city.length > maxInputLength || street > maxInputLength || streetNumber > maxInputLength) {
        validViews[timePlacePartialViewName] = false;
    }
    switch (timetable) {
        case 1:     // Per day timetable
            if (perDayReservationsValid == false) {
                validViews[timePlacePartialViewName] = false;
            }
            break;
        case 2:     // Per hour timetable
            if (scheduleItemsValid == false) {
                validViews[timePlacePartialViewName] = false;
            }
            break;
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
    } else {
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
    var html = '<div id="' + container + index.value + '">';
    html += '<select id="value' + container + index.value + '">';
    Object.keys(dropDownListContent).forEach(function (key) {
        if (selectedValue == dropDownListContent[key]) {
            html += '<option value="' + dropDownListContent[key] + ' selected">' + key + '</option>';
        } else {
            html += '<option value="' + dropDownListContent[key] + '">' + key + '</option>';
        }
    });

    html += '</select>';
    html += '<input type="text" id="key' + container + index.value + '" value="' + inputValue + '"/>';
    html += '<a href="#" onclick="removeItem(\'' + container + '\',' + index.name + ',' + index.value + ')">Remove</a>';
    html += '</div>';
    $('#' + container).append(html);

    var key = $('#key' + container + index.value);
    var value = $('#value' + container + index.value);
    index.items[index.value] = {
        key: key,
        value: value
    };

    index.value++;
}

function retrieveContacts(index, container, dropDownListContent) {
    var model = getModelFromSession();
    // Reset index
    index.value = 0;
    index.items = [];
    Object.entries(model.contactMethods).forEach(([key, value]) => {
        addItem(index, container, dropDownListContent, value, key);
    }
    );
}
function retrievePayments(index, container, dropDownListContent) {
    var model = getModelFromSession();
    // Reset index
    index.value = 0;
    index.items = [];
    Object.entries(model.paymentMethods).forEach(([key, value]) => {
        addItem(index, container, dropDownListContent, value, key);
    }
    );
}


function saveContacts(index) {
    var model = getModelFromSession();
    // Reset previous contact methods
    model.contactMethods = {};
    for (var i = 0; i < index.items.length; i++) {
        if (index.items[i] != null) {
            // Dictionary key does not support dot notation so it's necessary to replace it with unicode character (which is dot anyway -.-)
            model.contactMethods[index.items[i].key.val().replace(/\./g, "\u2024")] = parseInt(index.items[i].value.val(), 10);
        }
    }
    // Validate
    validViews[contactPartialViewName] = true;
    if (Object.keys(model.contactMethods).length > 0) {
        for (var key in model.contactMethods) {
            if (key == null) {
                // Key represents text field value
                validViews[contactPartialViewName] = false;
            }
        }
    } else {
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
            model.paymentMethods[index.items[i].key.val().replace(/\./g, "\u2024")] = parseInt(index.items[i].value.val(), 10);
        }
    }
    // Validate
    validViews[paymentPartialViewName] = true;
    if (Object.keys(model.paymentMethods).length > 0) {
        for (var key in model.paymentMethods) {
            if (key == null) {
                // Key represents text field value
                validViews[paymentPartialViewName] = false;
            }
        }
    } else {
        validViews[paymentPartialViewName] = false;
    }

    updateModelInSession(model);
    updateValidStatusForView(paymentPartialViewName);
}

function handleTimetableOptionChange(selectID, scheduleContainerID) {
    var scheduleContainer = document.getElementById(scheduleContainerID);
    var selectTagValue = parseInt(document.getElementById(selectID).value);
    switch (selectTagValue) {
        case 1:
            // Timetable per day
            sessionStorage.setItem('perDayMaxRes', JSON.stringify({ maxReservations: 10, maxReservationsValid: true }));
            var html = '<label>Reservations</label>';
            html += '<input onchange="savePerDayMaxReservations();" id="perDayMaxRes" value="10" type="text" size="5" maxlength="5" />';
            html += '<span id="perDayMaxResErrorMessage"></span>';
            scheduleContainer.innerHTML = html;
            break;
        case 2:
            // Timetable per hour
            var scheduleItems = [];
            sessionStorage.setItem('scheduleItems', JSON.stringify(scheduleItems));
            // Inject html.
            var html = '';
            html += '<a href="#" onClick="addTimetableScheduleItem(\'announcementScheduleItemsContainer\');">';
            html += 'Add schedule item</a>';
            html += '<div id="announcementScheduleItemsContainer"></div>';
            scheduleContainer.innerHTML = html;
            break;
        default:
            scheduleContainer.innerText = '';
            break;
    }
}

function retrieveTimetableOptions() {
    var model = getModelFromSession();
    var selectTag = document.getElementById('timetableOptionSelect');
    var timetableOptions = getTimetableOptions();
    var html = '';
    var index = 0;
    for (var option of timetableOptions) {
        if (index != model.timetable) {
            html += '<option value="' + index + '">' + option + '</option>';
        }
        else {
            html += '<option value="' + index + '" selected>' + option + '</option>';
        }
        index++;
    }
    selectTag.innerHTML = html;
    handleTimetableOptionChange('timetableOptionSelect', 'announcementScheduleContainer');
    switch (model.timetable) {
        case 1:
            document.getElementById('perDayMaxRes').value = model.maxReservations;
            break;
        case 2:
            var itemIndex = 0;
            // Per hour selection.
            for (var item of model.scheduleItems) {
                // For each item of schedule items add item to html page.
                addTimetableScheduleItem('announcementScheduleItemsContainer');
                var scheduleItems = getScheduleItems();
                // Update item values on html page.
                document.getElementById('from' + itemIndex).value = item.from.toString() + ':00';
                if (item.to != 24) {
                    document.getElementById('to' + itemIndex).value = item.to.toString() + ':00';
                }
                else {
                    document.getElementById('to' + itemIndex).value = '23:59';
                }
                document.getElementById('maxReservations' + itemIndex).value = item.maxReservations;
                // Update item values stored in session.
                scheduleItems[itemIndex].from = item.from;
                scheduleItems[itemIndex].to = item.to;
                scheduleItems[itemIndex].maxReservations = item.maxReservations;
                sessionStorage.setItem('scheduleItems', JSON.stringify(scheduleItems));
                itemIndex++;
            }
            itemIndex = 0;
            for(itemIndex; itemIndex < model.scheduleItems.length; itemIndex++){
                validateScheduleItem(itemIndex);
            }
            break;
    }


}

function validatePerDayMaxReservations() {
    var maxReservations = parseInt(document.getElementById('perDayMaxRes').value);
    var maxReservationsValid = true;
    if (maxReservations == null || isNaN(maxReservations) || maxReservations == 0) {
        maxReservationsValid = false;
    }
    updatePerDayMaxReservationsErrorMessage(maxReservationsValid);
    return maxReservationsValid;
}

function updatePerDayMaxReservationsErrorMessage(maxReservationsValid) {
    if (maxReservationsValid == false) {
        document.getElementById('perDayMaxResErrorMessage').innerText = 'Reservations amount error';
    }
    else {
        document.getElementById('perDayMaxResErrorMessage').innerText = '';
    }
}

function savePerDayMaxReservations(maxReservations) {
    var maxReservationsValid = validatePerDayMaxReservations();
    var maxReservations = parseInt(document.getElementById('perDayMaxRes').value);
    sessionStorage.setItem('perDayMaxRes', JSON.stringify({ maxReservations, maxReservationsValid }));
}

function getPerDayMaxReservations() {
    return JSON.parse(sessionStorage.getItem('perDayMaxRes'));
}

function addTimetableScheduleItem(scheduleItemsContainerID) {
    var scheduleItems = JSON.parse(sessionStorage.getItem('scheduleItems'));
    var itemIndex = scheduleItems.length;
    var itemContainer = $('#' + scheduleItemsContainerID);

    var html = '<div id="scheduleItem' + itemIndex + '"></div>';
    itemContainer.append(html);
    itemContainer = $('#scheduleItem' + itemIndex);
    html = '<label>From</label>';
    html += '<button onmousedown="toggleFunction(true,' + itemIndex + ',\'from\', decrement);" onmouseup="toggleFunction(false);">-</button>';
    html += '<input id="from' + itemIndex + '" type="text" size="5" readonly  />';
    html += '<button onmousedown="toggleFunction(true,' + itemIndex + ',\'from\', increment);" onmouseup="toggleFunction(false);">+</button>';
    itemContainer.append(html);
    html = '<label>To</label>';
    html += '<button onmousedown="toggleFunction(true,' + itemIndex + ',\'to\', decrement);" onmouseup="toggleFunction(false);">-</button>';
    html += '<input id="to' + itemIndex + '" type="text"  size="5" readonly  />';
    html += '<button onmousedown="toggleFunction(true,' + itemIndex + ',\'to\', increment);" onmouseup="toggleFunction(false);">+</button>';
    itemContainer.append(html);
    html = '<label>Reservations</label>';
    html += '<input onchange="saveScheduleItem(' + itemIndex + ');" id="maxReservations' + itemIndex + '" type="text" size="5" maxlength="5" />';
    itemContainer.append(html);
    html = '<a href="#" onclick="removeScheduleItem(' + itemIndex + ');">Remove</a>'
    itemContainer.append(html);
    html = '<span id="scheduleItemErrorMessage' + itemIndex + '"></span>'
    itemContainer.append(html);

    var fromTag = document.getElementById('from' + itemIndex);
    var toTag = document.getElementById('to' + itemIndex);
    var maxReservationsTag = document.getElementById('maxReservations' + itemIndex);

    // On first item added.
    if (scheduleItems.length == 0) {
        // Setup default values.
        fromTag.value = '8:00';
        toTag.value = '9:00';
        maxReservationsTag.value = 10;
        saveScheduleItem(itemIndex);
    } else {
        // First element of from hours is considered as 0:00.
        var availableFromHours = [];
        // First element of to hours is considered as 1:00.
        var availableToHours = [];
        for (var i = 0; i < 24; i++) {
            availableToHours.push(true);
            availableFromHours.push(true);
        }
        for (var item of scheduleItems) {
            if (item != null) {
                for (var i = item.from; i < item.to; i++) {
                    availableFromHours[i] = false;
                    availableToHours[i] = false;
                }
            }
        }
        var possibleHours = null;
        for (var i = 23; i >= 0; i--) {
            if (availableFromHours[i] == true && availableToHours[i] == true) {
                possibleHours = i;
            } else {
                break;
            }
        }
        // Has not found any free hours from the end.
        if (possibleHours == null) {
            // Try to find any from beginning.
            for (var i = 0; i < 24; i++) {
                if (availableFromHours[i] == true && availableToHours[i] == true) {
                    possibleHours = i;
                }
            }
        }
        if (possibleHours != null) {
            // Has found free hours.
            fromTag.value = possibleHours.toString() + ':00';
            toTag.value = (possibleHours + 1).toString() + ':00';
            maxReservationsTag.value = 10;
            saveScheduleItem(itemIndex);
        } else {
            // Cannot add any field.
            // Remove added tag.
            document.getElementById('scheduleItem' + itemIndex).remove();
        }
    }
}

function removeScheduleItem(itemIndex) {
    var scheduleItems = JSON.parse(sessionStorage.getItem('scheduleItems'));
    scheduleItems[itemIndex] = null;
    document.getElementById('scheduleItem' + itemIndex).remove();
    var scheduleItemsEmpty = true;
    for (var item of scheduleItems) {
        if (item != null) {
            scheduleItemsEmpty = false;
            break;
        }
    }
    // Clear array if there is nothing inside.
    if (scheduleItemsEmpty == true) {
        scheduleItems = [];
    }
    sessionStorage.setItem('scheduleItems', JSON.stringify(scheduleItems));
}

function saveScheduleItem(itemIndex) {
    var scheduleItems = JSON.parse(sessionStorage.getItem('scheduleItems'));

    var fromTag = document.getElementById('from' + itemIndex);
    var toTag = document.getElementById('to' + itemIndex);
    var maxReservationsTag = document.getElementById('maxReservations' + itemIndex);

    var from = parseInt(fromTag.value.split(':')[0]);
    var to;
    if (toTag.value != '23:59') {
        to = parseInt(toTag.value.split(':')[0]);
    } else {
        to = 24;
    }
    var maxReservations = parseInt(maxReservationsTag.value);
    var scheduleItem = scheduleItems[itemIndex];
    if (scheduleItem == null) {
        scheduleItems.push({
            from,
            to,
            maxReservations
        });
    } else {
        scheduleItem.from = from;
        scheduleItem.to = to;
        scheduleItem.maxReservations = maxReservations;
    }

    sessionStorage.setItem('scheduleItems', JSON.stringify(scheduleItems));
    for (var i = 0; i < scheduleItems.length; i++) {
        if (scheduleItems[i] != null) {
            validateScheduleItem(i);
        }
    }

}

function validateScheduleItem(itemIndex) {
    var scheduleItems = JSON.parse(sessionStorage.getItem('scheduleItems'));

    var from = scheduleItems[itemIndex].from;
    var to = scheduleItems[itemIndex].to;
    var maxReservations = scheduleItems[itemIndex].maxReservations;

    var maxReservationsValid = true;
    var timeValid = true;

    // Represents pairs like 0:00-1:00, 1:00-2:00, ...
    var currentItemAvailableHours = [];
    var availableHours = [];
    for (var i = 0; i < 24; i++) {
        availableHours.push(true);
        currentItemAvailableHours.push(true);
    }

    for (var i = 0; i < scheduleItems.length; i++) {
        if (i != itemIndex && scheduleItems[i] != null) {
            for (var j = scheduleItems[i].from; j < scheduleItems[i].to; j++) {
                availableHours[j] = false;
            }
        }
    }
    for (var i = from; i < to; i++) {
        currentItemAvailableHours[i] = false;
    }
    for (var i = 0; i < availableHours.length; i++) {
        if (availableHours[i] == false && currentItemAvailableHours[i] == false) {
            timeValid = false;
            break;
        }
    }
    if (maxReservations == null || maxReservations == 0) {
        maxReservationsValid = false;
    }
    // Append validation information to schedule objects list.
    if (scheduleItems[itemIndex] != null) {
        scheduleItems[itemIndex].timeValid = timeValid;
        scheduleItems[itemIndex].maxReservationsValid = maxReservationsValid;
    }
    sessionStorage.setItem('scheduleItems', JSON.stringify(scheduleItems));
    updateScheduleItemErrorMessage(itemIndex, timeValid, maxReservationsValid);
}

function getScheduleItems() {
    return JSON.parse(sessionStorage.getItem('scheduleItems'));
}

function updateScheduleItemErrorMessage(itemIndex, timeValid, maxReservationsValid) {
    switch (true) {
        case (timeValid == false):
            document.getElementById('scheduleItemErrorMessage' + itemIndex).innerText = 'Time range collision';
            break;
        case (maxReservationsValid == false):
            document.getElementById('scheduleItemErrorMessage' + itemIndex).innerText = 'Reservations amount error';
            break;
        default:
            document.getElementById('scheduleItemErrorMessage' + itemIndex).innerText = '';
            break;
    }
}

var tid = 0;
var speed = 150;

function toggleFunction(switchOn, itemIndex, fieldName, functionName) {
    if (switchOn == true) {
        functionName(itemIndex, fieldName);
        if (tid == 0) {
            tid = setInterval(function () {
                functionName(itemIndex, fieldName);
            }, speed);
        }
    } else {
        if (tid != 0) {
            clearInterval(tid);
            tid = 0;
        }
    }
}

function decrement(itemIndex, fieldName) {
    var tag = document.getElementById(fieldName + itemIndex);
    var tagValue;
    switch (fieldName) {
        case 'from':
            tagValue = parseInt(tag.value.split(':')[0]);
            if (tagValue > 0) {
                tag.value = (tagValue - 1).toString() + ':00';
            }
            break;
        case 'to':
            if (tag.value == '23:59') {
                tagValue = 24;
            } else {
                tagValue = parseInt(tag.value.split(':')[0]);
            }
            var fromTag = document.getElementById('from' + itemIndex);
            var fromTagValue = parseInt(fromTag.value.split(':')[0]);
            if ((fromTagValue + 1) == tagValue) {
                return;
            }
            if (tagValue > 1) {
                tag.value = (tagValue - 1).toString() + ':00';
            }
            break;
    }
    saveScheduleItem(itemIndex);
}
function increment(itemIndex, fieldName) {
    var upperLimit;
    var tag = document.getElementById(fieldName + itemIndex);
    var tagValue;
    switch (fieldName) {
        case 'from':
            tagValue = parseInt(tag.value.split(':')[0]);
            var toTag = document.getElementById('to' + itemIndex);
            var toTagValue = parseInt(toTag.value.split(':')[0]);
            if (parseInt(toTag.value) == (tagValue + 1)) {
                increment(itemIndex, 'to');
            }

            upperLimit = 23;
            if (tagValue < upperLimit) {
                tag.value = (tagValue + 1).toString() + ':00';
            }

            break;
        case 'to':
            upperLimit = 24;
            if (tag.value == '23:59') {
                tagValue = 24;
            } else {
                tagValue = parseInt(tag.value.split(':')[0]);
            }
            if (tagValue < upperLimit) {
                if (tagValue != 23) {
                    tag.value = (tagValue + 1).toString() + ':00';
                } else {
                    tag.value = '23:59';
                }
            }
            break;
    }
    saveScheduleItem(itemIndex);
}
