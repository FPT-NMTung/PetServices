function validateForm() {
    const fname = document.getElementById('RoomCategoriesName');
    const subject = document.getElementById('Desciptions');
    const fnameErrorMessage = document.getElementById('fname-error-message');
    const subjectErrorMessage = document.getElementById('subject-error-message');

    console.log('Debug: fname.value', fname.value);
    console.log('Debug: subject.value', subject.value);

    const specialChars = /[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]/;
    const specialChar = /[@#$%^&*{}\[\]~]/;

    fnameErrorMessage.textContent = "";
    subjectErrorMessage.textContent = "";

    let isValid = true;

    if (subject === null) {
        subjectErrorMessage.textContent = 'Mô tả không được để trống!';
        isValid = false;
    }

    if (subject === null) {
        fnameErrorMessage.textContent = 'Tên loại phòng không được để trống!';
        isValid = false;
    }

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
