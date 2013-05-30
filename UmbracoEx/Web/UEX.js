if (typeof (UEX) == 'undefined') {
    if (typeof (jQuery) == 'undefined') {
        document.write('<script type="text/javascript" src="http://code.jquery.com/jquery-1.9.1.min.js"></s' + 'cript>');
    };
    var UEX = {
        scripts: [],
        require: function (match, src) {
            if (match) {
                if ($.inArray(src, this.scripts) < 0) {
                    document.write('<script type="text/javascript" src="' + src + '"></s' + 'cript>');
                }
            }
        },
        post: function (options) {
            var opts = $.extend({}, { form: $('form:first'), showLoading: function () { }, hideLoading: function () { }, showMessage: function (err) { alert(err); }, callback: function () { } }, options);
            var action = opts.action || opts.form.attr('action');
            var fields = opts.form.serialize();
            var method = opts.method;
            if (fields.length > 0) { fields += '&AjaxPostMethod=' + method; } else { fields += 'AjaxPostMethod=' + method; }
            if (opts.data != null) {
                if (typeof (opts.data) == 'string') {
                    fields = fields + '&' + encodeURIComponent(opts.data);
                }
                if (typeof (opts.data) == 'object') {
                    for (var i in opts.data) { fields = fields + '&' + i + '=' + encodeURIComponent(opts.data[i]); }
                }
            }
            opts.showLoading();
            $.ajax({
                url: action,
                data: fields,
                type: 'POST',
                dataType: 'json',
                success: function (data, textStatus, jqXHR) {
                    if (typeof (data.Error) != 'undefined') {
                        opts.hideLoading(); opts.showMessage(data.Error);
                    } else if (typeof (data.RedirectUrl) != 'undefined') {
                        window.location = data.RedirectUrl;
                    } else {
                        opts.hideLoading();
                        if (opts.callback) { opts.callback(data); }
                    }
                }, error: function (err) { opts.showMessage(err.responseText); opts.hideLoading(); }
            });
        }
    };
}