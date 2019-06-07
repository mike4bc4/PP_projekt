
function handlePartialViewInitialLoad(initMode) {
    drawTypeDropDownList();
    drawSubtypeAndSharedPartDropDownList();
    drawImageInput();
    drawInput("contact");
    drawInput("payment");
    drawSubmitButtonValue(initMode);
}

function handleTypeSelectChange() {
    drawSubtypeAndSharedPartDropDownList();
}

function handleRequiredFieldValidation(itemID) {
    var item = document.getElementById(itemID);
    var errorField = document.getElementById(itemID + "-error");
    if (item.value == "") {
        errorField.innerText = "This field is required."
        errorField.setAttribute("data-valid", "false");
        return false;
    }
    errorField.innerText = "";
    errorField.setAttribute("data-valid", "true");
    return true;
}

function handleDateValidation() {
    var today = new Date();
    today.setHours(0, 0, 0, 0);
    var from = document.getElementById("from-date-input-field");
    var to = document.getElementById("to-date-input-field");
    var errorField = document.getElementById("date-range-error");
    if (from < (new Date("01-01-1900")) || today > (new Date(to.value)) || (new Date(to.value)) < (new Date(from.value))) {
        errorField.innerText = "Invalid date range.";
        errorField.setAttribute("data-valid", "false");
        return false;
    }
    errorField.innerText = "";
    errorField.setAttribute("data-valid", "true");
    return true;
}

function handleInputRemoval(inputID) {
    var input = document.getElementById(inputID);
    var divElement = input.parentNode;
    divElement.parentNode.removeChild(divElement);
}

function handleAddImageButton() {
    drawImageInput();
}

function handleAddContactButton() {
    drawInput("contact");
}

function handleAddPaymentButton() {
    drawInput("payment");
}

function handleTextAreaValidation(textareaID) {
    var maxTextAreaSize = 8192;
    var textarea = document.getElementById(textareaID);
    var textareaError = document.getElementById(textareaID + "-error");
    var textareaCounter = document.getElementById(textareaID + "-counter");
    var textareaValid = true;
    if (textarea.value.length > maxTextAreaSize) {
        textareaError.innerText = "Description is too long.";
        textareaValid = false;
    }
    else if (textarea.value.length == 0) {
        textareaError.innerText = "Description cannot be empty.";
        textareaValid = false;
    }
    else {
        textareaError.innerText = "";
        textareaValid = true;
    }
    textareaCounter.innerText = "Characters " + textarea.value.length + "/" + maxTextAreaSize + ".";
    textareaError.setAttribute("data-valid", textareaValid.toString());
    return textareaValid;
}

var submitButtonMouseoverFlag = false;
var submitButtonMouseoverTimer;
var submitButtonTimerValue;
function handleSubmitButtonMouseover() {
    if (submitButtonMouseoverFlag == true) {
        return;
    }
    submitButtonMouseoverFlag = true;
    var infoContainer = document.getElementById("submit-announcement-info");
    var counterContainer = document.getElementById("submit-announcement-button-counter");
    var button = document.getElementById("submit-announcement-button");
    button.setAttribute("disabled", "disabled");
    infoContainer.innerText = "Are you sure that all data related to your announcement is valid? Double-check recommended.";
    counterContainer.innerText = "Button will be unlocked in 5 seconds";
    submitButtonTimerValue = 5;
    handleSubmitButtonMouseoverTimer();
}

function handleSubmitButtonMouseoverTimer() {
    var infoContainer = document.getElementById("submit-announcement-info");
    var counterContainer = document.getElementById("submit-announcement-button-counter");
    var button = document.getElementById("submit-announcement-button");
    submitButtonMouseoverTimer = setTimeout(function () {
        submitButtonTimerValue -= 1;
        if (submitButtonTimerValue > 0) {
            counterContainer.innerText = "Button will be unlocked in " + submitButtonTimerValue + " seconds";
            handleSubmitButtonMouseoverTimer();
        }
        else{
            infoContainer.innerText = "";
            button.removeAttribute("disabled");
            counterContainer.innerText = "";
        }
    }, 1000);
}

