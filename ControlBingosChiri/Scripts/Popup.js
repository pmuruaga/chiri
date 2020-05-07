$(document).ready(function () {  
    $('#btnUpload').click(function () {  
        var fileUploadUrl = $('#FileUploadUrl').val();  
        var files = new FormData();  
        var file1 = document.getElementById("fileOne").files[0];  
        //var file2 = document.getElementById("fileTwo").files[0];  
        files.append('files[0]', file1);  
        //files.append('files[1]', file2);  
  
        $.ajax({  
            type: 'POST',  
            url: fileUploadUrl,  
            data: files,  
            dataType: 'json',  
            cache: false,  
            contentType: false,  
            processData: false,  
            success: function (response) {  
				$('#uploadMsg').text('Archivo subido correctamente: ' + response.ServerMessage);  
            },  
            error: function (error) {  
                $('#uploadMsg').text('Error has occured. Upload is failed');  
            }  
        });  
    });  
});  