using UnityEngine;

namespace Core
{
    public class InputManager
    {
        public GameInput GameInput { get; private set; }
        public ActionMapType CurrentActionMapType { get; private set; }

        public InputManager()
        {
            GameInput = new GameInput();
            SwitchActionMapType(ActionMapType.Game);
            Application.quitting += Dispose;
        }

        private void DisableAllActionMaps()
        {
            GameInput?.Player.Disable();
            GameInput?.UI.Disable();
        }

        public void SwitchActionMapType(ActionMapType mapType)
        {
            DisableAllActionMaps();

            switch (mapType)
            {
                case ActionMapType.Game:
                    GameInput?.Player.Enable();
                    break;
                case ActionMapType.UI:
                    GameInput?.UI.Enable();
                    break;
            }

            CurrentActionMapType = mapType;
        }

        private void Dispose()
        {
            Application.quitting -= Dispose;
            GameInput?.Dispose();
            GameInput = null;
        }
    }

    public enum ActionMapType
    {
        Game,
        UI
    }
}