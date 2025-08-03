using System;
using System.Collections;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static event Action OnGameStarted;
    public static event Action OnGameEnded;
    public static event Action<bool> OnGamePaused;
    public static event Action OnLevelStart;
    public static event Action OnLevelOver;

    [SerializeField] private GameManager gameManager;
    
    private bool _gameStarted;
    private bool _isPaused;
    
    private AudioManager _audioManager;
    private MenuManager _menuManager;

    private void Awake()
    {
        Player.OnDeath += TriggerDefeat;
        Enemy.OnDeath += TriggerLevelVictory;
        LevelsManager.OnGameWin += TriggerVictory;
        MenuButton.OnLevelButtonClicked += StartLevel;
    }

    private void OnDestroy()
    {
        Player.OnDeath -= TriggerDefeat;
        Enemy.OnDeath -= TriggerLevelVictory;
        LevelsManager.OnGameWin -= TriggerVictory;
        MenuButton.OnLevelButtonClicked -= StartLevel;
    }

    private void Start()
    {
        _audioManager = gameManager.GetAudioManager();
        _menuManager = gameManager.GetMenuManager();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    public void StartGame()
    {
        _gameStarted = !_gameStarted;
        OnGameStarted?.Invoke();
    }
    
    public void StartLevel()
    {
        OnLevelStart?.Invoke();
    }

    public void EndGame()
    {
        OnGameEnded?.Invoke();
        _audioManager.PlayAudio(AudioManager.AudioList.Click);
        Application.Quit();
    }
    
    public void Pause()
    {
        _isPaused = !_isPaused;
        OnGamePaused?.Invoke(_isPaused);
    }

    private void TriggerLevelVictory()
    {
        OnLevelOver?.Invoke();
        _audioManager.PlayAudio(AudioManager.AudioList.GameWin);
    }

    private void TriggerVictory()
    {
        OnGameEnded?.Invoke();
        _menuManager.GameOver(true);
    }

    private void TriggerDefeat()
    {
        OnLevelOver?.Invoke();
        OnGameEnded?.Invoke();
        
        _menuManager.GameOver(false);
        _audioManager.PlayAudio(AudioManager.AudioList.GameOver);
    }
}
