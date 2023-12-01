function validateForm() {
    const fname = document.getElementById('SerCategoriesName');
    /*const subject = document.getElementById('Desciptions');*/
    const fnameErrorMessage = document.getElementById('fname-error-message');
    const subjectErrorMessage = document.getElementById('subject-error-message');

    console.log('Debug: fname.value', fname.value);
    console.log('Debug: subject.value', subject.value);

    const specialChars = /[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]/; // Special character regex
    const specialChar = /[@#$%^&*{}\[\]~]/;
    // Reset error messages
    fnameErrorMessage.textContent = "";
    subjectErrorMessage.textContent = "";

    let isValid = true;

    if (specialChars.test(fname.value)) {
        fnameErrorMessage.textContent = "Không được chứa ký tự đặc biệt.";
        isValid = false;
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