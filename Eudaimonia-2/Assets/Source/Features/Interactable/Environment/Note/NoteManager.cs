using System;
using System.Collections.Generic;

namespace Features.Interactable.Environment.Note
{
    public class NoteManager
    {
        public List<NoteDataSO> NotesCollected { get; private set; } = new List<NoteDataSO>();

        public event Action<int> OnNoteCollected;

        public void AddNote(NoteDataSO note)
        {
            if (!NotesCollected.Contains(note))
            {
                NotesCollected.Add(note);
                OnNoteCollected?.Invoke(NotesCollected.Count);
            }
        }
    }
}