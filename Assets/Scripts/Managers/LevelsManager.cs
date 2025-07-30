using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Button[] levelsButtons;
    
    [Header("Config")]
    [SerializeField] int cantLevels;
    [SerializeField] private StatsRange firstSubject;
    [SerializeField] private StatsRange secondSubject;
    [SerializeField] private StatsRange thirdSubject;
    [SerializeField] private StatsRange boss;

    private State[] _levels;
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
        _levels = new State[cantLevels];
        _enemy = gameManager.GetEnemy();
    }

    private void Start()
    {
        Reset();
        UpdateLevelsButtons();
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

    public void SelectLevelDifficulty()
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
            levelsButtons[_currentLevel].interactable = true;
        }
        UpdateLevelsButtons();
    }

    private void UpdateLevelsButtons()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            State level = _levels[i]; 
            gameManager.GetUIManager().UpdateLevelButton(i, level, _actualSubject);
        }
    }

    #region Utilities
    private void InitializeLevelsState()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i] = State.Blocked;
            levelsButtons[i].interactable = false;
        }
    }
    
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
        InitializeLevelsState();
        InitializeSubjectList();
        
        ChooseRandomSubject();
        _levels[_currentLevel] = State.Able;
        levelsButtons[_currentLevel].interactable = true;
    }

    #endregion

    #region Getters
    public QuestionData.Subject GetActualSubject() => _actualSubject;
    #endregion
    
}
