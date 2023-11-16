
window.onload = function () {
    let startDateTime = document.getElementById("start-datetime");
    let endDateTime = document.getElementById("end-datetime");
    let status = document.querySelector(".status");

    var priceElements = document.querySelectorAll('.current-price');
    priceElements.forEach(function (priceElement) {
        var priceText = priceElement.textContent.trim();
        var price = parseFloat(priceText.replace(/\s/g, '').replace('vnđ/h', ''));
        var formattedPrice = price.toLocaleString('vi-VN');
        priceElement.textContent = formattedPrice + ' vnđ/giờ';

    });

    var priceElements = document.querySelectorAll('.content p');
    priceElements.forEach(function (priceElement) {
        var priceText = priceElement.textContent.trim();
        var price = parseFloat(priceText.replace(/\s/g, '').replace('vnđ', ''));
        var formattedPrice = price.toLocaleString('vi-VN');
        priceElement.textContent = formattedPrice + ' vnđ';

    });

    let startPicker = flatpickr(startDateTime, {
        minDate: "today",
        enableTime: true,
        time_24hr: true,
        minTime: new Date().getHours() + 1 + ":00", // Giờ hiện tại + 1 giờ
        maxTime: "17:00",
        dateFormat: "m-d-Y H:i",
        onChange: function (selectedDates, dateStr, instance) {
            let currentDate = new Date();
            let tomorrow = new Date(currentDate);

            tomorrow.setDate(currentDate.getDate() + 1);
            tomorrow.setHours(8, 0, 0, 0);

            if (currentDate.getHours() >= 16) {
                startPicker.set("minDate", tomorrow);
            }

            if (selectedDates[0] > currentDate) {
                startPicker.set("minTime", "08:00");
            } else {
                startPicker.set("minTime", currentDate.getHours() + 1 + ":00");
            }

            endPicker.set("minDate", selectedDates[0]);
            updateStatus();
        },
    });

    let endPicker = flatpickr(endDateTime, {
        enableTime: true,
        time_24hr: true,
        minTime: new Date().getHours() + 1 + ":00", // Giờ hiện tại + 1 giờ
        maxTime: "17:00",
        dateFormat: "m-d-Y H:i",
        onChange: function (selectedDates, dateStr, instance) {
            let startDate = startPicker.selectedDates[0];

            if (startDate && selectedDates[0].getDate() === startDate.getDate()
                && selectedDates[0].getMonth() === startDate.getMonth()
                && selectedDates[0].getFullYear() === startDate.getFullYear()) {
                if (startDate.getHours() >= 16) {
                    startPicker.set("minDate", tomorrow);
                }
                endPicker.set("minTime", startDate.getHours() + 1 + ":00");
            } else {
                endPicker.set("minTime", "8:00");
            }

            updateStatus();
        },
    });

    $(document).ready(function () {
        $('input[type="checkbox"]').change(function () {
            updateServices();
            updateTotal();
        });

        function updateServices() {
            $('.name-service').text('');
            $('.price-service').text('');

            $('input[type="checkbox"]').each(function () {
                if ($(this).is(':checked')) {
                    var serviceName = $(this).closest('.service-box').find('.title').text();
                    var price = $(this).closest('.service-box').find('p').text();
                    $('.name-service').append(serviceName + '<br>');

                    var formattedPriceservice = parseFloat(price.replace(' vnđ', '').replace('.', '')).toLocaleString('vi-VN') + ' vnđ';
                    $('.price-service').append(formattedPriceservice + '<br>');
                }
            });
        }
    });

    function updateStatus() {
        let startValue = startPicker.input.value;
        let endValue = endPicker.input.value;

        if (startValue && endValue) {
            let startDate = startPicker.parseDate(startValue);
            let endDate = endPicker.parseDate(endValue);
            let roomId = @Model.RoomId;

            let apiURL = `https://pet-service-api.azurewebsites.net/api/Room/CheckSlotInRoom?RoomId=${roomId}&startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`;
            console.log(apiURL);
            fetch(apiURL)
                .then(response => {
                    if (response.status === 200) {
                        status.textContent = "Còn phòng";
                        status.style.color = "#00cc4f";
                        document.getElementById("super-happy").checked = true;
                        document.querySelector('label[for="super-happy"]').style.display = 'block';

                        let duration = calculateDuration(startDate, endDate);
                        let totalPrice = duration * @Model.Price;
                        let timeduration = parseInt(duration / 24) + " ngày" + duration % 24 + " giờ";
                        let timeBookingLabel = `${startDate.toLocaleString()} - ${endDate.toLocaleString()} (${timeduration})`;
                        let priceTimeBooking = totalPrice.toLocaleString('vi-VN') + ' vnđ';

                        $('.time-booking').text(timeBookingLabel);
                        $('.price-time-booking').text(priceTimeBooking);

                        updateTotal();
                    } else {
                        status.textContent = "Không còn phòng";
                        status.style.color = "#e76555";
                        document.getElementById("super-sad").checked = true;
                        document.querySelector('label[for="super-sad"]').style.display = 'block';
                    }
                })
                .catch(error => {
                    console.error("Error fetching API:", error);
                    status.textContent = "Lỗi kết nối API";
                    hideLabels();
                });
        } else {
            status.textContent = "";
        }
    }

    function updateTotal() {
        let total = 0;

        $('input[type="checkbox"]').each(function () {
            if ($(this).is(':checked')) {
                var priceText = $(this).closest('.service-box').find('p').text();

                var price = priceText ? parseInt(priceText.replace(/,/g, '').replace(/\D/g, '')) : 0;
                total += price;
            }
        });

        var timeBookingText = $('.price-time-booking').text();
        var timeBookingPrice = timeBookingText ? parseInt(timeBookingText.replace(/,/g, '').replace(/\D/g, '')) : 0;
        total += timeBookingPrice;

        $('.order-total-amount').text(total.toLocaleString('vi-VN') + ' vnđ');
    }

    function calculateDuration(startDate, endDate) {
        const oneDayInMillis = 24 * 60 * 60 * 1000;
        const millisecondsDiff = endDate - startDate;
        const days = Math.floor(millisecondsDiff / oneDayInMillis);
        const hours = Math.floor((millisecondsDiff % oneDayInMillis) / (60 * 60 * 1000));
        const totalhours = days * 24 + hours;
        return totalhours;
    }

    function hideLabels() {
        document.querySelector('label[for="super-happy"]').style.display = 'none';
        document.querySelector('label[for="super-sad"]').style.display = 'none';
    }
}