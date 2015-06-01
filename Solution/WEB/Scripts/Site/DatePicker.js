
var ExinDatePicker;

$(function () {
    var date = $("[name=dateFromHidden]").val();
    if (!date)
        date = 0;

    ExinDatePicker = $(".exin-date-picker").datepicker({
        showOtherMonths: true,
        selectOtherMonths: true,
        //showButtonPanel: true,
        maxDate: 0,
        defaultDate: date,
        dateFormat: 'yy-mm-dd',
        onSelect: function (dateText, instance) {
            var controller = $("#controller-name").val();
            var action = $("#action-name").val();
            var relativeUrl = "/"+controller+"/"+action+"?date="+dateText;
            window.location.href = relativeUrl;
        }
    });
});