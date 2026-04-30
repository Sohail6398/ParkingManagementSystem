$(document).ready(function () {
    loadVehicleTypes();
});


// Load Vehicle Types
function loadVehicleTypes() {
    $.get('/Parking/GetAllVehicleTypes', function (res) {

        let html = '';

        res.data.forEach(v => {
            html += `
                <div class="col-md-3 mb-3">
                    <div class="card shadow-sm text-center p-3">
                        <h5>${v.name}</h5>
                        <p>₹ ${v.hourlyRate}/hr</p>

                        <button class="btn btn-primary"
                            onclick="loadSlots(${v.id}, '${v.name}')">
                            View Slots
                        </button>
                    </div>
                </div>
            `;
        });

        $("#vehicleTypeContainer").html(html);
    });
}


// Load Slots by Vehicle Type
function loadSlots(vehicleTypeId, vehicleName) {

    $("#selectedVehicleTitle").text(`Available Slots for ${vehicleName}`);

    $.get('/Parking/GetAvailableSlots?vehicleTypeId=' + vehicleTypeId, function (res) {

        let html = '';

        if (res.length === 0) {
            html = `<p class="text-danger">No slots available</p>`;
        } else {

            res.forEach(s => {
                html += `
                    <div class="col-md-2 mb-3">
                        <div class="card text-center p-2 slot-card 
                            ${s.isOccupied ? 'bg-danger text-white' : 'bg-success text-white'}"
                            
                            ${!s.isOccupied ? `onclick="openBookingModal(${s.id}, ${s.vehicleTypeId})"` : ''}>

                            <h6>${s.slotNumber}</h6>
                            <small>${s.isOccupied ? 'Occupied' : 'Available'}</small>
                        </div>
                    </div>
                `;
            });
        }

        $("#slotsContainer").html(html);
    });
}

// Open modal
function openBookingModal(slotId, vehicleTypeId) {
    $("#slotId").val(slotId);
    $("#vehicleTypeId").val(vehicleTypeId);
    $("#vehicleNumber").val("");

    var modal = new bootstrap.Modal(document.getElementById('bookingModal'));
    modal.show();   
}
function bookParking() {

    var data = {
        vehicleNumber: $("#vehicleNumber").val(),
        vehicleTypeId: $("#vehicleTypeId").val(),
        slotId: $("#slotId").val()
    };

    if (!data.vehicleNumber) {
        toastr.error("Vehicle number is required");
        return;
    }

    $.ajax({
        url: '/Parking/BookParking',
        type: 'POST',
        data: data,

        success: function (res) {

            if (res.success) {
                toastr.success(res.message);

                $("#bookingModal").modal("hide");

                // reload slots
                loadSlots(data.vehicleTypeId, '');
            } else {
                toastr.error(res.message);
            }
        },

        error: function () {
            toastr.error("Something went wrong");
        }
    });
}