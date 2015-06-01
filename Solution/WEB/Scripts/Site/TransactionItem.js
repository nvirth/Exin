$(function () {

    var categoryListClass = ".category-list";
    if ($(categoryListClass).length !== 0) {
        fillDropDown(categoryListClass, "Category");
        fillDropDown(".unit-list", "Unit");
    }

    refreshSumAmount();
});

function refreshSumAmount() {
    var sum = 0;
    var jQuery = $("[name$=Amount]");
    jQuery.each(function (i, elem) {
        var val = parseInt($(elem).val(), 10);
        sum += val;
    });
    var query = $(".sum-amount");
    query.html(toThousandSeparated(sum) + " Ft");
}

var $LastRow;
function removeItemRow(buttonClicked) {
    var tr = $(buttonClicked).parents("TR").first();
    var $tr = $(tr);

    if ($tr.siblings().length === 1) { // this is the last row item
        $LastRow = $tr;
    }

    fixIndexesAfter($tr);
    tr.remove();
}

function fixIndexesAfter($tr) {
    var siblingsAfter = $tr.nextAll("TR");
    var allElems = $("*", siblingsAfter).not("TR").not("TD");
    setIndexesOf(allElems, -1);
}

function setIndexesOf(elements, plusValue) {
    $.each(elements, function (index, elem) {
        var attributes = elem.attributes;
        $.each(attributes, function (i, attr) {
            var attrValue = $(attr).val();
            var regex = /^\[\d+\]\./;
            var matched = regex.exec(attrValue);
            if (matched) {
                var indexString = matched[0]; // eg: "[1]."
                var indexStrTrimmed = indexString.substring(1, indexString.length - 2);
                var indexNumber = parseInt(indexStrTrimmed, 10);
                indexNumber += plusValue;
                var newValue = attrValue.replace(indexString, "[" + indexNumber + "].");
                $(elem).attr(attr.name, newValue);
            }
        });
    });
}

function addNewItemRow() {
    var $TRs = $("#main TR");
    var $last = $($TRs.last());
    var tableIsEmpty = $last.siblings().length === 0; // there aren't any rows, except the header

    if (tableIsEmpty) {
        var clone = $LastRow.clone();
    } else {
        clone = $last.clone();
    }

    var inputs = $("INPUT", clone).not("[type=button]").not("[type=hidden]");
    //var inputs = clone.children("INPUT");
    inputs.val("");

    inputs.filter("[name$=Quantity]").val(1);
    //$("[name=Quantity]", inputs).val("1");

    var selects = $("SELECT", clone);
    resetSelectList("[name$=UnitID]", selects, 101);
    resetSelectList("[name$=CategoryID]", selects, 108);

    $last.after(clone);

    if (!tableIsEmpty)
        setIndexesOf($("*", clone), +1); // increment eg: name=[4].UnitID -> [5].UnitID
}

function resetSelectList(selector, selectsSet, selectedsValue) {
    var select = selectsSet.filter(selector);
    //var select = $(selector, selectsSet);
    select.children("OPTION").each(function (index, element) {
        var $element = $(element);
        $element.removeAttr("selected");
        if ($element.val() == selectedsValue)
            $element.prop("selected", "selected");
    });
}

function fillDropDown(selector, type) {

    var url = "/api/" + type + "/";

    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        success: function (data) {
            $(selector).each(function (index, element) {
                var $element = $(element);
                $element.fillSelect(data);

                var selectedId = $element.siblings("[name$=" + type + "ID]").val();
                $element.children("[value=" + selectedId + "]").prop("selected", "selected");
            });
        }
    });
}

// ---------------------------------------
// ---------------------------------------
// ---------------------------------------
// ---------------------------------------
// ---------------------------------------
// ---------------------------------------
// ---------------------------------------

function createAddressButtonInit() {
    $("#create-address-button").bind("click", function () {
        var $clickedButton = $(this);
        $.ajax({
            url: "/Profile/CreateAddress",
            type: "GET",
            dataType: "html",
            success: function (data) {
                var $data = $($.parseHTML(data)).filter('*');
                $clickedButton.hide().after($data);
                $data.hide().fadeIn(DEFAULT_ANIMATION_DURATION);
            }
        });
    });
}

