

function announcementViewInit() {
    // Get announcement view model.
    var announcementViewModel = getAnnouncementViewModel();
    // Create announcement library.
    window.AnnouncementLibrary = {};
    window.RequestLibrary = {};
    AnnouncementLibrary["announcementID"] = announcementViewModel.announcementPreviewModel.announcementID;
    AnnouncementLibrary["announcementTimetable"] = announcementViewModel.timetable;
    AnnouncementLibrary["scheduleItems"] = announcementViewModel.scheduleItems;
    AnnouncementLibrary["perDayReservations"] = announcementViewModel.perDayReservations;
    AnnouncementLibrary["middleDate"] = new Date();
    AnnouncementLibrary["middleDate"].setHours(12, 0, 0, 0);
    AnnouncementLibrary["announcementFrom"] = new Date(announcementViewModel.from);
    AnnouncementLibrary["announcementTo"] = new Date(announcementViewModel.to);
    AnnouncementLibrary["announcementFrom"].setHours(0, 0, 0, 0);
    AnnouncementLibrary["announcementTo"].setHours(23, 59, 59, 0);
    // Draw announcement images.
    drawAnnouncementImage(announcementViewModel.announcementPreviewModel.imagesByteArrays, parseInt(AnnouncementLibrary["announcementID"]));
    // Draw announcement map.
    var mapContainer = document.getElementById("announcement-map-container");
    var shortDescription = announcementViewModel.announcementPreviewModel.description;
    if (shortDescription.length > 100) {
        shortDescription = shortDescription.substr(0, 100) + "...";
    }
    var announcementAddressString = document.getElementById("announcement-address-container").innerText;
    var popupContent = "<strong>" + announcementAddressString + "</strong><p>" + shortDescription + "</p>"
    drawAnnouncementMap(announcementAddressString, popupContent)
    // Fill timetable container.

    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () {
            getAnnouncementReservations(requestSynchronizer);
        },
        function () {
            getReviews(requestSynchronizer);
        },
        function () {
            if (RequestLibrary["getAnnouncementReservationsResponse"] == null || RequestLibrary["getReviewsResponse"] == null) {
                // error
                return;
            }
            drawAnnouncementReviews();
            drawAnnouncementTimetable(document.getElementById("announcement-timetable-container"));
            return;
        },
    ];
    requestSynchronizer.run();
}

