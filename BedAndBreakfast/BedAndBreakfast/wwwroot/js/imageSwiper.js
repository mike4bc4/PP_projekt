
class ImageSwiper {

    static OpacityFade(element, speed, targetOpacity, uniqueID, removeFaded = false) {

        var delta = targetOpacity - element.style.opacity;

        if (removeFaded == true) {
            var currentOpacity = parseFloat(element.style.opacity);
            var timeToDelete = (Math.abs(targetOpacity - currentOpacity) / Math.abs(delta * speed)) * 20;
            setTimeout(function () {
                element.parentNode.removeChild(element);
            }, timeToDelete);
        }

        var intervalHandlerName = "imageSwiperOpacityFade" + uniqueID;
        if (window[intervalHandlerName] != null) {
            clearInterval(window[intervalHandlerName]);
        }

        window[intervalHandlerName] = setInterval(function () {
            if (parseFloat(element.style.opacity) + delta * speed < 0) {
                element.style.opacity = 0;
                clearInterval(window[intervalHandlerName]);
                return;
            }
            if (parseFloat(element.style.opacity) + delta * speed > 1) {
                element.style.opacity = 1;
                clearInterval(window[intervalHandlerName]);
                return;
            }
            element.style.opacity = parseFloat(element.style.opacity) + delta * speed;
            if (parseFloat(element.style.opacity) > targetOpacity + delta * speed && parseFloat(element.style.opacity) < targetOpacity - delta * speed) {
                clearInterval(window[intervalHandlerName]);
            }
        }, 20);
    }

    static ColorFade(element, speed, color, intervalHandlerName) {

        if (window[intervalHandlerName] != null) {
            clearInterval(window[intervalHandlerName]);
        }

        var baseColor = element.style.backgroundColor.split("(")[1].split(")")[0].split(",");
        baseColor = {
            r: parseInt(baseColor[0].trim()),
            g: parseInt(baseColor[1].trim()),
            b: parseInt(baseColor[2].trim()),
            a: parseFloat(baseColor[3].trim()),
        };

        window[intervalHandlerName] =
            setInterval(function () {
                var currentColor = element.style.backgroundColor.split("(")[1].split(")")[0].split(",");
                currentColor = {
                    r: parseInt(currentColor[0].trim()),
                    g: parseInt(currentColor[1].trim()),
                    b: parseInt(currentColor[2].trim()),
                    a: parseFloat(currentColor[3].trim()),
                };
                var delta = {
                    r: (color.r - baseColor.r) * speed,
                    g: (color.g - baseColor.g) * speed,
                    b: (color.b - baseColor.b) * speed,
                    a: (color.a - baseColor.a) * speed,
                }
                var nextColor = {
                    r: currentColor.r + delta.r,
                    g: currentColor.g + delta.g,
                    b: currentColor.b + delta.b,
                    a: currentColor.a + delta.a,
                };
                element.style.backgroundColor = "rgba(" + nextColor.r + ", " +
                    nextColor.g + ", " +
                    nextColor.b + ", " +
                    nextColor.a + ")";
                if (nextColor.r < color.r + delta.r && nextColor.r > color.r - delta.r) {
                    clearInterval(window[intervalHandlerName]);
                }
            }, 20);
    }

    static HandleImageSwiperButton(buttonNode, uniqueID, right = false) {
        var imagesArrayName = "imageSwiperByteArrays" + uniqueID;
        // Old image node is last of available images.
        var oldImageNode = buttonNode.parentNode.getElementsByTagName("img");
        oldImageNode = oldImageNode[oldImageNode.length - 1];
        var currentImageIndex = parseInt(buttonNode.parentNode.getAttribute("data-current-image"));

        if (right == false) {
            currentImageIndex -= 1;
            if (currentImageIndex < 0) {
                currentImageIndex = window[imagesArrayName].length - 1;
            }
        }
        else {
            currentImageIndex += 1;
            if (currentImageIndex >= window[imagesArrayName].length) {
                currentImageIndex = 0;
            }
        }

        buttonNode.parentNode.setAttribute("data-current-image", currentImageIndex);

        var newImageNode = oldImageNode.cloneNode(true);
        buttonNode.parentNode.appendChild(newImageNode);
        newImageNode.style.opacity = 0;
        newImageNode.src = "data:image/png;base64," + window[imagesArrayName][currentImageIndex];
        // Position image asynchronously as it may have different size.
        setTimeout(function () {
            newImageNode.style.top = (buttonNode.parentNode.clientHeight - newImageNode.clientHeight) / 2 + "px";
        }, 0);
        ImageSwiper.OpacityFade(oldImageNode, 0.05, 0, uniqueID + "old", true);
        ImageSwiper.OpacityFade(newImageNode, 0.05, 1, uniqueID + "new");
    }

