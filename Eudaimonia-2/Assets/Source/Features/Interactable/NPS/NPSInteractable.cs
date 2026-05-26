using Features.UI;
using UnityEngine;
using Zenject;
using Features.Player.Interact;

namespace Features.Interactable.NPS
{
    public class NPSInteractable : MonoBehaviour, IInteractable
    {
        public string HintText => "Говорить";

        [SerializeField] private NpsPhrasesSO npsData;

        private DialogueUI _dialogueUI;
        private int count = 0;
        private bool _isShowingPhrase = false;

        [Inject]
        public void Init(DialogueUI dialogueUI)
        {
            _dialogueUI = dialogueUI;
        }

        public void Interact(PlayerInteractor player)
        {
            if (npsData.NpsPhrases.Count == 0) return;

            if (_isShowingPhrase)
            {
                _dialogueUI.TryAdvance();
                return;
            }

            if (count >= 5)
            {
                _dialogueUI.ShowPhrase(npsData.NpsName, npsData.EndPhrase);
            }
            else
            {
                string randomPhrase = npsData.NpsPhrases[Random.Range(0, npsData.NpsPhrases.Count)];
                _dialogueUI.ShowPhrase(npsData.NpsName, randomPhrase);
                count++;
            }
        }
    }
}