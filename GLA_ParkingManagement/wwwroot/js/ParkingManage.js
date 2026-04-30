$(document).ready(function () {
    $('#vehicleTypeTable').DataTable({
        ajax: {
            url: '/Parking/GetAllVehicleTypes',
            type: 'GET',
            dataSrc: function (json) {
                return json.data || [];
            }
        },

        ordering: true, // keep sorting
        columnDefs: [
            { orderable: false, targets: 3 } // disable sort on Actions
        ],

        columns: [
            {
                data: 'id',
                className: "text-start fw-bold" 
            },
            {
                data: 'name'
            },
            {
                data: 'hourlyRate',
                render: function (data) {
                    return "₹ " + data;
                }
            },
            {
                data: null,
                className: "text-center",
                render: function (data, type, row) {
                    return `
                    <button class="btn btn-sm btn-warning me-1"
                        onclick="editVehicle(${row.id}, '${row.name}', ${row.hourlyRate})">
                        <i class="fa fa-edit"></i>
                    </button>

                    <button class="btn btn-sm btn-danger"
                        onclick="deleteVehicle(${row.id})">
                        <i class="fa fa-trash"></i>
                    </button>
                `;
                }
            }
        ]
    });

});
// Open Add Modal
function openAddModal() {
    $("#modalTitle").text("Add Vehicle Type");
    $("#vehicleForm")[0].reset();
    $("#vehicleId").val(0);
    $("#addVehicleModal").modal("show");
}

// Open Edit Modal
function editVehicle(id, name, rate) {
    $("#modalTitle").text("Update Vehicle Type");

    $("#modalTitle").text("Update Vehicle Type");
    $("#vehicleId").val(id);
    $("#vehicleName").val(name);
    $("#hourlyRate").val(rate);

    $("#addVehicleModal").modal("show");
}
function saveVehicleType() {
    var formData = $("#vehicleForm").serialize();
    $.ajax({
        url: '/Parking/AddUpdateVehicleType',
        type: 'POST',
        data: formData,

        success: function (res) {
            if (res.success) {
                toastr.success(res.message);
                var modal = bootstrap.Modal.getInstance(document.getElementById('addVehicleModal'));
                modal.hide();
                $("#vehicleForm")[0].reset();
                $('#vehicleTypeTable').DataTable().ajax.reload();
            }
            else {
                toastr.error(res.message);
            }
        },

        error: function (err) {
            toastr.error(err.responseText || "Something went wrong");
        }
    });
}