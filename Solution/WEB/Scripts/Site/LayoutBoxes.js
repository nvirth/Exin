$(function () {

    // LeftMenu boxes - open, close

    $(".box-title").click(function () {
        var boxMain = $(this).siblings(".box-main");
        var parentId = $(this).parent().attr("id");

        var display = $(boxMain).css("display");
        var isOpened = true;
        if (display === "block")
            isOpened = true;
        else if (display === "none")
            isOpened = false;

        createCookie(parentId, !isOpened);

        $(boxMain).toggle(DEFAULT_TOGGLE_DURATION);
    });

    // Openable things in boxes
    
    $("#main-menu-box .ul-openable").bind("click", function () {
        manageOpenableClick("main-menu-opened", this);
    });

    $("#category-box .ul-openable").bind("click", function () {
        manageOpenableClick("category-opened", this);
    });

    $("#main-menu-box A[href]").bind("click", function () {
        createCookie("main-menu-selected", $(this).attr("id"));
    });
});

function manageOpenableClick(cookieName, that) {
    var contentUl = $(that).siblings("UL")[0];

    if (typeof (contentUl) !== 'undefined') {
        var display = $(contentUl).css("display");
        var isOpened = true;
        if (display === "block")
            isOpened = true;
        else if (display === "none")
            isOpened = false;

        var oldCookieValue = readCookie(cookieName);
        if (oldCookieValue == null)
            oldCookieValue = "";

        var idInCookie = $(that).attr("id") + ",";
        var newCookieValue;

        if (isOpened) {
            newCookieValue = oldCookieValue.replace(idInCookie, "");
            createCookie(cookieName, newCookieValue);
            $(contentUl).toggle(DEFAULT_TOGGLE_DURATION);
            $(that).parent().removeClass("opened").addClass("closed");
        } else {
            newCookieValue = oldCookieValue.replace(idInCookie, "") + idInCookie;
            createCookie(cookieName, newCookieValue);
            $(contentUl).toggle(DEFAULT_TOGGLE_DURATION);
            $(that).parent().removeClass("closed").addClass("opened");
        }
    }
}