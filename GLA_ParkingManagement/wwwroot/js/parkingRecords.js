$(document).ready(function () {

    $('#parkingTable').DataTable({
        ajax: {
            url: '/Parking/GetAllParkingRecords',
            type: 'GET',
            dataSrc: 'data'
        },

        columns: [
            { data: 'vehicleNumber' },

            {
                data: 'entryTime',
                render: function (data) {
                    return formatDate(data);
                }
            },

            {
                data: 'exitTime',
                render: function (data) {
                    return data ? formatDate(data) : '-';
                }
            },

            {
                data: 'totalAmount',
                render: function (data) {
                    return data ? "₹ " + data : '-';
                }
            },

            {
                data: 'status',
                render: function (data) {

                    if (data === "Pending")
                        return '<span class="badge bg-warning">Pending</span>';

                    if (data === "Confirmed")
                        return '<span class="badge bg-success">Active</span>';

                    if (data === "Completed")
                        return '<span class="badge bg-secondary">Completed</span>';

                    return data;
                }
            },

            {
                data: 'id',
                className: "text-center",
                render: function (data, type, row) {

                    // Only show button if active
                    if (row.status === "Confirmed") {
                        return `
                            <button class="btn btn-danger btn-sm"
                                onclick="completeParking(${data})">
                                Complete
                            </button>
                        `;
                    }

                    return '-';
                }
            }
        ]
    });

});


// Complete Parking
function completeParking(id) {

    if (!confirm("Are you sure you want to complete this parking?"))
        return;

    $.ajax({
        url: '/Parking/CompleteParking',
        type: 'POST',
        data: { id: id },

        success: function (res) {

            if (res.success) {
                toastr.success(res.message);
                reloadTable();
            } else {
                toastr.error(res.message);
            }
        },

        error: function () {
            toastr.error("Something went wrong");
        }
    });
}


// Reload table
function reloadTable() {
    $('#parkingTable').DataTable().ajax.reload();
}


// Format date
function formatDate(dateStr) {
    var d = new Date(dateStr);
    return d.toLocaleString();
}