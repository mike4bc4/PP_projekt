


function handleSortOptionChange(dropDownList) {

}

function handleToggleVisibleOnMapClick(announcementID) {
    var announcementPreviewItem = document.getElementById("announcement-preview-" + announcementID);
    var toggleMapVisibleNode = announcementPreviewItem.getElementsByClassName("announcement-preview-toggle-map-visible")[0];
    if (toggleMapVisibleNode.getAttribute("data-selected") == "false") {
        toggleMapVisibleNode.setAttribute("data-selected", "true");
        toggleMapVisibleNode.style.backgroundColor = "lightgray";

        var address = announcementPreviewItem.getElementsByClassName("announcement-preview-address-container")[0].innerText;
        var popupContent = announcementPreviewItem.getElementsByClassName("announcement-preview-description-container")[0].innerText;
        // Shorten popup content if necessary.
        if (popupContent.length > 100) {
            popupContent = popupContent.substr(0, 100);
            popupContent += "...";
        }
        performGeocoding(address, "<strong>" + address + "</strong><p>" + popupContent + "</p>.", announcementID, 13);
    }
    else {
        toggleMapVisibleNode.setAttribute("data-selected", "false");
        toggleMapVisibleNode.style.backgroundColor = "white";
        removeMapMarker(announcementID)
    }
}

/**
 * Filters announcement previews by selected options. By default 
 * all found preview items are displayed.
 */
function handleFilterItemClick(clickedNode) {
    var filterID = parseInt(clickedNode.id.split("-").pop());
    // Toggle filter item selection.
    if (clickedNode.getAttribute("data-selected") == "false") {
        clickedNode.setAttribute("data-selected", "true");
        clickedNode.style.backgroundColor = "rgba(100,100,100)";
    }
    else {
        clickedNode.setAttribute("data-selected", "false");
        clickedNode.style.backgroundColor = "rgba(255,255,255)";
    }
    // Get selected filter options.
    var filterOptions = document.getElementById("announcements-preview-filter-options-container").getElementsByTagName("div");
    var selectedFilterOptions = [];
    for (var i = 0; i < filterOptions.length; i++) {
        if (filterOptions[i].getAttribute("data-selected") == "true") {
            selectedFilterOptions.push(parseInt(filterOptions[i].id.split("-").pop()));
        }
    }

    // Apply filters
    var announcements = getAnnouncements();
    var announcementsToDraw = announcements;
    // Apply sorting options only if there are any selected
    // otherwise print all announcements.
    if (selectedFilterOptions.length != 0) {
        announcementsToDraw = [];
        for (var i = 0; i < announcements.length; i++) {
            for (var j = 0; j < selectedFilterOptions.length; j++) {
                if (announcements[i].type == selectedFilterOptions[j]) {
                    announcementsToDraw.push(announcements[i]);
                    break;
                }
            }
        }
    }
    drawAnnouncementList(announcementsToDraw);
}

function browseAnnouncementsInit() {
    var announcements = getAnnouncements();
    drawAnnouncementList(announcements);
    createMap(2, [20, 50]);
    if (announcements.length != 0) {
        handleToggleVisibleOnMapClick(announcements[0].announcementID);
    }
    // setTimeout(function () {
    //     performGeocoding("australia melbourne albert park", "<strong>Some test popup message</strong><p>Popup description</p>.", 1, 13);
    // }, 3000);
}

/**
 * Fills announcement previews container with items containing
 * images (animated widget), type, address, description and rates.
 */
function drawAnnouncementList(announcements) {

    var announcementPreviewsContainer = document.getElementById("announcement-previews-container");
    var announcementPreviewPrototype = document.getElementById("announcement-preview-prototype");
    var announcementPreviewMessageContainer = document.getElementById("announcement-previews-message-container");
    // Clear container before drawing.
    announcementPreviewsContainer.innerHTML = "";
    // Keep prototype and message container.
    announcementPreviewsContainer.appendChild(announcementPreviewPrototype);
    announcementPreviewsContainer.appendChild(announcementPreviewMessageContainer);

    // If there are no announcement provided display message.
    if (announcements.length == 0) {
        announcementPreviewMessageContainer.innerText = "No results";
        announcementPreviewMessageContainer.hidden = false;
        return;
    }

    // If there are announcements provided hide message container.
    announcementPreviewMessageContainer.hidden = true;
    // Draw announcements.
    for (var i = 0; i < announcements.length; i++) {
        var newNode = announcementPreviewPrototype.cloneNode(true);
        // Setup new node, make ids unique.
        newNode.hidden = false;
        newNode.id = "announcement-preview-" + + announcements[i].announcementID;
        var typeContainer = newNode.getElementsByClassName("announcement-preview-type-data-container")[0];
        var addressContainer = newNode.getElementsByClassName("announcement-preview-address-container")[0];
        var descriptionContainer = newNode.getElementsByClassName("announcement-preview-description-container")[0];
        var ratingContainer = newNode.getElementsByClassName("announcement-preview-rating-container")[0];
        // Put values into containers.
        switch (parseInt(announcements[i].type)) {
            case 0:
                typeContainer.innerText = getAnnouncementTypes()[announcements[i].type] +
                    " " + getHouseSubtypes()[announcements[i].subtype] +
                    " " + getHouseSharedParts()[announcements[i].sharedPart];
                break;
            case 1:
                typeContainer.innerText = getAnnouncementTypes()[announcements[i].type] +
                    " " + getEntertainmentSubtypes()[announcements[i].subtype];
                break;
            case 2:
                typeContainer.innerText = getAnnouncementTypes()[announcements[i].type] +
                    " " + getFoodSubtypes()[announcements[i].subtype];
                break;
        }
        addressContainer.innerText = announcements[i].country +
            " " + announcements[i].region +
            " " + announcements[i].city +
            " " + announcements[i].street +
            " " + announcements[i].streetNumber;
        descriptionContainer.innerText = announcements[i].description;
        var ratingString = "No rates yet.";
        if (announcements[i].averageRating != null) {
            ratingString = averageRating + "/10";
        }
        ratingContainer.innerText = ratingString;

        // Add preview image.
        var imageContainerNode = newNode.getElementsByClassName("announcement-preview-images-container")[0];
        ImageSwiper.Add(imageContainerNode, 400, 255, announcements[i].imagesByteArrays, announcements[i].announcementID);

        // Update redirect form.
        newNode.getElementsByTagName("form")[0].getElementsByTagName("input")[0].value = announcements[i].announcementID;
        // Update toggle visible on map.
        newNode.getElementsByClassName("announcement-preview-toggle-map-visible")[0]
            .setAttribute("onclick", "handleToggleVisibleOnMapClick(" + announcements[i].announcementID + ");");

        // Add node to container.
        announcementPreviewsContainer.appendChild(newNode);
    }

}

