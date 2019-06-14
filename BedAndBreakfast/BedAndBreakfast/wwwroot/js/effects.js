class Effects {
    static CreateLibrary(){
        if(window.EffectsLibrary === undefined){
            window.EffectsLibrary = {};
        }
    }

    static OpacityFade(element, speed, targetOpacity, uniqueID, removeFaded = false) {

        Effects.CreateLibrary();
        var delta = targetOpacity - element.style.opacity;

        if (removeFaded == true) {
            var currentOpacity = parseFloat(element.style.opacity);
            var timeToDelete = (Math.abs(targetOpacity - currentOpacity) / Math.abs(delta * speed)) * 20;
            setTimeout(function () {
                element.parentNode.removeChild(element);
            }, timeToDelete);
        }

        var intervalHandlerName = "imageSwiperOpacityFade" + uniqueID;
        if (window.EffectsLibrary[intervalHandlerName] != null) {
            clearInterval(window.EffectsLibrary[intervalHandlerName]);
        }

        window.EffectsLibrary[intervalHandlerName] = setInterval(function () {
            if (parseFloat(element.style.opacity) + delta * speed < 0) {
                element.style.opacity = 0;
                clearInterval(window.EffectsLibrary[intervalHandlerName]);
                return;
            }
            if (parseFloat(element.style.opacity) + delta * speed > 1) {
                element.style.opacity = 1;
                clearInterval(window.EffectsLibrary[intervalHandlerName]);
                return;
            }
            element.style.opacity = parseFloat(element.style.opacity) + delta * speed;
            if (parseFloat(element.style.opacity) > targetOpacity + delta * speed && parseFloat(element.style.opacity) < targetOpacity - delta * speed) {
                clearInterval(window.EffectsLibrary[intervalHandlerName]);
            }
        }, 20);
    }

    static ColorFade(element, speed, color, intervalHandlerName) {
        
        Effects.CreateLibrary();
        if (window.EffectsLibrary[intervalHandlerName] != null) {
            clearInterval(window.EffectsLibrary[intervalHandlerName]);
        }

        var baseColor = element.style.backgroundColor.split("(")[1].split(")")[0].split(",");
        baseColor = {
            r: parseInt(baseColor[0].trim()),
            g: parseInt(baseColor[1].trim()),
            b: parseInt(baseColor[2].trim()),
            a: parseFloat(baseColor[3].trim()),
        };

        window.EffectsLibrary[intervalHandlerName] =
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
                    clearInterval(window.EffectsLibrary[intervalHandlerName]);
                }
            }, 20);
    }
}