function handleSubmitReservationsButton() {

    function makeReservations(requestSynchronizer) {
        if (window.RequestLibrary === undefined) {
            window.RequestLibrary = {};
        }
        $.ajax({
            url: "/Announcement/MakeReservations",
            data: { reservations: RequestLibrary["reservations"] },
            dataType: "json",
            method: "post",
            success: function (response) {
                RequestLibrary["makeReservationsResponse"] = response;
                requestSynchronizer.generator.next();
            }
        });
    }

    function setErrorSpanMessage(message) {
        var errorSpan = document.getElementById("announcement-reservation-error-span");
        if (AnnouncementLibrary["submitErrorSpanTimeout"] != null) {
            clearTimeout(AnnouncementLibrary["submitErrorSpanTimeout"]);
        }
        errorSpan.innerText = message;
        AnnouncementLibrary["submitErrorSpanTimeout"] = setTimeout(function () {
            errorSpan.innerText = "";
        }, 5000);
    }

    var reservationItemsContainer = document.getElementById("announcement-reservations-container");
    // If contains only prototype.
    if (reservationItemsContainer.children.length == 1) {
        setErrorSpanMessage("Please add some reservations.");
        return;
    }

    var reservations = [];
    if (AnnouncementLibrary["announcementTimetable"] == 1) {
        for (var i = 1; i < reservationItemsContainer.children.length; i++) {
            reservations.push({
                announcementID: AnnouncementLibrary["announcementID"],
                date: reservationItemsContainer.children[i].getAttribute("data-date"),
                reservations: reservationItemsContainer.children[i].getElementsByClassName("announcement-reservation-input-field")[0].value,
            });
        }
    }
    else if (AnnouncementLibrary["announcementTimetable"] == 2) {
        for (var i = 1; i < reservationItemsContainer.children.length; i++) {
            reservations.push({
                announcementID: AnnouncementLibrary["announcementID"],
                date: reservationItemsContainer.children[i].getAttribute("data-date"),
                reservations: parseInt(reservationItemsContainer.children[i].getElementsByClassName("announcement-reservation-input-field")[0].value),
                from: parseInt(reservationItemsContainer.children[i].getAttribute("data-from")),
                to: parseInt(reservationItemsContainer.children[i].getAttribute("data-to")),
                maxReservations: parseInt(reservationItemsContainer.children[i].getAttribute("data-maxReservations")),
            });
        }
    }

    var requestSynchronizer = new RequestSynchronizer();
    RequestLibrary["reservations"] = reservations;
    requestSynchronizer.requestQueue = [
        function () {
            makeReservations(requestSynchronizer);
        },
        function () {
            if (RequestLibrary["makeReservationsResponse"] == null) {
                setErrorSpanMessage("An error occurred while making reservations.");
                return;
            }
            else {
                setErrorSpanMessage("Your reservation has been added successfully.");
                // Send message.
                var context = {};
                context.announcementID = AnnouncementLibrary["announcementID"];
                context.userNames = [];
                context.title = "New reservation";
                context.dateStarted = new Date();
                // Schedule items IDs are returned with response.
                context.scheduleItemsIDs = reservations.scheduleItemsIDs;
                context.readOnly = false;
                context.content = MessageContentCreator.CreateNewReservationContent(reservations);
                context.dateSend = new Date();
                // Get user names, create conversation and send message.
                var messageSynchronizer = new RequestSynchronizer();
                messageSynchronizer.requestQueue = [
                    function () {
                        getCurrentUserName(context, messageSynchronizer);
                    },
                    function () {
                        getAnnouncementOwnerUserName(context, messageSynchronizer);
                    },
                    function () {
                        createConversation(context, messageSynchronizer);
                    },
                    function () {
                        addMessage(context, messageSynchronizer);
                    },
                ];
                messageSynchronizer.run();


                getAnnouncementReservations(requestSynchronizer);
            }
        },
        function () {
            // Redraw announcement timetable and clear reservations.
            drawAnnouncementTimetable(document.getElementById("announcement-timetable-container"));
            while (reservationItemsContainer.children.length != 1) {
                reservationItemsContainer.removeChild(reservationItemsContainer.children[reservationItemsContainer.children.length - 1]);
            }
            return;
        },


    ];
    requestSynchronizer.run();

}

function handleAskAboutAnnouncementButtonClick() {
    var context = {};
    context.announcementID = AnnouncementLibrary["announcementID"];
    context.userNames = [];
    context.title = "Question about announcement";
    context.dateStarted = new Date();
    // Schedule items IDs are returned with response.
    context.scheduleItemsIDs = [];
    context.readOnly = false;
    context.content = MessageContentCreator.CreateAskAboutAnnouncementContent(context.announcementID);
    context.dateSend = new Date();
    // Get user names, create conversation and send message.
    var messageSynchronizer = new RequestSynchronizer();
    messageSynchronizer.requestQueue = [
        function () {
            getCurrentUserName(context, messageSynchronizer);
        },
        function () {
            getAnnouncementOwnerUserName(context, messageSynchronizer);
        },
        function () {
            createConversation(context, messageSynchronizer);
        },
        function () {
            addMessage(context, messageSynchronizer);
        },
    ];
    messageSynchronizer.run();
    document.getElementById("ask-about-announcement-message-span").innerText = "Conversation has been created. Check out your conversations.";
}

