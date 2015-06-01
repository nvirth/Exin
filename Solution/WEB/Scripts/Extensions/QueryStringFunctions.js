function getQueryString() {
    if (window.location.href.indexOf('?') != -1)
        return window.location.href.slice(window.location.href.indexOf('?') + 1);
    else
        return "";
}
function getQueryStringParameters() {
    if (window.location.href.indexOf('?') != -1) {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    } else {
        return "";
    }
}