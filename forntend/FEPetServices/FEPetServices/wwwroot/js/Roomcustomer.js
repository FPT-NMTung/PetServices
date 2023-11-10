document.addEventListener("DOMContentLoaded", function () {
    var priceElements = document.querySelectorAll('.current-price');
    priceElements.forEach(function (priceElement) {
        var priceText = priceElement.textContent.trim();
        var price = parseFloat(priceText.replace(/\s/g, '').replace('vnđ/h', ''));
        var formattedPrice = price.toLocaleString('vi-VN');
        priceElement.textContent = formattedPrice + ' vnđ/h';
    });

    
});

    $(function () {
        // Set up the slider
        $("#slider-range").slider({
            range: true,
            min: 0,
            max: 1000000, // Set your desired max value
            values: [0, 1000000], // Set your desired initial values
            slide: function (event, ui) {
                // Format the numbers with dots
                $("#amount").val(ui.values[0].toLocaleString() + " - " + ui.values[1].toLocaleString());
            }
        });

    // Initialize the amount input with the initial values
    $("#amount").val($("#slider-range").slider("values", 0).toLocaleString() +
    " - " + $("#slider-range").slider("values", 1).toLocaleString());
    });