function handlePostReviewButtonClick() {

    function setErrorSpanMessage(message) {
        var errorSpan = document.getElementById("review-creator-error-span");
        if (AnnouncementLibrary["postReviewErrorSpanTimeout"] != null) {
            clearTimeout(AnnouncementLibrary["postReviewErrorSpanTimeout"]);
        }
        errorSpan.innerText = message;
        AnnouncementLibrary["postReviewErrorSpanTimeout"] = setTimeout(function () {
            errorSpan.innerText = "";
        }, 5000);
    }

    function postReview(requestSynchronizer) {
        if (window.RequestLibrary === undefined) {
            RequestLibrary = {};
        }
        $.ajax({
            url: "/Announcement/PostReview",
            data: {
                announcementID: AnnouncementLibrary["announcementID"],
                reviewModel: {
                    name: RequestLibrary["reviewName"],
                    rating: RequestLibrary["reviewRating"],
                    content: RequestLibrary["reviewContent"],
                    reviewDate: new Date().toISOString(),
                },
            },
            method: "post",
            dataType: "json",
            success: function (response) {
                RequestLibrary["postReviewResponse"] = response;
                requestSynchronizer.generator.next();
            }
        });
    }

    var rate = handleRateValidation();
    var content = handleReviewTextareaInput();
    var name = document.getElementById("review-creator-nickname-input").value;
    if (name == "") {
        name = null;
    }
    if (rate == null || content == null) {
        return;
    }
    var requestSynchronizer = new RequestSynchronizer();
    RequestLibrary["reviewName"] = name;
    RequestLibrary["reviewRating"] = rate;
    RequestLibrary["reviewContent"] = content;
    requestSynchronizer.requestQueue = [
        function () {
            postReview(requestSynchronizer);
        },
        function () {
            if (RequestLibrary["postReviewResponse"] == null) {
                // error
                setErrorSpanMessage("An error occurred while posting review.");
                return;
            }
            else {
                setErrorSpanMessage("Review successfully created.");
                getReviews(requestSynchronizer);
            }
        },
        function () {
            // Clear and redraw.
            document.getElementById("review-creator-rating-input").value = "";
            document.getElementById("review-creator-nickname-input").value = "";
            document.getElementById("review-creator-textarea").value = "";
            document.getElementById("review-creator-textarea-counter").innerText = "";
            drawAnnouncementReviews();
        },
    ];
    requestSynchronizer.run();


}

function handleTimetablePreviousButtonClick() {
    if (parseInt(AnnouncementLibrary["announcementTimetable"]) == 1) {
        AnnouncementLibrary["middleDate"] = new Date(AnnouncementLibrary["middleDate"].getTime() - 24 * 60 * 60 * 1000 * 7);
    }
    else if (parseInt(AnnouncementLibrary["announcementTimetable"]) == 2) {
        AnnouncementLibrary["middleDate"] = new Date(AnnouncementLibrary["middleDate"].getTime() - 24 * 60 * 60 * 1000);
    }
    else {
        return;
    }
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () {
            getAnnouncementReservations(requestSynchronizer);
        },
        function () {
            if (RequestLibrary["getAnnouncementReservationsResponse"] == null) {
                // error
                return;
            }
            drawAnnouncementTimetable(document.getElementById("announcement-timetable-container"));
            return;
        },
    ];
    requestSynchronizer.run();
}

function handleTimetableNextButtonClick() {
    if (parseInt(AnnouncementLibrary["announcementTimetable"]) == 1) {
        AnnouncementLibrary["middleDate"] = new Date(AnnouncementLibrary["middleDate"].getTime() + 24 * 60 * 60 * 1000 * 7);
    }
    else if (parseInt(AnnouncementLibrary["announcementTimetable"]) == 2) {
        AnnouncementLibrary["middleDate"] = new Date(AnnouncementLibrary["middleDate"].getTime() + 24 * 60 * 60 * 1000);
    }
    else {
        return;
    }
    var requestSynchronizer = new RequestSynchronizer();
    requestSynchronizer.requestQueue = [
        function () {
            getAnnouncementReservations(requestSynchronizer);
        },
        function () {
            if (RequestLibrary["getAnnouncementReservationsResponse"] == null) {
                // error
                return;
            }
            drawAnnouncementTimetable(document.getElementById("announcement-timetable-container"));
            return;
        },
    ];
    requestSynchronizer.run();
}

