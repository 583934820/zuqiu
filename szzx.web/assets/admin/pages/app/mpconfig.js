var mpconfig = function () {

    return {

        initAdd: function (saveUrl) {

            //function initVal() {
            //    $('#startPayMonth').val($('#startPayMonthHidden').val() || '1');
            //    $('#startPayDay').val($('#startPayDayHidden').val() || '1');
            //    $('#endPayMonth').val($('#endPayMonthHidden').val() || '1');
            //    $('#endPayDay').val($('#endPayDayHidden').val() || '1');
            //}

            //function formatMonth(el) {
            //    for (var i = 1; i <= 12; i++) {
            //        el.append('<option value="' + i + '">' + i +'</option>');
            //    }
            //}

            //function formatDay(el) {
            //    for (var i = 1; i <= 31; i++) {
            //        el.append('<option value="' + i + '">' + i +'</option>');
            //    }
            //}

            //formatMonth($('#startPayMonth'));
            //formatMonth($('#endPayMonth'));

            //formatDay($('#startPayDay'));
            //formatDay($('#endPayDay'));

            //initVal();


            $('.datepicker').datepicker({
                format: 'yyyy-mm-dd'
            });

            //保存
            $('#btnSave').click(function () {
                var formData = {};
                formData.id = $('#id').val();
                formData.RejoinTeamStartDate = $('#rejoinTeamStartDate').val();
                formData.RejoinTeamEndDate = $('#rejoinTeamEndDate').val();
                formData.PayDateRange = {};

                //formData.PayDateRange.StartMonth = $('#startPayMonth').val();
                //formData.PayDateRange.StartDay = $('#startPayDay').val();
                //formData.PayDateRange.EndMonth = $('#endPayMonth').val();
                //formData.PayDateRange.EndDay = $('#endPayDay').val();

                formData.PayDateRange.StartDate = $('#startPayDate').val();
                formData.PayDateRange.EndDate = $('#endPayDate').val();

                formData.VipFee = $('#vipFee').val();
                formData.TeamFee = $('#teamFee').val();

                

                $main.ajaxPost({
                    url: saveUrl,
                    data: formData,
                    formId: 'addForm',
                    success: function () {
                        //window.location.href = '/News/Index';
                    }
                });
            });

            $('#addForm').validate({
                errorElement: 'span', //default input error message container
                errorClass: 'help-error', // default input error message class
                rules: {
                    ReJoinTeamStartDate: {
                        required: true
                    },
                    ReJoinTeamEndDate: {
                        required: true
                    },
                    StartPayDate: {
                        required: true
                    },
                    EndPayDate: {
                        required: true
                    }
                }
            });
            
        }
    };
}();