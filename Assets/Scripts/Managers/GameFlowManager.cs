using System;
using System.Collections;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static event Action OnGameStarted;
    public static event Action<bool> OnGameEnded;
    public static event Action<bool> OnGamePaused;
    public static event Action OnLevelStart;
    public static event Action OnLevelOver;
    
    private bool _gameStarted;
    private bool _levelEnded;
    private bool _isPaused;
    
    private AudioManager _audioManager;
    private MenuManager _menuManager;

    private void Awake()
    {
        Player.OnDeath += TriggerDefeat;
        Enemy.OnDeath += TriggerLevelVictory;
        MenuButton.OnLevelButtonClicked += StartLevel;
    }

    private void OnDestroy()
    {
        Player.OnDeath -= TriggerDefeat;
        Enemy.OnDeath -= TriggerLevelVictory;
        MenuButton.OnLevelButtonClicked -= StartLevel;
    }

    private void Start()
    {
        _audioManager = GameManager.Instance.GetAudioManager();
        _menuManager = GameManager.Instance.GetMenuManager();
        _levelEnded = true;
    }

    private void Update()
    {
        if (!_gameStarted) return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    public void StartGame()
    {
        _gameStarted = true;
        OnGameStarted?.Invoke();
    }
    
    public void StartLevel()
    {
        if (_levelEnded)
        {
            _levelEnded = false;
            OnLevelStart?.Invoke();
        }
        else
        {
            _menuManager.DisableAllCanvas();
            _menuManager.TogglePauseCanvas(true);
        }
    }

    public void EndGame()
    {
        _audioManager.PlayAudio(AudioManager.AudioList.Click);
        Application.Quit();
    }
    
    public void Pause()
    {
        _isPaused = !_isPaused;
        OnGamePaused?.Invoke(_isPaused);
    }

    private void TriggerLevelVictory(bool isEnemyDead)
    {
        if (!isEnemyDead) return;
        if (GameManager.Instance.GetLevelsManager().GetActualSubject() == QuestionData.Subject.Principal)
        {
            TriggerVictory();
            return;
        }
        if (_levelEnded) return;
        
        _levelEnded = true;

        GameManager.Instance.DeactivateGameObjects(false);
        
        OnLevelOver?.Invoke();
        _audioManager.PlayAudio(AudioManager.AudioList.GameWin);
    }

    private void TriggerDefeat(bool isPalyerDead)
    {
        if (!isPalyerDead) return;
        
        OnGameEnded?.Invoke(false);
        _audioManager.PlayAudio(AudioManager.AudioList.GameOver);
    }
    
    private void TriggerVictory()
    {
        OnGameEnded?.Invoke(true);
        _audioManager.PlayAudio(AudioManager.AudioList.GameWin);
    }
}
