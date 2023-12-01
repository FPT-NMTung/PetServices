function validateForm() {
    const fname = document.getElementById('ProductName');
    const price = document.getElementById('Price');
    const quantity = document.getElementById('Quantity');
    /*const subject = document.getElementById('Desciption');*/
    const fnameErrorMessage = document.getElementById('fname-error-message');
    const priceErrorMessage = document.getElementById('price-error-message');
    const quantityErrorMessage = document.getElementById('quantity-error-message');
    /*const subjectErrorMessage = document.getElementById('subject-error-message');*/

    const specialChars = /[!@#%^*{}\[\]<>?-]/
    const specialChar = /[@#$%^&*{}\[\]~]/;
    const numbersOnly = /^[0-9]+$/;

    fnameErrorMessage.textContent = "";
    priceErrorMessage.textContent = "";
    quantityErrorMessage.textContent = "";
    subjectErrorMessage.textContent = "";

    let isValid = true;

    if (specialChars.test(fname.value)) {
        fnameErrorMessage.textContent = "Không được chứa ký tự đặc biệt.";
        isValid = false;
    }

    if (!numbersOnly.test(price.value)) {
        priceErrorMessage.textContent = "Giá phải là số.";
        isValid = false;
    } else {
        const priceValue = parseInt(price.value);
        if (priceValue <= 0) {
            priceErrorMessage.textContent = "Giá phải lớn hơn 0.";
            isValid = false;
        }
    }

    if (!numbersOnly.test(quantity.value)) {
        quantityErrorMessage.textContent = "Số lượng phải là số.";
        isValid = false;
    } else {
        const quantityValue = parseInt(quantity.value);
        if (quantityValue <= 0) {
            quantityErrorMessage.textContent = "Số lượng phải lớn hơn 0.";
            isValid = false;
        }
    }

    if (specialChar.test(subject.value)) {
        subjectErrorMessage.textContent = "Không được chứa ký tự đặc biệt.";
        isValid = false;
    }

    return isValid;
}

function changeImageSource() {
    const imageInput = document.getElementById('image');
    const imagePreview = document.getElementById('imagePreview');

    if (imageInput.files && imageInput.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            imagePreview.src = e.target.result;
        };
        reader.readAsDataURL(imageInput.files[0]);
    }
}