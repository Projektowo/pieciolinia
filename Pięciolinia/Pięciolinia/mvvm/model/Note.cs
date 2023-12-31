using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pięciolinia.MVVM.Model
{
    public class Note
    {
        public int Position { get; set; }
        public NoteType NoteType { get; set; }

        public Note(int position, NoteType noteType)
        {
            Position = position;
            NoteType = noteType;
        }
    }
}