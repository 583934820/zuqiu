var functionManager = function () {
    var dtTableResult;
    var funcTree;
    return {
        init: function (url, editUrl,deleteUrl,getFunUrl) {
            dtTableResult = $('#tableResult').dataTable({
                "ajax": {
                    "url": url,
                    "type": "GET"
                },
                "columns": [

                    { "data": "FunctionName", "orderable": true, title: "功能名称" },
                    { "data": "FunctionLevel", "orderable": false, title: "等级" },
                    { "data": "ParentName", "orderable": false, title: "所属父级" },
                    { "data": "PathUrl", "orderable": false, title: "页面路径" },
                    { "data": "FunctionSort", "orderable": false, title: "排序" },
                    {
                        "render": function (data, type, row) {
                            return '<a href="#" class="btn btn-xs btn-success" name="btnEdit" data-id="' + row.Id + '">' +
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
                                  functionid: id
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
                              functionid: id
                          },
                          success: function (jsonData) {
                              $('#FunctionName').val(jsonData.Data.FunctionName);
                              $('#PathUrl').val(jsonData.Data.PathUrl);
                              $('#FunctionLevel').val(jsonData.Data.FunctionLevel);
                              $('#FunctionSort').val(jsonData.Data.FunctionSort);
                              $('#functionId').val(jsonData.Data.Id);
                              $('#ENFunctionName').val(jsonData.Data.ENFunctionName);
                              GetParentFunction($("#FunctionLevel").val(), jsonData.Data.ParentID,getFunUrl);
                              $('#editModal').modal();

                          }
                      });
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

            $('#FunctionLevel').change(function () {
                GetParentFunction($("#FunctionLevel").val(), undefined, getFunUrl);
            })

        },

        initAdd: function (saveUrl, getFunUrl) {
            $('#btnSave').click(function () {
                var formData = $main.getFormData('addForm');

                $main.ajaxPost({
                    url: saveUrl,
                    data: formData,
                    blockEleId: 'addDiv',
                    formId: 'addForm'
                });
            });
             $('#Level').change(function () {
                 GetParentFunction($(this).val(), undefined, getFunUrl);
            })
            $('#addForm').validate({
                errorElement: 'span', //default input error message container
                errorClass: 'help-error', // default input error message class
                rules: {
                    PermissionName: {
                        required: true,
                        maxlength: 10
                    }
                },

                messages: {
                    PermissionName: {
                        required: "此项必填"
                    }
                }
            });
        }
    };

    function GetParentFunction(level,parentId,getFunUrl)
    {
        $("#ParentID").empty();
        var level = level;
        $main.ajaxGet({
            url: getFunUrl,
            data: { level: level },
            success: function (jsonData) {
                if (parentId != undefined || level==1)
                {
                    $("#ParentID").append("<option value='0'>无</option>");
                }
                
                for (var i = 0; i < jsonData.Data.length; i++) {
                   
                    if (jsonData.Data[i].Id == parentId)
                    {
                        $("#ParentID").append("<option selected value='" + jsonData.Data[i].Id + "'>" + jsonData.Data[i].FunctionName + "</option>");
                    } else
                    {
                        $("#ParentID").append("<option value='" + jsonData.Data[i].Id + "'>" + jsonData.Data[i].FunctionName + "</option>");
                    }
                    
                }
            }
        });
    }
}();