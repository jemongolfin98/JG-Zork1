using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Zork
{
    public class Room : IEquatable<Room>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty(Order = 1)]
        public string Name { get; set; }

        [JsonProperty(Order = 2)]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Neighbors", Order = 3)]
        private Dictionary<Directions, string> NeighborNames { get; set; } = new Dictionary<Directions, string>();

        [JsonIgnore]
        public IReadOnlyDictionary<Directions, Room> Neighbors => _neighbors;

        public Room(string name = null)
        {
            Name = name;
        }

        public static bool operator ==(Room lhs, Room rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (lhs is null || rhs is null)
            {
                return false;
            }

            return string.Compare(lhs.Name, rhs.Name, ignoreCase: true) == 0;
        }

        public static bool operator !=(Room lhs, Room rhs) => !(lhs == rhs);

        public override bool Equals(object obj) => obj is Room room && this == room;

        public bool Equals(Room other) => this == other;

        public override string ToString() => Name;

        public override int GetHashCode() => Name.GetHashCode();

        public void UpdateNeighbors(World world)
        {
            _neighbors.Clear();
            foreach (var entry in NeighborNames)
            {
                _neighbors.Add(entry.Key, world.RoomsByName[entry.Value]);
            }
        }

        public void RemoveNeighbor(Directions direction)
        {
            _neighbors.Remove(direction);
            NeighborNames.Remove(direction);
        }

        public void AssignNeighbor(Directions direction, Room neighbor)
        {
            _neighbors[direction] = neighbor;
            NeighborNames[direction] = neighbor.Name;
        }

        private Dictionary<Directions, Room> _neighbors = new Dictionary<Directions, Room>();
    }
}