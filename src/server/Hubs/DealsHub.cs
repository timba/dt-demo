using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DTDemo.Server.Hubs
{
    public class DealsHub : Hub<IDealsHub>
    {
        public override async Task OnConnectedAsync()
        {
            await this.Clients.Caller.id(this.Context.ConnectionId);
            await base.OnConnectedAsync();
        }
    }

    public interface IDealsHub
    {
        Task id(string connectionId);

        Task start();

        Task stat(Controllers.DealsDataController.MostSoldVehicleView stat);

        Task error(string message);

        Task deals(IEnumerable<Controllers.DealsDataController.DealRecordView> deals);

    }

}
