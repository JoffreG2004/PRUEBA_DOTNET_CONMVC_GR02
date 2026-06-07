function confirmarAccion(config) {
    var form = config.form || null;
    var url = config.url || null;
    var titulo = config.titulo || "Confirmar accion";
    var texto = config.texto || "Esta accion no se puede deshacer.";
    var confirmText = config.confirmText || "Confirmar";

    if (window.Swal) {
        Swal.fire({
            title: titulo,
            text: texto,
            icon: "question",
            showCancelButton: true,
            confirmButtonText: confirmText,
            cancelButtonText: "Cancelar"
        }).then(function (result) {
            if (result.isConfirmed) {
                if (form) {
                    form.submit();
                } else if (url) {
                    window.location.href = url;
                }
            }
        });
        return;
    }

    if (confirm(titulo + "\n\n" + texto)) {
        if (form) {
            form.submit();
        } else if (url) {
            window.location.href = url;
        }
    }
}

(function () {
    document.addEventListener("click", function (event) {
        var link = event.target.closest(".js-confirm-link");
        if (!link) {
            return;
        }

        event.preventDefault();
        confirmarAccion({
            url: link.href,
            titulo: link.dataset.confirmTitle,
            texto: link.dataset.confirmText,
            confirmText: link.dataset.confirmButton
        });
    });

    document.addEventListener("submit", function (event) {
        if (event.defaultPrevented) {
            return;
        }

        var form = event.target.closest(".js-confirm-form");
        if (!form) {
            return;
        }

        event.preventDefault();
        confirmarAccion({
            form: form,
            titulo: form.dataset.confirmTitle,
            texto: form.dataset.confirmText,
            confirmText: form.dataset.confirmButton
        });
    });
})();
