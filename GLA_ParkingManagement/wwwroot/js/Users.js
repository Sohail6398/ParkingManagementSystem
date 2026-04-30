$(document).ready(function () {
    $.ajax({
        url: '/Admin/GetAllUsers',
        type: 'GET',  
        dataType: 'json',
        success: function (response) {
            // Runs if request succeeds
            console.log(response);
            bindUserTbl(response)
        },
        error: function (xhr, status, error) {
            // Runs if request fails
            console.error(error);
        }
    })

});

function bindUserTbl(responseData) {
   
    $('#usersTable').DataTable({
        "data": responseData.data,
        "columns": [
            {
                "data": "id",
                "visible": false
            },
            {
                "data": null,
                "render": function (data, type, row) {
                    return `<strong>${row.firstName} ${row.lastName}</strong>`;
                }
            },
            {
                "data": "gender",
                "render": function (data) {
                    // Badge for Gender
                    let cssClass = data === "Male" ? "bg-primary" : "bg-info";
                    return `<span class="badge ${cssClass}">${data}</span>`;
                }
            },
            { "data": "email" },
            { "data": "phoneNumber" },
            { "data": "address" },
            {
                "data": "createdAt",
                "render": function (data) {
                    return new Date(data).toLocaleDateString(); // Formats date
                }
            },
            {
                "data": "isActive",
                "render": function (data) {
                    // Badge for Status (IsActive)
                    let statusText = data ? "Active" : "Inactive";
                    let statusClass = data ? "bg-success" : "bg-danger";
                    return `<span class="badge ${statusClass}">${statusText}</span>`;
                }
            },
            {
                "data": null,
                "defaultContent": '<span class="badge bg-secondary">User</span>'
            },
            {
                "data": null,
                "className": "text-center",
                "orderable": false,
                "render": function (data, type, row) {
                    return `
                        <div class="btn-group">
                            <button class="btn btn-sm btn-outline-primary m-1" onclick="editUser('${row.id}')">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="btn btn-sm btn-outline-danger m-1" onclick="deleteUser('${row.id}')">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    `;
                }
            }
        ],
        "responsive": true,
        "order": [[6, "desc"]] // Sort by Created At by default
    });
}
