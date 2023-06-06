using AkademiPlusSignalRAPI.DAL;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AkademiPlusSignalRAPI.Hubs
{
    public class MyHub : Hub
    {
        //Kişileri tutacak olan liste
        public static List<string> Names { get; set; } = new List<string>();
        //Kaç Client bağlı olduğunu gösterecek prop
        public static int ClientCount { get; set; } = 0;
        // Bir odada max bulunacak kişi sayısı 
        public static int RoomCount { get; set; } = 5;
        private readonly Context _context;

        public MyHub(Context context)
        {
            _context = context;
        }

        public async Task SendName(string name)
        {
            if (Names.Count >= RoomCount)
            {
                await Clients.Caller.SendAsync("error", $"Bu odada en fazla {RoomCount} kişi olabilir");
            }
            Names.Add(name);
            await Clients.All.SendAsync("recievename", name);
        }
        public async Task GetNames()
        {
            await Clients.All.SendAsync("recieveNames", Names);  //İsim listesinin hepsini getirir.
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
        public async Task SendNameByGroup(string name, string roomName)
        {
            var room = _context.Rooms.Where(x => x.RoomName == roomName).FirstOrDefault();
            if (room != null)
            {
                room.Users.Add(new User
                {
                    Name = name
                });
            }
            else
            {
                var newRoom = new Room() { RoomName = roomName };
                newRoom.Users.Add(new User { Name = name });
                _context.Rooms.Add(newRoom);
            }
            await _context.SaveChangesAsync();
            await Clients.Groups(roomName).SendAsync("receiveMessageByGroup", name, room.RoomID);
        }
        public async Task GetNameByGroup()
        {
            var rooms = _context.Rooms.Include(x => x.Users).Select(y => new
            {
                RoomID = y.RoomID,
                Users = y.Users.ToList()
            });
            await Clients.All.SendAsync("recieveNamesByGroup", rooms);
        }
        public async Task AddToGroup(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }
        public async Task RemoveFromGroup(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
    }
}
