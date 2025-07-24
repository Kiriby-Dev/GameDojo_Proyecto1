using System;
using System.Collections.Generic;
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
    
    public enum State
    {
        Able,
        Blocked,
        Approved
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
        _subjects.Add(QuestionData.Subject.Logic);
    }

    private void ChooseRandomSubject()
    {
        if (_subjects.Count == 0)
        {
            Debug.LogWarning("No hay más materias disponibles para elegir.");
            return;
        }

        int index = Random.Range(0, _subjects.Count);
        _actualSubject = _subjects[index];
        _subjects.RemoveAt(index);
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
            gameManager.GetUIManager().UpdateLevelButton(i, level, _actualSubject.ToString());
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
