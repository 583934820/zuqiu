var vip = function () {
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
                        data['vipName'] = $('#vipName').val();
                        return data;
                    }
                },
                "columns": [
                    { "data": "VipName", "orderable": false, title: '会员名' },

                    { "data": "MobileNo", "orderable": false, title: '手机号' },
                    { "data": "CardNo", "orderable": false, title: '身份证号' },

                    {
                        "render": function (data, type, row) {
                            //if (row.WXStatus != 0) {
                            //    return '';
                            //}

                            return '<a href="#" class="btn btn-xs btn-success" name="btnEdit" data-id="' + row.Id + '">' +
										'<i class="fa fa-pencil-square-o"></i> 编辑 </a>' ;
                        }, title: '操作'
                    }
                ]
            })
              .on('draw.dt', function () {

                  $('#tableResult').find('[name=btnEdit]').click(function () {
                      var id = $(this).data('id');

                      searchVip(id, true);
                  });

                  //$('#tableResult').find('[name=btnDetail]').click(function () {
                  //    var id = $(this).data('id');

                  //    searchVip(id, false);
                  //});

              });

            function searchVip(id, isShowBtn){
                $main.ajaxGet({
                    url: addOrEditUrl,
                    data: {
                        id: id
                    },
                    success: function (jsonData) {

                        $('#id').val(jsonData.Data.Id);
                        $('#vipName2').val(jsonData.Data.VipName);
                        $('#pwd').val('');

                        $('#mobileNo').val(jsonData.Data.MobileNo);
                        $('#cardNo').val(jsonData.Data.CardNo);
                        $('#cardImgFront').attr('src', jsonData.Data.CardImg);
                        $('#cardImgFront').parent().attr('href', jsonData.Data.CardImg);

                        $('#hasCert').val(jsonData.Data.HasCert);
                        $('#certNo').val(jsonData.Data.CertNo);


                        $('#editModal .modal-title').html('编辑');
                        $('#editModal').modal();

                    }
                });
            }

            $('#editModal').delegate('[name=btnSave]', 'click', function () {
                var formData = $main.getFormData('editForm');

                $main.ajaxPost({
                    url: addOrEditUrl,
                    data: formData,
                    blockEleId: 'editModalContent',
                    formId: 'editForm',
                    success: function () {
                        $('#tableResult').DataTable().draw(false);
                        $('#editModal').modal('toggle');
                    }
                });

                
            });



        }
    };
}();