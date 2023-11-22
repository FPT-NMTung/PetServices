function getnewLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            function (position) {
                var latitude = position.coords.latitude;
                var longitude = position.coords.longitude;

                // Show position on the map
                initnewMap(latitude, longitude);

                // Update hidden input fields with latitude and longitude
                document.getElementById("inputLatt").value = latitude;
                document.getElementById("inputLngg").value = longitude;
            },
            function (error) {
                showError(error);
            }
        );
    } else {
        alert("Trình duyệt của bạn không hỗ trợ Geolocation.");
    }
}

function initnewMap(latitude, longitude) {
    var map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: latitude, lng: longitude },
        zoom: 15
    });

    var marker = new google.maps.Marker({
        position: { lat: latitude, lng: longitude },
        map: map,
        title: 'Vị trí của bạn'
    });
}

function showError(error) {
    // Handle error messages as needed
    switch (error.code) {
        case error.PERMISSION_DENIED:
            alert("Truy cập vào vị trí bị từ chối bởi người dùng.");
            break;
        case error.POSITION_UNAVAILABLE:
            alert("Không thể lấy được vị trí của bạn.");
            break;
        case error.TIMEOUT:
            alert("Yêu cầu lấy vị trí của bạn đã quá thời gian.");
            break;
        case error.UNKNOWN_ERROR:
            alert("Đã xảy ra lỗi không xác định.");
            break;
    }
}