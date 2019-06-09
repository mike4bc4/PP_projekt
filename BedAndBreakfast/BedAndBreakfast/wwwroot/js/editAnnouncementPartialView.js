
function handlePartialViewInitialLoad(initMode) {
    handleTypeVisibility();
    handleTimetableVisibility();
}

/**
 * Handles timetable validation. While timetable option is equal 0
 * then no validation is performed. While timetable option is equal 1 
 * only reservations amount is validated (number larger than 0 smaller
 * than 1001). While timetable option is equal to 2 every schedule item
 * is validated (data presence, proper time range, proper reservations
 * amount, non colliding time ranges). If validation fails, null is returned,
 * otherwise object {timetable, perDayReservations, scheduleItems} is
 * returned.
 */
function handleTimetableValidation() {
    /**
     * Returns false if from1-to1 time range collides with
     * from2-to2 time range.
     */
    function timeRangeValidation(from1, to1, from2, to2) {
        // No schedule item may start or end at the same time.
        if (from1.getTime() == from2.getTime() || to1.getTime() == to2.getTime()) {
            return false;
        }
        // No schedule item may start inside other schedule item time range.
        if (from1.getTime() > from2.getTime() && from1.getTime() < to2.getTime()) {
            return false;
        }
        // No schedule item may end inside other schedule item time range.
        if (to1.getTime() < to2.getTime() && to1.getTime() > from2.getTime()) {
            return false;
        }
        // Check opposite case.
        // No schedule item may start inside other schedule item time range.
        if (from2.getTime() > from1.getTime() && from2.getTime() < to1.getTime()) {
            return false;
        }
        // No schedule item may end inside other schedule item time range.
        if (to2.getTime() < to1.getTime() && to2.getTime() > from1.getTime()) {
            return false;
        }
        return true;
    }

    var timetableOptionsNode = document.getElementById("timetable-drop-down-list");
    var perDayTimetableContainer = document.getElementById("per-day-timetable-container");
    var perHourTimetableContainer = document.getElementById("per-hour-timetable-container");
    var errorSpan = document.getElementById("timetable-error-span");
    var timetable = {};

    switch (parseInt(timetableOptionsNode.value)) {
        case 0:     // Timetable off
            timetable.timetable = 0;
            timetable.perDayReservations = null;
            timetable.scheduleItems = null;
            errorSpan.setAttribute("data-valid", "true");
            errorSpan.innerText = "";
            return timetable;
        case 1:     // Timetable per day
            var inputs = perDayTimetableContainer.getElementsByTagName("input");
            var perDayReservations = parseInt(inputs[0].value);
            if (isNaN(perDayReservations) ||    // Amount of reservations cannot be text value.
                perDayReservations < 1 ||       // Amount of reservations must be larger than 0.
                perDayReservations > 1000) {    // Amount of reservations must be smaller than 1000.
                errorSpan.setAttribute("data-valid", "false");
                errorSpan.innerText = "Amount of reservations is invalid.";
                return null;
            }
            errorSpan.setAttribute("data-valid", "true");
            errorSpan.innerText = "";
            timetable.timetable = 1;
            timetable.perDayReservations = perDayReservations;
            timetable.scheduleItems = null;
            return timetable;
        case 2:     // Timetable per hour.
            var outputScheduleItems = [];
            var scheduleItems = perHourTimetableContainer.children;
            // There must be at least two schedule items (one of them is empty).
            if (scheduleItems.length == 1) {
                errorSpan.setAttribute("data-valid", "false");
                errorSpan.innerText = "Please add at least one schedule item.";
                return null;
            }
            // Validate reservations amount.
            // Break loop if at least one reservations amount is invalid.
            for (var i = 0; i < scheduleItems.length; i++) {
                var inputs = scheduleItems[i].getElementsByTagName("input");
                // Skip this item if its empty.
                if (inputs[0].value == "" &&
                    inputs[1].value == "" &&
                    inputs[2].value.trim() == "") {
                    continue;
                }
                var reservations = parseInt(inputs[2].value);
                if (isNaN(reservations) ||          // Amount of reservations cannot be text value.
                    reservations < 1 ||             // Amount of reservations must be larger than 0.
                    reservations > 1000) {          // Amount of reservations must be smaller than 1000.
                    errorSpan.setAttribute("data-valid", "false");
                    errorSpan.innerText = "Schedule item number " + (i + 1) + " has invalid amount of reservations.";
                    return null;
                }
                outputScheduleItems.push({ from: null, to: null, maxReservations: reservations });
            }
            // Validate time ranges empty.
            // Break loop if at least one time range is empty.
            for (var i = 0; i < scheduleItems.length; i++) {
                var inputs = scheduleItems[i].getElementsByTagName("input");
                // Skip this item if its empty.
                if (inputs[0].value == "" &&
                    inputs[1].value == "" &&
                    inputs[2].value.trim() == "") {
                    continue;
                }
                var from = inputs[0].value;
                var to = inputs[1].value;
                // Check time range only if reservations amount is not empty.
                if (from == "" || to == "") {
                    errorSpan.setAttribute("data-valid", "false");
                    errorSpan.innerText = "Schedule item number " + (i + 1) + " has empty time range.";
                    return null;
                }
            }
            // Validate single time range
            // Break loop if at least one time range is invalid.
            for (var i = 0; i < scheduleItems.length; i++) {
                var inputs = scheduleItems[i].getElementsByTagName("input");
                // Skip this item if its empty.
                if (inputs[0].value == "" &&
                    inputs[1].value == "" &&
                    inputs[2].value.trim() == "") {
                    continue;
                }
                var from = new Date("January 01, 2000 " + inputs[0].value);
                var to = new Date("January 01, 2000 " + inputs[1].value);
                if (from.getTime() >= to.getTime()) {
                    errorSpan.setAttribute("data-valid", "false");
                    errorSpan.innerText = "Schedule item number " + (i + 1) + " have invalid time range.";
                    return null;
                }
            }

            // Validate time collisions.
            // Break loop if at least one time collision occurs.
            for (var i = 0; i < scheduleItems.length; i++) {
                var inputs = scheduleItems[i].getElementsByTagName("input");
                // Skip this item if its empty.
                if (inputs[0].value == "" &&
                    inputs[1].value == "" &&
                    inputs[2].value.trim() == "") {
                    continue;
                }
                var from1 = new Date("January 01, 2000 " + inputs[0].value);
                var to1 = new Date("January 01, 2000 " + inputs[1].value);
                for (var j = 0; j < scheduleItems.length; j++) {
                    // Check collision for different schedule items.
                    if (i != j) {
                        var inputs2 = scheduleItems[j].getElementsByTagName("input");
                        // Skip this item if its empty.
                        if (inputs2[0].value == "" &&
                            inputs2[1].value == "" &&
                            inputs2[2].value.trim() == "") {
                            continue;
                        }
                        var from2 = new Date("January 01, 2000 " + inputs2[0].value);
                        var to2 = new Date("January 01, 2000 " + inputs2[1].value);
                        if (timeRangeValidation(from1, to1, from2, to2) == false) {
                            errorSpan.setAttribute("data-valid", "false");
                            errorSpan.innerText = "Schedule item number " + (i + 1) + " and " + (j + 1) + " have colliding time range.";
                            return null;
                        }
                    }
                }
                outputScheduleItems[i].from = from1.getHours();
                outputScheduleItems[i].to = to1.getHours();
                if (outputScheduleItems[i].to == 0) {
                    outputScheduleItems[i].to = 24;
                }
            }
            // Everything is correct.
            errorSpan.setAttribute("data-valid", "true");
            errorSpan.innerText = "";
            timetable.timetable = 2;
            timetable.perDayReservations = null;
            timetable.scheduleItems = outputScheduleItems;
            return timetable;
    }
}

