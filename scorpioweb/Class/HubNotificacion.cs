using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace scorpioweb.Class
{
    public class HubNotificacion:Hub
    {

        public async Task Send(string name, string area )
        {
            await Clients.All.SendAsync("Recive", name, area );  

        }
    }
}
