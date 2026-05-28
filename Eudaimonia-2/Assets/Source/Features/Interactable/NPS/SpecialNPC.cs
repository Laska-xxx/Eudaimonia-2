using Features.Player.Interact;
using UnityEngine;

namespace Features.Interactable.NPS
{
    public class SpecialNPC : NPCInteractable
    {
        public bool DialogueWasPlayed => _dialogueWasPlayed;

        private bool _dialogueWasPlayed = false;
        private bool _isCurrentlyTalking = false;

        public override void Interact(PlayerInteractor player)
        {
            if (NpsData.NpsPhrases.Count == 0) return;

            if (_isCurrentlyTalking)
            {
                _dialogueUI.TryAdvance();
                return;
            }

            if (!_dialogueWasPlayed)
            {
                _dialogueUI.ShowDialogue(NpsData.NpsName, NpsData.NpsPhrases, DialogueComplete);
                _isCurrentlyTalking = true;
            }
            else
            {
                _dialogueUI.ShowPhrase(NpsData.NpsName, NpsData.EndPhrase);
            }
        }

        public void DialogueComplete()
        {
            _dialogueWasPlayed = true;
            _isCurrentlyTalking = false;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isCurrentlyTalking = false;
                _dialogueUI.Close();
            }
        }
    }
}