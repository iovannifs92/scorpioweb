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
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Administradores");
                    break;
                case "Uespa":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Adolecentes ");
                    break;
                case "Ejecucion":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Ejecucion");
                    break;
                case "AdminLC":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "LiberdadCondicionada");
                    break;
                case "Coordinador":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Administradores");
                    break;
                case "SupervisorLC":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "LiberdadCondicionada");
                    break;
                case "Archivo":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Archivo");
                    break;
                case "Director":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Administradores");
                    break;
                case "AuxiliarEjecucion":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Ejecucion");
                    break;
                case "AuxiliarMCSCP":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "MCYSCP");
                    break;
                case "Vinculacion":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "seCerrocaso");
                    await Groups.AddToGroupAsync(Context.ConnectionId, "nuevaCanalizacion");
                    break;
                case "Administrador":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Administradores");
                    break;
                case "AdminVinculacion":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Canalizacion");
                    break;
                case "AdminMCSCP":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "MCYSCP");
                    break;
                case "Coordinador Ejecucion":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Ejecucion");
                    break;
                case "SupervisorMCSCP":
                    await Groups.AddToGroupAsync(Context.ConnectionId, "MCYSCP");
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
