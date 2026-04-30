$(document).ready(function () {

    $('#historyTable').DataTable({
        ajax: {
            url: '/Parking/GetBookingHistory',
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
                    return "₹ " + data;
                }
            },

            {
                data: 'status',
                render: function (data) {

                    if (data === "Pending")
                        return '<span class="badge bg-warning">Pending</span>';

                    if (data === "Confirmed")
                        return '<span class="badge bg-success">Confirmed</span>';

                    if (data === "Completed")
                        return '<span class="badge bg-secondary">Completed</span>';

                    return data;
                }
            }
        ]
    });

});


// Format date
function formatDate(dateStr) {
    var d = new Date(dateStr);
    return d.toLocaleString();
}