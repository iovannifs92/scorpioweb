
$(document).ready(function () {

    var divBotonPaso1 = document.getElementById('divBotonPaso1');
    var idReinsercion = document.getElementById('IdReinsercion').value;
    var tipoCanalizacion = document.getElementById('selectTipoCanalizacion');
    var lugar = "DGEPMS";//PUEDE CAMBIAR HASTA FUTURAS INSTRUCCIONES
    var estado = "ACTIVO";//PUEDE CAMBIAR HASTA FUTURAS INSTRUCCIONES

    $(document).on('click', '.step', function () {
        var porcentajeBarra = $(this).data('value');
        var NombreClaseSiguientePunto = $(this).data('value2');
        var numeroPaso = $(this).data('value3');
        botonSiguiente(numeroPaso, NombreClaseSiguientePunto, porcentajeBarra);
    });



    //primera tarjeta de ficha de canalizacion
    $("#selectTipoCanalizacion").change(function () {
        divBotonPaso1.classList.add('visible');
        if (tipoCanalizacion.value === "TERAPIA")
            generarPasosTerapia();
        else if (tipoCanalizacion.value === "EJESREINSERCION")
            generarPasosEjesReinsercion();
    });


    //variables para ejes de reinsercion

    var checkboxesEjes = document.querySelectorAll('.divCheckBoxesEjes input[type="checkbox"]');
    var otroEjeCheckbox = document.getElementById('CheckboxOtroEje');

    var divInputOtroEje = document.getElementById('divInputOtroEje');
    var inputOtroEje = document.getElementById('InputOtroEje');

    var divInputObsevarciones = document.getElementById('divInputObsevarciones');
    var inputObservacionesEjes = document.getElementById('InputObservacionesEjes');

    checkboxesEjes.forEach(function (checkbox) {
        checkbox.addEventListener('change', function () {
            var algunCheckboxSeleccionado = Array.from(checkboxesEjes).some(c => c.checked);
            if (algunCheckboxSeleccionado)
                divInputObsevarciones.classList.add('visible');
            else
                divInputObsevarciones.classList.remove('visible');
        });
    });

    document.getElementById('CheckboxOtroEje').addEventListener('change', function () {
        if (this.checked)
            divInputOtroEje.classList.add('visible');
        else {
            inputOtroEje.value = "";
            divInputOtroEje.classList.remove('visible');
        }
    });

    $("#btnFinalizarEjes").on('click', function () {
        var todoVerificado = VerificarCamposEjesReinsercion();
        if (todoVerificado)
            SweetAlertFinalizarFicha();
        else
            SweetAlert("Error", "Selecciona un tipo de servico o especifica el servicio", "error");
    });

    function VerificarCamposEjesReinsercion() {
        var todosLosCamposRequeridos = true;

        checkboxesEjes.forEach(function (checkbox) {
            var algunCheckboxSeleccionado = Array.from(checkboxesEjes).some(c => c.checked);
            if (!algunCheckboxSeleccionado)
                todosLosCamposRequeridos = false;
        });
        if (todosLosCamposRequeridos) {
            if (otroEjeCheckbox.checked && inputOtroEje.value === "")
                todosLosCamposRequeridos = false;
        }
        else
            todosLosCamposRequeridos = false;

        return todosLosCamposRequeridos;
    }


    //variables para la segunda tarjeta de terapia
    var botonSiguienteTerapia = document.getElementById('btnSiguienteTerapia');
    var checkboxesTerapia = document.querySelectorAll('.divCheckBoxesTerapia input[type="checkbox"]');

    var divInputOtroTipoTerapia = document.getElementById('divInputOtroTipoTerapia');
    var otraTerapiaCheckbox = document.getElementById('CheckboxOtraTerapia');
    var inputOtroTipoTerapia = document.getElementById('InputOtroTipoTerapia');


    //variables para la tercera tarjeta de terapia
    var tiempoTerapia = document.getElementById('tiempoTerapia');
    var selectPeriodicidadTerapia = document.getElementById('selectPeriodicidadTerapia');
    var fInicioTerapia = document.getElementById('fInicioTerapia');
    var fTerminoTerapia = document.getElementById('fTerminoTerapia');
    var selectGrupo = document.getElementById('selectGrupo');
    var selectTerapeuta = document.getElementById('selectTerapeuta');
    var inputObservacionesTerapia = document.getElementById('InputObservacionesTerapia')

    //segunda tarjeta de terapia
    checkboxesTerapia.forEach(function (checkbox) {
        checkbox.addEventListener('change', function () {
            var algunCheckboxSeleccionado = Array.from(checkboxesTerapia).some(c => c.checked);
            if (algunCheckboxSeleccionado) {
                if (otraTerapiaCheckbox.checked && inputOtroTipoTerapia.value === "")
                    botonSiguienteTerapia.classList.remove('visible');
                else
                    botonSiguienteTerapia.classList.add('visible');
            }
            else
                botonSiguienteTerapia.classList.remove('visible');
        });
    });

    document.getElementById('CheckboxOtraTerapia').addEventListener('change', function () {
        if (this.checked)
            divInputOtroTipoTerapia.classList.add('visible');
        else {
            inputOtroTipoTerapia.value = "";
            divInputOtroTipoTerapia.classList.remove('visible');
        }
    });

    $('#InputOtroTipoTerapia').on('change', function () {
        if (otraTerapiaCheckbox.checked) {
            if (inputOtroTipoTerapia.value === "")
                botonSiguienteTerapia.classList.remove('visible');
            else
                botonSiguienteTerapia.classList.add('visible');
        }
    });

    //verificar datos tercera tarjeta de terapia
    function verificarCampos(campos) {
        var todosLosCamposValidos = true;
        var camposVacios = [];

        campos.forEach(function (item) {
            if (item.campo === null) {
                camposVacios.push(item.nombre);
                todosLosCamposValidos = false;
            }
            else if (item.campo.value.trim() === '' || item.campo.value === 'Seleccione una opción' || item.campo.value === null) {
                camposVacios.push(item.nombre);
                todosLosCamposValidos = false;
            }
        });
        if (todosLosCamposValidos)
            SweetAlertFinalizarFicha();
        else
            SweetAlert("Error", "Estos valores necesitan ser llenados: " + camposVacios.join(', '), "error")
        return todosLosCamposValidos;
    }

    $("#btnFinalizarTerapia").on('click', function () {
        var campos = [
            { campo: tiempoTerapia, nombre: 'Tiempo total de la terapia' },
            { campo: selectPeriodicidadTerapia, nombre: 'Periodicidad de la Terapia' },
            { campo: fInicioTerapia, nombre: 'Fecha de inicio' },
            { campo: fTerminoTerapia, nombre: 'Fecha de término' },
            { campo: selectGrupo, nombre: 'Grupo' },
            { campo: selectTerapeuta, nombre: 'Terapeuta' }
        ];
        verificarCampos(campos);
    });

    function generarPasosTerapia() {

        $("#divBotonPaso1").html(`
                     <button  class="step botonSiguiente fa fa-arrow-right" data-value3=".step04" data-value2=".TerapiaPaso2" data-value="50%" style="margin-right: 20px;">  Siguiente</button>
                `);
        $("#ProgresionPasos").html(`
                     <li class="step01 active">
                         <div class="step-inner">Terapia</div>
                     </li>
                     <li  class="step02" style="list-style: none; visibility: hidden;"></li>
                     <li class="step03">
                         <div class="step-inner">Tipo de terapia</div>
                     </li>
                     <li class="step04" style="list-style: none; visibility: hidden;">
                     </li>
                     <li class="step05">
                         <div class="step-inner">Datos de terapia</div>
                     </li>
                `);
        mostrarPasosAnimacion(250);
    }

    function generarPasosEjesReinsercion() {
        $("#divBotonPaso1").html(`
                   <button  class="step botonSiguiente fa fa-arrow-right" data-value3=".step05" data-value2=".EjesPaso2" data-value="100%" style="margin-right: 20px;">  Siguiente</button>
                `);
        $("#ProgresionPasos").html(`
                    <li class="step01 active">
                        <div class="step-inner">Ejes de reinsercion</div>
                    </li>
                    <li  class="step02" style="list-style: none; visibility: hidden;"></li>
                    <li  class="step03" style="list-style: none; visibility: hidden;"></li>
                    <li  class="step04" style="list-style: none; visibility: hidden;"></li>
                    <li class="step05">
                       <div class="step-inner">Servicios de vinculacion</div>
                    </li>
                `);
        mostrarPasosAnimacion(120);
    }

    function mostrarPasosAnimacion(delayEnviado) {
        var delay = 0;
        $("#ProgresionPasos li").each(function () {
            $(this).delay(delay).fadeIn(delayEnviado);
            delay += delayEnviado;
        });
    }

    function botonSiguiente(numeroPaso, NombreClaseSiguientePunto, porcentajeBarra) {
        $(numeroPaso).addClass("active").prevAll().addClass("active");
        $(numeroPaso).nextAll().removeClass("active");

        $("#line-progress").css("width", porcentajeBarra);
        $(NombreClaseSiguientePunto).addClass("active").siblings().removeClass("active");
    }


    function SweetAlert(Titulo, Mensaje, Icono) {
        Swal.fire({
            title: Titulo,
            text: Mensaje,
            icon: Icono
        });
    }

    function SweetAlertFinalizarFicha() {
        Swal.fire({
            icon: "question",
            title: "¿Deseas guardar la ficha de canalizacion?",
            showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: "Guardar",
            denyButtonText: `Cancelar`
        }).then((result) => {

            if (result.isConfirmed)
                CrearFichaDeCanalizacion(tipoCanalizacion);
            else if (result.isDenied)
                Swal.fire("Cancelaste el guardado de la ficha", "", "info");

        });
    }

    function CrearFichaDeCanalizacion() {
        //variable que se llena para el modelo DatosFichaCanalizacion

        var datosFichaCanalizacion = { TipoCanalizacion: tipoCanalizacion.value, IdReinsercion: idReinsercion, datos: null };

        if (tipoCanalizacion.value === "TERAPIA") {
            var tiposTerapiaSeleccionados = Array.from(checkboxesTerapia)
                .filter(checkbox => checkbox.checked)
                .map(checkbox => checkbox.value);

            datosFichaCanalizacion.datos = {
                TiposTerapiaSeleccionados: tiposTerapiaSeleccionados,
                EspecificarOtraTerapia: inputOtroTipoTerapia.value,
                Terapeuta: selectTerapeuta.value,
                FechaCanalizacion: new Date(),
                TiempoTerapia: tiempoTerapia.value,
                FechaInicio: fInicioTerapia.value,
                FechaTermino: fTerminoTerapia.value,
                FechaTerapia: fInicioTerapia.value, //es la fecha en la que se presenta a las terapias al principio es igual que la fecha de inicio
                PeriodicidadTerapia: selectPeriodicidadTerapia.value,
                Estado: estado,
                GrupoId: selectGrupo.value,
                Observaciones: inputObservacionesTerapia.value

            };
        }
        else if (tipoCanalizacion.value === "EJESREINSERCION") {
            var ejesSeleccionados = Array.from(checkboxesEjes)
                .filter(checkbox => checkbox.checked)
                .map(checkbox => checkbox.value);

            datosFichaCanalizacion.datos = {
                EjesSeleccionados: ejesSeleccionados,
                EspecificarOtroEje: inputOtroEje.value,
                Observaciones: inputObservacionesEjes.value,
                Lugar: lugar,
                Estado: estado,
                FechaCanalizacion: new Date()
            };
        }

        $.ajax({
            type: "POST",
            dataType: "json",
            async: true,
            url: "/Reinsercion/CrearFichaCanalizacion",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(datosFichaCanalizacion),
            success: function (response) {
                //PENDIENTE: POSIBLEMENTE DEBE DE REDIRIGIR A OTRA VISTA 
                Swal.fire("Éxito", response.responseText, "success");

                setTimeout(function () {
                    window.location.href = response.viewUrl;
                }, 1700);
            },
            error: function (xhr, status, error) {
                console.error('Error al enviar datos al controlador:', status, error);
                Swal.fire("Error", "Hubo un problema al enviar los datos, comuniquese con el administrador del sistema", "error");
            }
        });
    }
});