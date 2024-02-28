var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'title', "width": "20%" },
            { data: 'vendorCode', "width": "10%" },
            { data: 'brand', "width": "10%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'price', "width": "5%" },
            { data: 'category.name', "width": "14%" },
            {
                data: null,
                render: function (data, type, row) {
                    return row.carModel ? (row.carModel.brand + ' ' + row.carModel.model) : '';
                },
                "width": "10%"
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-100 btn-group" role="group">
                     <a href="/admin/product/upsert?id=${data}" class="btn btn-primary p-1 mx-1"> <i class="bi bi-pencil-square"></i> Змінити</a>               
                     <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger p-1 mx-1"> <i class="bi bi-trash-fill"></i> Видалити</a>
                    </div>`
                },
                "width": "15%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Ви впевнені?',
        text: "Ви не зможете скасувати це!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Так, відалити!',
        cancelButtonText: 'Відміна'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}