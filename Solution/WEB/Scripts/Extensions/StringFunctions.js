function toThousandSeparated(str) {
    var amount = new String(str);
    amount = amount.split("").reverse();

    var output = "";
    for (var i = 0; i <= amount.length - 1; i++) {
        output = amount[i] + output;
        if ((i + 1) % 3 == 0 && (amount.length - 1) !== i) output = ' ' + output;
    }
    return output;
}

function rgb2hex(rgb) {
    if (rgb.search("rgb") == -1) {
        return rgb;
    }
    else if (rgb == 'rgba(0, 0, 0, 0)') {
        return 'transparent';
    }
    else {
        rgb = rgb.match(/^rgba?\((\d+),\s*(\d+),\s*(\d+)(?:,\s*(\d+))?\)$/);
        function hex(x) {
            return ("0" + parseInt(x).toString(16)).slice(-2);
        }
        return "#" + hex(rgb[1]) + hex(rgb[2]) + hex(rgb[3]);
    }
}