using System;
using System.Collections.Generic;
using Jonas.UI.ShopUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//provides functions to control the main menu
namespace Jonas.UI.MainUI
{
    public class MainMenuControl : MonoBehaviour
    {
        [Tooltip("Integer of the start scene, which is loaded with the game start.")]
        public int startSceneInt = 1;
        [Tooltip("Parent for the main menu.")]
        public GameObject mainMenu;
        [Tooltip("Parent for the main menu.")]
        public GameObject creditsMenu;
        [Tooltip("Parent for the main menu.")]
        public Button creditsBackButton;
        [Tooltip("Parent for the main menu.")]
        public Button creditsButton;

        //start the game
        public void StartNewGame()
        {
            SceneManager.LoadScene(startSceneInt);
        }

        //open about menu
        public void OpenCreditsMenu()
        {
            mainMenu.SetActive(false);
            creditsMenu.SetActive(true);
            creditsBackButton.Select();
        }

        //go back to main menu
        public void GoBackToMainMenu()
        {
            mainMenu.SetActive(true);

            creditsMenu.SetActive(false);
            creditsButton.Select();
        }

        //quit the application
        public void QuitGame()
        {
            Application.Quit();
            Debug.Log("Quit game.");
        }
    }
}
