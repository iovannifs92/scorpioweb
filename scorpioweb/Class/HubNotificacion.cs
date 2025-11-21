using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using scorpioweb.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace scorpioweb.Class
{

    public class HubNotificacion:Hub
    {
        //public async Task Send(string name, string area)
        //{
        //    await Clients.All.SendAsync("Recive", name, area);
        //}

        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var roles = user.FindAll(ClaimTypes.Role);

            Console.WriteLine($"Usuario conectado: {user.Identity.Name}");
            foreach (var r in roles)
            {
                Console.WriteLine($"Rol detectado: {r.Value}");
            }

            foreach (var roleClaim in roles)
            {
                string userRole = roleClaim.Value;
                await AddToGroupBasedOnRole(userRole);
            }

            await base.OnConnectedAsync();
        }

        private async Task AddToGroupBasedOnRole(string role)
        {
            switch (role)
            {
                case "Masteradmin":
                    break;
                case "Uespa":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondenciaUESPA");
                    break;
                case "Coordinador":
                    break;
                case "SupervisorLC":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondencialc");
                    break;
                case "AdminLC":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondencialc");
                    break;
                case "Archivo":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondenciaA");
                    break;
                case "Director":
                    break;
                case "AuxiliarEjecucion":
                    break;
                case "AdminMCSCP":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondenciamc");
                    break;
                case "SupervisorMCSCP":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondenciamc");
                    break;
                case "AuxiliarMCSCP":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondenciamc");
                    break;
                case "Vinculacion":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "seCerrocaso");
                    await Groups.AddToGroupAsync(Context.ConnectionId, "nuevaCanalizacion");
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondenciaVin");
                    break;
                case "Administrador":
                    break;
                case "AdminVinculacion":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Canalizacion");
                    break;
                case "Coordinador Ejecucion":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondenciaEje");
                    break;
                case "Ejecucion":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondenciaEje");
                    break;
                case "EnviarCorrespondencia":
                    //await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondencia");
                    break;
                case "Servicios previos":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondenciaSP");
                    break;
                case "Servicios Legales":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviaraCorrespondenciaSL");
                    break;
       
                default:
                    Console.WriteLine("Rol no reconocido");
                    break;
            }
        }

        //public async Task nuevaCanalizacion(string group, string name, string area)
        //{
        //    await Clients.Group(group).SendAsync("Canalizar", name, area);
        //}

    }
}
