var dataTable

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
   dataTable =  $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Category/GetAll"
        },
        "columns": [  
            { "data": "name", "width": "70%" },
            {
                "data": "id",
                "render" : function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Category/Upsert/${data}" class="btn btn-info">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a class="btn btn-danger" onclick="Delete('/Admin/Category/Delete/${data}')">
                                <i class="fas fa-trash-alt"></i>
                            </a>
                        </div>
                    `;
                }
            }
        ]
    })
}

function Delete(url) {
    swal({
        title: "Want to delete data?",
        text: "Delete information",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    swal("Success", data.message, "success");
                    dataTable.ajax.reload();
                },
                error: function (xhr, status, error) {
                    toaster.error("Error occurred while deleting data.");
                }
            });
        }
    });
}

