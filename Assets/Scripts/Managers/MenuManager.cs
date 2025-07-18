using System;
using System.Collections;
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
    public Canvas canvasOptions;

    public Button[] levelsButtons;
    
    private TransitionManager _transitionManager;

    private void Start()
    {
        ResetMenus();
        DisableBlockedLevels();
        _transitionManager = gameManager.GetTransitionManager();
    }

    private void ResetMenus()
    {
        game.SetActive(false);
        menu.SetActive(true);
        canvasMenu.enabled = true;
        canvasLevelsMenu.enabled = false;
    }

    public void StartGame()
    {
        menu.SetActive(false);
        game.SetActive(true);
        gameManager.StartGame();
    }

    private IEnumerator PlayCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        print("transition");
        canvasMenu.enabled = false;
        canvasLevelsMenu.enabled = true;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
    }
    
    private IEnumerator MenuCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        print("transition");
        canvasOptions.enabled = false;
        canvasLevelsMenu.enabled = false;
        canvasMenu.enabled = true;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void OptionsButton()
    {
        canvasOptions.enabled = true;
    }

    public void TogglePause(bool pause)
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

    #region Utilities
    
    public void PlayButton()
    {
        StartCoroutine(PlayCoroutine());
    }
    
    public void MenuButton()
    {
        StartCoroutine(MenuCoroutine());
    }

    #endregion
}
