

document.querySelectorAll('.seat-icon').forEach(button => {
    button.addEventListener('click', function () {
        const seatId = this.dataset.seatId;
        const seatStatus = this.dataset.seatStatus;

        if (seatStatus === "Booked") {
            alert("This seat is already booked.");
            return;
        }

        // Open the booking modal
        const modalTitle = document.getElementById('seatModalLabel');
        const modalContent = document.getElementById('seatModalContent');

        modalTitle.innerText = `Seat ${seatId} - Available`;
        modalContent.innerHTML = `
            <form id="bookingForm">
                <input type="hidden" name="seatId" value="${seatId}" />
                <div class="form-group">
                    <label for="bookingDate">Choose Date:</label>
                    <input type="date" class="form-control" name="bookingDate" required />
                </div>
                <div class="form-group mt-2">
                    <label for="bookingTime">Choose Time:</label>
                    <input type="time" class="form-control" name="bookingTime" required />
                </div>
                <button type="submit" class="btn btn-success mt-3">Book Seat</button>
            </form>
        `;

        const modal = new bootstrap.Modal(document.getElementById('seatModal'));
        modal.show();

        // Handle booking form submission
        const bookingForm = document.getElementById('bookingForm');
        bookingForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const formData = new FormData(bookingForm);

            fetch('/Employee/BookSeat', {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        alert("Seat booked successfully!");

                        // Update seat status in the view
                        const seatButton = document.querySelector(`button[data-seat-id="${seatId}"]`);
                        seatButton.classList.remove('available');
                        seatButton.classList.add('booked');
                        seatButton.dataset.seatStatus = "Booked";

                        // Update booking history
                        const historyTable = document.querySelector('table tbody');
                        historyTable.innerHTML = ""; // Clear current history

                        data.bookingHistory.forEach(booking => {
                            const row = `
                            <tr>
${booking.id}</td>
${booking.seat.seatNumber}</td>
                                <td>${new Date(booking.bookingDate).toLocaleDateString()}</td>
                                <td>${new Date(booking.bookingDate).toLocaleTimeString()}</td>
                            </tr>
                        `;
                            historyTable.innerHTML += row;
                        });

                        modal.hide();
                    } else {
                        alert(data.error || "An error occurred while booking the seat.");
                    }
                })
                .catch(error => {
                    console.error("Error booking seat:", error);
                    alert("A network error occurred. Please try again.");
                });
        });
    });
});
        