function handleSubmitButtonClick(){
    
}

function drawSubmitButtonValue(value){
    var button = document.getElementById("submit-announcement-button");
    button.innerText = value;
}

var paymentInputErrorTimeout;
var contactInputErrorTimeout;
function drawInput(type) {
    var container = document.getElementById(type + "-info-container");
    var errorContainer = document.getElementById(type + "-info-error");
    var currentItems = container.children;

    if (currentItems.length == 0) {
        container.innerHTML = "<div>\
            <select id='"+ type + "-input-type-0'>\
            </select>\
            <input onblur='handleRequiredFieldValidation(this.id);' id='"+ type + "-input-value-0' type='text' />\
            <button onclick='handleInputRemoval(\""+ type + "-input-value-0\");'>Remove</button>\
            <span id='"+ type + "-input-value-0-error' data-valid='false'></span>\
        </div>";
        var optionItems = [];
        if (type == "contact") {
            optionItems = getContactMethods();
        }
        else {
            optionItems = getPaymentMethods();
        }
        var dropDownList = document.getElementById(type + "-input-type-0");
        for (var i = 0; i < optionItems.length; i++) {
            dropDownList.innerHTML += "<option value='" + i + "'>" + optionItems[i] + "</option>";
        }
    }
    else if (currentItems.length < 5) {
        // Enumerate current items.
        var selectItems = container.getElementsByTagName("select");
        var inputItems = container.getElementsByTagName("input");
        var buttonElements = container.getElementsByTagName("button");
        var spanElements = container.getElementsByTagName("span");
        for (var i = 0; i < currentItems.length; i++) {
            selectItems[i].setAttribute("id", type + "-input-type-" + i);
            inputItems[i].setAttribute("id", type + "-input-value-" + i);
            buttonElements[i].setAttribute("onclick", "handleInputRemoval(\"" + type + "-input-value-" + i + "\");");
            spanElements[i].setAttribute("id", type + "-input-value-" + i + "-error");
        }
        // Create new contact input.
        container.innerHTML += "<div>\
            <select id='"+ type + "-input-type-" + currentItems.length + "'>\
            </select>\
            <input onblur='handleRequiredFieldValidation(this.id);' id='"+ type + "-input-value-" + currentItems.length + "' type='text' />\
            <button onclick='handleInputRemoval(\""+ type + "-input-value-" + currentItems.length + "\");'>Remove</button>\
            <span id='"+ type + "-input-value-" + currentItems.length + "-error' data-valid='false'></span>\
        </div>";
        // Fill drop down list.
        var optionItems = [];
        if (type == "contact") {
            optionItems = getContactMethods();
        }
        else {
            optionItems = getPaymentMethods();
        }
        var dropDownList = document.getElementById(type + "-input-type-" + (currentItems.length - 1));
        for (var i = 0; i < optionItems.length; i++) {
            dropDownList.innerHTML += "<option value='" + i + "'>" + optionItems[i] + "</option>";
        }
    }
    else {
        var timeout;
        if (type == "contact") {
            timeout = contactInputErrorTimeout;
        }
        else {
            timeout = paymentInputErrorTimeout;
        }
        if (timeout != null) {
            clearTimeout(timeout);
        }
        errorContainer.innerText = "You cannot add more than 5 " + type + " methods.";
        timeout = setTimeout(function () {
            errorContainer.innerText = "";
        }, 5000);
    }
}

