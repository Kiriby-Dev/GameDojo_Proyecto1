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
    public Canvas canvasLevelsMenu;
    public Canvas canvasOptions;
    public Canvas canvasTutorial;
    
    private TransitionManager _transitionManager;

    private void Start()
    {
        ResetMenus();
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
        DisableAllCanvas();
        game.SetActive(true);
        gameManager.StartGame();
    }

    private void DisableAllCanvas()
    {
        canvasMenu.enabled = false;
        canvasLevelsMenu.enabled = false;
        canvasOptions.enabled = false;
        canvasTutorial.enabled = false;
    }

    private IEnumerator PlayCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        game.SetActive(false);
        canvasMenu.enabled = false;
        canvasLevelsMenu.enabled = true;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
    }
    
    private IEnumerator MenuCoroutine()
    {
        gameManager.ToggleFreeze(1);
        yield return new WaitForSeconds(0.5f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        gameManager.EndGame();
        game.SetActive(false);
        DisableAllCanvas();
        canvasMenu.enabled = true;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
    }
    
    private IEnumerator MenuLevelsCoroutine()
    {
        gameManager.ToggleFreeze(1);
        yield return new WaitForSeconds(0.5f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        gameManager.EndGame();
        game.SetActive(false);
        DisableAllCanvas();
        canvasLevelsMenu.enabled = true;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
    }
    
    private IEnumerator TutorialCoroutine()
    {
        gameManager.ToggleFreeze(1);
        yield return new WaitForSeconds(0.5f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        game.SetActive(false);
        DisableAllCanvas();
        canvasTutorial.enabled = true;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void ToggleOptions(bool optionsToggle)
    {
        if (canvasLevelsMenu.isActiveAndEnabled)
            MenuButton();
        else
            canvasOptions.enabled = optionsToggle;
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

    public void MenuLevelsButton()
    {
        StartCoroutine(MenuLevelsCoroutine());
    }

    public void TutorialButton()
    {
        StartCoroutine(TutorialCoroutine());
    }
    
    #endregion
}
