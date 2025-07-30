// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const input = document.getElementById('profileImageInput');
const preview = document.getElementById('previewImage');

if (input && preview) {
    input.addEventListener('change', function () {
        const file = this.files && this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                preview.src = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });
}