using Features.UI.DoTween;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Features.UI
{
    public class NoteUIController : MonoBehaviour
    {
        public bool IsNoteOpen { get; private set; }

        [SerializeField] private UIPanel notePanel;
        [SerializeField] private TextMeshProUGUI noteText;

        private InputManager _inputManager;

        [Inject]
        private void Init(InputManager inputManager)
        {
            _inputManager = inputManager;

            _inputManager.GameInput.UI.ClouseNote.performed += CloseNote;
        }

        private void Awake()
        {
            notePanel.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (_inputManager.GameInput != null)
            {
                _inputManager.GameInput.UI.ClouseNote.performed -= CloseNote;
            }
        }

        public void OpenNote(string text)
        {
            IsNoteOpen = true;
            noteText.text = text;
            notePanel.Show();

            _inputManager.SwitchActionMapType(ActionMapType.UI);
        }

        public void CloseNote(InputAction.CallbackContext ctx)
        {
            IsNoteOpen = false;
            notePanel.Hide();

            _inputManager.SwitchActionMapType(ActionMapType.Game);
        }
    }
}