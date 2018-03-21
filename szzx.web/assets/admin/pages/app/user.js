var userManager = function () {
    var dtTableResult;
    var funcTree;
    return {
        init: function (url, editUrl, deleteUrl,saveRoleUrl,getUserRoleUrl) {
            dtTableResult = $('#tableResult').dataTable({
                "ajax": {
                    "url": url,
                    "type": "GET"
                },
                "columns": [

                    { "data": "LoginName", "orderable": true, title: "登录名称" },
                    { "data": "UserName", "orderable": false, title: "用户名称" },
                    {
                        "render": function (data, type, row) {
                            return '<a href="#" class="btn btn-xs btn-success" name="btnRole" data-id="' + row.Id + '">' +
                                '<i class="fa fa-pencil-square-o"></i>角色</a><a href="#" class="btn btn-xs btn-success" name="btnEdit" data-id="' + row.Id + '">' +
										'<i class="fa fa-pencil-square-o"></i> 编辑 </a><a href="#" data-toggle="confirmation" class="btn btn-xs btn-warning" name="btnDelete" data-id="' + row.Id + '">' +
										'<i class="fa fa-trash-o"></i> 删除 </a>';
                        }, title: "操作"
                    }
                ]
            })
              .on('draw.dt', function () {
                  $('#tableResult').find('[name=btnDelete]').confirmation({
                      title: "是否删除？",
                      onConfirm: function () {
                          var id = $(this).data('id');
                          $main.ajaxPost({
                              url: deleteUrl,
                              data: {
                                  userId: id
                              },
                              success: window.location.reload()
                          });
                      }
                  });

                  $('#tableResult').find('[name=btnEdit]').click(function () {
                      $(".alert-danger").css("display", "none");
                      var id = $(this).data('id');
                      $main.ajaxGet({
                          url: editUrl,
                          data: {
                              userId: id
                          },
                          success: function (jsonData) {
                              $('#LoginName').val(jsonData.Data.LoginName);
                              $('#UserName').val(jsonData.Data.UserName);
                              $('#Password').val(jsonData.Data.Password);
                              $("#userId").val(jsonData.Data.Id);
                              $('#editModal').modal();

                          }
                      });
                  });

                  $('#tableResult').find('[name=btnRole]').click(function () {
                      $(".left-list li").remove();
                      $(".right-list li").remove();
                      var id = $(this).data('id');
                      $main.ajaxGet({
                          url: getUserRoleUrl,
                          data: {userId:id},
                         
                          success: function (jsonData) {
                              $("#userId_role").val(id);
                              var left = jsonData.Data.left;
                              var right = jsonData.Data.right;
                            
                          
                                  for(var i=0;i<left.length; i++)
                                  {
                                      $(".left-list").append("<li class='left-list-li' value=" + left[i].Id + ">" + left[i].RoleName + "</li>");
                                  }
                       
                                  for (var i=0;i<right.length;i++) {
                                      $(".right-list").append(" <li class='right-list-li' value=" + right[i].Id + ">" + right[i].RoleName + "</li>");
                                  }
                              }
                      });
                      $('#roleModal').modal();
                  });
              });

            $('#editModal').delegate('[name=btnSave]', 'click', function () {
                var formData = $main.getFormData('editForm');
                $main.ajaxPost({
                    url: editUrl,
                    data: formData,
                    blockEleId: 'editModalContent',
                    formId: 'editForm',
                    success: function () {
                        $('#tableResult').DataTable().draw(false);
                        $('#editModal').modal('toggle');
                    }
                });
            });
            $('#roleModal').delegate('[name=btnSave]', 'click', function () {
                var userId =$("#userId_role").val();
                var model =new Array();
                 $(".right-list-li").each(function () {
                     var obj =  {
                         RoleId: $(this).attr("value"),
                         UserId:userId
                     };
                     model.push(obj);
                 });
                 
                $main.ajaxPost({
                    url: saveRoleUrl,
                    data: { model: model, userId: userId },
                    success: function () {
                        $('#tableResult').DataTable().draw(false);
                        $('#roleModal').modal('toggle');
                    }
                });
            });

            $('#roleForm').delegate('.right-list-li', 'click', function () {
                $(".left-list").append(this);
                $(this).attr("class", "left-list-li");
            });
            $('#roleForm').delegate('.left-list-li', 'click', function () {
                $(".right-list").append(this);
                $(this).attr("class", "right-list-li");
            });
            

        },

        initAdd: function (saveUrl) {
            $('#btnSave').click(function () {
                var formData = $main.getFormData('addForm');

                $main.ajaxPost({
                    url: saveUrl,
                    data: formData,
                    blockEleId: 'addDiv',
                    formId: 'addForm'
                });
            });
           
            $('#addForm').validate({
                errorElement: 'span', //default input error message container
                errorClass: 'help-error', // default input error message class
                rules: {
                    LoginName: {
                        required: true,
                        maxlength: 10,
                        minlength:3
                    },
                    UserName: {
                        required: true,
                        maxlength: 10,
                        minlength: 3
                    },
                      Password: {
                required: true,
                maxlength: 20,
                minlength:6
                    }
                },

                messages: {
                    LoginName: {
                        required: "此项必填"
                    },
                    UserName: {
                        required: "此项必填"
                    },
                    Password: {
                        required: "此项必填"
                    }
                }
            });
        }
    };
}();