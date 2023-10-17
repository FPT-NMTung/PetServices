function handleImageFileChange(event) {
    const fileInput = event.target;
    const imagePreview = fileInput.previousElementSibling;
    const file = fileInput.files[0];

    // Check if a file is selected
    if (file) {
        // Create a FileReader to read the selected image file
        const reader = new FileReader();

        reader.onload = function (e) {
            // Set the image source to the data URL of the selected image
            imagePreview.src = e.target.result;
        };

        // Read the selected image file as a data URL
        reader.readAsDataURL(file);
    }
}

// Add an event listener to the file input
document.addEventListener('DOMContentLoaded', function () {
    const fileInput = document.getElementById('imageFile');
    fileInput.addEventListener('change', handleImageFileChange);
});