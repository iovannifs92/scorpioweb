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

var marker = null;//si local, sale primero la infowindow vacia
var result;
var map;
var infowindow;
var cnt = 0;//todo

function iniciarMap() {
    var coord = { lat: 24.023601543486198, lng: -104.66070401364269 };//DGEP
    map = new google.maps.Map(document.getElementById('map'),{
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
		if(marker == null) {
			marker = new google.maps.Marker({
				position: event.latLng,
                map: map
            });
		}
		else {
			marker.setPosition( new google.maps.LatLng( event.latLng ) );
        }
        geocodeLatLng(new google.maps.Geocoder(), event.latLng, infowindow);
    });
}

//https://stackoverflow.com/questions/6478914/reverse-geocoding-code
function getGeocodingData(calle, no, nombre, cp, municipio, estado) {
    // This is making the Geocode request
    var geocoder = new google.maps.Geocoder();
    var address;
    if(no == "") {
        address = calle;
    }
    else {
        address = calle + " " + no;
    }
    address += ", " + nombre;
	if(cp != "") {
		address += ", " + cp;
	}
    if(municipio != "Sin municipio") {
        address += ", " + municipio;
    }
    if(estado != "Selecciona" && estado != "Sin estado") {
        address += ", " + estado;
    }

    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status !== google.maps.GeocoderStatus.OK) {
            alert("No hay direcciones");//alert(status);
        }
        // This is checking to see if the Geoeode Status is OK before proceeding
        if (status == google.maps.GeocoderStatus.OK) {
            var colonia = getColonia(results[0]);
            var cp = getCP(results[0]);

            if (estado == "Durango" && municipio == "Durango") {
                setZona(colonia, cp);
            }
            else {
                var z = document.getElementById("zona");
                z.value = "SIN ZONA ASIGNADA";
            }

            var coord = { lat: results[0].geometry.location.lat(), lng: results[0].geometry.location.lng() };
			if(marker == null) {
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

        if(latlng.lat() != 24.023601543486198 || latlng.lng() != -104.66070401364269) {//no asignar zona en DGEP
            var municipio = getMunicipio(result);
            for (const component of result.address_components) {
                const componentType = component.types[0];
                if (componentType == "political") {
                    if (municipio == "Durango") {
                        setZona(component.long_name);
                    }
                    else {
                        var z = document.getElementById("zona");
                        z.value = "SIN ZONA ASIGNADA";
                    }
                }
            }
        }
      } else {
        window.alert("No results found");
      }
    })
    .catch((e) => window.alert("Geocoder failed due to: " + e));
}

function fillInAddress(place) {
  document.getElementById("nombreCF").value = "";
  document.getElementById("no").value = "";
  document.getElementById("calle").value = "";
  document.getElementById("cp").value = "";
  document.getElementById("municipioD").value = 0;
  document.getElementById("estadoD").value = 0;
  
  var municipio = getMunicipio(place);
  for (const component of place.address_components) {
     const componentType = component.types[0];

     switch (componentType) {
       case "political": {
         document.getElementById("nombreCF").value = component.long_name;
         if (municipio == "Durango") {
             setZona(component.long_name);
         }
         else {
             var z = document.getElementById("zona");
             z.value = "SIN ZONA ASIGNADA";
         }
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
       case "locality": {
		 municipio = component.long_name;
         break;
	   }
       case "administrative_area_level_1": {
		 var e = document.getElementById("estadoD");
		 for (let i = 0; i < e.length; i++) {
			 if(e.options[i].text == component.long_name) {
				document.getElementById("estadoD").value = e.options[i].value;
			 }
		 }
         document.getElementById("municipioD").text = municipio;
         $("#estadoD").change();
         break;
       }
	   default: {
		 break;
	   }
     }
    }
  //https://stackoverflow.com/questions/29534194/select-drop-down-on-change-reload-reverts-to-first-option
  if (localStorage.getItem('municipioD')) {
      $('#municipioD').val(localStorage.getItem('municipioD'));
  }
}

function getColonia(place) {
    for (const component of place.address_components) {
        const componentType = component.types[0];
        if (componentType == "political") {
            return component.long_name;
        }
    }
    return "Sin colonia";
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

function setZona(colonia, cp) {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: "/Personas/GetZona",
        traditional: true,
        success: function (response) {
            if (response.success == false) {
                alert("response.success == false");
            }
            else {
                var z = document.getElementById("zona");
                z.value = "SIN ZONA ASIGNADA";
                var matches = 0;
                for (let i = 0; i < response.zonas.length; i++) {
                    if (response.zonas[i].colonia == colonia) {
                        matches++;
                    }
                }
                for (let i = 0; i < response.zonas.length; i++) {
                    if (response.zonas[i].colonia == colonia && (matches <= 1 || response.zonas[i].cp == cp)) {
                        for (let j = 0; j < z.length; j++) {
                            if (z.options[j].text == response.zonas[i].zona) {
                                z.value = z.options[j].value;
                            }
                        }
                    }
                }
            }
        },
        error: function (response) {
            alert("Hubo un error, no se pudieron guardar los datos, contacte con el administrador del sistema");
        }
    });
}