function handleAddReservationButtonClick(itemClickedIndex) {
	if (AnnouncementLibrary["announcementTimetable"] == 1) {

		var availableReservations = window.AnnouncementLibrary["perDayReservations"] -
			window.RequestLibrary["getAnnouncementReservationsResponse"].reservations[itemClickedIndex];
		if (availableReservations < 1) {
			return;
		}
		
        var dateString = document.getElementById("timetable-date-container-" + itemClickedIndex).innerText;
        var reservationItem = document.getElementById("reservation-item-" + dateString);
        if (reservationItem == null) {
            reservationItem = drawReservationItem(dateString, itemClickedIndex);
            reservationItem.setAttribute("data-date", dateString);
        }
        else {
            handleReservationsItemIncreaseButtonClick("reservation-item-" + dateString, itemClickedIndex, 1);
        }
    }
	else if (AnnouncementLibrary["announcementTimetable"] == 2) {

		var availableReservations = window.AnnouncementLibrary["scheduleItems"][itemClickedIndex].maxReservations -
			window.RequestLibrary["getAnnouncementReservationsResponse"].reservations[itemClickedIndex];

		if (availableReservations < 1) {
			return;
		}

        var dateString = document.getElementById("timetable-date-container").innerText + ", " +
            document.getElementById("timetable-schedule-item-container-" + itemClickedIndex).innerText;
        var reservationItem = document.getElementById("reservation-item-" + dateString);
        if (reservationItem == null) {
            reservationItem = drawReservationItem(dateString, itemClickedIndex);
            reservationItem.setAttribute("data-from", AnnouncementLibrary["scheduleItems"][itemClickedIndex].from);
            reservationItem.setAttribute("data-to", AnnouncementLibrary["scheduleItems"][itemClickedIndex].to);
            reservationItem.setAttribute("data-maxReservations", AnnouncementLibrary["scheduleItems"][itemClickedIndex].maxReservations);
            reservationItem.setAttribute("data-date", document.getElementById("timetable-date-container").innerText);
        }
        else {
            handleReservationsItemIncreaseButtonClick("reservation-item-" + dateString, itemClickedIndex, 1);
        }
    }

}

function handleRateValidation() {
    var rateInput = document.getElementById("review-creator-rating-input");
    var errorSpan = document.getElementById("review-creator-rating-error-span");
    if (errorSpan.value = "") {
        errorSpan.innerText = "This field is required.";
        return null;
    }

    var rate = parseInt(rateInput.value);
    if (isNaN(rate) ||
        rate < 1 ||
        rate > 10) {
        errorSpan.innerText = "Rate should be number from 1 to 10.";
        return null;
    }

    errorSpan.innerText = "";
    return rate;
}

function handleReviewTextareaInput() {
    var textarea = document.getElementById("review-creator-textarea");
    var errorSpan = document.getElementById("review-creator-textarea-error-span");
    var counterNode = document.getElementById(textarea.id + "-counter");
    var textareaText = textarea.value.trim();
    counterNode.innerText = "Characters: " + textareaText.length + "/512"

    if (textareaText.length > 512) {
        errorSpan.innerText = "Review is too long.";
        return null;
    }
    if (textareaText.length == 0) {
        errorSpan.innerText = "Review cannot be empty.";
        return null;
    }
    errorSpan.innerText = "";

    return textareaText;
}

