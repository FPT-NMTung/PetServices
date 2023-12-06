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
});
