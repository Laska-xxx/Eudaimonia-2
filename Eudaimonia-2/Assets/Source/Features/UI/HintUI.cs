using TMPro;
using UnityEngine;

namespace Features.UI
{
    public class HintUI : MonoBehaviour
    {
        [SerializeField] private GameObject _hintContainer;
        [SerializeField] private TextMeshProUGUI _hintText;

        private void Awake()
        {
            HideHint();
        }

        public void ShowHint(string message)
        {
            _hintText.text = $"{message} [E]";
            _hintContainer.SetActive(true);
        }

        public void HideHint()
        {
            _hintContainer.SetActive(false);
        }
    }
}