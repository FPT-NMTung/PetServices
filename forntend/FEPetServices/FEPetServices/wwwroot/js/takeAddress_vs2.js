function checkPhoneNumberFormat() {
    var phoneNumberInput = document.getElementById("phoneNumber");
    var phoneNumber = phoneNumberInput.value;
    var phoneErrorSpan = document.getElementById("phoneErrorSpan");

    var phoneNumberRegex = /^0\d{9}$/;
    if (!phoneNumberRegex.test(phoneNumber) || phoneNumber.length !== 10) {
        phoneErrorSpan.innerText = "Số điện thoại không hợp lệ. Vui lòng nhập số bắt đầu từ 0 và có 10 chữ số!";
    } else {
        phoneErrorSpan.innerText = "";
    }
}

function loadDistricts() {
    var provinceDropdown = document.getElementById("provinceDropdown");
    var selectedProvince = provinceDropdown.value;
    var districtDropdown = document.getElementById("districtDropdown");
    var selectedDistrict = districtDropdown.value;

    fetch(`https://provinces.open-api.vn/api/p/${selectedProvince}?depth=2`)
        .then(response => response.json())
        .then(data => {
            var districts = data.districts;
            var districtDropdown = document.getElementById("districtDropdown");
            var wardDropdown = document.getElementById("wardDropdown");

            // Clear existing options in district and ward dropdowns
            districtDropdown.innerHTML = "";
            wardDropdown.innerHTML = "";

            // Add new district options
            for (var i = 0; i < districts.length; i++) {
                var option = document.createElement("option");
                option.value = districts[i].code;
                option.text = districts[i].name;
                districtDropdown.appendChild(option);
            }

            // Trigger the change event on district dropdown to load wards
            districtDropdown.dispatchEvent(new Event("change"));
        })
        .catch(error => {
            console.error("Error:", error);
        });

    fetch(`https://provinces.open-api.vn/api/d/${selectedDistrict}?depth=2`)
        .then(response => response.json())
        .then(data => {
            var wards = data.wards;

            // Clear existing options in ward dropdown
            wardDropdown.innerHTML = "";

            // Add new ward options
            for (var i = 0; i < wards.length; i++) {
                var option = document.createElement("option");
                option.value = wards[i].code;
                option.text = wards[i].name;
                wardDropdown.appendChild(option);
            }
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

function loadWards() {
    var districtDropdown = document.getElementById("districtDropdown");
    var selectedDistrict = districtDropdown.value;
    var wardDropdown = document.getElementById("wardDropdown");

    if (selectedDistrict === "") {
        // Get the first district option
        var firstDistrictOption = districtDropdown.querySelector("option:not([value=''])");
        selectedDistrict = firstDistrictOption.value;
        firstDistrictOption.selected = true;
    }

    fetch(`https://provinces.open-api.vn/api/d/${selectedDistrict}?depth=2`)
        .then(response => response.json())
        .then(data => {
            var wards = data.wards;

            // Clear existing options in ward dropdown
            wardDropdown.innerHTML = "";

            // Add new ward options
            for (var i = 0; i < wards.length; i++) {
                var option = document.createElement("option");
                option.value = wards[i].code;
                option.text = wards[i].name;
                wardDropdown.appendChild(option);
            }
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

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

var provinceDropdown = document.getElementById("provinceDropdown");
provinceDropdown.addEventListener("change", loadDistricts);

var districtDropdown = document.getElementById("districtDropdown");
districtDropdown.addEventListener("change", loadWards);

var wardDropdown = document.getElementById("wardDropdown");
wardDropdown.addEventListener("change", updateDisplayedNames);
