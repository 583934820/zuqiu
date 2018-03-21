var menu = function () {
    var dtTableResult;

    return {
        init: function (url, addOrEditUrl, deleteUrl, getPMenusUrl) {
            dtTableResult = $('#tableResult').dataTable({
                "ajax": {
                    "url": url,
                    "type": "GET"
                },
                "columns": [

                    { "data": "MenuName", "orderable": false, title: '菜单名称' },
                    { "data": "MenuLevel", "orderable": false, title: '菜单级别' },
                    { "data": "MenuParentName", "orderable": false, title: '父级菜单' },
                    { "data": "Url", "orderable": false, title: '链接' },
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
                              formId: 'editForm',
                              success: function () {
                                  $('#tableResult').DataTable().draw(false);
                              }
                          });
                      }
                  });

                  $('#tableResult').find('[name=btnEdit]').click(function () {
                      var id = $(this).data('id');

                      $main.ajaxGet({
                          url: addOrEditUrl,
                          data: {
                              id: id
                          },
                          success: function (jsonData) {
                              $('#url').val(jsonData.Data.Url);
                              $('#id').val(jsonData.Data.Id);
                              $('#menuName').val(jsonData.Data.MenuName);
                              $('#menuLevel').val(jsonData.Data.MenuLevel);
                              $main.ajaxGet({
                                  url: getPMenusUrl,
                                  data: {
                                      level: jsonData.Data.MenuLevel
                                  },
                                  success: function (data) {
                                      $('#pMenuId').html(data);
                                      $('#pMenuId').val(jsonData.Data.MenuParentId);

                                      $('#editModal .modal-title').html('编辑');
                                      $('#editModal').modal();

                                  }
                              });
                             

                          }
                      });
                  });

              });

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

            $('#btnAdd').click(function () {
                $main.clearFormData('editForm');
                $('#id').val(0);
                $('#pMenuId').val(0);
                $('#editModal .modal-title').html('添加');
                $('#editModal').modal();
            });

            $('#menuLevel').change(function () {
                $main.ajaxGet({
                    url: getPMenusUrl,
                    data: {
                        level: $(this).val()
                    },
                    success: function (data) {
                        $('#pMenuId').html(data);

                    }
                });
            })


            $('#editForm').validate({
                errorElement: 'span', //default input error message container
                errorClass: 'help-error', // default input error message class
                rules: {
                    
                    MenuLevel: {
                        required: true
                    },
                    MenuName: {
                        required: true
                    },
                }
            });
        }
    };
}();