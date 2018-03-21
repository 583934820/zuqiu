var news = function () {

    return {

        init: function (url, editUrl, deleteUrl) {

            $('#btnSearch').click(function () {

                $('#tableResult').DataTable().draw(false);
            })

            $('#btnAdd').click(function () {
                $main.ajaxGet({
                    url: editUrl,
                    data: {
                        id: 0
                    },
                    success: function (data) {
                        $('.modal-title').html('添加');
                        $('.modal-body').html(data);
                        $('#editModal').modal();
                    }
                });
            });

            $('#btnSave').click(function () {
                var formData = $main.getFormData('editForm');

                $main.ajaxPost({
                    url: editUrl,
                    data: formData,
                    success: function () {
                        window.location.reload();
                    }
                });
            });

            dtTableResult = $('#tableResult').dataTable({
                "ajax": {
                    "url": url,
                    "type": "GET"
                },
                "columns": [
                    { "data": "ClassName", "orderable": false, title: '分类名称' },
                    { "data": "ClassLevel", "orderable": false, title: '分类级别' },

                    {
                        "render": function (data, type, row) {
                            return '<a href="#" class="btn btn-xs btn-success" name="btnEdit" data-id="' + row.Id + '">' +
										'<i class="fa fa-pencil-square-o"></i> 编辑 </a><a href="#" data-toggle="confirmation" class="btn btn-xs btn-warning" name="btnDelete" data-id="' + row.Id + '">' +
										'<i class="fa fa-trash-o"></i> 删除 </a>';
                        }, title: '操作'
                    }
                ]
            })
              .on('draw.dt', function () {
                  $('#tableResult').find('[name=btnDelete]').confirmation({
                      title: '确认删除?',
                      onConfirm: function () {
                          var id = $(this).data('id');

                          $main.ajaxPost({
                              url: deleteUrl,
                              data: {
                                  id: id
                              },
                              success: function () {
                                  $('#tableResult').DataTable().draw(false);
                              }
                          });
                      }
                  });

                  $('#tableResult').find('[name=btnEdit]').click(function () {
                      var id = $(this).data('id');

                      $main.ajaxGet({
                          url: editUrl,
                          data: {
                              id: id
                          },
                          success: function (data) {
                              $('.modal-title').html('编辑');
                              $('.modal-body').html(data);
                              $('#editModal').modal();

                          }
                      });
                  });
              });
        },

        initAdd: function (saveUrl, getTypeUrl) {

            //富文本
            //var editor1 = UE.getEditor('container', {
            //    initialFrameHeight: 500,
            //    autoHeightEnabled: false
            //});

            //$('.datepicker').datepicker({
            //    format:'yyyy-mm-dd'
            //});

            //保存
            $('#btnSave').click(function () {
                var formId = $(this).data('formId');
                var formData = $main.getFormData(formId);
                formData.ImgPath = $('#imgPath').attr('src');

                $main.ajaxPost({
                    url: saveUrl,
                    data: formData,
                    formId: formId,
                    success: function () {
                        window.location.reload();
                    }
                });
            });

            //$('#classParentId').change(function () {
            //    $main.ajaxGet({
            //        url: getTypeUrl,
            //        data: {
            //            id: $(this).val()
            //        },
            //        success: function (data) {
            //            $('#classId').html(data);
            //        }
            //    });
            //})


            //上传
            //$('.btnUpload').click(function () {
            //    var formId = $(this).data('formId');
            //    $('#' + formId + ' input[name=file]').click();
            //});

            //$('.uploadForm input[name=file]').change(function () {
            //    if (!$(this).val()) {
            //        $main.showError('addForm', $main.language[langCode].chooseImg);
            //    }
            //    $(this).parent().find('input[type=submit]').click();
            //});



            $('#addForm').validate({
                errorElement: 'span', //default input error message container
                errorClass: 'help-error', // default input error message class
                rules: {
                    ArticleTitle: {
                        required: true
                    },
                    ArticleContent: {
                        required: true
                    },
                    PublishDate: {
                        required: true
                    }
                }
            });

        }
    };
}();