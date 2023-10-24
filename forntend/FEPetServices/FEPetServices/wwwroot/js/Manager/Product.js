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
function returnProduct() {
    window.location.href = '/Manager/Product/Index';
}
function validateForm() {
    const fname = document.getElementById('ProductName');
    const subject = document.getElementById('Desciption');
    const fnameErrorMessage = document.getElementById('fnameErrorMessage');
    const subjectErrorMessage = document.getElementById('subjectErrorMessage');

    console.log('Debug: fname.value', fname.value);
    console.log('Debug: subject.value', subject.value);

    const specialChars = /[!#$%^&*()_+{}\[\]:;<>,.?~\\/-]/; // Regular expression kiểm tra ký tự đặc biệt

    // Đặt lại thông báo lỗi
    fnameErrorMessage.textContent = "";
    subjectErrorMessage.textContent = "";

    let isValid = true;

    if (specialChars.test(fname.value)) {
        fnameErrorMessage.textContent = "Không được chứa ký tự đặc biệt.";
        isValid = false;
    }

    if (specialChars.test(subject.value)) {
        subjectErrorMessage.textContent = "Không được chứa ký tự đặc biệt.";
        isValid = false;
    }

    return isValid;
}