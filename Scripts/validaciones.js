document.addEventListener("DOMContentLoaded", function () {
    aplicarFiltrosDeEntrada();

    document.querySelectorAll("form.js-validate-client").forEach(function (formulario) {
        formulario.addEventListener("submit", validarFormularioCliente);
    });
});

function aplicarFiltrosDeEntrada() {
    document.querySelectorAll("[data-filter='letters']").forEach(function (input) {
        input.addEventListener("input", function () {
            this.value = this.value.replace(/[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]/g, "");
        });
    });

    document.querySelectorAll("[data-filter='numbers']").forEach(function (input) {
        input.addEventListener("input", function () {
            var max = parseInt(this.getAttribute("maxlength") || "20", 10);
            this.value = this.value.replace(/\D/g, "").slice(0, max);
        });
    });

    document.querySelectorAll("[data-filter='email']").forEach(function (input) {
        input.addEventListener("input", function () {
            this.value = this.value.replace(/\s/g, "").toLowerCase();
        });
    });
}

function validarFormularioCliente(event) {
    limpiarErrores();

    var form = event.target;
    var errores = [];

    if (!validarItemsSeleccionados(form, errores)) {
        mostrarErrores(errores);
        event.preventDefault();
        return;
    }

    var tipoDatos = form.querySelector("input[name='tipoCliente'][value='datos']");
    var validarDatosCliente = !tipoDatos || tipoDatos.checked;

    if (!validarDatosCliente) {
        return;
    }

    var cedula = obtenerValor(form, "cedula", "Cedula");
    var nombres = obtenerValor(form, "nombres", "Nombres");
    var apellidos = obtenerValor(form, "apellidos", "Apellidos");
    var telefono = obtenerValor(form, "telefono", "Telefono");
    var email = obtenerValor(form, "email", "Email");

    if (cedula && !validarCedulaEcuatoriana(cedula)) {
        errores.push("La cedula ecuatoriana no es valida.");
    }

    if (nombres && !validarSoloLetras(nombres)) {
        errores.push("Los nombres solo deben tener letras y espacios.");
    }

    if (apellidos && !validarSoloLetras(apellidos)) {
        errores.push("Los apellidos solo deben tener letras y espacios.");
    }

    if (telefono && !validarCelularEcuador(telefono)) {
        errores.push("El celular debe tener 10 digitos y empezar con 09.");
    }

    if (email && !validarEmail(email)) {
        errores.push("El correo debe tener formato usuario@dominio.com.");
    }

    if (errores.length > 0) {
        mostrarErrores(errores);
        event.preventDefault();
        return;
    }

    confirmarSiCorresponde(event, form);
}

function confirmarSiCorresponde(event, form) {
    if (!form.dataset.confirmTitle) {
        return;
    }

    event.preventDefault();

    if (window.confirmarAccion) {
        confirmarAccion({
            form: form,
            titulo: form.dataset.confirmTitle,
            texto: form.dataset.confirmText,
            confirmText: form.dataset.confirmButton
        });
        return;
    }

    if (confirm(form.dataset.confirmTitle + "\n\n" + (form.dataset.confirmText || ""))) {
        form.submit();
    }
}

function validarItemsSeleccionados(form, errores) {
    var cantidades = form.querySelectorAll(".qty-input");
    if (cantidades.length === 0) {
        return true;
    }

    var total = 0;
    cantidades.forEach(function (input) {
        total += parseInt(input.value || "0", 10);
    });

    if (total <= 0) {
        errores.push("Elige al menos un plato o bebida para confirmar el pedido.");
        return false;
    }

    return true;
}

function obtenerValor(form, nombreMinuscula, nombreModelo) {
    var input = form.querySelector("[name='" + nombreMinuscula + "'], [name='" + nombreModelo + "']");
    return input ? input.value.trim() : "";
}

function validarSoloLetras(texto) {
    return /^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]+$/.test(texto);
}

function validarEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(email);
}

function validarCelularEcuador(telefono) {
    return /^09\d{8}$/.test(telefono);
}

function validarCedulaEcuatoriana(cedula) {
    if (!/^\d{10}$/.test(cedula)) {
        return false;
    }

    if (cedula === "9999999999") {
        return true;
    }

    var provincia = parseInt(cedula.substring(0, 2), 10);
    var tercerDigito = parseInt(cedula.charAt(2), 10);

    if (provincia < 1 || provincia > 24 || tercerDigito > 5) {
        return false;
    }

    var suma = 0;
    for (var i = 0; i < 9; i++) {
        var digito = parseInt(cedula.charAt(i), 10);
        if (i % 2 === 0) {
            digito *= 2;
            if (digito > 9) {
                digito -= 9;
            }
        }
        suma += digito;
    }

    var verificador = (10 - (suma % 10)) % 10;
    return verificador === parseInt(cedula.charAt(9), 10);
}

function mostrarErrores(errores) {
    var divErrores = document.getElementById("errores-validacion");

    if (window.Swal) {
        Swal.fire({
            title: "Revisa los datos",
            html: errores.join("<br>"),
            icon: "warning"
        });
        if (divErrores) {
            divErrores.classList.remove("mostrar");
            divErrores.innerHTML = "";
        }
        return;
    }

    if (!divErrores) {
        alert(errores.join("\n"));
        return;
    }

    divErrores.innerHTML = "";

    var titulo = document.createElement("h3");
    titulo.textContent = "Por favor revisa lo siguiente:";
    divErrores.appendChild(titulo);

    var lista = document.createElement("ul");
    errores.forEach(function (error) {
        var item = document.createElement("li");
        item.textContent = error;
        lista.appendChild(item);
    });

    divErrores.appendChild(lista);
    divErrores.classList.add("mostrar");
    divErrores.scrollIntoView({ behavior: "smooth", block: "nearest" });
}

function limpiarErrores() {
    var divErrores = document.getElementById("errores-validacion");
    if (divErrores) {
        divErrores.classList.remove("mostrar");
        divErrores.innerHTML = "";
    }
}
