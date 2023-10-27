document.addEventListener('DOMContentLoaded', function () {
    const registerButton = document.getElementById('registerButton');

    registerButton.addEventListener('click', function (event) {
        // Kiểm tra tất cả các trường
        const emailInput = document.querySelector('input[name="Email"]');
        const passwordInput = document.querySelector('input[name="Password"]');
        const phoneInput = document.querySelector('input[name="Phone"]');

        // Kiểm tra email không được để trống
        if (!emailInput.value) {
            alert('Vui lòng nhập email.');
            event.preventDefault(); // Ngăn chặn việc đăng ký nếu có lỗi
            return;
        } else if (!emailInput.value.includes('@')) {
            alert('Email phải chứa ký tự @.');
            event.preventDefault();
            return;
        }

        if (!phoneInput.value) {
            alert('Vui lòng nhập SĐT!!');
            event.preventDefault();
        } else if (!/^0\d{9}$/.test(phoneInput.value)) {
            alert('SĐT cần bắt đầu bằng số 0 và có 10 số');
            event.preventDefault();
            return;
        }

        // Kiểm tra mật khẩu không được để trống
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
