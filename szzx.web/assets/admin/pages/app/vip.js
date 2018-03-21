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
                        data['status'] = $('#status').val();
                        data['vipName'] = $('#vipName').val();
                        return data;
                    }
                },
                "columns": [
                    { "data": "VipName", "orderable": false, title: '会员名' },
                    { "data": "VipNo", "orderable": false, title: '会员编号' },
                    { "data": "Age", "orderable": false, title: '年龄' },
                    { "data": "MobileNo", "orderable": false, title: '手机号' },
                    {
                        "data": "IsInTeam", "orderable": false, title: '是否在球队中',
                        "render": function (data) {
                            return data > 0 ? '是' : '否';
                        }
                    },

                    {
                        "data": "WXStatus", "orderable": false, title: '审核状态',
                        "render": function (data) {
                            return data == 1 ? '待审核' : data == 2 ? '审核成功' : '审核失败';
                        }
                    },
                    { "data": "RemoveReason", "orderable": false, title: '审核失败原因' },

                    {
                        "render": function (data, type, row) {
                            //if (row.WXStatus != 0) {
                            //    return '';
                            //}

                            return '<a href="#" class="btn btn-xs btn-success" name="btnEdit" data-id="' + row.Id + '">' +
										'<i class="fa fa-pencil-square-o"></i> 审核 </a>' + '<a href="#" class="btn btn-xs btn-success" name="btnDetail" data-id="' + row.Id + '">' +
										'<i class="fa fa-search"></i> 查看详情 </a>';
                        }, title: '操作'
                    }
                ]
            })
              .on('draw.dt', function () {

                  $('#tableResult').find('[name=btnEdit]').click(function () {
                      var id = $(this).data('id');

                      searchVip(id, true);
                  });

                  $('#tableResult').find('[name=btnDetail]').click(function () {
                      var id = $(this).data('id');

                      searchVip(id, false);
                  });

              });

            function searchVip(id, isShowBtn){
                $main.ajaxGet({
                    url: addOrEditUrl,
                    data: {
                        id: id
                    },
                    success: function (jsonData) {
                        $('#divRemove').hide();
                        $('#removeReason').val('');

                        $('#id').val(jsonData.Data.Id);
                        $('#vipName2').val(jsonData.Data.VipName);
                        $('#age').val(jsonData.Data.Age);
                        $('#mobileNo').val(jsonData.Data.MobileNo);
                        $('#email').val(jsonData.Data.Email);
                        $('#cardNo').val(jsonData.Data.CardNo);
                        $('#cardImgFront').attr('src', jsonData.Data.CardImgFront);
                        $('#cardImgFront').parent().attr('href', jsonData.Data.CardImgFront);

                        $('#cardImgBack').attr('src', jsonData.Data.CardImgBack);
                        $('#cardImgBack').parent().attr('href', jsonData.Data.CardImgBack);

                        $('#juzhuFront').attr('src', jsonData.Data.JuzhuFront);
                        $('#juzhuFront').parent().attr('href', jsonData.Data.JuzhuFront);

                        $('#juzhuBack').attr('src', jsonData.Data.JuzhuBack);
                        $('#juzhuBack').parent().attr('href', jsonData.Data.JuzhuBack);

                        $('#wxStatus').val(jsonData.Data.WXStatus);
                        $('#vipNo').val(jsonData.Data.VipNo);

                        if (isShowBtn) {
                            $('#divStatus').show();
                            $('div.modal-footer').show();
                        }
                        else {
                            $('#divStatus').hide();
                            $('div.modal-footer').hide();
                        }

                        $('#editModal .modal-title').html('编辑');
                        $('#editModal').modal();

                    }
                });
            }

            $('#editModal').delegate('[name=btnSave]', 'click', function () {
                var formData = $main.getFormData('editForm');

                var wxStatus = $('#wxStatus').val();
                var flag = true;
                if (wxStatus == '0') {
                    flag = confirm('移除操作将清空会员信息，所在球队信息和缴费记录，移除后是否需要退还注册费？');
                    if (flag) {
                        formData.isRefund = 1;                        
                    }
                }
                else if (wxStatus == '3') {
                    flag = confirm('审核失败后会自动退费到会员账户，确认提交？');

                    if (!flag) return false;

                }

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

            $('#wxStatus').change(function () {
                var flag = $(this).val();
                if (flag == '0' || flag == '3') {
                    $('#divRemove').show();
                }
                else {
                    $('#divRemove').hide();
                    $('#removeReason').val('');
                }
            })


        }
    };
}();