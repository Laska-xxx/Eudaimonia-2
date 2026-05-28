using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Features.UI
{
    public class TutorialUI : MonoBehaviour
    {
        [Header("Hint")]
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private GameObject uiPanel;

        [Header("Start Panel")]
        [SerializeField] private GameObject starPanel;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

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

            yesButton.onClick.AddListener(ChoiceYes);
            noButton.onClick.AddListener(ChoiceNo);
        }

        private void OnDisable()
        {
            if (_tutorialManager != null)
            {
                _tutorialManager.OnHintShow -= ShowHint;
                _tutorialManager.OnHintHide -= HideHint;
            }

            yesButton.onClick.RemoveListener(ChoiceYes);
            noButton.onClick.RemoveListener(ChoiceNo);
        }

        private void ChoiceYes()
        {
            starPanel.SetActive(false);
            _tutorialManager.StartTutorial();
        }

        private void ChoiceNo()
        {
            starPanel.SetActive(false); 
            _tutorialManager.SkipTutorial(); 
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