var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();

function plainBarChart(dataStr, layoutStr, optionsStr) {
    var data = JSON.parse(dataStr);
    var layout = JSON.parse(layoutStr);
    var options = JSON.parse(optionsStr);

    Plotly.newPlot('plotly-div', data, layout, options);

    var $window = $(window);
    function reLayout() {
        Plotly.relayout('plotly-div', {
            width: $window.width(),
            height: $window.height()
        });
    }

    $window.resize(function (e) {
        delay(reLayout, 500);
    });

    reLayout();
}
