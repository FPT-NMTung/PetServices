document.addEventListener("DOMContentLoaded", function () {
    var priceElements = document.querySelectorAll('.current-price');
    priceElements.forEach(function (priceElement) {
        var priceText = priceElement.textContent.trim();
        var price = parseFloat(priceText.replace(/\s/g, '').replace('vnđ/h', ''));
        var formattedPrice = price.toLocaleString('vi-VN');
        priceElement.textContent = formattedPrice + ' vnđ/giờ';

    });
});