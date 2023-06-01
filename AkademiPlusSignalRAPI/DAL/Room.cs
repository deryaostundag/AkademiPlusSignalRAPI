using System.Collections.Generic;

namespace AkademiPlusSignalRAPI.DAL
{
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public List<User> Users { get; set; }
    }
}
