jQuery.extend($.fn.datepicker.defaults, {
    format: 'yyyy-mm-dd',
    todayBtn: 'linked'
});

if ($main.langCode == 'CN') {
    jQuery.extend($.fn.datepicker.defaults, {
        language: 'zh-CN'
    });
}
