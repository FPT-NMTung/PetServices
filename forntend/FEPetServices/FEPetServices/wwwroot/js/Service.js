function validateForm() {
    const name = document.getElementById('ServiceName');
    const price = document.getElementById('Price');
    const time = document.getElementById('Time');
    const desciptions = document.getElementById('Desciptions');
    const fnameErrorMessage = document.getElementById('fname-error-message');
    const timeErrorMessage = document.getElementById('time-error-message');
    const priceErrorMessage = document.getElementById('price-error-message');
   /* const subjectErrorMessage = document.getElementById('subject-error-message');*/

    const specialChars = /[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]/; // Special character regex
    const specialChar = /[@#$%^&*{}\[\]~]/;
    const numbersOnly = /^[0-9]+$/; // Only numbers regex

    // Reset error messages
    fnameErrorMessage.textContent = "";
    priceErrorMessage.textContent = "";
    subjectErrorMessage.textContent = "";
    timeErrorMessage.textContent = "";

    let isValid = true;

    if (specialChars.test(name.value)) {
        fnameErrorMessage.textContent = "Không được chứa ký tự đặc biệt.";
        isValid = false;
    }

    if (!numbersOnly.test(price.value)) {
        priceErrorMessage.textContent = "Giá phải là số dương.";
        isValid = false;
    } else {
        const priceValue = parseInt(price.value);
        if (priceValue <= 0) {
            priceErrorMessage.textContent = "Giá phải là số dương.";
            isValid = false;
        }
    }

    if (!numbersOnly.test(time.value)) {
        timeErrorMessage.textContent = "Thời gian phải là số dương.";
        isValid = false;
    } else {
        const timeValue = parseInt(time.value);
        if (timeValue < 0 || timeValue > 6) {
            timeErrorMessage.textContent = "Thời gian phải từ 0 đến 6 tiếng.";
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