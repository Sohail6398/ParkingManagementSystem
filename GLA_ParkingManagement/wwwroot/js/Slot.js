$(document).ready(function () {

    loadVehicleTypes();

    $('#slotTable').DataTable({
        ajax: {
            url: '/Parking/GetAllSlots',
            dataSrc: 'data'
        },
        columns: [
            { data: 'id' },
            { data: 'slotNumber' },
            { data: 'vehicleTypeName' },
            {
                data: 'isOccupied',
                render: d => d ? '<span class="badge bg-danger">Occupied</span>' :
                    '<span class="badge bg-success">Available</span>'
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
                        <button onclick="editSlot(${row.id}, '${row.slotNumber}', ${row.vehicleTypeId})" class="btn btn-warning btn-sm"><i class="fa fa-edit"></i></button>
                        <button onclick="deleteSlot(${row.id})" class="btn btn-danger btn-sm"><i class="fa fa-trash"></i></button>
                    `;
                }
            }
        ]
    });
});

function loadVehicleTypes() {
    $.get('/Parking/GetAllVehicleTypes', function (res) {
        let options = '<option value="">Select</option>';
        res.data.forEach(x => {
            options += `<option value="${x.id}">${x.name}</option>`;
        });
        $('#vehicleTypeId').html(options);
    });
}

function saveSlot() {

    $.post('/Parking/AddUpdateSlot', $("#slotForm").serialize(), function (res) {

        if (res.success) {
            toastr.success(res.message);

            bootstrap.Modal.getInstance(document.getElementById('slotModal')).hide();

            $('#slotTable').DataTable().ajax.reload();
        } else {
            toastr.error(res.message);
        }
    });
}
function deleteSlot(id) {

    if (!confirm("Delete this slot?")) return;

    $.ajax({
        url: '/Parking/DeleteSlot?id=' + id,
        type: 'DELETE',
        success: function (res) {
            toastr.success(res.message);
            $('#slotTable').DataTable().ajax.reload();
        }
    });
}

function openSlotModal() {
    $("#slotForm")[0].reset();
    $("#slotId").val(0);
    $("#slotModalLabel").text("Add Slot");
    $("#slotModal").modal("show");
}