
    const inputs = document.querySelectorAll('input[type="number"]');

        inputs.forEach((input, index) => {
        input.addEventListener('input', (event) => {
            if (event.target.value.length === 1 && index < inputs.length - 1) {
                inputs[index + 1].removeAttribute('disabled');
                inputs[index + 1].focus();
            } else if (event.target.value.length === 0 && index < inputs.length - 1) {
                for (let i = index + 1; i < inputs.length; i++) {
                    inputs[i].setAttribute('disabled', 'disabled');
                    inputs[i].value = '';
                }
            }
        });
        });
