using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cheese.GOAP.Demo
{
    public class DemoPauseMenu : MonoBehaviour
    {
        private bool paused;
        private bool showUI = true;

        public GameObject pauseMenu;
        public GameObject gameUI;

        public int sceneId = 1;

        public float gameSpeed = 1f;

        private void Start()
        {
            UpdatePausedState();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                TogglePause();
            }
            if (Input.GetKeyDown(KeyCode.Tab) && !paused)
            {
                showUI = !showUI;
                UpdatePausedState();
            }
            if (Input.GetKeyDown(KeyCode.CapsLock) && !paused)
            {
                if (gameSpeed == 0f)
                {
                    gameSpeed = 1f;
                }
                else
                {
                    gameSpeed = 0f;
                }
                UpdatePausedState();
            }
        }

        public void TogglePause()
        {
            paused = !paused;
            showUI = true;
            gameSpeed = 1f;
            UpdatePausedState();
        }

        private void UpdatePausedState()
        {
            Time.timeScale = paused ? 0f : gameSpeed;
            pauseMenu.SetActive(paused);

            gameUI.SetActive(!paused && showUI);
            Cursor.visible = paused || showUI;
        }

        public void Reload()
        {
            SceneManager.LoadScene(sceneId);
        }

        public void Quit()
        {
            SceneManager.LoadScene(0);
        }
    }
}