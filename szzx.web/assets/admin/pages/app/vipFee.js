var vipFee = function () {
    var dtTableResult;

    return {
        init: function (url, addOrEditUrl) {
            $('#btnSearch').click(function () {
                $('#tableResult').DataTable().draw(false);
            });

            dtTableResult = $('#tableResult').dataTable({
                "ajax": {
                    "url": url,
                    "type": "GET",
                    data: function (data) {
                        data['keyword'] = $('#keyword').val();
                        return data;
                    }
                },
                "columns": [
                    { "data": "OrderCode", "orderable": false, title: '订单号' },
                    { "data": "VipName", "orderable": false, title: '会员名' },

                    {
                        "data": "FeeTime", "orderable": false, title: '缴费时间',
                        "render": function (data) {
                            return data ? $main.formatDateTime(data) : '';
                        }
                    },
                    {
                        "data": "Fee", "orderable": false, title: '缴费金额',
                        "render": function (data) {
                            return (parseFloat(data) / 100).toFixed(2);
                        }
                    }
                ]
            })
              .on('draw.dt', function () {


              });


        }
    };
}();