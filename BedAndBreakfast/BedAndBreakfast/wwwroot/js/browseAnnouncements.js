
function sortAnnouncements(announcements, sortingFunctionIndex = 0) {
    if (announcements == undefined || announcements == null) {
        return;
    }
    // Sort announcements.
    var sortedAnnouncements = [];
    switch (sortingFunctionIndex) {
        case 0:
            sortedAnnouncements = announcements.sort(SortingExtensions.RatingAscending);
            break;
        case 1:
            sortedAnnouncements = announcements.sort(SortingExtensions.RatingDescending);
            break;
        case 2:
            sortedAnnouncements = announcements.sort(SortingExtensions.ReviewAmountAscending);
            break;
        case 3:
            sortedAnnouncements = announcements.sort(SortingExtensions.ReviewAmountDescending);
            break;
        case 4:
            sortedAnnouncements = announcements.sort(SortingExtensions.ReservationsAscending);
            break;
        case 5:
            sortedAnnouncements = announcements.sort(SortingExtensions.ReservationsDescending);
            break;
    }
    return sortedAnnouncements
}

function handleSortOptionChange(dropDownList) {
    function getCurrentlyVisibleAnnouncementsIDs() {
        var announcementPreviewsContainer = document.getElementById("announcement-previews-container");
        var IDsArray = [];
        // First two items are prototype and message div.
        for (var i = 2; i < announcementPreviewsContainer.children.length; i++) {
            IDsArray.push(parseInt(announcementPreviewsContainer.children[i].id.split("-").pop()));
        }
        return IDsArray;
    }

    // Base announcement data.
    var announcements = getAnnouncements();
    if (announcements.length == 0) {
        return;
    }

    // Get ids of visible announcements.
    var announcementPreviewsIDs = getCurrentlyVisibleAnnouncementsIDs();
    // Get visible announcements data.
    var announcementPreviews = [];
    for (var i = 0; i < announcements.length; i++) {
        for (var j = 0; j < announcementPreviewsIDs.length; j++) {
            if (announcementPreviewsIDs[j] == announcements[i].announcementID) {
                announcementPreviews.push(announcements[i]);
                break;
            }
        }
    }
    // Remove map markers for visible announcements.
    for (var i = 0; i < announcementPreviews.length; i++) {
        handleToggleVisibleOnMapClick(announcementPreviews[i].announcementID, true);
    }

    // Sort announcements.
    var announcementsToDraw = sortAnnouncements(announcementPreviews, parseInt(dropDownList.value));

    // Draw sorted announcements.
    drawAnnouncementList(announcementsToDraw);

    // Mark first filtered element on map if possible.
    if (announcementsToDraw.length != 0) {
        handleToggleVisibleOnMapClick(announcementsToDraw[0].announcementID);
    }


}

class SortingExtensions {
    static RatingAscending(announcement1, announcement2) {
        var rating1 = announcement1.averageRating;
        var rating2 = announcement2.averageRating;
        if (rating1 == null) {
            rating1 = 0;
        }
        if (rating2 == null) {
            rating2 = 0;
        }
        if (rating1 < rating2) {
            return 1;
        }
        else if (rating1 > rating2) {
            return -1;
        }
        else {
            return 0;
        }
    }
    static RatingDescending(announcement1, announcement2) {
        var rating1 = announcement1.averageRating;
        var rating2 = announcement2.averageRating;
        if (rating1 == null) {
            rating1 = 0;
        }
        if (rating2 == null) {
            rating2 = 0;
        }
        if (rating1 > rating2) {
            return 1;
        }
        else if (rating1 < rating2) {
            return -1;
        }
        else {
            return 0;
        }
    }
    static ReviewAmountAscending(announcement1, announcement2) {
        if (announcement1.reviewsCount > announcement2.reviewsCount) {
            return 1;
        }
        else if (announcement1.reviewsCount < announcement2.reviewsCount) {
            return -1;
        }
        else {
            return 0;
        }
    }
    static ReviewAmountDescending(announcement1, announcement2) {
        if (announcement1.reviewsCount < announcement2.reviewsCount) {
            return 1;
        }
        else if (announcement1.reviewsCount > announcement2.reviewsCount) {
            return -1;
        }
        else {
            return 0;
        }
    }
    static ReservationsAscending(announcement1, announcement2) {
        var reservations1 = announcement1.reservationsPerMonth;
        var reservations2 = announcement2.reservationsPerMonth;
        if (reservations1 == null) {
            reservations1 = 0;
        }
        if (reservations2 == null) {
            reservations2 = 0;
        }
        if (reservations1 > reservations2) {
            return 1;
        }
        else if (reservations1 < reservations2) {
            return -1;
        }
        else {
            return 0;
        }
    }
    static ReservationsDescending(announcement1, announcement2) {
        var reservations1 = announcement1.reservationsPerMonth;
        var reservations2 = announcement2.reservationsPerMonth;
        if (reservations1 == null) {
            reservations1 = 0;
        }
        if (reservations2 == null) {
            reservations2 = 0;
        }
        if (reservations1 < reservations2) {
            return 1;
        }
        else if (reservations1 > reservations2) {
            return -1;
        }
        else {
            return 0;
        }
    }
}



