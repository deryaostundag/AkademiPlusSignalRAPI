using AkademiPlusSignalRAPI.DAL;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AkademiPlusSignalRAPI.Hubs
{
    public class MyHub:Hub
    {
        //Kişileri tutacak olan liste
        public static List<string> Names { get; set; }= new List<string>();
        //Kaç Client bağlı olduğunu gösterecek prop
        public int ClientCount { get; set; } = 0;
        // Bir odada max bulunacak kişi sayısı 
        public static int RoomCount { get; set; } = 5;
        private readonly Context _context;

        public MyHub(Context context)
        {
            _context = context;
        }
        public async Task sendname(string name)
        {
            Names.Add(name);
            await Clients.All.SendAsync("recievename", name);
        }
        public override async Task OnConnectedAsync()
        {
            ClientCount++;
            await Clients.All.SendAsync("receiveclientcount", ClientCount);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("receiveclientcount", ClientCount);
        }
    }
}
