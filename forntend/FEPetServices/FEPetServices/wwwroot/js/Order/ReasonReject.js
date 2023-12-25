function validateForm() {
    const fname = document.getElementById('ReasonOrderTitle');
    const description = document.getElementById('ReasonOrderDescription');

    const fnameErrorMessage = document.getElementById('fname-error-message');
    const descriptionErrorMessage = document.getElementById('description-error-message');

    const specialChars = /[!@#%^*{}\[\]<>?-]/;
    const specialChar = /[@#$%^&*{}\[\]~]/;

    fnameErrorMessage.textContent = "";
    descriptionErrorMessage.textContent = "";

    let isValid = true;

    if (specialChars.test(fname.value)) {
        fnameErrorMessage.textContent = "Không được chứa ký tự đặc biệt.";
        isValid = false;
    }

    if (specialChar.test(description.value)) {
        descriptionErrorMessage.textContent = "Không được chứa ký tự đặc biệt.";
        isValid = false;
    }

    return isValid;
}