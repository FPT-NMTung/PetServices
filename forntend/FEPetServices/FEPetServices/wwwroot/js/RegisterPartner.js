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

function toggleContent() {
    var contentToShow = document.getElementById("contentToShow");

    if (contentToShow.style.display === "none" || contentToShow.style.display === "") {
        contentToShow.style.display = "block";
    } else {
        contentToShow.style.display = "none";
    }
}

toggleContent()

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
}
displayImage()
