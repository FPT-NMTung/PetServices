function addToCart(productId) {
    $.ajax({
        url: '/ProductList/AddToCart',
        type: 'POST',
        data: { ProductId: productId },
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            $('#successModal').modal('show');
            console.log(result);
            updateCartCount(result.totalQuantity);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function addToCartService(serviceId) {
    $.ajax({
        url: '/Home/AddToCart',
        type: 'POST',
        data: { ServiceId: serviceId },
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            $('#successModal').modal('show');
            console.log(result);
            updateCartCount(result.totalQuantity);
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function updateHiddenQuantity() {
    var quantityInput = document.getElementById('quantityInput');
    var hiddenQuantity = document.getElementById('hiddenQuantity');
    hiddenQuantity.value = quantityInput.value;

    console.log("Updated hidden quantity: " + hiddenQuantity.value);
}

function updateCartCount(totalQuantity) {
    $('#cart-count').text(totalQuantity);
}