function createAddress(clickedButton) {
    var $form = $(clickedButton.form);
    var $isFirstHidden = $("#is-first-address");
    var isFirst = $isFirstHidden.val() === "True";
    var postData = $("#address-create-form").serialize();
    postData += "&userOrderId=" + $("#user-order-id").val();
    postData += "&IsDefault=" + isFirst;

    $.ajax({
        url: "/Profile/CreateAddress",
        type: "POST",
        dataType: "html",
        data: postData,
        success: function (data) {
            var $data = $($.parseHTML(data)).filter('*');

            // Siker esetén a táblázat 1 új sora jön vissza
            if (data.indexOf("<tr>") != -1) {
                $form.slideUp(500, function () {
                    $form.remove();
                });
                $("#create-address-button").show();
                $("TBODY").append($data);
                $data.flashGreenThenTransparent();
                if (isFirst) {
                    handelAddressEmptyOrFirst($isFirstHidden, /*becameEmpty*/ false);
                }
            }
                // Ha nem sikerül, akkor visszajön a form
            else {
                $form.after($data);
                $form.remove();
                $data.children("FIELDSET").flashRedThenTransparent();
            }
        }
    });
}

function modifyAddress(clickedButton) {
    var $clickedButton = $(clickedButton);
    var $tr = $clickedButton.parent().parent();
    var postData = $tr.children("TD").children("INPUT").serialize();

    $.ajax({
        url: "/Profile/EditAddress",
        type: "POST",
        data: postData,
        dataType: "json",
        success: function (data) {
            if (data) {
                $tr.flashGreenThenTransparent();
            } else {
                $tr.animateBgColorRed();
            }
        }
    });
}

function deleteAddress(clickedButton) {
    var $clickedButton = $(clickedButton);
    var $tr = $clickedButton.parent().parent();
    var postData = "userAddressId=" + $clickedButton.siblings("[name=ID]").val();
    postData += "&isDefault=" + $("[name=IsDefault]", $tr).val();

    $.ajax({
        url: "/Profile/DeleteAddress",
        type: "POST",
        data: postData,
        dataType: "json",
        success: function (data) {
            if (data) {
                $tr.animateBgColorGreen(function () {
                    $tr.fadeOut(1000, function () {
                        var becameEmpty = $tr.siblings("TR").length == 1;
                        if (becameEmpty) {
                            var $isFirstHidden = $("#is-first-address");
                            handelAddressEmptyOrFirst($isFirstHidden, /*becameEmpty*/ true);
                        }
                        $tr.remove();
                    });
                });
            } else {
                $tr.flashRedThenTransparent();
            }
        }
    });
}

function makeAddressDefault(clickedRadio) {
    var $clickedRadio = $(clickedRadio);
    if ($clickedRadio.val() === "True") // Ugyanarra kattintott, amelyik már default volt
        return;

    var $tr = $clickedRadio.parent().parent();
    var postData = "userAddressId=" + $tr.children("TD").children("[name=ID]").first().val();

    $.ajax({
        url: "/Profile/MakeAddressDefault",
        type: "POST",
        data: postData,
        dataType: "json",
        success: function (data) {
            if (data) {
                var $allRadios = $("INPUT[type=radio]", $tr.parent());
                $allRadios.each(function () {
                    var $thisRadio = $(this);
                    $thisRadio.val("False");
                });
                $clickedRadio.val("True");
                $tr.flashGreenThenBack();
            } else {
                $tr.animateBgColorRed();
            }
        }
    });
}

function makeOrdersAddress(clickedButton) {
    var $clickedButton = $(clickedButton);
    var $tr = $clickedButton.parent().parent();
    var postData = "userAddressId=" + $tr.children("TD").children("[name=ID]").first().val();
    postData += "&userOrderId=" + $("#user-order-id").val();

    $.ajax({
        url: "/Profile/MakeOrdersAddress",
        type: "POST",
        data: postData,
        dataType: "json",
        success: function (data) {
            if (data) {
                $tr.flashGreenThenTransparent();

                var a = $tr.parent("TBODY");

                var toTransactionsAddressButtons = $("[name=to-transactions-address-button]", $tr.parent("TBODY"));
                toTransactionsAddressButtons.removeAttr("disabled");
                $clickedButton.attr("disabled", "disabled");
            } else {
                $tr.flashRedThenTransparent();
            }
        }
    });
}

// -- HELPERS

function handelAddressEmptyOrFirst($isFirstHidden, becameEmpty) {
    var $noAddressDiv = $isFirstHidden.parent("DIV");
    if (becameEmpty) {
        $isFirstHidden.val("True");
        $noAddressDiv.show();
        $noAddressDiv.siblings("TABLE").hide();
    } else {
        $isFirstHidden.val("False");
        $noAddressDiv.hide();
        $noAddressDiv.siblings("TABLE").show();
    }
}