function getAnnouncementReservations(requestSynchronizer) {
    // Create request library if it does not exist.
    if (window.RequestLibrary === undefined) {
        window.RequestLibrary = {};
    }
    $.ajax({
        url: "/Announcement/GetReservations",
        data: {
            announcementID: window.AnnouncementLibrary["announcementID"],
            date: AnnouncementLibrary["middleDate"].toISOString(),
        },
        dataType: "json",
        method: "post",
        success: function (response) {
            window.RequestLibrary["getAnnouncementReservationsResponse"] = response;
            requestSynchronizer.generator.next();
        }
    });
}

function getReviews(requestSynchronizer) {
    if (window.RequestLibrary === undefined) {
        window.RequestLibrary = {};
    }
    $.ajax({
        url: "/Announcement/GetReviews",
        data: { announcementID: AnnouncementLibrary["announcementID"] },
        dataType: "json",
        method: "post",
        success: function (response) {
            RequestLibrary["getReviewsResponse"] = response;
            requestSynchronizer.generator.next();
        }
    });
}

function drawAnnouncementReviews() {
    if (RequestLibrary["getReviewsResponse"] == undefined || RequestLibrary["getReviewsResponse"] == null) {
        return;
    }
    var reviewsContainer = document.getElementById("announcement-reviews-container");
    var reviewPrototype = document.getElementById("announcement-review-prototype");
    // Clear before drawing.
    reviewsContainer.innerHTML = "";
    reviewsContainer.appendChild(reviewPrototype);

    if (RequestLibrary["getReviewsResponse"].length == 0) {
		var p = document.createElement("p");
		p.className = "text-segue-16";
        p.innerText = "This announcement has no reviews. Consider adding one.";
        reviewsContainer.appendChild(p);
        return;
    }
    // Draw all reviews.
    for (var i = 0; i < RequestLibrary["getReviewsResponse"].length; i++) {
        var newNode = reviewPrototype.cloneNode(true);
        newNode.hidden = false;
        newNode.id = "review-item-" + i;
        newNode.getElementsByClassName("announcement-review-rating-container")[0]
            .innerText = RequestLibrary["getReviewsResponse"][i].rating + "/10";
        newNode.getElementsByClassName("announcement-review-date-container")[0]
            .innerText = new Date(RequestLibrary["getReviewsResponse"][i].reviewDate).toLocaleDateString("en-US");
        newNode.getElementsByClassName("announcement-review-name-container")[0]
            .innerText = RequestLibrary["getReviewsResponse"][i].name;
        newNode.getElementsByClassName("announcement-review-message-container")[0]
            .innerText = RequestLibrary["getReviewsResponse"][i].content;
        reviewsContainer.appendChild(newNode);
    }
}

function drawAnnouncementImage(imagesByteArrays, announcementID, width = 1000) {
    var imageContainerNode = document.getElementById("announcement-images-container");
    ImageSwiper.Add(imageContainerNode, width, Math.ceil((width / 16) * 9), imagesByteArrays, announcementID);
}

function drawAnnouncementMap(announcementAddressString, popupContent) {
    createMap(document.getElementById("announcement-map-container"));
    performGeocoding(announcementAddressString, popupContent, AnnouncementLibrary["announcementID"]);
}

