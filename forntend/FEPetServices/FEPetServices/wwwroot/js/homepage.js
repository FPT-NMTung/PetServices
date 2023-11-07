<script>
    function logClickEvent() {
        // Ghi log bằng JavaScript
        console.log("Button was clicked.");

        // Gửi log thông qua AJAX hoặc gửi lên máy chủ nếu cần
        // Ví dụ: sử dụng AJAX để gửi log lên máy chủ
        // $.ajax({
        //     url: '/Log/LogClickEvent',
        //     type: 'POST',
        //     data: { message: 'Button was clicked.' },
        //     success: function (response) {
        //         // Xử lý kết quả từ máy chủ (nếu cần)
        //     },
        //     error: function (error) {
        //         console.error("Error while logging the event.");
        //     }
        // });
    }
</script>