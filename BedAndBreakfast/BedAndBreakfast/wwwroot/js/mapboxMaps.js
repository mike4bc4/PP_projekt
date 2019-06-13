function createMap(zoom, center) {

    mapboxgl.accessToken = 'pk.eyJ1IjoicGlja2FzaG9lIiwiYSI6ImNqd3V4d3pzczAwYTIzeWxnb3B0Z2tmbDAifQ.Pp1RsX3lhGM56Pmfu-q2AQ';
    var mapboxClient = mapboxSdk({ accessToken: mapboxgl.accessToken });

    var map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/streets-v11',
        center: center,
        zoom: zoom
    });

    window["mapboxClient"] = mapboxClient;
    window["mapboxMap"] = map;
}


function performGeocoding(geocoderQuery, popupHtmlContent, markerID, zoom = 15, speed = 0.5) {
    var mapboxClient = window["mapboxClient"];
    var map = window["mapboxMap"];
    // Do not perform geocoding if any of conditions is not met.
    if (map === undefined ||
        mapboxClient == undefined ||
        geocoderQuery == "" ||
        popupHtmlContent == "") {
        return;
    }
    // Start geocoding.
    mapboxClient.geocoding.forwardGeocode({
        query: geocoderQuery,
        autocomplete: false,
        limit: 1
    })
        .send()
        .then(function (response) {
            if (response && response.body && response.body.features && response.body.features.length) {
                var feature = response.body.features[0];
                // Move map camera to decoded position.
                map.flyTo({
                    center: feature.center,
                    zoom: zoom,
                    speed: speed,
                });
                // Create popup.
                var popup = new mapboxgl.Popup({ offset: 25 })
                    .setHTML(popupHtmlContent);

                // Add marker and store it in array.
                if (window["mapboxMapMarkers"] === undefined) {
                    window["mapboxMapMarkers"] = [];
                }
                var marker = new mapboxgl.Marker()
                    .setLngLat(feature.center)
                    .setPopup(popup) // sets a popup on this marker
                    .addTo(map);
                window["mapboxMapMarkers"].push({
                    marker: marker,
                    markerPopup: popup,
                    markerID: markerID,
                });
            }
            else {
                alert("Geocoding failed.");
            }
        });
}

function removeMapMarker(markerID) {
    if (window["mapboxMapMarkers"] === undefined) {
        return -1;
    }
    var markerItemsToRemove = [];
    // Find items with given ID.
    for (var i = 0; i < window["mapboxMapMarkers"].length; i++) {
        if (window["mapboxMapMarkers"][i].markerID == markerID) {
            markerItemsToRemove.push({
                markerItem: window["mapboxMapMarkers"][i],
                markerItemIndex: i,
            });
        }
    }
    if (markerItemsToRemove.length == 0) {
        return 0;
    }
    // Perform removal.
    for (var i = 0; i < markerItemsToRemove.length; i++) {
        markerItemsToRemove[i].markerItem.markerPopup.remove();
        markerItemsToRemove[i].markerItem.marker.remove();
        window["mapboxMapMarkers"][markerItemsToRemove[i].markerItemIndex] = null;
    }
    for (var i = 0; i < window["mapboxMapMarkers"].length; i++) {
        if (window["mapboxMapMarkers"][i] == null) {
            window["mapboxMapMarkers"].splice(i, 1);
            i--;
        }
    }
    return 1;
}