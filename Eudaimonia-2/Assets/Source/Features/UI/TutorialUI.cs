using TMPro;
using UnityEngine;

namespace Features.UI
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private GameObject uiPanel;

        public void ShowHint(string text)
        {
            hintText.text = text;
            uiPanel.SetActive(true);
        }

        public void HideHint()
        {
            uiPanel.SetActive(false);
        }
    }
}