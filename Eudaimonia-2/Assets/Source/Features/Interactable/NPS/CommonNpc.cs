using UnityEngine;
using Features.Player.Interact;

namespace Features.Interactable.NPS
{
    public class CommonNpc : NPCInteractable
    {
        private int count = 0;
        private bool _isShowingPhrase = false;

        public override void Interact(PlayerInteractor player)
        {
            if (NpsData.NpsPhrases.Count == 0) return;

            if (_isShowingPhrase)
            {
                _dialogueUI.TryAdvance();
                return;
            }

            if (count >= 5)
            {
                _dialogueUI.ShowPhrase(NpsData.NpsName, NpsData.EndPhrase);
            }
            else
            {
                string randomPhrase = NpsData.NpsPhrases[Random.Range(0, NpsData.NpsPhrases.Count)];
                _dialogueUI.ShowPhrase(NpsData.NpsName, randomPhrase);
                count++;
            }
        }
    }
}