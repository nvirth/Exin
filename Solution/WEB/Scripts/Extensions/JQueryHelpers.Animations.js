$(function () {
    $.fn.originalBackground = function () {
        var $originalBgColorHidden = this.children("INPUT.original-background-color");
        if ($originalBgColorHidden.length == 0) {
            var originalBgColor = rgb2hex(this.css("background-color"));
            this.append("<input type=hidden class=original-background-color value=" + originalBgColor + " />");
        } else {
            originalBgColor = this.children(".original-background-color").val();
        }
        return originalBgColor;
    };

    $.fn.animateBgColor = function (flashColor, duration, callBack) {
        if (typeof (callBack) === "function") {
            this.animate({ backgroundColor: flashColor }, duration, callBack);
        } else {
            this.animate({ backgroundColor: flashColor }, duration);
        }
    };
    $.fn.flash = function (flashColor, backColor, duration, callBack) {
        //this.animate({ backgroundColor: flashColor }, duration, function () {
        var $that = this;
        this.animateBgColor(flashColor, duration, function () {
            if (typeof (callBack) === "function") {
                $that.animate({ backgroundColor: backColor }, duration, callBack);
            } else {
                $that.animate({ backgroundColor: backColor }, duration);
            }
        });
    };
    $.fn.animateBgColorGreen = function (callBack) {
        this.stop(false, true);
        this.animateBgColor(COLOR_GOOD_GREEN, DEFAULT_FLASH_DURATION, callBack);
    };
    $.fn.animateBgColorRed = function (callBack) {
        this.stop(false, true);
        this.animateBgColor(COLOR_BAD_RED, DEFAULT_FLASH_DURATION, callBack);
    };
    $.fn.animateBgColorTransparent = function (callBack) {
        this.stop(false, true);
        this.animateBgColor(COLOR_TRANSPARENT, DEFAULT_FLASH_DURATION, callBack);
    };
    $.fn.flashGreenThenTransparent = function (callBack) {
        this.stop(false, true);
        this.flash(COLOR_GOOD_GREEN, COLOR_TRANSPARENT, DEFAULT_FLASH_DURATION, callBack);
    };
    $.fn.flashRedThenTransparent = function (callBack) {
        this.stop(false, true);
        this.flash(COLOR_BAD_RED, COLOR_TRANSPARENT, DEFAULT_FLASH_DURATION, callBack);
    };
    $.fn.flashGreenThenBack = function (callBack) {
        this.stop(false, true);
        var originalBgColor = this.originalBackground();
        this.flash(COLOR_GOOD_GREEN, originalBgColor, DEFAULT_FLASH_DURATION, callBack);
    };
    $.fn.flashRedThenBack = function (callBack) {
        this.stop(false, true);
        var originalBgColor = this.originalBackground();
        this.flash(COLOR_BAD_RED, originalBgColor, DEFAULT_FLASH_DURATION, callBack);
    };
})