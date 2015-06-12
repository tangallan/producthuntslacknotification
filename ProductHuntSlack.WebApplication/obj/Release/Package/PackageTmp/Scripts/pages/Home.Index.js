(function ($) {
    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };
}(jQuery));

$(function () {
    $('[name="addWebHook"]').click(function (e) {
        e.preventDefault();

        var isValid = $('[name="webHookForm"]').valid();

        if (isValid) {
            $('[name="addWebHook"]').attr('disabled', 'disabled');
            var model = $('[name="webHookForm"]').serializeObject();
            
            $.ajax({
                url: '/api/webhook/addnew',
                dataType: 'json',
                type: 'POST',
                data: model,
                success: function (data) {
                    console.log('success?');
                    $('[name="webHookForm"]')[0].reset();
                    $('[name="addWebHook"]').removeAttr('disabled');
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log('fail?');
                    var data = JSON.parse(jqXHR.responseText);
                    if (data.ModelState != undefined) {
                        alert('Validation error, please try again');
                    }
                    else {
                        alert(data.Message);
                    }

                    $('[name="addWebHook"]').removeAttr('disabled');
                }
            });
        }
    });
});