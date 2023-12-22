
function showMap() {
    var partnerSelect = document.getElementById("PartnerInfoId");
    var mapModal = document.getElementById("mapModall");

    // Lấy vị trí hiện tại của bạn
    var myLatitude;
    var myLongitude;

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            myLatitude = position.coords.latitude;
            myLongitude = position.coords.longitude;

            // Tạo một đối tượng bản đồ
            var map = new google.maps.Map(document.getElementById('mapService'), {
                zoom: 10, // Điều chỉnh mức độ thu phóng
                center: { lat: myLatitude, lng: myLongitude } // Trung tâm của bản đồ
            });

            // Marker cho vị trí hiện tại của bạn
            var myMarker = new google.maps.Marker({
                position: { lat: myLatitude, lng: myLongitude },
                map: map,
                title: 'Vị trí của tôi',
                icon: 'http://maps.google.com/mapfiles/ms/icons/blue-dot.png',
                    });

        // Duyệt qua danh sách đối tác và thêm đánh dấu cho mỗi đối tác
        for (var i = 0; i < partnerSelect.options.length; i++) {
            var partnerOption = partnerSelect.options[i];
            var lat = partnerOption.getAttribute("data-lat");
            var lng = partnerOption.getAttribute("data-lng");
            var lastName = partnerOption.text.split(" - ")[0];
            var mnv = partnerOption.text.split(" - ")[1];

            if (lat && lng) {
                // Tạo một đối tượng LatLng cho mỗi đối tác
                var latLng = new google.maps.LatLng(parseFloat(lat), parseFloat(lng));

                // Thêm đánh dấu cho mỗi đối tác trên bản đồ
                addMarker(map, latLng, lastName, mnv);

            }
        }

        // Mở modal
        mapModal.style.display = "block";
    }, function (error) {
        console.error("Error getting current location:", error);
    });
} else {
    alert("Trình duyệt không hỗ trợ lấy vị trí.");
}
        }

function addMarker(map, position, lastName, mnv) {
    // Thêm đánh dấu cho mỗi đối tác trên bản đồ
    var marker = new google.maps.Marker({
        position: position,
        map: map,
        title: lastName,
        label: {
            text: lastName,
            color: 'yellow'
        }
    });

    // Thêm sự kiện click cho marker để hiển thị thông tin
    marker.addListener('click', function () {
        // Hiển thị thông tin của đối tác
        alert(lastName + ' - ' + mnv);
    });
}

function closeMapModal() {
    var mapModal = document.getElementById("mapModall");
    mapModal.style.display = "none";
}
