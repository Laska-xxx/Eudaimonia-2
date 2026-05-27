using Features.UI;
using UnityEngine;
using Zenject;
using Features.Player.Interact;

namespace Features.Interactable.NPS
{
    public abstract class NPCInteractable : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public NpsPhrasesSO NpsData { get; private set; }
        public string HintText => "Говорить";

        protected DialogueUI _dialogueUI;
        
        [Inject]
        public void Init(DialogueUI dialogueUI)
        {
            _dialogueUI = dialogueUI;
        }

        public virtual void Interact(PlayerInteractor player)
        {
        }
    }
}