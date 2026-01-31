using UnityEngine;
using UnityEngine.SceneManagement;

//provides functions to control the main menu
namespace Jonas.UI.MainUI
{
    public class MainMenuControl : MonoBehaviour
    {
        [Tooltip("Integer of the start scene, which is loaded with the game start.")]
        public int startSceneInt = 1;
        [Tooltip("List of sub menus.")]
        public GameObject[] subMenus;
        [Tooltip("Parent for the main menu.")]
        public GameObject mainMenu;
        [Tooltip("Sound played when clicking a button")]
        public AudioClip buttonClick;
        
        private AudioSource _audioSource;
        private AudioSource AudioSource
        {
            get
            {
                if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
                return _audioSource;
            }
        }
        
        
        //start the game
        public void StartGame()
        {
            SceneManager.LoadScene(startSceneInt);
        }

        //open about menu
        public void OpenSubMenu(GameObject subMenu)
        {
            mainMenu.SetActive(false);
            subMenu.SetActive(true);
        }

        //go back to main menu
        public void GoBackToMainMenu()
        {
            mainMenu.SetActive(true);

            foreach (GameObject subMenu in subMenus)
            {
                subMenu.SetActive(false);
            }
        }

        //quit the application
        public void QuitGame()
        {
            Application.Quit();
            Debug.Log("Quit game.");
        }

        public void PlayButtonClickSound()
        {
            AudioSource.PlayOneShot(buttonClick);
        }
    }
}
