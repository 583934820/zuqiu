var vipComment = function () {

    return {

        init: function (url, editUrl, deleteUrl) {

            $('#btnSearch').click(function () {

                $('#tableResult').DataTable().draw(false);
            })

            $('#btnSave').click(function () {
                var formData = $main.getFormData('editForm');

                $main.ajaxPost({
                    url: editUrl,
                    data: formData,
                    success: function () {
                        $('#tableResult').DataTable().draw(false);
                    }
                });
            });

            dtTableResult = $('#tableResult').dataTable({
                "ajax": {
                    "url": url,
                    "type": "GET",
                    data: function (data) {
                        data['isReply'] = $('#isReply').val();
                        data['videoName'] = $('#videoName').val();
                        return data;
                    }
                },
                "columns": [
                    { "data": "Title", "orderable": false, title: '课程标题' },
                    { "data": "VipName", "orderable": false, title: '评论者' },
                    { "data": "Comment", "orderable": false, title: '评论' },
                    { "data": "Reply", "orderable": false, title: '回复' },
                    {
                        "data": "ReplyTime", "orderable": false, title: '回复时间',
                        "render": function (data) {
                            return data ? $main.formatDateTime(data) : '';
                        }
                    },

                    {
                        "render": function (data, type, row) {
                            return '<a href="#" class="btn btn-xs btn-success" name="btnEdit" data-id="' + row.Id + '">' +
										'<i class="fa fa-pencil-square-o"></i> 回复 </a><a href="#" data-toggle="confirmation" class="btn btn-xs btn-warning" name="btnDelete" data-id="' + row.Id + '">' +
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

                              $('#comment').val(data.Data.Comment);
                              $('#reply').val(data.Data.Reply);
                              $('#id').val(data.Data.Id);

                              $('#editModal').modal();

                          }
                      });
                  });
              });
        }
    };
}();