var imageInputErrorTimeout;
function drawImageInput() {
    var container = document.getElementById("image-input-field-container");
    var errorContainer = document.getElementById("image-input-error");
    var currentImageInputs = container.children;

    if (currentImageInputs.length == 0) {
        container.innerHTML = "<div>\
            <input id='image-input-0' type='file' accept='image/png, image/jpeg'/>\
            <button id='image-input-removal-button-0' onclick='handleInputRemoval(\"image-input-0\");'>Remove</button>\
        </div>";
    }
    else if (currentImageInputs.length < 5) {
        // Enumerate created image inputs.
        for (var i = 0; i < currentImageInputs.length; i++) {
            var currentImageInputID = currentImageInputs[i].children[0].getAttribute("id").split("-")[2];
            currentImageInputs[i].children[0].setAttribute("id", "image-input-" + i);
            document.getElementById("image-input-removal-button-" + currentImageInputID).setAttribute("id", "image-input-removal-button-" + i);
            document.getElementById("image-input-removal-button-" + i).setAttribute("onclick", "handleInputRemoval(\"image-input-" + i + "\");");
        }
        // Create new image input.
        container.innerHTML += "<div>\
            <input id='image-input-"+ currentImageInputs.length + "' type='file' accept='image/png, image/jpeg'/>\
            <button id='image-input-removal-button-"+ currentImageInputs.length + "' \
            onclick='handleInputRemoval(\"image-input-"+ currentImageInputs.length + "\");'>Remove</button>\
        </div>";
    }
    else {
        if (imageInputErrorTimeout != null) {
            clearTimeout(imageInputErrorTimeout);
        }
        errorContainer.innerText = "You cannot add more than 5 images.";
        imageInputErrorTimeout = setTimeout(function () {
            errorContainer.innerText = "";
        }, 5000);
    }

}

function drawTypeDropDownList() {
    // Fill select type drop down list.
    var typeDropDownList = document.getElementById("type-drop-down-list");
    var announcementTypes = getAnnouncementTypes();
    var index = 0;
    for (var type of announcementTypes) {
        typeDropDownList.innerHTML += "<option value='" + index + "'>" + type + "</option>";
        index++;
    }
}

function drawSubtypeAndSharedPartDropDownList() {
    // Get drop down list and other necessary elements.
    var typeDropDownList = document.getElementById("type-drop-down-list");
    var subtypeDropDownList = document.getElementById("subtype-drop-down-list");
    var sharedPartDropDownList = document.getElementById("shared-part-drop-down-list");
    var sharedPartDropDownListContainer = document.getElementById("shared-part-drop-down-list-container");

    // Clear drop down lists if they exists before drawing.
    subtypeDropDownList.innerHTML = "";
    if (sharedPartDropDownList != null) {
        sharedPartDropDownList.innerHTML = "";
    }

    // Fill lists with proper values.
    var selectedType = parseInt(typeDropDownList.value);
    switch (selectedType) {
        case 0: // House
            var index = 0;
            var houseSubtypes = getHouseSubtypes();
            var sharedParts = getSharedParts();
            for (var subtype of houseSubtypes) {
                subtypeDropDownList.innerHTML += "<option value='" + index + "'>" + subtype + "</option>";
                index++;
            }

            // If shared parts drop down list was removed it should be recreated.
            if (sharedPartDropDownList == null) {
                sharedPartDropDownListContainer.innerHTML = "<select id='shared-part-drop-down-list'></select>";
                sharedPartDropDownList = document.getElementById("shared-part-drop-down-list");
            }

            index = 0;
            for (var sharedPart of sharedParts) {
                sharedPartDropDownList.innerHTML += "<option value='" + index + "'>" + sharedPart + "</option>";
                index++;
            }
            break;
        case 1: // Entertainment
            var index = 0;
            var entertainmentSubtypes = getEntertainmentSubtypes();
            for (var subtype of entertainmentSubtypes) {
                subtypeDropDownList.innerHTML += "<option value='" + index + "'>" + subtype + "</option>";
                index++;
            }
            sharedPartDropDownListContainer.innerHTML = "";
            break;
        case 2: // Food
            var index = 0;
            var foodSubtypes = getFoodSubtypes();
            for (var subtype of foodSubtypes) {
                subtypeDropDownList.innerHTML += "<option value='" + index + "'>" + subtype + "</option>";
                index++;
            }
            sharedPartDropDownListContainer.innerHTML = "";
            break;
        default:
            break;
    }
}