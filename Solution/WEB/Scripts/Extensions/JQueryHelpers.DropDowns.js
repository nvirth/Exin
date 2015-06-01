$(function () {

    $.fn.clearSelect = function () {
        return this.each(function () {
            if (this.tagName == 'SELECT')
                this.options.length = 0;
        });
    };

    $.fn.fillSelect = function (data) {
        return this.clearSelect().each(function () {
            if (this.tagName == 'SELECT') {
                var dropdownList = this;
                $.each(data, function (index, optionData) {
                    var option = new Option(optionData.Text, optionData.Value);

                    if(optionData.Class)
                        $(option).addClass(optionData.Class);

                    if (optionData.Selected)
                        option.setAttribute("selected", "selected");

                    //if ($.browser.msie) { // not supported from 1.9
                    if (navigator.userAgent.match(/msie/i)) {
                        dropdownList.add(option);
                    }
                    else {
                        dropdownList.add(option, null);
                    }
                });
            }
        });
    };
});