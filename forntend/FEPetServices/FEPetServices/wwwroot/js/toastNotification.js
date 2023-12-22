function toast({
    title = '',
    message = '',
    type = '',
    duration = '' }) {
    const main = document.getElementById('toast');
    if (main) {
        const toast = document.createElement('div');
        const icons = {
            success: 'fa-solid fa-circle-check',
            error: 'fa-solid fa-circle-xmark',
            wating: 'fa-solid fa-circle-info',
        }

        const newDuration = duration + 3000;
        const icon = icons[type];
        const autoRemoveId = setTimeout(function () {
            main.removeChild(toast);
        }, newDuration + 380);

        toast.onclick = function (e) {
            if (e.target.closest('.toast_close')) {
                main.removeChild(toast);
                clearTimeout(autoRemoveId);
            }
        }

        toast.style.setProperty('--animation-duration', `${newDuration}ms`);
        toast.classList.add('toast', `toast--${type}`);
        toast.style.animation = `slideInLeft ease .8s forwards, toastHide ease .3s ${newDuration}ms forwards`;
        toast.innerHTML = `
                                 <div class="toast_icon">
                                    <i class="${icon}"></i>
                                </div>
                                <div class="toast_body">
                                     <h3 class="toast_title">${title}</h3>
                                     <p class="toast_msg">${message}</p>
                                </div>
                                <div class="toast_close">
                                    <i class="fa-solid fa-xmark"></i>
                                </div>
                            `;
        main.appendChild(toast);
    }
}