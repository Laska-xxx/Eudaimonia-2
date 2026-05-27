using Features.Player.Interact;
using Features.UI;
using UnityEngine;
using Zenject;

namespace Features.Interactable.NPS
{
    public class SpecialNPSInteractable : MonoBehaviour, IInteractable
    {
        public string HintText => "Говорить";
        public bool DialogueWasPlayed => _dialogueWasPlayed;

        [SerializeField] private NpsPhrasesSO npsData;

        private DialogueUI _dialogueUI;
        private bool _dialogueWasPlayed = false;
        private bool _isCurrentlyTalking = false;

        [Inject]
        public void Init(DialogueUI dialogueUI)
        {
            _dialogueUI = dialogueUI;
        }

        public void Interact(PlayerInteractor player)
        {
            if (npsData.NpsPhrases.Count == 0) return;

            if (_isCurrentlyTalking)
            {
                _dialogueUI.TryAdvance();
                return;
            }

            if (!_dialogueWasPlayed)
            {
                _dialogueUI.ShowDialogue(npsData.NpsName, npsData.NpsPhrases, DialogueComplete);
                _isCurrentlyTalking = true;
            }
            else
            {
                _dialogueUI.ShowPhrase(npsData.NpsName, npsData.EndPhrase);
            }
        }

        public void DialogueComplete()
        {
            _dialogueWasPlayed = true;
            _isCurrentlyTalking = false;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                _isCurrentlyTalking = false;
                _dialogueUI.Close();
            }
        }
    }
}