function drawAnnouncementTimetable(timetableContainer) {

    function parseIntToTimeString(integer) {
        if (integer == 24) {
            return "23:59";
        }
        else {
            return integer + ":00";
        }
    }

    timetableContainer.innerHTML = "";
    switch (parseInt(AnnouncementLibrary["announcementTimetable"])) {
        case 0:     // Off
			var p = document.createElement("p");
			p.className = "text-segue-16";
            p.innerText = "There is no reservations timetable for this announcement. Ask its owner about reservations!";
			var button = document.createElement("button");
			button.className = "button-09 text-segue-14";
            button.innerText = "Start conversation";
            button.onclick = function () { handleAskAboutAnnouncementButtonClick() };
            var span = document.createElement("span");
			span.id = "ask-about-announcement-message-span";
			span.className = "error-span-label-box-01 text-segue-16";
            timetableContainer.appendChild(p);
            timetableContainer.appendChild(button);
            timetableContainer.appendChild(span);
            break;
        case 1:     // Per day
            var table = document.createElement("table");
			table.className = "table-box-03 text-segue-16";
            var tr1 = document.createElement("tr");
            var tr2 = document.createElement("tr");
            // Create first row content.
            for (var i = 0; i < 9; i++) {
				var td = document.createElement("td");
				td.className = "table-box-cell-05";
                switch (i) {
                    case 0:
                        var button = document.createElement("button");
						button.onclick = function () { handleTimetablePreviousButtonClick() };
						button.className = "button-07 text-segue-14";
                        button.innerText = "<"
                        td.appendChild(button);
                        td.rowSpan = 2;
                        tr1.appendChild(td);
                        break;
                    case 8:
                        var button = document.createElement("button");
						button.onclick = function () { handleTimetableNextButtonClick() };
						button.className = "button-07 text-segue-14";
                        button.innerText = ">";
                        td.appendChild(button);
                        td.rowSpan = 2;
                        tr1.appendChild(td);
                        break;
                    default:
						var p = document.createElement("p");
						p.className = "indent-05";
                        var date = new Date(AnnouncementLibrary["middleDate"].getTime() - 24 * 60 * 60 * 1000 * 3);
                        var correctedDate = new Date(date.getTime() + 24 * 60 * 60 * 1000 * (i - 1));
                        correctedDate.setHours(0, 0, 0, 0);

                        p.innerText = correctedDate.toLocaleDateString("en-US");
                        p.id = "timetable-date-container-" + (i - 1);
                        td.appendChild(p);
                        tr1.appendChild(td);

                        // Mark cells out of time range.
                        var today = new Date();
                        today.setHours(0, 0, 0, 0);
                        if (correctedDate.getTime() < today.getTime() ||
							correctedDate.getTime() > AnnouncementLibrary["announcementTo"].getTime()) {
							p.className = "indent-05-disabled";
                        }

                        break;
                }
            }
            // Create second row content.
            for (var i = 0; i < 7; i++) {
                var td = document.createElement("td");
				td.className = "table-box-cell-05";
                td.setAttribute("data-day", i);

				var button = document.createElement("button");
				button.className = "button-08 text-segue-14";
                button.innerText = "Add reservation";
                button.setAttribute("onclick", "handleAddReservationButtonClick(" + i + ")");
                var p = document.createElement("p");


                var reservationString = "0/" + window.AnnouncementLibrary["perDayReservations"];
                if (window.RequestLibrary["getAnnouncementReservationsResponse"].reservations[i] != null) {
                    reservationString = window.RequestLibrary["getAnnouncementReservationsResponse"].reservations[i] +
                        "/" + window.AnnouncementLibrary["perDayReservations"];
                }

				p.innerText = "Current reservations: " + reservationString;
				p.className = "indent-06";
                td.appendChild(p);
                td.appendChild(button);
                tr2.appendChild(td);

                // Disable buttons out of time range.
                var itemDate = new Date(AnnouncementLibrary["middleDate"].getTime() - 24 * 60 * 60 * 1000 * (3 - i));
                itemDate.setHours(0, 0, 0, 0);
                var today = new Date();
                today.setHours(0, 0, 0, 0);
                if (itemDate.getTime() < today.getTime() ||
                    itemDate.getTime() > AnnouncementLibrary["announcementTo"].getTime()) {
                    button.disabled = true;
                }

            }
            table.appendChild(tr1);
            table.appendChild(tr2);
            timetableContainer.appendChild(table);
            break;
        case 2:     // Per hour
            var table = document.createElement("table");
			table.className = "table-box-03 text-segue-16";
            // Create first row.
            var tr1 = document.createElement("tr");
            for (var i = 0; i < 3; i++) {
                var td = document.createElement("td");
                switch (i) {
                    case 0:
						var button = document.createElement("button");
						button.className = "button-07 text-segue-14";
                        button.innerText = "<";
                        button.onclick = function () { handleTimetablePreviousButtonClick() };
                        td.appendChild(button);
                        td.rowSpan = window.AnnouncementLibrary["scheduleItems"].length + 1;
                        break;
                    case 2:
						var button = document.createElement("button");
						button.className = "button-07 text-segue-14";
                        button.innerText = ">";
                        button.onclick = function () { handleTimetableNextButtonClick() };
                        td.appendChild(button);
                        td.rowSpan = window.AnnouncementLibrary["scheduleItems"].length + 1;
                        break;
                    default:
                        var correctedDate = AnnouncementLibrary["middleDate"];
                        correctedDate.setHours(0, 0, 0, 0);

						var p = document.createElement("p");
						p.className = "indent-05";
                        td.appendChild(p);
                        p.innerText = correctedDate.toLocaleDateString("en-US");
                        p.id = "timetable-date-container";

                        // Mark cells out of time range.
                        var today = new Date();
                        today.setHours(0, 0, 0, 0);
                        if (correctedDate.getTime() < today.getTime() ||
                            correctedDate.getTime() > AnnouncementLibrary["announcementTo"].getTime()) {
							p.className = "indent-05-disabled";
                        }

                        break;
                }
                tr1.appendChild(td);
            }
            table.appendChild(tr1);
            // Create rows for schedule items.
            for (var i = 0; i < window.AnnouncementLibrary["scheduleItems"].length; i++) {

                var tr = document.createElement("tr");
                var td = document.createElement("td");
                tr.appendChild(td);
				var button = document.createElement("button");
				button.className = "button-08 text-segue-14";
                button.innerText = "Add reservation";
                button.setAttribute("onclick", "handleAddReservationButtonClick(" + i + ")");

                var p1 = document.createElement("p");
                p1.id = "timetable-schedule-item-container-" + i;
                p1.innerText = parseIntToTimeString(AnnouncementLibrary["scheduleItems"][i].from) + " - " +
					parseIntToTimeString(AnnouncementLibrary["scheduleItems"][i].to);
				p1.className = "indent-06";

                var p2 = document.createElement("p");
                var reservationString = "0/" + window.AnnouncementLibrary["scheduleItems"][i].maxReservations;
                if (window.RequestLibrary["getAnnouncementReservationsResponse"].reservations[i] != null) {
                    reservationString = window.RequestLibrary["getAnnouncementReservationsResponse"].reservations[i] +
                        "/" + window.AnnouncementLibrary["scheduleItems"][i].maxReservations;
                }
                p2.innerText = "Current reservations: " + reservationString;
				p2.className = "indent-06";

                // Disable buttons out of time range.
                var itemDate = new Date(AnnouncementLibrary["middleDate"].getTime());
                itemDate.setHours(0, 0, 0, 0);
                var today = new Date();
                today.setHours(0, 0, 0, 0);
                if (itemDate.getTime() < today.getTime() ||
                    itemDate.getTime() > AnnouncementLibrary["announcementTo"].getTime()) {
                    button.disabled = true;
                }

                td.appendChild(p1);
                td.appendChild(p2);
                td.appendChild(button);
                table.appendChild(tr);
            }
            timetableContainer.appendChild(table);
            break;
    }

}

