
document.addEventListener('DOMContentLoaded', function () {
    const formulario = document.querySelector('form');
    if (formulario) {
        formulario.addEventListener('submit', validarFormulario);
        const inputs = formulario.querySelectorAll('input');
        inputs.forEach(input => {
            input.addEventListener('input', limpiarErrores);
            input.addEventListener('change', limpiarErrores);
        });
    }
});

function validarFormulario(event) {
    limpiarErrores();

    const nombres = document.querySelector('input[name="nombres"]').value.trim();
    const telefono = document.querySelector('input[name="telefono"]').value.trim();
    const email = document.querySelector('input[name="email"]').value.trim();
    const entrada = document.querySelector('input[name="rbtnEntrada"]:checked');
    const sopa = document.querySelector('input[name="rbtnSopa"]:checked');
    const platoFuerte = document.querySelector('input[name="rbtnPlatoFuerte"]:checked');
    const postre = document.querySelector('input[name="rbtnPostre"]:checked');

    const errores = [];

    if (nombres === '') {
        errores.push('Por favor, cuéntanos tu nombre');
    } else if (!validarSoloLetras(nombres)) {
        errores.push('Tu nombre solo debe contener letras y espacios');
    }

    if (telefono === '') {
        errores.push('Necesitamos tu número de teléfono');
    } else if (!validarSoloNumeros(telefono)) {
        errores.push('El teléfono solo debe contener números');
    } else if (telefono.length < 7) {
        errores.push('El teléfono debe tener al menos 7 dígitos');
    }

    if (email === '') {
        errores.push('Por favor, ingresa tu correo electrónico');
    } else if (!validarEmail(email)) {
        errores.push('Parece que el correo no es válido (ejemplo: usuario@dominio.com)');
    }

    if (!entrada) {
        errores.push('Elige una entrada para tu pedido');
    }
    if (!sopa) {
        errores.push('Selecciona una sopa que te apetezca');
    }
    if (!platoFuerte) {
        errores.push('No olvides elegir un plato fuerte');
    }
    if (!postre) {
        errores.push('Termina con un delicioso postre');
    }

    if (errores.length > 0) {
        mostrarErrores(errores);
        event.preventDefault();
    }
}

function validarSoloLetras(texto) {
    const regex = /^[a-záéíóúñA-ZÁÉÍÓÚÑ\s]+$/;
    return regex.test(texto);
}

function validarSoloNumeros(texto) {
    const regex = /^[0-9]+$/;
    return regex.test(texto);
}

function validarEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

function mostrarErrores(errores) {
    const divErrores = document.getElementById('errores-validacion');

    if (!divErrores) {
        console.error('No se encontró el div de mensajes');
        return;
    }

    divErrores.innerHTML = '';

    const titulo = document.createElement('h3');
    titulo.textContent = 'Por favor revisa lo siguiente:';
    divErrores.appendChild(titulo);

    const lista = document.createElement('ul');

    errores.forEach(error => {
        const item = document.createElement('li');
        item.textContent = error;
        lista.appendChild(item);
    });

    divErrores.appendChild(lista);

    divErrores.classList.add('mostrar');

    divErrores.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
}

function limpiarErrores() {
    const divErrores = document.getElementById('errores-validacion');
    if (divErrores) {
        divErrores.classList.remove('mostrar');
        divErrores.innerHTML = '';
    }
}
