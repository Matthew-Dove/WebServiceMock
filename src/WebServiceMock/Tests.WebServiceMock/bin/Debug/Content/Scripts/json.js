/* A small wrapper around jQuery's AJAX functions. This script needs be be loaded after jQuery. */
var _json = (function ($) {

    function runRequest(method, url, request, success, error) {
        $(function () {
            $.ajax({
                dataType: 'json',
                contentType: 'application/json; charset=UTF-8',
                type: method,
                url: url,
                data: JSON.stringify(request || {}),
                complete: function (xhr) {
                    var status = { code: xhr.status, text: xhr.statusText };
                    var response = null;
                    if (xhr.responseText) {
                        response = JSON.parse(xhr.responseText);
                    }
                    if (success && xhr.status >= 200 && xhr.status <= 205) {
                        success(response, status);
                    }
                    else if (error) {
                        error(response, status);
                    }
                }
            });
        });
    }

    return {
        /* usage: _json.run('POST', '/Api/Person', {name: 'John Smith', age: 10}, function(r, s) { alert('Success: ' + s.text)); }, function(r,s) { alert('Error: ' + s.text); }); */
        run: function (method, url, request, success, error) {
            runRequest(method, url, request, success, error);
        }
    };
})(jQuery);