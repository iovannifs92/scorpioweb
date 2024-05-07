

<script src="~/lib/Microsoft/signalr.min.js"></script>

var hubUrl = '@Url.Content("~/Class/HubNotificacion")';

var connection = new signalR.HubConnectionBuilder().withUrl(hubUrl).build();
connection.start().then(function () {
    console.log("Conexion exitosa SIGNAL R");
}).catch(function (err) {
    console.log(err);
});

connection.on("Recive", function (name, area) {
    let divnames = document.createElement("div");
    divnames.textContent = name + " " + area;
    document.getElementById("divname").appendChild(divnames);
    alert(name + " " + area)
});