/* ========================================================================= */
/*	Codigo extra
/* ========================================================================= */
function confirmacionQR() {
    if (confirm('¿Estas seguro de Guardar los datos?')) {
        document.QRForm.submit();
    }
}

function probando() {
    alert('si funciona');
}

$(function () {
    $('#btnGuardar').click(function (e) {
        e.preventDefault();
    });
});

$("#confirmDialog").dialog({
    autoOpen: false,
    modal: true,
    resizable: false,
    buttons: {
        "Ok": function () {
            $('QRForm').submit();
        },
        "Cancel": function (e) {
            $(this).dialog("close");
        }
    },
});

var marker = null;//si es variable local, sale primero la infowindow vacia
var result;
var map;
var infowindow;
var cnt = 0;//todo

function iniciarMap() {
    var coord = { lat: 24.0234962, lng: -104.6606269 };//DGEP
    map = new google.maps.Map(document.getElementById('map'), {
        zoom: 12,//12: Town, or city district
        center: coord
    });
    infowindow = new google.maps.InfoWindow();
    if (document.getElementById("lat").value != "" && document.getElementById("lng").value != "") {
        var savedCoord = { lat: parseFloat(document.getElementById("lat").value), lng: parseFloat(document.getElementById("lng").value) };
        geocodeLatLng(new google.maps.Geocoder(), new google.maps.LatLng(savedCoord), infowindow, 20);
        marker = new google.maps.Marker({
            position: savedCoord,
            map: map
        });
    }
    else {
        geocodeLatLng(new google.maps.Geocoder(), new google.maps.LatLng(coord), infowindow, 12);
        marker = new google.maps.Marker({
            position: coord,
            map: map
        });
    }

    google.maps.event.addListener(map, "click", (event) => {
        if (marker == null) {
            marker = new google.maps.Marker({
                position: event.latLng,
                map: map
            });
        }
        else {
            marker.setPosition(new google.maps.LatLng(event.latLng));
        }
        geocodeLatLng(new google.maps.Geocoder(), event.latLng, infowindow);
    });
}

//https://stackoverflow.com/questions/6478914/reverse-geocoding-code
function getGeocodingData(calle, no, nombre, cp, municipio, estado) {
    // This is making the Geocode request
    var geocoder = new google.maps.Geocoder();
    var address;
    if (no == "") {
        address = calle;
    }
    else {
        address = calle + " " + no;
    }
    address += ", " + nombre;
    if (cp != "") {
        address += ", " + cp;
    }
    if (municipio != "Sin municipio") {
        address += ", " + municipio;
    }
    if (estado != "Selecciona" && estado != "Sin estado") {
        address += ", " + estado;
    }

    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status !== google.maps.GeocoderStatus.OK) {
            alert("No hay direcciones");
        }
        // This is checking to see if the Geocode Status is OK before proceeding
        if (status == google.maps.GeocoderStatus.OK) {
            var coord = { lat: results[0].geometry.location.lat(), lng: results[0].geometry.location.lng() };
            if (marker == null) {
                marker = new google.maps.Marker({
                    position: coord,
                    map: map
                });
            }
            else {
                marker.setPosition(new google.maps.LatLng(coord));
                map.setCenter(coord);
            }
            geocodeLatLng(geocoder, results[0].geometry.location, infowindow, 20);
        }
    });
}

function geocodeLatLng(geocoder, latlng, infowindow, zoom) {
    geocoder
        .geocode({ location: latlng })
        .then((response) => {
            if (response.results[0]) {
                if (zoom != undefined) {
                    map.setZoom(zoom);
                }

                document.getElementById("lat").value = response.results[0].geometry.location.lat();
                document.getElementById("lng").value = response.results[0].geometry.location.lng();
                result = response.results[0];
                infowindow.setContent(response.results[0].formatted_address + ' <button href="/" onclick="event.preventDefault();fillInAddress(result)">Usar dirección</button>');
                infowindow.open(map, marker);
            } else {
                window.alert("No results found");
            }
        })
        .catch((e) => window.alert("Geocoder failed due to: " + e));
}

