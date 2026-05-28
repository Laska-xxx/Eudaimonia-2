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

        [SerializeField] private NoteDataSO _noteData;

        private NoteUI _noteUIcontroller;

        public event Action OnOpened;

        [Inject]
        public void Init(NoteUI noteUIController)
        {
            _noteUIcontroller = noteUIController;
        }

        public void Interact(PlayerInteractor player)
        {
            if (_noteData == null || _noteData.NoteImage == null) return;

            print("open image note");

            _noteUIcontroller.OpenNote(_noteData);

            OnOpened?.Invoke();
        }
    }
}