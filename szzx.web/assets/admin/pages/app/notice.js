var notice = function () {
    var dtTableResult;

    return {
        init: function (url, addOrEditUrl, deleteUrl) {
            dtTableResult = $('#tableResult').dataTable({
                "ajax": {
                    "url": url,
                    "type": "GET"
                },
                "columns": [

                    {
                        "data": "ImgPath", "orderable": false, title: '图片',
                        "render": function (data) {
                            return '<img src="' + (data || '') + '" style="width:50px;height:50px" />';
                        }
                    },
                    { "data": "Title", "orderable": false, title: '标题' },
                    {
                        "render": function (data, type, row) {
                            return '<a href="' + addOrEditUrl + '?id=' + row.Id + '" class="btn btn-xs btn-success" name="btnEdit" data-id="' + row.Id + '">' +
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
                              formId: 'editForm',
                              success: function () {
                                  $('#tableResult').DataTable().draw(false);
                              }
                          });
                      }
                  });

                  //$('#tableResult').find('[name=btnEdit]').click(function () {
                  //    var id = $(this).data('id');

                  //    $main.ajaxGet({
                  //        url: addOrEditUrl,
                  //        data: {
                  //            id: id
                  //        },
                  //        success: function (jsonData) {
                  //            $('#img1')[0].src = jsonData.Data.ImgPath || '';
                  //            $('#title').val(jsonData.Data.Title);
                  //            $('#id').val(jsonData.Data.Id);
                  //            $('#content').val(jsonData.Data.Content);
                  //            $('#editModal .modal-title').html('编辑');
                  //            $('#editModal').modal();

                  //        }
                  //    });
                  //});

              });

            //$('#editModal').delegate('[name=btnSave]', 'click', function () {
            //    var formData = $main.getFormData('editForm');
            //    formData.ImgPath = $('#img1').attr('src');

            //    $main.ajaxPost({
            //        url: addOrEditUrl,
            //        data: formData,
            //        blockEleId: 'editModalContent',
            //        formId: 'editForm',
            //        success: function () {
            //            $('#tableResult').DataTable().draw(false);
            //            $('#editModal').modal('toggle');
            //        }
            //    });
            //});

            //$('#btnAdd').click(function () {
            //    $('#img1')[0].src = '';
            //    $main.clearFormData('editForm');
            //    $('#id').val(0);
            //    $('#editModal .modal-title').html('添加');
            //    $('#editModal').modal();
            //});

            //$('.btnUpload').click(function () {
            //    var formId = $(this).data('formId');
            //    $('#' + formId + ' input[name=file]').click();
            //});

            //$('.uploadForm input[name=file]').change(function () {
            //    if (!$(this).val()) {
            //        alert('请选择图片');
            //    }
            //    $(this).parent().find('input[type=submit]').click();
            //});

            

            //$('#editForm').validate({
            //    errorElement: 'span', //default input error message container
            //    errorClass: 'help-error', // default input error message class
            //    rules: {
            //        Title: {
            //            required: true
            //        }
            //    }
            //});
        },
        initAdd: function (addOrEditUrl) {
            var editor1 = UE.getEditor('container', {
                initialFrameHeight: 500
            });

            $('.btnUpload').click(function () {
                var formId = $(this).data('formId');
                $('#' + formId + ' input[name=file]').val('');
                $('#' + formId + ' input[name=file]').click();
            });

            $('.uploadForm input[name=file]').change(function () {
                if (!$(this).val()) {
                    //alert('请选择图片');
                    return;
                }
                $(this).parent().find('input[type=submit]').click();
            });

            $('#btnSave').click(function () {
                var formData = $main.getFormData('editForm');
                formData.ImgPath = $('#img1').attr('src');

                $main.ajaxPost({
                    url: addOrEditUrl,
                    data: formData,
                    formId: 'editForm'
                });
            })
        }
    };
}();