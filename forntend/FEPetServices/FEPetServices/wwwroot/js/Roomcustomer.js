document.addEventListener("DOMContentLoaded", function () {
    var priceElements = document.querySelectorAll('.current-price');
    priceElements.forEach(function (priceElement) {
        var roomCategory = priceElement.closest('.product-content').dataset.roomCategory;
        var priceText = priceElement.textContent.trim();
        var price = parseFloat(priceText.replace(/\s/g, '').replace('vnđ/h', ''));
        var formattedPrice = price.toLocaleString('vi-VN');
        if (roomCategory == 4) {
            priceElement.textContent = formattedPrice + ' vnđ/ngày';
        } else {
            priceElement.textContent = formattedPrice + ' vnđ/giờ';
        }
    });
});