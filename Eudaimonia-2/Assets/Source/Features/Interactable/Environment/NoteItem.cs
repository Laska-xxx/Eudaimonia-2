using Features.UI;
using Features.Player.Interact;
using UnityEngine;
using Zenject;
using System;

namespace Features.Interactable.Environment
{
    public class NoteItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string hintText = "Читать";
        public string HintText => hintText;
        [TextArea(5, 10)][SerializeField] private string noteText;

        private NoteUI _noteUIcontroller;

        public event Action OnOpened;

        [Inject]
        public void Init(NoteUI noteUIController) 
        {
            _noteUIcontroller = noteUIController;
        }

        public void Interact(PlayerInteractor player)
        {
            print("open");
            _noteUIcontroller.OpenNote(noteText);
            OnOpened?.Invoke();
        }
    }
}