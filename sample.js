
// Function to handle the "Click to See Your Booked Seats" button
function showBookedSeatsModal() {
    const bookedSeatsModal = new bootstrap.Modal(document.getElementById("bookedSeatsModal"));
    bookedSeatsModal.show();
}

// Existing functions in the file
//function changeLocation(location) {
//    window.location.href = `?location=${location}`;
//}

function changeLocation(location) {
    window.location.href = `?location=${location}`;
}






// Function to toggle visibility of the "Book Your Seats" button
function toggleBookingButton() {
    const selectedSeats = document.querySelectorAll('.seat-checkbox:checked').length;
    const bookSeatsBtn = document.getElementById('bookSeatsBtn');

    if (selectedSeats >= 3 && selectedSeats <= 5) {
        bookSeatsBtn.style.display = 'block'; // Show button
    } else {
        bookSeatsBtn.style.display = 'none'; // Hide button
    }

    //if (selectedSeats >= 3 && selectedSeats <= 5) {
    //    bookSeatsBtn.classList.add('visible'); // Show button
    //} else {
    //    bookSeatsBtn.classList.remove('visible'); // Hide button
    //}
}

// Add event listeners to all checkboxes
function setupCheckboxListeners() {
    const checkboxes = document.querySelectorAll('.seat-checkbox');
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', toggleBookingButton);
    });
}

// Call this function after dynamically loading seats or on page load
document.addEventListener('DOMContentLoaded', () => {
    setupCheckboxListeners(); // Set up listeners
    toggleBookingButton(); // Initial check on page load
});


function openBookingModal() {
    const selectedSeats = Array.from(document.querySelectorAll('.seat-checkbox:checked')).map(cb => cb.dataset.seatId);
    const selectedSeatNumbers = Array.from(document.querySelectorAll('.seat-checkbox:checked')).map(cb => cb.dataset.seatNumber);

    if (selectedSeats.length < 3 || selectedSeats.length > 5) {
        alert("Please select between 3 and 5 seats to book.");
        return;
    }

    document.getElementById('selectedSeats').value = selectedSeats.join(',');

    const seatDetailsContainer = document.getElementById('seatDetailsContainer');
    seatDetailsContainer.innerHTML = '';

    // Generate date inputs for each seat
    selectedSeatNumbers.forEach((seatNumber, index) => {
        const seatDetail = document.createElement('div');
        seatDetail.classList.add('form-group', 'mt-2');
        seatDetail.innerHTML = `
            <label for="seatDate${index}">Seat ${seatNumber} - Choose Date:</label>
            <input type="date" class="form-control" id="seatDate${index}" name="seatDate${index}" required />
        `;
        seatDetailsContainer.appendChild(seatDetail);
    });

    // Generate time inputs that apply to all seats
    const timeInputs = document.createElement('div');
    timeInputs.classList.add('form-group', 'mt-2');
    timeInputs.innerHTML = `
        <label for="startTime">Start Time:</label>
        <input type="time" class="form-control" id="startTime" name="startTime" required />
        <label for="endTime" class="mt-2">End Time:</label>
        <input type="time" class="form-control" id="endTime" name="endTime" required />
    `;
    seatDetailsContainer.appendChild(timeInputs);

    const bookingModal = new bootstrap.Modal(document.getElementById('bookingModal'));
    bookingModal.show();
}

function showSeatDetails(element) {
    const hoverCard = element.querySelector('.hover-card');
    hoverCard.style.display = 'block';
}

function hideSeatDetails(element) {
    const hoverCard = element.querySelector('.hover-card');
    hoverCard.style.display = 'none';
}

document.querySelectorAll('.seat-checkbox').forEach(checkbox => {
    checkbox.addEventListener('change', function () {
        const selectedSeats = document.querySelectorAll('.seat-checkbox:checked').length;
        const bookSeatsBtn = document.getElementById('bookSeatsBtn');

        if (selectedSeats >= 3 && selectedSeats <= 5) {
            bookSeatsBtn.classList.add('visible');
        } else {
            bookSeatsBtn.classList.remove('visible');
        }
    });
});

document.getElementById('bookingForm').addEventListener('submit', function (e) {
    e.preventDefault();

    const selectedSeats = document.getElementById('selectedSeats').value.split(',');
    const startTime = document.getElementById('startTime').value;
    const endTime = document.getElementById('endTime').value;
    const timeSlot = `${startTime} - ${endTime}`;

    const seatDetails = selectedSeats.map((seatId, index) => {
        const bookingDate = document.getElementById(`seatDate${index}`).value;

        // Check if the selected date is a weekend
        const selectedDate = new Date(bookingDate);
        const dayOfWeek = selectedDate.getUTCDay(); // 0 = Sunday, 6 = Saturday

        if (dayOfWeek === 0 || dayOfWeek === 6) {
            alert("Please don't try to book on a non-working day.");
            throw new Error("Non-working day selected");
        }

        return { seatIds: parseInt(seatId), bookingDate, timeSlot };
    });

    console.log('Seat Details:', seatDetails); // Debugging line

    fetch('/Employee/BookSeats', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(seatDetails)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert('Seats booked successfully!');
                location.reload(); // Reload page to reflect changes
            } else {
                alert(data.message || 'An error occurred while booking the seats.');
            }
        })
        .catch(err => console.error(err));
});

// Show booking history when the button is clicked
document.getElementById('viewBookingHistoryBtn').addEventListener('click', function () {
    const bookingHistoryContainer = document.getElementById('bookingHistoryContainer');
    bookingHistoryContainer.style.display = bookingHistoryContainer.style.display === 'none' ? 'block' : 'none';
});










//<div class="d-flex justify-content-between align-items-center mt-4">
//    <div class="text-center w-100">
//        <button id="bookSeatsBtn" class="btn btn-primary mt-3" onclick="openBookingModal()"
//            style="display:none;">
//            Book Selected Seats
//        </button>
//    </div>
//</div>