using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameManager gameManager;
    public GameObject game;
    public GameObject menu;
    
    [Header("Canvas")]
    public Canvas canvasMenu;
    public Canvas canvasPause;
    public Canvas canvasLevelsMenu;

    public Button[] levelsButtons;

    private void Start()
    {
        ResetMenus();
        DisableBlockedLevels();
    }

    private void ResetMenus()
    {
        game.SetActive(false);
        menu.SetActive(true);
    }

    public void StartGame()
    {
        menu.SetActive(false);
        game.SetActive(true);
        gameManager.StartGame();
    }

    public void TogglePauseGameCanvas(bool pause)
    {
        canvasPause.enabled = pause;
    }

    private void DisableBlockedLevels()
    {
        foreach (Button button in levelsButtons)
        {
            button.interactable = false;
        }
        
        int cantButtons = levelsButtons.Length;
        levelsButtons[cantButtons - 1].interactable = true;
    }
}
