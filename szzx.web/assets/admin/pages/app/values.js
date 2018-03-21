var $values = function () {
    var dtTableResult;
    return {
        init: function (url, addOrEditUrl, deleteUrl, langCode) {
            dtTableResult = $('#tableResult').dataTable({
                "ajax": {
                    "url": url,
                    "type": "GET"
                },
                "columns": [

                    {
                        "data": "IconName", "orderable": false, title: $main.language[langCode].values.icon,
                        "render": function (data) {
                            return '<div style="width:20px;height:20px;margin-top:10px"><i style="font-size:35px" class="' + data + '"></i></div>';
                        }
                    },
                    { "data": "Title", "orderable": false, title: $main.language[langCode].slide.title },
                    { "data": "Content", "orderable": false, title: $main.language[langCode].milestone.content, width:'40%' },
                    {
                        "render": function (data, type, row) {
                            return '<a href="#" class="btn btn-xs btn-success" name="btnEdit" data-id="' + row.Id + '">' +
										'<i class="fa fa-pencil-square-o"></i> ' + $main.language[langCode].common.update + ' </a><a href="#" data-toggle="confirmation" class="btn btn-xs btn-warning" name="btnDelete" data-id="' + row.Id + '">' +
										'<i class="fa fa-trash-o"></i> ' + $main.language[langCode].common['delete'] + ' </a>';
                        }, title: $main.language[langCode].common.action
                    }
                ]
            })
              .on('draw.dt', function () {
                  $('#tableResult').find('[name=btnDelete]').confirmation({
                      title: $main.language[langCode].common['delete'] + '?',
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
                              $('#title').val(jsonData.Data.Title);
                              $('#id').val(jsonData.Data.Id);
                              $('#content').val(jsonData.Data.Content);
                              $('#iconName').val(jsonData.Data.IconName);
                              $('.' + jsonData.Data.IconName.replace('fa ', '')).parent().addClass('fa-item-active');
                              $('#editModal .modal-title').html($main.language[langCode].common.update);
                              $('#editModal').modal();

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

            $('.fa-item').click(function () {
                $('.fa-item').removeClass('fa-item-active');
                $(this).addClass('fa-item-active');
                $('#iconName').val($(this).find('i').attr('class'));
            });

            $('#btnAdd').click(function () {
                $main.clearFormData('editForm');
                $('#id').val(0);
                $('.fa-item').removeClass('fa-item-active');
                $('#editModal .modal-title').html($main.language[langCode].common.add);
                $('#editModal').modal();
            });


            $('#editForm').validate({
                errorElement: 'span', //default input error message container
                errorClass: 'help-error', // default input error message class
                rules: {
                    Title: {
                        required: true
                    },
                    Content: {
                        required: true
                    }
                }
            });
        }
    };
}();