function handleReservationsItemIncreaseButtonClick(reservationItemID, itemClickedIndex, amount) {
    var reservationItem = document.getElementById(reservationItemID);
    if (reservationItem == null) {
        return;
    }
    var availableReservations = 0;
    // Calculate available reservations amount.
    if (window.AnnouncementLibrary["announcementTimetable"] == 1) {
        // For per day schedule.
		if (window.RequestLibrary["getAnnouncementReservationsResponse"].reservations[itemClickedIndex] != null) {
			availableReservations = window.AnnouncementLibrary["perDayReservations"] -
				window.RequestLibrary["getAnnouncementReservationsResponse"].reservations[itemClickedIndex];
        }
        else {
            availableReservations = window.AnnouncementLibrary["perDayReservations"];
        }
    }
    else if (window.AnnouncementLibrary["announcementTimetable"] == 2) {
        // For per hour schedule.
		if (window.RequestLibrary["getAnnouncementReservationsResponse"].reservations[itemClickedIndex] != null) {
            availableReservations = window.AnnouncementLibrary["scheduleItems"][itemClickedIndex].maxReservations -
				window.RequestLibrary["getAnnouncementReservationsResponse"].reservations[itemClickedIndex];
        }
        else {
            availableReservations = window.AnnouncementLibrary["scheduleItems"][itemClickedIndex].maxReservations;
        }
    }
    else {
        return;
    }
    // Increase reservations only if it is possible.
    var inputNode = reservationItem.getElementsByClassName("announcement-reservation-input-field")[0];
    var currentInputValue = parseInt(inputNode.value);
    if (availableReservations - currentInputValue > 0 && amount < availableReservations - currentInputValue) {
        inputNode.value = currentInputValue + amount;
    }
    else {
        inputNode.value = availableReservations;
    }
}

