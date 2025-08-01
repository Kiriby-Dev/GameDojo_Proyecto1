using System.Collections;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject game;
    [SerializeField] private GameObject menu;
    
    [Header("Canvas")]
    [SerializeField] private Canvas canvasMenu;
    [SerializeField] private Canvas canvasLevelsMenu;
    [SerializeField] private Canvas canvasOptions;
    [SerializeField] private Canvas canvasTutorial;
    [SerializeField] private Canvas canvasGameOver;

    [SerializeField] private TextMeshProUGUI pauseBottomButton;
    
    private bool _gameStarted;
    private bool _isPaused;
    
    private TransitionManager _transitionManager;
    private AudioManager _audioManager;

    private void Awake()
    {
        GameFlowManager.OnGamePaused += ToggleOptions;
        GameFlowManager.OnGameStarted += StartGame;
        GameFlowManager.OnLevelStart += StartLevel;
    }

    private void OnDestroy()
    {
        GameFlowManager.OnGamePaused -= ToggleOptions;
        GameFlowManager.OnGameStarted -= StartGame;
        GameFlowManager.OnLevelStart -= StartLevel;
    }

    private void Start()
    {
        ResetMenus();
        canvasMenu.enabled = true;
        
        _transitionManager = gameManager.GetTransitionManager();
        _audioManager = gameManager.GetAudioManager();
    }
    
    private void StartGame()
    {
        _gameStarted = !_gameStarted;
        StartCoroutine(TransitionCoroutine(canvasLevelsMenu));
        _audioManager.PlayAudio(AudioManager.AudioList.Click);
    }

    private void StartLevel()
    {
        game.SetActive(true);
    }
    
    public void GameOver(bool win)
    {
        canvasGameOver.enabled = true;
        if (win)
            canvasGameOver.gameObject.GetComponent<Animator>().SetBool("Win", true);
        else
            canvasGameOver.gameObject.GetComponent<Animator>().SetBool("Lose", true);
    }

    private IEnumerator TransitionCoroutine(Canvas canvas = null)
    {
        yield return new WaitForSeconds(0.5f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        DisableAllCanvas();
        if (canvas)
            canvas.enabled = true;

        if (pauseBottomButton)
        {
            if (_gameStarted)
                pauseBottomButton.text = "Reanudar";
            else
                pauseBottomButton.text = "Tutorial";
        }
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
    }

    #region Utilities
    private void DisableAllCanvas()
    {
        canvasMenu.enabled = false;
        canvasLevelsMenu.enabled = false;
        canvasOptions.enabled = false;
        canvasTutorial.enabled = false;
        canvasGameOver.enabled = false;
        game.SetActive(false);
    }
    
    private void ResetMenus()
    {
        menu.SetActive(true);
        DisableAllCanvas();
    }
    #endregion
    
    #region Buttons
    public void MenuButton()
    {
        StartCoroutine(TransitionCoroutine(canvasMenu));
        _audioManager.PlayAudio(AudioManager.AudioList.Click);
    }

    public void MenuLevelsButton()
    {
        StartCoroutine(TransitionCoroutine(canvasLevelsMenu));
        _audioManager.PlayAudio(AudioManager.AudioList.Click);
    }

    public void PauseBottomButton()
    {
        if (!_gameStarted)
            TutorialButton();
        else
            gameManager.GetGameFlowManager().Pause(); //Si el juego comenzó actúa como un reanudar
        _audioManager.PlayAudio(AudioManager.AudioList.Click);
    }

    public void TutorialButton()
    {
        StartCoroutine(TransitionCoroutine(canvasTutorial));
        _audioManager.PlayAudio(AudioManager.AudioList.Click);
    }
    
    public void ToggleOptions(bool pause)
    {
        if (_gameStarted)
            MenuButton();
        else
            canvasOptions.enabled = pause;
        _audioManager.PlayAudio(AudioManager.AudioList.Click);
    }
    #endregion
}
