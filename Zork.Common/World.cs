using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Zork
{
    public class World : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<Room> Rooms { get; set; }

        [JsonIgnore]
        public IReadOnlyDictionary<string, Room> RoomsByName => _roomsByName;

        public World()
        {
            Rooms = new List<Room>();
            _roomsByName = new Dictionary<string, Room>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            _roomsByName = Rooms.ToDictionary(room => room.Name, room => room);

            foreach (Room room in Rooms)
            {
                room.UpdateNeighbors(this);
            }
        } 

        private Dictionary<string, Room> _roomsByName;
    }
}