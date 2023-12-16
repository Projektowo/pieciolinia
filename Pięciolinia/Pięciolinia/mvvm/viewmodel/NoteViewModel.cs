using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pięciolinia.MVVM.Model;
public class NoteViewModel
{
    public List<Note> Notes { get; set; }

    public NoteViewModel()
    {
        Notes = new List<Note>();
    }

    public void AddNote(int position, NoteType noteType)
    {
        Note newNote = new Note(position, noteType);
        Notes.Add(newNote);
    }

    public void RemoveNoteByPosition(int position)
    {
        Note noteToRemove = Notes.Find(note => note.Position == position);

        if (noteToRemove != null)
        {
            Notes.Remove(noteToRemove);
        }
        else
        {
            Console.WriteLine("Note not found at position " + position);
        }
    }

    public void ModifyNoteByPosition(int position, NoteType newNoteType)
    {
        Note noteToModify = Notes.Find(note => note.Position == position);

        if (noteToModify != null)
        {
            noteToModify.NoteType = newNoteType;
        }
        else
        {
            Console.WriteLine("Note not found at position " + position);
        }
    }
}