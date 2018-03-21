var roleManager = function () {
    var dtTableResult;
    var funcTree;
    return {
        init: function (url, editUrl, getFunUrl, saveFunUrl, deleteUrl) {
          dtTableResult = $('#tableResult').dataTable({
                "ajax": {
                    "url": url,
                    "type": "GET"
                },
                "columns": [

                    { "data": "RoleName", "orderable": true, title: "角色名称" },
                    { "data": "Description", "orderable": false, title: "角色描述" },
                    {
                        "data": "AddTime", "orderable": true, title: "添加时间",
                        "render": function (data) {
                            var time = data.replace("/Date(", "").replace(")/", "");
                            var date = new Date(parseInt(time));
                            return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
                        }
                    },
                    {
                        "render": function (data, type, row) {
                            return '<a href="#" class="btn btn-xs btn-success" name="btnFunction" data-id="' + row.Id + '">' +
										'<i class="fa fa-pencil-square-o"></i> 权限 </a><a href="#" class="btn btn-xs btn-success" name="btnEdit" data-id="' + row.Id + '">' +
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
                        $main.ajaxPost({
                            url: deleteUrl,
                            data: {roleid:$(this).data('id')},
                            success:window.location.reload()
                            
                        });
                    }
                });

                $('#tableResult').find('[name=btnEdit]').click(function () {
                    var id = $(this).data('id');                    

                    $main.ajaxGet({
                        url: editUrl,
                        data: {
                            roleId: id
                        },
                        success: function (jsonData) {
                            $('#roleName').val(jsonData.Data.RoleName);
                            $('#roleDesc').val(jsonData.Data.Description);
                            $('#roleId').val(jsonData.Data.Id);
                            $('#editModal').modal();
                            
                        }
                    });
                });

                $('#tableResult').find('[name=btnFunction]').click(function () {
                    var id = $(this).data('id');

                    $main.ajaxGet({
                        url: getFunUrl,
                        data: {
                            roleId: id
                        },
                        success: function (data) {
                            $('#roleId').val(id);
                            $('#funModal').find('.form-body').html(data);
                            funcTree = $('#functionList').jstree({
                                core:{
                                    themes: {
                                        icons: false
                                    }
                                },
                                checkbox: {
                                    'keep_selected_style': true
                                    //,'three_state': false
                                },
                                plugins: [
                                    'checkbox'
                                ]
                            });
                            $('#funModal').modal();
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

          $('#funModal').delegate('[name=btnSave]', 'click', function () {
              var data = {
                  roleId: $('#roleId').val(),
                  funcIds: ''
              };

              var checkedNodes = funcTree.jstree('get_selected', true);
              var undetermindedNodes = $('#functionList').find('.jstree-undetermined').each(function (i, ele) {
                  var node = $(ele).closest('.jstree-node');
                  var jstreeData = JSON.parse(node[0].dataset.jstree);
                  data.funcIds += ',' + jstreeData.funcId;
              });
              
              $.each(checkedNodes, function () {
                  data.funcIds += ',' + this.state.funcId;
              });

              $main.ajaxPost({
                  url: saveFunUrl,
                  data: data,
                  blockEleId: 'funModalContent'
              });

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
                    RoleName: {
                        required: true
                    }
                }
            });
        }
    };
}();