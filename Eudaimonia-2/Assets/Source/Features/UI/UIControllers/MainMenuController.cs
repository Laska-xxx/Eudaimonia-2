using UnityEngine;
using UnityEngine.SceneManagement;

namespace Features.UI.UIControllers
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private int gameSceneId = 1;

        public void Play()
        {
            SceneManager.LoadScene(gameSceneId);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}