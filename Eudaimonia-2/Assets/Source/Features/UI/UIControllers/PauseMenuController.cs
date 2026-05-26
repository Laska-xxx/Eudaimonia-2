using Features.UI.DoTween;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace Features.UI.UIControllers
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private string menuSceneName = "MainMenu";
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private UIPanel settingsPanel;

        private InputManager _inputManager;

        [Inject]
        private void Init(InputManager inputManager)
        {
            _inputManager = inputManager;

            _inputManager.GameInput.Player.Esc.performed += TryPause;
            _inputManager.GameInput.UI.Esc.performed += TryContinue;
        }

        private void OnDisable()
        {
            if (_inputManager.GameInput != null)
            {
                _inputManager.GameInput.Player.Esc.performed -=  TryPause;
                _inputManager.GameInput.UI.Esc.performed -= TryContinue;
            }
        }

        private void Start()
        {
            pausePanel.SetActive(false);
        }

        public void ToMainMenu()
        {
            settingsPanel?.Hide();
            SceneManager.LoadScene(menuSceneName);
        }

        public void TryContinue()
        {
            pausePanel.SetActive(false);
            _inputManager.SwitchActionMapType(ActionMapType.Game);
            Cursor.lockState = CursorLockMode.Locked;
            settingsPanel?.Hide();
        }

        public void OpenSettings()
        {
            settingsPanel.Show();
        }

        public void CloseSettings()
        {
            settingsPanel.Hide();
        }

        public void Lobotomy()
        {
            Debug.Log("Lobotomy");
        }

        private void TryContinue(InputAction.CallbackContext ctx)
        {
            TryContinue();
        }

        private void TryPause(InputAction.CallbackContext ctx)
        {
            pausePanel.SetActive(true);
            _inputManager.SwitchActionMapType(ActionMapType.UI);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}