/**
 * Toggles geocoding and marker placing on the map.
 * If item was selected marker an popup will be removed otherwise
 * geocoding will be performed and map will fly to new location.
 * If geocoding is not possible simple alert will be displayed.
 */
function handleToggleVisibleOnMapClick(announcementID, forceSwitchOff = false) {

    var announcementPreviewItem = document.getElementById("announcement-preview-" + announcementID);
    // Return if no such announcement preview item.
    if (announcementPreviewItem == null) {
        return;
    }

    var toggleMapVisibleNode = announcementPreviewItem.getElementsByClassName("announcement-preview-toggle-map-visible")[0];

    // Only switching off.
    if (forceSwitchOff == true) {
        if (toggleMapVisibleNode.getAttribute("data-selected") == "false") {
            return;
        }
        else {
            toggleMapVisibleNode.setAttribute("data-selected", "false");
			toggleMapVisibleNode.style.backgroundColor = "rgba(255, 70, 50, 0.25)";
			toggleMapVisibleNode.style.color = "rgb(50, 50, 50)";
			
            removeMapMarker(announcementID)
            return;
        }
    }

    if (toggleMapVisibleNode.getAttribute("data-selected") == "false") {
        toggleMapVisibleNode.setAttribute("data-selected", "true");
		toggleMapVisibleNode.style.backgroundColor = "rgb(255, 110, 90)";
		toggleMapVisibleNode.style.color = "white";

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
		toggleMapVisibleNode.style.backgroundColor = "rgba(255, 70, 50, 0.25)";
		toggleMapVisibleNode.style.color = "rgb(50, 50, 50)";
        removeMapMarker(announcementID)
    }
}

/**
 * Filters announcement previews by selected options. By default 
 * all found preview items are displayed.
 */
function handleFilterItemClick(clickedNode) {
    var announcements = getAnnouncements();
    // Remove map tracking for all announcements.
    for (var i = 0; i < announcements.length; i++) {
        handleToggleVisibleOnMapClick(announcements[i].announcementID, true);
    }


    // Toggle filter item selection.
    if (clickedNode.getAttribute("data-selected") == "false") {
        clickedNode.setAttribute("data-selected", "true");
		clickedNode.style.backgroundColor = "rgb(255, 110, 90)";
		clickedNode.style.color = "white";
    }
    else {
        clickedNode.setAttribute("data-selected", "false");
		clickedNode.style.backgroundColor = "rgba(255, 70, 50, 0.25)";
		clickedNode.style.color = "rgb(50, 50, 50)";
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

    // Sort after filtering.
    announcementsToDraw = sortAnnouncements(announcementsToDraw, parseInt(document.getElementById("announcements-preview-sort-option").value));

    drawAnnouncementList(announcementsToDraw);

    // Mark first filtered element on map if possible.
    if (announcementsToDraw.length != 0) {
        handleToggleVisibleOnMapClick(announcementsToDraw[0].announcementID);
    }
}

function browseAnnouncementsInit() {
    var announcements = getAnnouncements();
    // Sort before drawing.
    announcements = sortAnnouncements(announcements, parseInt(document.getElementById("announcements-preview-sort-option").value));
    drawAnnouncementList(announcements);
    createMap(document.getElementById("announcement-previews-map-container"));
    if (announcements.length != 0) {
        handleToggleVisibleOnMapClick(announcements[0].announcementID);
    }
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
        announcementPreviewMessageContainer.innerText = "There are no results. Try different search query.";
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
        // Set short description.
        var descriptionString = announcements[i].description;
        if (announcements[i].description.length > 100) {
            descriptionString = announcements[i].description.substr(0, 100) + "...";
        }
        descriptionContainer.innerText = descriptionString;
        // Set rating.
        var ratingString = "No rates yet.";
        if (announcements[i].averageRating != null) {
			ratingString = "Rate: " + announcements[i].averageRating.toFixed(1); + "/10, based on: " + announcements[i].reviewsCount + " review(s).";
        }
        ratingContainer.innerText = ratingString;

        // Add preview image.
        var imageContainerNode = newNode.getElementsByClassName("announcement-preview-images-container")[0];
        ImageSwiper.Add(imageContainerNode, 300, 170, announcements[i].imagesByteArrays, announcements[i].announcementID);

        // Update redirect form.
        newNode.getElementsByTagName("form")[0].getElementsByTagName("input")[0].value = announcements[i].announcementID;
        // Update toggle visible on map.
        newNode.getElementsByClassName("announcement-preview-toggle-map-visible")[0]
            .setAttribute("onclick", "handleToggleVisibleOnMapClick(" + announcements[i].announcementID + ");");

        // Add node to container.
        announcementPreviewsContainer.appendChild(newNode);

        // Add fade effect to announcement.
        newNode.style.opacity = 0;
        Effects.OpacityFade(newNode, 0.05, 1, "announcementPreview" + announcements[i].announcementID, false);
    }

}

