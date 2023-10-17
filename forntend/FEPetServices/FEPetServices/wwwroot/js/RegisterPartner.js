document.querySelector('#showTerms').addEventListener('click', function (event) {
    // Ngăn trình duyệt thực hiện hành động mặc định của thẻ <a>
    event.preventDefault();

    // Hiện form-box2 khi nút "Điều khoản" được nhấn
    document.querySelector('.form-box2').style.display = 'block';
});

document.addEventListener('DOMContentLoaded', function () {
    const registerButton = document.getElementById('registerButton');
    const agreeCheckbox = document.getElementById('agreeCheckbox');
    const checkImage = document.getElementById('checkimage');
    

    registerButton.addEventListener('click', function (event) {
        const passwordInput = document.querySelector('input[name="Password"]');
        if (!agreeCheckbox.checked) {
            event.preventDefault(); // Ngăn chặn việc thực hiện hành động mặc định của nút "Đăng ký"
            alert('Bạn cần đồng ý với các điều khoản trước khi đăng ký.');
        }
        else if (checkImage.files.length === 0) {
            event.preventDefault();
            alert('Vui lòng chọn hình ảnh chứng chỉ trước khi đăng ký.');
        }

        const passwordPattern = /^(?=.*[A-Za-z0-9]{8,})$/;
        if (!passwordInput.value) {
            alert('Vui lòng xác nhận mật khẩu!!');
            event.preventDefault(); // Ngăn chặn việc đăng ký nếu có lỗi
            return;
        } else if (passwordInput.value.length < 8 || !/^[A-Za-z0-9]+$/.test(passwordInput.value)) {
            alert('Mật khẩu của bạn phải tối thiểu 8 ký tự và không được chứa ký tự đặc biệt.');
            event.preventDefault();
            return;
        }
    });
});
