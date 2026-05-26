using Features.UI;
using Features.Player.Interact;
using UnityEngine;
using Zenject;

namespace Features.Interactable.Environment
{
    public class NoteItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string hintText = "Читать на [E]";
        public string HintText => hintText;
        [TextArea(5, 10)][SerializeField] private string noteText;

        private NoteUIController _noteUIcontroller;

        [Inject]
        public void Init(NoteUIController noteUIController) 
        {
            _noteUIcontroller = noteUIController;
        }

        public void Interact(PlayerInteractor player)
        {
            _noteUIcontroller.OpenNote(noteText);
        }
    }
}