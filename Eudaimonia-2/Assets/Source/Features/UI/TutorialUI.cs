using TMPro;
using UnityEngine;
using Zenject;

namespace Features.UI
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private GameObject uiPanel;

        private TutorialManager _tutorialManager;

        [Inject]
        private void Init(TutorialManager tutorialManager)
        {
            _tutorialManager = tutorialManager;
        }

        private void OnEnable()
        {
            if (_tutorialManager != null)
            {
                _tutorialManager.OnHintShow += ShowHint;
                _tutorialManager.OnHintHide += HideHint;
            }
        }

        private void OnDisable()
        {
            if (_tutorialManager != null)
            {
                _tutorialManager.OnHintShow -= ShowHint;
                _tutorialManager.OnHintHide -= HideHint;
            }
        }

        private void ShowHint(string text)
        {
            hintText.text = text;
            uiPanel.SetActive(true);
        }

        private void HideHint()
        {
            uiPanel.SetActive(false);
        }
    }
}