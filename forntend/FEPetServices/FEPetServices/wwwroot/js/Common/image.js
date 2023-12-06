function displayImage() {
    var input = document.getElementById('checkimage');
    var img = document.getElementById('previewImage');

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            img.src = e.target.result;
        };

        reader.readAsDataURL(input.files[0]);
    }

    if (input.files && input.files[0]) {
        var file = input.files[0];
        var fileType = file.type.toLowerCase(); // Lấy loại tệp tin

        // Kiểm tra nếu loại tệp tin không phải là PNG hoặc JPG
        if (!(fileType === 'image/png' || fileType === 'image/jpeg')) {
            alert('Vui lòng chọn tệp tin hình ảnh có định dạng PNG hoặc JPG.');
            input.value = ''; // Đặt lại giá trị của input để ngăn người dùng tải lên tệp không hợp lệ
            previewImage.src = ''; // Xóa hình ảnh xem trước
        }
    }
}

displayImage()