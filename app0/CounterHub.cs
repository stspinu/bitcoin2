using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class CounterHub : Hub
{
    public async Task SendCounter(int counter)
    {
        await Clients.All.SendAsync("ReceiveCounter", counter);
    }
}
