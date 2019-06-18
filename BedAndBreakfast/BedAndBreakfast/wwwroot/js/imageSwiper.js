
class ImageSwiper {
    static HandleImageSwiperButton(buttonNode, uniqueID, right = false) {
        var imagesArrayName = "imageSwiperByteArrays" + uniqueID;
        // Old image node is last of available images.
        var oldImageNode = buttonNode.parentNode.getElementsByTagName("img");
        oldImageNode = oldImageNode[oldImageNode.length - 1];
        var currentImageIndex = parseInt(buttonNode.parentNode.getAttribute("data-current-image"));

        if (right == false) {
            currentImageIndex -= 1;
            if (currentImageIndex < 0) {
                currentImageIndex = window.ImageSwiperLibrary[imagesArrayName].length - 1;
            }
        }
        else {
            currentImageIndex += 1;
            if (currentImageIndex >= window.ImageSwiperLibrary[imagesArrayName].length) {
                currentImageIndex = 0;
            }
        }

        buttonNode.parentNode.setAttribute("data-current-image", currentImageIndex);

        var newImageNode = oldImageNode.cloneNode(true);
        buttonNode.parentNode.appendChild(newImageNode);
        newImageNode.style.opacity = 0;
        newImageNode.src = "data:image/png;base64," + window.ImageSwiperLibrary[imagesArrayName][currentImageIndex];
        // Position image asynchronously as it may have different size.
        setTimeout(function () {
            newImageNode.style.top = (buttonNode.parentNode.clientHeight - newImageNode.clientHeight) / 2 + "px";
        }, 0);
        Effects.OpacityFade(oldImageNode, 0.05, 0, uniqueID + "old", true);
        Effects.OpacityFade(newImageNode, 0.05, 1, uniqueID + "new");
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
        swiperBox.style.overflow = "hidden";
		swiperBox.setAttribute("data-current-image", "0");
		swiperBox.style.textAlign = "left";
		swiperBox.style.marginLeft = "auto";
		swiperBox.style.marginRight = "auto";

        // Add buttons images and event listeners only if images are provided.
        if (imagesByteArrays.length != 0) {
            // Create new library if undefined.
            if (window.ImageSwiperLibrary === undefined) {
                window.ImageSwiperLibrary = {};
            }
            window.ImageSwiperLibrary["imageSwiperByteArrays" + uniqueID] = Array.from(imagesByteArrays);
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
				swiperButtonLeft.style.width = "18%";
                swiperButtonLeft.style.zIndex = 30;
                swiperButtonLeft.style.borderTopLeftRadius = "4px";
                swiperButtonLeft.style.borderBottomLeftRadius = "4px";
                swiperButtonLeft.onmouseenter = function () {
                    Effects.ColorFade(swiperButtonLeft, 0.1, { r: 255, g: 255, b: 255, a: 0.25 }, "imageSwiperLeftInterval" + uniqueID);
                    swiperButtonLeft.style.cursor = "pointer";
                }
                swiperButtonLeft.onmouseleave = function () {
                    Effects.ColorFade(swiperButtonLeft, 0.1, { r: 10, g: 10, b: 10, a: 0.25 }, "imageSwiperLeftInterval" + uniqueID);
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
				swiperButtonRight.style.width = "18%";
                swiperButtonRight.style.zIndex = 30;
                swiperButtonRight.style.borderTopRightRadius = "4px";
                swiperButtonRight.style.borderBottomRightRadius = "4px";
                swiperButtonRight.onmouseenter = function () {
                    Effects.ColorFade(swiperButtonRight, 0.1, { r: 255, g: 255, b: 255, a: 0.25 }, "imageSwiperRightInterval" + uniqueID);
                    swiperButtonRight.style.cursor = "pointer";
                }
                swiperButtonRight.onmouseleave = function () {
                    Effects.ColorFade(swiperButtonRight, 0.1, { r: 10, g: 10, b: 10, a: 0.25 }, "imageSwiperRightInterval" + uniqueID);
                    swiperButtonRight.style.cursor = "default";
                }
                swiperButtonRight.setAttribute("onclick", "ImageSwiper.HandleImageSwiperButton(this," + uniqueID + ",true);");
            }

            // Set first image
            var swiperImage = document.createElement("img");
            swiperBox.appendChild(swiperImage);
            swiperImage.src = "data:image/png;base64," + window.ImageSwiperLibrary["imageSwiperByteArrays" + uniqueID][0];
			swiperImage.style.width = width + "px";
            swiperImage.style.position = "absolute";
            swiperImage.style.zIndex = 20;
            swiperImage.style.userSelect = "none";
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
            swiperBox.style.paddingTop = height / 2 - 30 + "px";
            swiperBox.style.color = "white";

            swiperBox.className = "image-swiper-no-image-class";
			swiperBox.innerText = "No image";
        }
        container.appendChild(swiperBox);
    }
}