// Load provinces and set up change event listeners
fetch("https://provinces.open-api.vn/api/p/")
    .then(response => response.json())
    .then(data => {
        var provinces = data;
        var provinceDropdown = document.getElementById("provinceDropdown");

        // Add province options
        for (var i = 0; i < provinces.length; i++) {
            var option = document.createElement("option");
            option.value = provinces[i].code;
            option.text = provinces[i].name;
            provinceDropdown.appendChild(option);
        }

        // Trigger the change event on province dropdown to load districts
        provinceDropdown.dispatchEvent(new Event("change"));
    })
    .catch(error => {
        console.error("Error:", error);
    });

function loadDistricts() {
    var provinceDropdown = document.getElementById("provinceDropdown");
    var selectedProvinceOption = provinceDropdown.options[provinceDropdown.selectedIndex];
    selectedProvinceName = selectedProvinceOption.text;

    fetch(`https://provinces.open-api.vn/api/p/${selectedProvinceOption.value}?depth=2`)
        .then(response => response.json())
        .then(data => {
            var districts = data.districts;
            var districtDropdown = document.getElementById("districtDropdown");
            var wardDropdown = document.getElementById("wardDropdown");

            // Clear existing options in district and ward dropdowns
          

            // Add new district options
            for (var i = 0; i < districts.length; i++) {
                var option = document.createElement("option");
                option.value = districts[i].code;
                option.text = districts[i].name;
                districtDropdown.appendChild(option);
            }

            // Update displayed names after loading districts
            updateDisplayedNames();

            // Trigger the change event on district dropdown to load wards
            districtDropdown.dispatchEvent(new Event("change"));
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

function loadWards() {
    var districtDropdown = document.getElementById("districtDropdown");
    var selectedDistrictOption = districtDropdown.options[districtDropdown.selectedIndex];
    selectedDistrictName = selectedDistrictOption.text;

    if (selectedDistrictOption.value === "") {
        // Get the first district option
        var firstDistrictOption = districtDropdown.querySelector("option:not([value=''])");
        selectedDistrictName = firstDistrictOption.text;
        firstDistrictOption.selected = true;
    }

    fetch(`https://provinces.open-api.vn/api/d/${selectedDistrictOption.value}?depth=2`)
        .then(response => response.json())
        .then(data => {
            var wards = data.wards;
            var wardDropdown = document.getElementById("wardDropdown");

            // Clear existing options in ward dropdown
            wardDropdown.innerHTML = "";

            // Add new ward options
            for (var i = 0; i < wards.length; i++) {
                var option = document.createElement("option");
                option.value = wards[i].code;
                option.text = wards[i].name;
                wardDropdown.appendChild(option);
            }

            // Update displayed names after loading wards
            updateDisplayedNames();
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

function updateDisplayedNames() {
    var provinceDropdown = document.getElementById("provinceDropdown");
    var districtDropdown = document.getElementById("districtDropdown");
    var wardDropdown = document.getElementById("wardDropdown");

    var selectedProvinceOption = provinceDropdown.options[provinceDropdown.selectedIndex];
    var selectedDistrictOption = districtDropdown.options[districtDropdown.selectedIndex];
    var selectedWardOption = wardDropdown.options[wardDropdown.selectedIndex];

    var provinceNameInput = document.getElementById("provinceNameInput");
    var districtNameInput = document.getElementById("districtNameInput");
    var wardsNameInput = document.getElementById("wardsNameInput");

    provinceNameInput.value = selectedProvinceOption.text;
    districtNameInput.value = selectedDistrictOption.text;
    wardsNameInput.value = selectedWardOption.text;
}

var provinceDropdown = document.getElementById("provinceDropdown");
provinceDropdown.addEventListener("change", loadDistricts);

var districtDropdown = document.getElementById("districtDropdown");
districtDropdown.addEventListener("change", loadWards);

var wardDropdown = document.getElementById("wardDropdown");
wardDropdown.addEventListener("change", updateDisplayedNames);

function validateForm() {
    var cashChecked = document.getElementById("radio4").checked;
    var momoChecked = document.getElementById("radio5").checked;

    // Chỉ cho phép submit nếu chỉ một trong hai radio được chọn
    if (!(cashChecked ^ momoChecked)) {
        alert("Chỉ chọn một phương thức thanh toán.");
        return false; // Ngăn chặn submit form
    }

    // Nếu muốn xử lý thêm các trường hợp khác, bạn có thể thực hiện ở đây

    return true; // Cho phép submit form
}