function handleReservationsItemDecreaseButtonClick(reservationItemID, amount) {
    var reservationItem = document.getElementById(reservationItemID);
    if (reservationItem == null) {
        return;
    }
    var inputNode = reservationItem.getElementsByClassName("announcement-reservation-input-field")[0];
    var currentInputValue = parseInt(inputNode.value);
    if (currentInputValue - amount <= 0) {
        inputNode.value = 1;
    }
    else {
        inputNode.value = currentInputValue - amount;
    }
}

function handleReservationsItemRemoveButtonClick(reservationItemID) {
    document.getElementById(reservationItemID)
        .parentNode.removeChild(document.getElementById(reservationItemID));
}

function drawReservationItem(dateString, itemClickedIndex) {
    var reservationsContainer = document.getElementById("announcement-reservations-container");
    var reservationItemPrototype = document.getElementById("announcement-reservation-prototype");
    var newNode = reservationItemPrototype.cloneNode(true);
    newNode.id = "reservation-item-" + dateString;
    newNode.hidden = false;
    var newNodeDateContainer = newNode.getElementsByClassName("announcement-reservation-date-container")[0];
    var newNodeDecreaseButton1 = newNode.getElementsByClassName("announcement-reservation-decrease-1-button")[0];
    var newNodeDecreaseButton10 = newNode.getElementsByClassName("announcement-reservation-decrease-10-button")[0];
    var newNodeInputField = newNode.getElementsByClassName("announcement-reservation-input-field")[0];
    var newNodeIncreaseButton1 = newNode.getElementsByClassName("announcement-reservation-increase-1-button")[0];
    var newNodeIncreaseButton10 = newNode.getElementsByClassName("announcement-reservation-increase-10-button")[0];
    var newNodeRemoveButton = newNode.getElementsByClassName("announcement-reservation-remove-button")[0];

    newNodeDateContainer.innerText = dateString;
    newNodeDecreaseButton1.setAttribute("onclick", "handleReservationsItemDecreaseButtonClick(\"" + newNode.id + "\", 1);");
    newNodeDecreaseButton10.setAttribute("onclick", "handleReservationsItemDecreaseButtonClick(\"" + newNode.id + "\", 10);");
    newNodeInputField.value = 1;
    newNodeIncreaseButton1.setAttribute("onclick", "handleReservationsItemIncreaseButtonClick(\"" + newNode.id + "\"," + itemClickedIndex + ", 1);");
    newNodeIncreaseButton10.setAttribute("onclick", "handleReservationsItemIncreaseButtonClick(\"" + newNode.id + "\"," + itemClickedIndex + ", 10);");
    newNodeRemoveButton.setAttribute("onclick", "handleReservationsItemRemoveButtonClick(\"" + newNode.id + "\");");
    reservationsContainer.appendChild(newNode);
    return newNode;
}