    static Add(container, width, height, imagesByteArrays, uniqueID) {

        var swiperBox = document.createElement("div");
        swiperBox.style.borderRadius = "4px";
        swiperBox.style.position = "relative";
        swiperBox.style.display = "block";
        swiperBox.style.backgroundColor = "rgba(10, 10, 10)";
        swiperBox.style.width = width + "px";
        swiperBox.style.height = height + "px";
        swiperBox.style.zIndex = 10;
        swiperBox.setAttribute("data-current-image", "0");

        // Add buttons images and event listeners only if images are provided.
        if (imagesByteArrays.length != 0) {
            window["imageSwiperByteArrays" + uniqueID] = Array.from(imagesByteArrays);
            // Add buttons only if there are more than one image.
            if (imagesByteArrays.length != 1) {
                // Left button setup.
                var swiperButtonLeft = document.createElement("div");
                swiperBox.appendChild(swiperButtonLeft);
                swiperButtonLeft.style.position = "absolute";
                swiperButtonLeft.style.top = "0px";
                swiperButtonLeft.style.left = "0px";
                swiperButtonLeft.style.backgroundColor = "rgba(0, 0, 0, 0.25)";
                swiperButtonLeft.style.height = height + "px";
                swiperButtonLeft.style.width = width / 6 + "px";
                swiperButtonLeft.style.zIndex = 30;
                swiperButtonLeft.style.borderTopLeftRadius = "4px";
                swiperButtonLeft.style.borderBottomLeftRadius = "4px";
                swiperButtonLeft.onmouseenter = function () {
                    ImageSwiper.ColorFade(swiperButtonLeft, 0.1, { r: 255, g: 255, b: 255, a: 0.25 }, "imageSwiperLeftInterval" + uniqueID);
                    swiperButtonLeft.style.cursor = "pointer";
                }
                swiperButtonLeft.onmouseleave = function () {

                    ImageSwiper.ColorFade(swiperButtonLeft, 0.1, { r: 10, g: 10, b: 10, a: 0.25 }, "imageSwiperLeftInterval" + uniqueID);
                    swiperButtonLeft.style.cursor = "default";
                }
                swiperButtonLeft.setAttribute("onclick", "ImageSwiper.HandleImageSwiperButton(this," + uniqueID + ");");

                // Right button setup.
                var swiperButtonRight = document.createElement("div");
                swiperBox.appendChild(swiperButtonRight);
                swiperButtonRight.style.position = "absolute";
                swiperButtonRight.style.top = "0px";
                swiperButtonRight.style.right = "0px";
                swiperButtonRight.style.backgroundColor = "rgba(0, 0, 0, 0.25)";
                swiperButtonRight.style.height = height + "px";
                swiperButtonRight.style.width = width / 6 + "px";
                swiperButtonRight.style.zIndex = 30;
                swiperButtonRight.style.borderTopRightRadius = "4px";
                swiperButtonRight.style.borderBottomRightRadius = "4px";
                swiperButtonRight.onmouseenter = function () {
                    ImageSwiper.ColorFade(swiperButtonRight, 0.1, { r: 255, g: 255, b: 255, a: 0.25 }, "imageSwiperRightInterval" + uniqueID);
                    swiperButtonRight.style.cursor = "pointer";
                }
                swiperButtonRight.onmouseleave = function () {

                    ImageSwiper.ColorFade(swiperButtonRight, 0.1, { r: 10, g: 10, b: 10, a: 0.25 }, "imageSwiperRightInterval" + uniqueID);
                    swiperButtonRight.style.cursor = "default";
                }
                swiperButtonRight.setAttribute("onclick", "ImageSwiper.HandleImageSwiperButton(this," + uniqueID + ",true);");
            }

            // Set first image
            var swiperImage = document.createElement("img");
            swiperBox.appendChild(swiperImage);
            swiperImage.src = "data:image/png;base64," + window["imageSwiperByteArrays" + uniqueID][0];
            swiperImage.style.width = width + "px";
            swiperImage.style.position = "absolute";
            swiperImage.style.zIndex = 20;
            swiperImage.style.userSelect = "none";
            swiperImage.style.borderRadius = "4px";
            swiperImage.style.opacity = 1;
            // Position image asynchronously.
            setTimeout(function () {
                swiperImage.style.top = (height - swiperImage.clientHeight) / 2 + "px";
            }, 0);
        }
        else {
            // Setup style for no image display.
            swiperBox.style.boxSizing = "border-box";
            swiperBox.style.position = "auto";
            swiperBox.style.display = "auto";
            swiperBox.style.fontFamily = "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";
            swiperBox.style.display = "inline-block";
            swiperBox.style.backgroundColor = "rgb(10, 10, 10)";
            swiperBox.style.textAlign = "center";
            swiperBox.style.fontSize = "36px";
            swiperBox.style.paddingTop = height / 2 - 36 + "px";
            swiperBox.style.color = "white";

            swiperBox.className = "image-swiper-no-image-class";
            swiperBox.innerText = "No image";
        }
        container.appendChild(swiperBox);
    }
}