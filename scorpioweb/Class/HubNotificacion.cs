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
                    break;
                case "Ejecucion":
                    break;
                case "AdminLC":
                    break;
                case "Coordinador":
                    break;
                case "SupervisorLC":
                    break;
                case "Archivo":
                    break;
                case "Director":
                    break;
                case "AuxiliarEjecucion":
                    break;
                case "AdminMCSCP":
                    break;
                case "SupervisorMCSCP":
                    break;
                case "AuxiliarMCSCP":
                    break;
                case "Vinculacion":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "seCerrocaso");
                    await Groups.AddToGroupAsync(Context.ConnectionId, "nuevaCanalizacion");
                    break;
                case "Administrador":
                    break;
                case "AdminVinculacion":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Canalizacion");
                    break;
                case "Coordinador Ejecucion":
                    break;
                case "EnviarCorrespondencia":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "EnviarCorrespondencia");
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
