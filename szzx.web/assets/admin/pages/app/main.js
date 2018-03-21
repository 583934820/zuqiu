var $main = function () {

    return {
        log: function (msg) {
            if (console && console.log) {
                console.log(msg);
            }
        },
        getFormData: function (formId) {
            var data = {};
            $('#' + formId).find('input[type=password],input[type=text],input[type=radio]:checked,input[type=hidden],select,textarea').each(function () {
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

        fromTimeStamp(timestamp) {
            var reg = /\d+/;
            return new Date(parseInt(timestamp.match(reg)))
        },

        formatDateTime(timestamp) {
            return $.formatDateTime('yy-mm-dd hh:ii:ss', this.fromTimeStamp(timestamp));
        },

        clearFormData: function (formId) {
            $('#' + formId).find('input,textarea').each(function () {
                $(this).val('');
            });
        },

        ajaxPost: function (opt) {
            var url = opt.url,
                data = opt.data,
                blockEleId = opt.blockEleId,
                formId = opt.formId,
                success = opt.success;

            if (formId && !$('#' + formId).valid())
                return false;

            var target = blockEleId ? '#' + blockEleId : 'body';
            Metronic.blockUI({
                target: target,
                boxed: true,
                message: '正在提交....'
            });

            $.ajax({
                url: url,
                data: data,
                type: 'POST',
                dataType: 'json',
                success: function (data) {
                    Metronic.unblockUI(target);
                    if (data.Code == 0) {
                        toastr.success('操作成功');
                        if (success) {
                            success();

                        }
                    }
                    else {
                        if (formId) {
                            $main.showError(formId, data.Message);
                        }
                        else {
                            toastr.warning(data.Message);
                        }
                    }
                },
                error: function (data) {
                    Metronic.unblockUI(target);
                    toastr.error('操作失败');
                }
            });
        },

        ajaxGet: function (opt) {
            var url = opt.url,
                data = opt.data,
                blockEleId = opt.blockEleId,
                success = opt.success;

            var target = blockEleId ? '#' + blockEleId : 'body';
            Metronic.blockUI({
                target: target,
                boxed: true,
                message: '加载中....'
            });

            $.ajax({
                url: url,
                data: data,
                type: 'GET',
                success: function (data) {
                    Metronic.unblockUI(target);
                    if (success) {
                        success(data);
                    }
                },
                error: function (data) {
                    Metronic.unblockUI(target);
                    toastr.error('加载失败');
                }
            });
        },

        showError: function (formId, error) {
            $('#' + formId).find('div.alert-danger span').html(error);
            $('#' + formId).find('div.alert-danger').show();
        },

        language: {
            CN: {
                common: {
                    add: '添加',
                    update: '编辑',
                    'delete': '删除',
                    'action': '操作'
                },
                milestone: {
                    chooseImg: '选择图片',
                    img: '图片',
                    msYear: '年份',
                    content: '内容'
                },
                product: {
                    configValue: '值',
                    productName: '产品名称',
                    reference: '编号'
                },
                slide: {
                    title: '标题',
                    link: '链接',
                    category: '分类'
                },
                values: {
                    icon: '图标'
                },
                news: {
                    publishDate: '发布日期'
                },
                partnerlist: {
                    name: '姓名'
                },
            },
            EN: {
                common: {
                    add: 'Add',
                    update: 'Edit',
                    'delete': 'Delete',
                    'action': 'Actions'
                },
                milestone: {
                    chooseImg: 'Choose an image',
                    img: 'Image',
                    msYear: 'MsYear',
                    content: 'Content'
                },
                product: {
                    configValue: 'Value',
                    productName: 'Product Name',
                    reference: 'Reference'
                },
                slide: {
                    title: 'Title',
                    link: 'Link',
                    category: 'Category'
                },
                values: {
                    icon: 'Icon'
                },
                news: {
                    publishDate: 'Publish Date'
                },
                partnerlist: {
                    name: 'Name'
                },
            }
        },

        langCode: 'CN'
    };
}();