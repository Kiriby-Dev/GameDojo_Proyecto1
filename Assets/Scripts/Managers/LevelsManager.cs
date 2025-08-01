using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private GameObject levelsContainer;
    
    [Header("Config")]
    [SerializeField] int cantLevels;
    [SerializeField] private StatsRange firstSubject;
    [SerializeField] private StatsRange secondSubject;
    [SerializeField] private StatsRange thirdSubject;
    [SerializeField] private StatsRange boss;

    private State[] _levels;
    private Button[] _levelsButtons;
    private List<QuestionData.Subject> _subjects;
    private QuestionData.Subject _actualSubject;
    
    private int _currentLevel = 0;
    
    private Enemy _enemy;
    
    public static event Action<QuestionData.Subject> OnSubjectChosen;
    
    public enum State
    {
        Able,
        Blocked,
        Approved
    }
    
    [System.Serializable]
    public class StatsRange
    {
        public int minAttack;
        public int maxAttack;
        public int minDefense;
        public int maxDefense;
    }

    private void Awake()
    {
        GameFlowManager.OnLevelStart += SelectLevelDifficulty;
        
        _levels = new State[cantLevels];
        _enemy = gameManager.GetEnemy();
    }

    private void OnDestroy()
    {
        GameFlowManager.OnLevelStart -= SelectLevelDifficulty;
    }

    private void Start()
    {
        InitializeLevels();
    }

    private void InitializeLevels()
    {
        InstantiateLevels();
        Reset();
        UpdateLevelsButtons();
    }

    private void InstantiateLevels()
    {
        _levelsButtons = new Button[cantLevels];
        for (int i = 0; i < cantLevels; i++)
        {
            GameObject level = Instantiate(levelButtonPrefab, levelsContainer.transform);
            Button levelButton = level.GetComponentInChildren<Button>();
            _levelsButtons[i] = levelButton;
            _levelsButtons[i].interactable = false;
            _levels[i] = State.Blocked;
        }
    }

    private void ChooseRandomSubject()
    {
        if (_subjects.Count == 0)
        {
            BossBattle();
            return;
        }

        int index = Random.Range(0, _subjects.Count);
        _actualSubject = _subjects[index];
        OnSubjectChosen?.Invoke(_actualSubject);
        _subjects.RemoveAt(index);
    }

    private void BossBattle()
    {
        _actualSubject = QuestionData.Subject.Principal;
        OnSubjectChosen?.Invoke(_actualSubject);
    }

    private void SelectLevelDifficulty()
    {
        StatsRange stats = null;
        
        switch (_subjects.Count)
        {
            case 0:
                stats = boss;
                break;
            case 1:
                stats = thirdSubject;
                break;
            case 2:
                stats = secondSubject;
                break;
            case 3:
                stats = firstSubject;
                break;
        }
        
        if (stats != null)
            _enemy.GenerateStats(stats.minAttack, stats.maxAttack, stats.minDefense, stats.maxDefense);
    }

    public void AdvanceLevel()
    {
        _levels[_currentLevel] = State.Approved;
        _currentLevel++;
        
        ChooseRandomSubject();
        
        if (_currentLevel < _levels.Length)
        {
            _levels[_currentLevel] = State.Able;
            _levelsButtons[_currentLevel].interactable = true;
        }
        UpdateLevelsButtons();
    }

    private void UpdateLevelsButtons()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            State level = _levels[i]; 
            //gameManager.GetUIManager().UpdateLevelButton(i, level, _actualSubject);
        }
    }

    #region Utilities
    private void InitializeSubjectList()
    {
        _subjects = new List<QuestionData.Subject>();
        
        _subjects.Add(QuestionData.Subject.History);
        _subjects.Add(QuestionData.Subject.Science);
        _subjects.Add(QuestionData.Subject.Entertainment);
        _subjects.Add(QuestionData.Subject.Geography);
    }

    private void Reset()
    {
        _currentLevel = 0;
        InitializeSubjectList();
        
        ChooseRandomSubject();
        _levels[_currentLevel] = State.Able;
        _levelsButtons[_currentLevel].interactable = true;
    }

    #endregion

    #region Getters
    public QuestionData.Subject GetActualSubject() => _actualSubject;
    #endregion
    
}
