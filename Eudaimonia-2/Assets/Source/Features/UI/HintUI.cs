using Features.Player.Interact;
using TMPro;
using UnityEngine;
using Zenject;

namespace Features.UI
{
    public class HintUI : MonoBehaviour
    {
        [SerializeField] private GameObject _hintContainer;
        [SerializeField] private TextMeshProUGUI _hintText;

        private PlayerInteractor _playerInteractor;

        [Inject]
        private void Init(PlayerInteractor playerInteractor)
        {
            _playerInteractor = playerInteractor;
        }

        private void Awake()
        {
            HideHint();
        }

        private void OnEnable()
        {
            if (_playerInteractor != null)
            {
                _playerInteractor.OnInteractableFound += ShowHint;
                _playerInteractor.OnInteractableLost += HideHint;
            }
        }

        private void OnDisable()
        {
            if (_playerInteractor != null)
            {
                _playerInteractor.OnInteractableFound -= ShowHint;
                _playerInteractor.OnInteractableLost -= HideHint;
            }
        }

        private void ShowHint(string message)
        {
            _hintText.text = $"{message} [E]";
            _hintContainer.SetActive(true);
        }

        private void HideHint()
        {
            _hintContainer.SetActive(false);
        }
    }
}