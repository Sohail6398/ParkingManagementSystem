$(document).ready(function () {

    $('#pendingTable').DataTable({
        ajax: {
            url: '/Parking/GetPendingRequests',
            type: 'GET',
            dataSrc: 'data'
        },

        columns: [
            { data: 'vehicleNumber' },
            { data: 'vehicleType' },
            { data: 'slotNumber' },

            {
                data: 'entryTime',
                render: function (data) {
                    return new Date(data).toLocaleString();
                }
            },

            { data: 'userName' },

            {
                data: 'id',
                className: "text-center",
                render: function (data) {

                    return `
                        <button class="btn btn-success btn-sm me-1"
                            onclick="approve(${data})">
                            ✔
                        </button>

                        <button class="btn btn-danger btn-sm"
                            onclick="reject(${data})">
                            ✖
                        </button>
                    `;
                }
            }
        ]
    });

});


// Approve
function approve(id) {

    $.post('/Parking/ApproveParking', { id: id }, function (res) {

        if (res.success) {
            toastr.success(res.message);
            reloadTable();
        } else {
            toastr.error(res.message);
        }
    });
}


// Reject
function reject(id) {

    $.post('/Parking/RejectParking', { id: id }, function (res) {

        if (res.success) {
            toastr.success(res.message);
            reloadTable();
        } else {
            toastr.error(res.message);
        }
    });
}


// Reload table
function reloadTable() {
    $('#pendingTable').DataTable().ajax.reload();
}