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
    
    private State[] _levels = new State[5];
    private int _currentLevel = 0;
    private QuestionData.Subject _actualSubject;

    private List<QuestionData.Subject> _subjects = new List<QuestionData.Subject>();
    private Enemy _enemy;
    
    public enum State
    {
        Able,
        Blocked,
        Approved
    }

    private void Awake()
    {
        _enemy = gameManager.GetEnemy();
    }

    private void Start()
    {
        InitializeSubjectList();
        InitializeLevelsState();
        _levels[_currentLevel] = State.Able;
        UpdateLevelsButtons();
        DisableBlockedLevels();
    }

    private void InitializeSubjectList()
    {
        _subjects.Add(QuestionData.Subject.History);
        _subjects.Add(QuestionData.Subject.Science);
        _subjects.Add(QuestionData.Subject.Entertainment);
        _subjects.Add(QuestionData.Subject.Geography);
    }

    private void ChooseRandomSubject()
    {
        if (_actualSubject == QuestionData.Subject.Principal)
        {
            WinGame();
        }
        
        if (_subjects.Count == 0)
        {
            BossBattle();
            return;
        }

        int index = Random.Range(0, _subjects.Count);
        _actualSubject = _subjects[index];
        gameManager.GetUIManager().UpdateEnemyName(_actualSubject);
        _subjects.RemoveAt(index);
    }

    private void BossBattle()
    {
        _actualSubject = QuestionData.Subject.Principal;
        gameManager.GetUIManager().UpdateEnemyName(QuestionData.Subject.Principal);
    }

    private void WinGame()
    {
        gameManager.GetUIManager().UpdateGameOverCanvas(true);
    }

    public void SelectLevelDifficulty()
    {
        switch (_subjects.Count)
        {
            case 0:
                _enemy.GenerateStats(2, 4, 2, 4);
                break;
            case 1:
                _enemy.GenerateStats(2, 3, 2, 3);
                break;
            case 2:
                _enemy.GenerateStats(1, 3, 1, 3);
                break;
            case 3:
                _enemy.GenerateStats(1, 2, 1, 2);
                break;
        }
    }

    public void AdvanceLevel()
    {
        _levels[_currentLevel] = State.Approved;
        _currentLevel++;
        if (_currentLevel < _levels.Length)
        {
            _levels[_currentLevel] = State.Able;
            levelsButtons[_currentLevel].interactable = true;
        }
        UpdateLevelsButtons();
    }

    private void UpdateLevelsButtons()
    {
        ChooseRandomSubject();
        for (int i = 0; i < _levels.Length; i++)
        {
            State level = _levels[i]; 
            gameManager.GetUIManager().UpdateLevelButton(i, level, _actualSubject);
        }
    }
    
    private void DisableBlockedLevels()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            if (_levels[i] == State.Blocked)
                levelsButtons[i].interactable = false;
        }
    }

    private void InitializeLevelsState()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i] = State.Blocked;
        }
    }
    
    public QuestionData.Subject GetActualSubject() => _actualSubject;
}
