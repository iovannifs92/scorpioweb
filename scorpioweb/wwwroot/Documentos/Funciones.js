function candado(id, candado) {
    var personaidpersona = id;
    $.ajax({
        type: "POST",
        dataType: "json",
        url: "/Personas/LoockCandado",
        traditional: true,
        data: {
        datoCandado: [candado, personaidpersona],
        },
        success: function (response) {
            if (response.success) {
        location.reload();
            } else {
        alert(response.responseText);
            }
        },
        // if (response.success) {
        //     alert("Dentro del response");
        //     if (response.responseText[0] == 1) {
        //         //var x = getElementById("PersonaIdPersona");
        //         alert("Dentro de color primary");
        //         $("#on").removeClass("btn-danger");
        //         $("#on").addClass("btn-primary");
        //         $("#des").removeClass("fa-lock");
        //         $("#des").addClass("fa-unlock");
        //        } else {
        //         alert("Dentro de color danger");
        //         $("#on").removeClass("btn-primary");
        //          $("#on").addClass("btn-danger");
        //         $("#des").removeClass("fa-unlock");
        //         $("#des").addClass("fa-lock");
        //        }
        //    } else {
        //        alert(response.responseText);
        //    }
        //},
        error: function (response) {
        location.reload();
        }
    });

    function hola() {
        alert("en funcion ")
    };
