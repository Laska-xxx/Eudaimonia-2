using Features.Player.Interact;
using Features.UI;
using System;
using UnityEngine;
using Zenject;

namespace Features.Interactable.Environment.Note
{
    public class NoteItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string hintText = "Читать";
        public string HintText => hintText;

        [SerializeField] private NoteDataSO noteData;

        private NoteUI _noteUIcontroller;
        private NoteManager _noteManager;

        public event Action OnOpened;

        [Inject]
        public void Init(NoteUI noteUIController, NoteManager noteManager)
        {
            _noteUIcontroller = noteUIController;
            _noteManager = noteManager;
        }

        public void Interact(PlayerInteractor player)
        {
            if (noteData == null || noteData.NoteImage == null) return;

            _noteManager.AddNote(noteData);

            _noteUIcontroller.OpenNote(noteData);

            OnOpened?.Invoke();
        }
    }
}