/**
 * Handles payments validation. At least one field with value
 * is required. If just single field is detected null is returned 
 * and proper message is printed, otherwise array of objects {type,value}
 * containing payments methods is returned.
 */
function handlePaymentInfoValidation() {
    var container = document.getElementById("payments-container");
    var errorSpan = document.getElementById("payments-error-span");
    var inputFields = container.getElementsByTagName("input");
    var selectFields = container.getElementsByTagName("select");
    var paymentMethods = [];

    // Single input field.
    if (inputFields.length == 1) {
        errorSpan.setAttribute("data-valid", "false");
        errorSpan.innerText = "Please insert at least one payment method.";
        return null;
    }

    // Collect data.
    for (var i = 0; i < inputFields.length; i++) {
        if (inputFields[i].value.trim() != "") {
            paymentMethods.push({ type: parseInt(selectFields[i].value), value: inputFields[i].value });
        }
    }

    errorSpan.setAttribute("data-valid", "true");
    errorSpan.innerText = "";
    return paymentMethods;
}

/**
 * Handles contacts validation. At least one field with value
 * is required. If just single field is detected null is returned 
 * and proper message is printed, otherwise array of objects {type,value}
 * containing contact methods is returned.
 */
function handleContactInfoValidation() {
    var container = document.getElementById("contacts-container");
    var errorSpan = document.getElementById("contacts-error-span");
    var inputFields = container.getElementsByTagName("input");
    var selectFields = container.getElementsByTagName("select");
    var contactMethods = [];

    // Single input field.
    if (inputFields.length == 1) {
        errorSpan.setAttribute("data-valid", "false");
        errorSpan.innerText = "Please insert at least one contact.";
        return null;
    }

    // Collect data.
    for (var i = 0; i < inputFields.length; i++) {
        if (inputFields[i].value.trim() != "") {
            contactMethods.push({ type: parseInt(selectFields[i].value), value: inputFields[i].value });
        }
    }

    errorSpan.setAttribute("data-valid", "true");
    errorSpan.innerText = "";
    return contactMethods;
}

