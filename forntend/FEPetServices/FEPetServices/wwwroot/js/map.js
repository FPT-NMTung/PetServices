var map;

function initMap() {
    // You can customize the map initialization here
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: -34.397, lng: 150.644 },
        zoom: 8
    });
}

function closeMapModal() {
    document.getElementById('mapModal').style.display = 'none';
}

$(document).ready(function () {
    // Open modal on button click
    $(".view-map-btn").click(function () {
        $("#mapModal").css("display", "block");
    });

    // Close modal on close button click
    $(".close").click(function () {
        $("#mapModal").css("display", "none");
    });

    // Close modal on outside click
    $(window).click(function (event) {
        if (event.target.id === "mapModal") {
            $("#mapModal").css("display", "none");
        }
    });
});