function fillInAddress(place) {
    var municipio = getMunicipio(place);
    var e = document.getElementById("estadoD");
    var estadoAnterior = e.value;
    var estado = getEstado(place);
    e.value = 0;
    for (let i = 0; i < e.length; i++) {
        if (e.options[i].text == estado) {
            e.value = e.options[i].value;
            break;
        }
    }
    var m = document.getElementById("municipioD");
    var municipioAnterior = m.options[m.selectedIndex].text;
    m.text = municipio;
    $("#estadoD").change();
    var esMunicipio = false;
    for (i = 0; i < m.length; i++) {
        if (m.options[i].text == municipio) {
            m.value = m.options[i].value;
            esMunicipio = true;
            break;
        }
    }
    var colonia = getColonia(place);
    if (esMunicipio == false || municipio == "Sin municipio" || colonia != "Sin colonia" || document.getElementById("inputAutocomplete").value == "") {
        document.getElementById("no").value = "";
        document.getElementById("calle").value = "";
        document.getElementById("cp").value = "";
        localStorage.setItem('colonia', '');

        var municipio = getMunicipio(place);
        for (const component of place.address_components) {
            const componentType = component.types[0];

            switch (componentType) {
                case "neighborhood": {
                    setColonia(component.long_name);
                    break;
                }
                case "political": {
                    setColonia(component.long_name);
                    break;
                }
                case "street_number": {
                    document.getElementById("no").value = component.long_name;
                    break;
                }
                case "route": {
                    document.getElementById("calle").value = component.long_name;
                    break;
                }
                case "postal_code": {
                    document.getElementById("cp").value = component.long_name;
                    break;
                }
                default: {
                    break;
                }
            }
        }
        if (esMunicipio == false && municipio != "Sin municipio") {
            if (localStorage.getItem('colonia') != "") {
                localStorage.setItem('colonia', localStorage.getItem('colonia') + ", " + municipio);
            }
            else {
                localStorage.setItem('colonia', municipio);
            }
        }
        $('#combobox').change();
        //https://stackoverflow.com/questions/29534194/select-drop-down-on-change-reload-reverts-to-first-option
        if (localStorage.getItem('municipioD')) {
            $('#municipioD').val(localStorage.getItem('municipioD'));
        }
    }
    else {
        e.value = estadoAnterior;
        m.text = municipioAnterior;
        $("#estadoD").change();
    }
}

function setColonia(colonia) {
    var cb = document.getElementById("combobox");
    localStorage.setItem('colonia', colonia);
    colonia = colonia.toUpperCase();
    for (var i = 0; i < cb.options.length; i++) {
        var coloniaCP = cb.options[i].text;
        var index = coloniaCP.lastIndexOf(",");
        if (coloniaCP.substr(0, index).toUpperCase() == colonia) {
            cb.selectedIndex = i;
            localStorage.setItem('colonia', coloniaCP.substr(0, index));
        }
    }
}

function getColonia(place) {
    var colonia = "Sin colonia";
    for (const component of place.address_components) {
        const componentType = component.types[0];
        switch (componentType) {
            case "neighborhood": {
                colonia = component.long_name;
                break;

            }
            case "political": {// political overwrites neighborhood
                colonia = component.long_name;
                break;
            }
            default: {
                break;
            }
        }
    }
    return colonia;
}

function getCP(place) {
    for (const component of place.address_components) {
        const componentType = component.types[0];
        if (componentType == "postal_code") {
            return component.long_name;
        }
    }
    return "Sin CP";
}

function getMunicipio(place) {
    for (const component of place.address_components) {
        const componentType = component.types[0];
        if (componentType == "locality") {
            return component.long_name;
        }
    }
    return "Sin municipio";
}

function getEstado(place) {
    for (const component of place.address_components) {
        const componentType = component.types[0];
        if (componentType == "administrative_area_level_1") {
            return component.long_name;
        }
    }
    return "Sin estado";
}
