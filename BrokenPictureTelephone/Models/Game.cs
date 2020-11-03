using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrokenPictureTelephone.Models
{
    public class Game
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public bool isClosed { get; set; }

        public virtual List<Entry> Entries { get; set; }
    }

    public class Entry
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual Game Game { get; set; }
    }
}
