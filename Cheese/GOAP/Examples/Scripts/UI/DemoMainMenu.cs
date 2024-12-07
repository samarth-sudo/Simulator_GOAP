using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cheese.GOAP.Demo
{
    public class DemoMainMenu : MonoBehaviour
    {
        public const string websiteURL = "https://www.thegreatoverlordofallcheese.com/";
        public const string itchURL = "https://thegreatoverlordofcheese.itch.io/";
        public const string youtubeURL = "https://www.youtube.com/c/THEGREATOVERLORDOFALLCHEESE";

        private void Start()
        {
            Time.timeScale = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void CommandDemo()
        {
            SceneManager.LoadScene(1);
        }

        public void PerformanceDemo()
        {
            SceneManager.LoadScene(2);
        }

        public void OpenWebsite()
        {
            Application.OpenURL(websiteURL);
        }

        public void OpenItch()
        {
            Application.OpenURL(itchURL);
        }

        public void OpenYoutube()
        {
            Application.OpenURL(youtubeURL);
        }
    }
}