/**
 * Handles description validation, also updates character counter.
 * If text length is too large or equal zero, null is returned and
 * proper message printed, otherwise function returns textarea value.
 */
function handleTextareaValidation() {
    var textarea = document.getElementById("description-text-area");
    var textMaxLength = 8192;
    var text = textarea.value.trim();
    var errorSpan = document.getElementById("text-area-counter-span");
    var counterSpan = document.getElementById("text-area-error-span");

    // Update counter.
    counterSpan.innerText = "Characters: " + text.length + "/" + textMaxLength;

    // Validate.
    if (text == "") {
        errorSpan.innerText = "Description cannot be empty.";
        errorSpan.setAttribute("data-valid", "false");
        return null;
    }
    if (text.length > textMaxLength) {
        errorSpan.innerText = "Description is too long.";
        errorSpan.setAttribute("data-valid", "false");
        return null;
    }
    errorSpan.innerText = "";
    errorSpan.setAttribute("data-valid", "true");
    return text;

}

/**
 * Handles date range validation. If one of fields is empty or
 * date range is invalid (to before from, to before or equal today,
 * from before 01-01-2000) returns null and proper message. If
 * everything is correct, returns array with two Date() objects (from and to).
 */
function handleDateRangeValidation() {
    var container = document.getElementById("date-range-input-row");
    var errorSpan = document.getElementById("date-error-span");
    var dateRange = [];
    var inputFields = container.getElementsByTagName("input");
    var anyFieldEmpty = false;
    for (var i = 0; i < inputFields.length; i++) {
        if (inputFields[i].value.trim() == "") {
            anyFieldEmpty = true;
            break;
        }
        else {
            dateRange.push(inputFields[i].value);
        }
    }
    // Required field validation.
    if (anyFieldEmpty) {
        errorSpan.setAttribute("data-valid", "false");
        errorSpan.innerText = "One of required fields is empty.";
        return null;
    }
    // Proper data range validation.
    var fromDate = new Date(dateRange[0]);
    var toDate = new Date(dateRange[1]);
    var today = new Date();
    fromDate.setHours(0, 0, 0, 0);
    toDate.setHours(0, 0, 0, 0);
    today.setHours(0, 0, 0, 0);
    if (fromDate.getTime() > toDate.getTime() ||                    // From cannot be after to.
        toDate.getTime() <= today.getTime() ||                      // To cannot be today or before.
        fromDate.getTime() < (new Date("01-01-2000")).getTime()     // From cannot be before 01-01-2000
    ) {
        errorSpan.setAttribute("data-valid", "false");
        errorSpan.innerText = "Invalid date range.";
        return null;
    }
    errorSpan.setAttribute("data-valid", "false");
    errorSpan.innerText = "";
    return [fromDate, toDate];
}

/**
 * Performs address validation. If any field is empty, proper
 * error message is displayed and null is returned. If address
 * is correct this function returns array of strings that
 * contain country, region, city, street, street number.
 */
function handleAddressValidation() {
    var addressContainer = document.getElementById("address-input-row");
    var errorSpan = document.getElementById("address-error-span");
    var inputFields = addressContainer.getElementsByTagName("input");
    var address = [];
    var anyFieldEmpty = false;
    for (var i = 0; i < inputFields.length; i++) {
        if (inputFields[i].value.trim() == "") {
            anyFieldEmpty = true;
            break;
        }
        else {
            address.push(inputFields[i].value);
        }
    }
    if (anyFieldEmpty) {
        errorSpan.innerText = "One of required fields is empty.";
        errorSpan.setAttribute("data-valid", "false");
        return null;
    }
    else {
        errorSpan.innerText = "";
        errorSpan.setAttribute("data-valid", "true");
        return address;
    }
}

