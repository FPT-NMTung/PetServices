function validateForm() {
    const name = document.getElementById('ServiceName');
    const price = document.getElementById('Price');
    const desciptions = document.getElementById('Desciptions');
    const fnameErrorMessage = document.getElementById('fname-error-message');
    const priceErrorMessage = document.getElementById('price-error-message');
    const subjectErrorMessage = document.getElementById('subject-error-message');

    console.log('Debug: fname.value', name.value);
    console.log('Debug: subject.value', desciptions.value);

    const specialChars = /[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]/; // Special character regex
    const specialChar = /[@#$%^&*{}\[\]~]/;
    const numbersOnly = /^[0-9]+$/; // Only numbers regex

    // Reset error messages
    fnameErrorMessage.textContent = "";
    priceErrorMessage.textContent = "";
    subjectErrorMessage.textContent = "";

    let isValid = true;

    if (specialChars.test(name.value)) {
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

    if (specialChar.test(desciptions.value)) {
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