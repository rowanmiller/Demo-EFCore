using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteTaker
{
    public class Note
    {
        public int NoteId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public bool IsSynced { get; set; }
    }
}