function addTimeTableInput(item, maxInputCount) {
    var container = item.parentNode.parentNode;
    var items = container.children;
    var itemClone = items[0].cloneNode(true);
    var itemCloneInputs = itemClone.getElementsByTagName("input");
    // Clear clone inputs.
    for (var i = 0; i < itemCloneInputs.length; i++) {
        itemCloneInputs[i].value = "";
    }
    var itemInnerHTML = itemClone.innerHTML;
    // Mark all nodes that should be removed.
    var nodesToRemove = [];
    for (var i = 0; i < items.length; i++) {
        var inputs = items[i].getElementsByTagName("input");
        var allInputsEmpty = true;
        for (var j = 0; j < inputs.length; j++) {
            if (inputs[j].value.trim() != "") {
                allInputsEmpty = false;
                break;
            }
        }
        if (allInputsEmpty == true) {
            nodesToRemove.push(items[i]);
        }
    }

    // Remove marked nodes.
    while (nodesToRemove.length != 0) {
        var child = nodesToRemove.pop();
        var parent = child.parentNode;
        parent.removeChild(child);
    }

    // Add empty input at the end if possible.
    if (items.length < maxInputCount) {
        var newItem = document.createElement("div");
        container.appendChild(newItem);
        newItem.innerHTML = itemInnerHTML;
    }
}

function addInput(item, maxInputCount) {
    var container = item.parentNode.parentNode; //  Div collection container.
    var items = container.children;         // Collection of div nodes.
    var itemInnerHTML = items[0].innerHTML; // Inner html of first div node.
    // Mark all nodes that should be removed.
    var nodesToRemove = [];
    for (var i = 0; i < items.length; i++) {
        var input = items[i].getElementsByTagName("input");
        // Verify last input.
        if (input[input.length - 1].value.trim() == "") {
            nodesToRemove.push(items[i]);
        }
    }

    // Remove marked nodes.
    while (nodesToRemove.length != 0) {
        var child = nodesToRemove.pop();
        var parent = child.parentNode;
        parent.removeChild(child);
    }

    // Add empty input at the end if possible.
    if (items.length < maxInputCount) {
        var newItem = document.createElement("div");
        container.appendChild(newItem);
        newItem.innerHTML = itemInnerHTML;
    }
}

/**
 * Handles timetable visibility. Every time timetable option is changed
 * proper additional options (per day reservations or schedule items manager)
 * are hidden or turned vi
 */
function handleTimetableVisibility() {
    var timetableOptionsNode = document.getElementById("timetable-drop-down-list");
    var perDayReservationsNode = document.getElementById("per-day-timetable-container");
    var perHourReservationsNode = document.getElementById("per-hour-timetable-container");
    switch (parseInt(timetableOptionsNode.value)) {
        case 0:
            perDayReservationsNode.hidden = true;
            perHourReservationsNode.hidden = true;
            break;
        case 1:
            perDayReservationsNode.hidden = false;
            perHourReservationsNode.hidden = true;
            break;
        case 2:
            perDayReservationsNode.hidden = true;
            perHourReservationsNode.hidden = false;
            break;
    }
    // Perform validation to correct error span value.
    handleTimetableValidation();
}

function handleTypeVisibility() {
    var typeElement = document.getElementById("announcement-type-drop-down-list");
    var houseSubtypeElement = document.getElementById("house-subtype-drop-down-list");
    var entertainmentSubtypeElement = document.getElementById("entertainment-subtype-drop-down-list");
    var foodSubtypeElement = document.getElementById("food-subtype-drop-down-list");
    var sharedPartElement = document.getElementById("house-shared-part-drop-down-list");
    var sharedPartLabelElement = document.getElementById("house-shared-part-label");
    switch (parseInt(typeElement.value)) {
        case 0:
            houseSubtypeElement.hidden = false;
            entertainmentSubtypeElement.hidden = true;
            foodSubtypeElement.hidden = true;
            sharedPartElement.hidden = false;
            sharedPartLabelElement.hidden = false;
            break;
        case 1:
            houseSubtypeElement.hidden = true;
            entertainmentSubtypeElement.hidden = true;
            foodSubtypeElement.hidden = false;
            sharedPartElement.hidden = true;
            sharedPartLabelElement.hidden = true;
            break;
        case 2:
            houseSubtypeElement.hidden = true;
            entertainmentSubtypeElement.hidden = true;
            foodSubtypeElement.hidden = false;
            sharedPartElement.hidden = true;
            sharedPartLabelElement.hidden = true;
            break;
    }
}