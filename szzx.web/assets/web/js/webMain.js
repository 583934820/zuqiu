var $webmain = function () {

    return {

        log: function (msg) {
            if (console && console.log) {
                console.log(msg);
            }
        },
        getFormData: function (formId) {
            var data = {};
            $('#' + formId).find('input[type=number],input[type=password],input[type=text],input[type=radio]:checked,input[type=hidden],select,textarea').each(function () {
                var elName = $(this).attr('name');
                var elValue = $(this).val();
                if (elName) {
                    data[elName] = elValue;
                }

            });

            $('#' + formId).find('input[type=checkbox]:checked').each(function () {
                var elName = $(this).attr('name');
                var elValue = $(this).val();

                if (elName in data) {
                    data[elName].push(elValue);
                }
                else {
                    data[elName] = [elValue];
                }
            })

            return data;
        },

        clearFormData: function (formId) {
            $('#' + formId).find('input,textarea').each(function () {
                $(this).val('');
            });
        },

        ajaxPost: function (opt) {
            var url = opt.url,
                data = opt.data,
                formId = opt.formId,
                success = opt.success,
                fail = opt.fail;

            var el = $.loading({
                content: '正在提交...',
            });

            $.ajax({
                url: url,
                data: data,
                type: 'POST',
                dataType: 'json',
                success: function (data) {
                    el.loading('hide');

                    if (data.Code == 0) {
                        if (success) {
                            success(data);
                        }
                    }
                    else {

                        if (fail) {
                            fail(data);
                        }
                        else {
                            $.tips({
                                content: data.Message,
                                stayTime: 2000,
                                type: "warn"
                            })
                        }
                        
                    }
                },
                error: function (data) {
                    el.loading('hide');
                    $.tips({
                        content: '请求失败',
                        stayTime: 2000,
                        type: "warn"
                    })
                }
            });
        },

        ajaxGet: function (opt) {
            var url = opt.url,
                data = opt.data,
                success = opt.success;

            var el = $.loading({
                content: '加载中...',
            });

            $.ajax({
                url: url,
                data: data,
                type: 'GET',
                success: function (data) {
                    el.loading('hide');

                    if (success) {
                        success(data);
                    }
                },
                error: function (data) {
                    el.loading('hide');

                    $.tips({
                        content: '请求失败',
                        stayTime: 2000,
                        type: "warn"
                    })
                }
            });
        }
    };
}();