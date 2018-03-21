var video = function () {

    return {

        init: function (url, editUrl, deleteUrl) {

            $('#btnSearch').click(function () {

                $('#tableResult').DataTable().draw(false);
            })
            dtTableResult = $('#tableResult').dataTable({
                "ajax": {
                    "url": url,
                    "type": "GET",
                    "data": function (params) {
                        params["title"] = $('#title').val();
                        return params;
                    }
                },
                "columns": [
                    { "data": "Title", "orderable": false, title: '标题' },
                    {
                        "data": "ImgPath", "orderable": false, title: "封面图",
                        "render": function (data) {
                            return '<img src="' + (data || '') + '" style="width:100px;height:100px" />';
                        }
                    },
                    { "data": "ClassName", "orderable": false, title: '分类' },
                    {
                        "render": function (data, type, row) {
                            return '<a href="' + editUrl + '/' + row.Id + '" class="btn btn-xs btn-success" name="btnEdit" data-id="' + row.Id + '">' +
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
              });
        },

        initAdd: function (saveUrl, getClassUrl) {

            var BLOCK_SIZE = 4 * 1024 * 1024;
            var observable;
            var subscription;
            //var config = {
            //    useCdnDomain: false,
            //    region: qiniu.region.z0
            //};
            var putExtra = {
                fname: "",
                params: {},
                mimeType: [] || null
            };

            function initUploader(token, startFun, successFun) {
                return new Qiniu.UploaderBuilder()
                    .debug(true)//开启debug，默认false
	//.button('uploadButton')//指定上传按钮
	//.domain({http: "http://img.yourdomain.com", https: "https://img.yourdomain.com"})//默认为{http: "http://upload.qiniu.com", https: "https://up.qbox.me"}
	//.scheme("https")//默认从 window.location.protocol 获取，可以通过指定域名为 "http://img.yourdomain.com" 来忽略域名选择。
	.retry(0)//设置重传次数，默认0，不重传
	//.compress(0.5)//默认为1,范围0-1
	//.scale([200,0])第一个参数是宽度，第二个是高度,[200,0],限定高度，宽度等比缩放.[0,100]限定宽度,高度等比缩放.[200,100]固定长宽
	.size(1024*1024 * 4)//分片大小，最多为4MB,单位为字节,默认1MB
	.chunk(true)//是否分块上传，默认true，当chunk=true并且文件大于4MB才会进行分块上传
	.auto(true)//选中文件后立即上传，默认true
	.multiple(false)//是否支持多文件选中，默认true
	//.accept(['.gif','.png','video/*'])//过滤文件，默认无，详细配置见http://www.w3schools.com/tags/att_input_accept.asp

	// 在一次上传队列中，是否分享token，如果为false每上传一个文件都需要请求一次token，默认true。
	//
	// 如果saveKey中有需要在客户端解析的变量，则忽略该值。
	.tokenShare(true)

	// 设置token获取函数，token获取完成后，必须调用`setToken(token);`不然上传任务不会执行。
	//
	// 覆盖tokenUrl的设置。
	.tokenFunc(function (setToken,task) {
	    setToken(token);
	})
	.listener({
	    onReady(tasks) {
	        //该回调函数在图片处理前执行,也就是说task.file中的图片都是没有处理过的
	        //选择上传文件确定后,该生命周期函数会被回调。
	        
	    },onStart(tasks){
	        //所有内部图片任务处理后执行
	        //开始上传
	        
	    },
	    onTaskGetKey(task) {
	        //为每一个上传的文件指定key,如果不指定则由七牛服务器自行处理
	        //console.log(task);

	        startFun(task._file.name);

	        return new Date().getTime();
	    },
        onTaskProgress: function (task) {
	        //每一个任务的上传进度,通过`task.progress`获取
            $main.log(task.progress);
            $('#uploadBody div.colorDiv')
                             .css(
                               "width",
                               task.progress + "%"
                             );
			
	    },onTaskSuccess(task){
	        //一个任务上传成功后回调
	        $main.log(task.result.key);//文件的key
	        $main.log(task.result.hash);//文件hash
	        if (successFun) {
	            successFun(task.result.key + '-' + task._file.name);
	        }
	    }, onTaskFail(task) {
	        $main.log(JSON.stringify(task));
	        //一个任务在经历重传后依然失败后回调此函数
        	
	    },onTaskRetry(task) {
	        //开始重传
        	
	    },onFinish(tasks){
	        //所有任务结束后回调，注意，结束不等于都成功，该函数会在所有HTTP上传请求响应后回调(包括重传请求)。
            
	    }
	}).build();
            }
           
            var qiniuUploader = initUploader($('#upToken').val(), function (key) {
                Metronic.blockUI({
                    target: 'body',
                    boxed: true,
                    message: '上传中....'
                });

                addVideoHtml(key);

            }, function (key) {
                Metronic.unblockUI('body');
                alert('上传成功');
                $('#attachFile1').val(key);
            });

            $('#btnUploadFile1').click(function () {
                //$('#videoFile').click();
                qiniuUploader.chooseFile();
            })

            //$('#videoFile').change(videoChange);

            //function videoChange() {
            //    var file = $('#videoFile')[0].files[0];
            //    if (file) {
            //        var key = file.name;
            //        putExtra.fname = key;
            //        //var borad = addUploadBorad(file, key);
            //        var token = $('#upToken').val();

            //        var complete = function (res) {
            //            $main.log(res);
            //            $('#attachFile1').val(res.key);
                        
            //        };

            //        var error = function (err) {
            //            subscription.unsubscribe();
            //            $('#uploadBody').html('');
            //            alert(err.message);
            //        };


            //        var next = function (response) {
            //            var chunks = response.chunks || [];
            //            var total = response.total;
            //            $('#uploadBody div.colorDiv')
            //                 .css(
            //                   "width",
            //                   total.percent + "%"
            //                 );
                        
            //        };

            //        var subObject = {
            //            next: next,
            //            error: error,
            //            complete: complete
            //        };

            //        observable = qiniu.upload(file, key, token, putExtra, config);

            //        addVideoHtml(key, subObject);
            //    }
            //}

            function addVideoHtml(key) {
                var html = '<tr>';
                html += '<td>' + key + '</td>';
                html += '<td>' + '<div style="overflow:hidden"><div style="float:left;width:80%;height:30px;border:1px solid;border-radius:3px">' +
                        '<div class="colorDiv" style="width:0;border:0;background-color:rgba(232,152,39,0.8);height:28px;"></div></div>' +
                        ' <button type="button" class="btn btn-default btnRemove">删除</button>' + '</div></td></tr>';

                $('#uploadBody').html(html);

                $('.btnRemove').click(function () {
                    $(this).parent().parent().parent().remove();
                });
            }

            //富文本
            var editor1 = UE.getEditor('container', {
                initialFrameHeight: 500,
                autoHeightEnabled: false
            });

            $('.datepicker').datepicker({
                format: 'yyyy-mm-dd'
            });

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

            (function () {
                $main.ajaxGet({
                    url: getClassUrl,
                    data: {},
                    success: function (data) {
                        $('#videoClass').html(data);
                        $('#videoClass').val($('#classId').val());
                    }
                });
            })();


            //上传
            $('.btnUpload').click(function () {
                var formId = $(this).data('formId');
                $('#' + formId + ' input[type=file]').click();
            });

            $('.uploadForm input[type=file]').change(function () {
                if (!$(this).val()) {
                    $main.showError('addForm', '请选择文件');
                    return;
                }
                $(this).parent().find('input[type=submit]').click();
            });


            $('#addForm').validate({
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