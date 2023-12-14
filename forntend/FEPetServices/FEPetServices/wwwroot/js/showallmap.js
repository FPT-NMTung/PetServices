function showMap() {
    var partnerSelect = document.getElementById("PartnerInfoId");
    var mapModal = document.getElementById("mapModal");

    // Tạo một đối tượng bản đồ
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 10, // Điều chỉnh mức độ thu phóng
        center: { lat: 0, lng: 0 } // Trung tâm của bản đồ
    });

    // Duyệt qua danh sách đối tác và thêm đánh dấu cho mỗi đối tác
    for (var i = 0; i < partnerSelect.options.length; i++) {
        var partnerOption = partnerSelect.options[i];
        var lat = partnerOption.getAttribute("data-lat");
        var lng = partnerOption.getAttribute("data-lng");
        var lastName = partnerOption.text.split(" - ")[0];

        if (lat && lng) {
            // Tạo một đối tượng LatLng cho mỗi đối tác
            var latLng = new google.maps.LatLng(parseFloat(lat), parseFloat(lng));

            // Thêm đánh dấu cho mỗi đối tác trên bản đồ
            var marker = new google.maps.Marker({
                position: latLng,
                map: map,
                title: lastName
            });

            // Tạo một cửa sổ thông tin để hiển thị tên của đối tác khi nhấp vào đánh dấu
            var infowindow = new google.maps.InfoWindow({
                content: lastName
            });

            marker.addListener('click', function () {
                infowindow.open(map, marker);
            });
        }
    }

    // Mở modal
    mapModal.style.display = "block";
}

function closeMapModal() {
    var mapModal = document.getElementById("mapModal");
    mapModal.style.display = "none";
}