$(document).ready(function () {
    $("#showPassword").click(function () {
        togglePasswordVisibility("NewPassword", "eyeIcon");
    });

    $("#showConfirmPassword").click(function () {
        togglePasswordVisibility("ConfirmNewPassword", "eyeConfirmIcon");
    });

    function togglePasswordVisibility(passwordFieldId, eyeIconId) {
        var passwordField = $("#" + passwordFieldId);
        var eyeIcon = $("#" + eyeIconId);

        if (passwordField.attr("type") === "password") {
            passwordField.attr("type", "text");
            eyeIcon.removeClass("far fa-eye");
            eyeIcon.addClass("far fa-eye-slash");
        } else {
            passwordField.attr("type", "password");
            eyeIcon.removeClass("far fa-eye-slash");
            eyeIcon.addClass("far fa-eye");
        }
    }

    $("#ConfirmNewPassword").on('input', function () {
        var newPassword = $("#NewPassword").val();
        var confirmPassword = $(this).val();
        var passwordMismatchMessage = $("#passwordMismatchMessage");

        if (newPassword !== confirmPassword) {
            passwordMismatchMessage.text("Mật khẩu không khớp.");
            passwordMismatchMessage.show();
        } else {
            passwordMismatchMessage.text("");
            passwordMismatchMessage.hide();
        }
    });
});