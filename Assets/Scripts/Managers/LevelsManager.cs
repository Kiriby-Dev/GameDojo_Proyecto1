using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelsManager : MonoBehaviour
{
    public static event Action<QuestionData.Subject> OnSubjectChosen;
    
    [Header("GameObjects")]
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private GameObject levelsContainer;
    
    [Header("Config")]
    [SerializeField] private int cantLevels;
    [SerializeField] private StatsRange firstSubject;
    [SerializeField] private StatsRange secondSubject;
    [SerializeField] private StatsRange thirdSubject;
    [SerializeField] private StatsRange boss;
    
    [Header("Levels Icons")]
    [SerializeField] private Sprite approved;
    [SerializeField] private Sprite blocked;
    [SerializeField] private Sprite able;

    private State[] _levels;
    private Button[] _levelsButtons;
    
    private List<QuestionData.Subject> _subjects;
    private QuestionData.Subject _actualSubject;
    
    private int _currentLevel = 0;
    
    private Enemy _enemy;
    
    private enum State
    {
        Able,
        Blocked,
        Approved
    }

    private void Awake()
    {
        GameFlowManager.OnLevelStart += SelectLevelDifficulty;
        
        _levels = new State[cantLevels];
        _enemy = GameManager.Instance.GetEnemy();
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
        {
            _enemy.SetEnemyStats(stats.minAttack, stats.maxAttack, stats.minDefense, stats.maxDefense);
            _enemy.GenerateStats();
        }
        
    }

    public void AdvanceLevel()
    {
        _levels[_currentLevel] = State.Approved;
        UpdateLevelButton(_currentLevel); // Actualiza el nivel actual como aprobado
    
        _currentLevel++;
    
        ChooseRandomSubject();
    
        if (_currentLevel < _levels.Length)
        {
            _levels[_currentLevel] = State.Able;
            _levelsButtons[_currentLevel].interactable = true;
            UpdateLevelButton(_currentLevel); // Actualiza el nuevo nivel desbloqueado
        }
    }

    private void UpdateLevelButton(int levelIndex = 0)
    {
        Button levelButton = _levelsButtons[levelIndex];
        Image icon = null;
        Transform stateIcon = levelButton.transform.parent.Find("StateIcon");

        if (!stateIcon) return;
        
        icon = stateIcon.GetComponent<Image>();
        
        TextMeshProUGUI label = levelButton .GetComponentInChildren<TextMeshProUGUI>();

        State state = _levels[levelIndex];
        QuestionData.Subject subject = (levelIndex == _currentLevel) ? _actualSubject : QuestionData.Subject.Principal;

        switch (state)
        {
            case State.Approved:
                icon.sprite = approved;
                icon.color = Color.green;
                label.text = GetSubjectText(subject);
                break;

            case State.Blocked:
                icon.sprite = blocked;
                icon.color = Color.white;
                label.text = "???";
                break;

            case State.Able:
                icon.sprite = able;
                icon.color = Color.white;
                label.text = GetSubjectText(subject);
                break;
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
    
    private string GetSubjectText(QuestionData.Subject subject)
    {
        return subject switch
        {
            QuestionData.Subject.History => "Historia",
            QuestionData.Subject.Science => "Ciencia",
            QuestionData.Subject.Entertainment => "Entretenimiento",
            QuestionData.Subject.Geography => "Geografia",
            QuestionData.Subject.Principal => "Director",
            _ => "???"
        };
    }
    
    private void UpdateLevelsButtons()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            UpdateLevelButton(i);
        }
    }
    
    [Serializable]
    public class StatsRange
    {
        public int minAttack;
        public int maxAttack;
        public int minDefense;
        public int maxDefense;
    }
    #endregion

    #region Getters
    public QuestionData.Subject GetActualSubject() => _actualSubject;
    #endregion
}
