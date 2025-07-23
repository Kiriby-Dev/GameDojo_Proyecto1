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
    public Canvas canvasTutorial;

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
        canvasOptions.enabled = false;
        canvasTutorial.enabled = false;
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
        
        canvasMenu.enabled = false;
        canvasLevelsMenu.enabled = true;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
    }
    
    private IEnumerator MenuCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        canvasOptions.enabled = false;
        canvasLevelsMenu.enabled = false;
        canvasMenu.enabled = true;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
    }
    
    private IEnumerator TutorialCoroutine()
    {
        canvasOptions.enabled = false;
        yield return new WaitForSeconds(0.5f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        canvasLevelsMenu.enabled = false;
        canvasTutorial.enabled = true;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void ToggleOptions(bool optionsToggle)
    {
        canvasOptions.enabled = optionsToggle;
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
            button.gameObject.GetComponentInParent<LevelButtonImage>().UpdateSprite(LevelButtonImage.State.Blocked);
        }
        
        int cantButtons = levelsButtons.Length;
        levelsButtons[cantButtons - 1].interactable = true;
        levelsButtons[cantButtons - 1].gameObject.GetComponentInParent<LevelButtonImage>().UpdateSprite(LevelButtonImage.State.Able);
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

    public void TutorialButton()
    {
        StartCoroutine(TutorialCoroutine());
    }
    
    #endregion
}
