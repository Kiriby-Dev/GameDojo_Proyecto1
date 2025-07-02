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

    public Button[] levelButtons;

    private void Start()
    {
        foreach (Button button in levelButtons)
        {
            button.interactable = false;
        }
        
        int cantButtons = levelButtons.Length;
        levelButtons[cantButtons - 1].interactable = true;
    }

    public void StartGame()
    {
        menu.SetActive(false);
        game.SetActive(true);
        gameManager.ResetVariables();
        gameManager.StartGame();
    }

    public void TogglePauseGameCanvas(bool pause)
    {
        canvasPause.enabled = pause;
    }
}
