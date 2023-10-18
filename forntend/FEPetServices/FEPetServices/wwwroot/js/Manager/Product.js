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
