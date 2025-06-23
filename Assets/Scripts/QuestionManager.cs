using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestionManager : MonoBehaviour
{
    public GameManager gameManager;
    public int cantQuestions;
    public int timePerQuestion;
    public QuestionData[] allQuestions;

    private List<QuestionData> _easy;
    private List<QuestionData> _medium;
    private List<QuestionData> _hard;

    private bool _playerHasAnswered = false;
    private QuestionData _selectedQuestion = null;

    private QuestionData _emptyQuestion;
    private Coroutine _countdownCoroutine;
    private bool _playerAnswersCorrectly;
    private bool _timeRanOut = false;

    private void Start()
    {
        _easy = new List<QuestionData>();
        _medium = new List<QuestionData>();
        _hard = new List<QuestionData>();
        LoadQuestions();
    }

    public IEnumerator StartQuestions()
    {
        ActionZone attackZone = gameManager.GetAttackZone();
        ActionZone defenseZone = gameManager.GetDefenseZone();
        yield return StartCoroutine(GoThroughCards(attackZone));
        yield return StartCoroutine(GoThroughCards(defenseZone));
    }

    private IEnumerator GoThroughCards(ActionZone zone)
    {
        int count = zone.GetCantCardsInZone();
        for (int i = 0; i < count; i++)
        {
            GameObject cardGO = zone.GetActualCardZone(i);
            yield return ProcessCard(cardGO);
        }
    }

    private IEnumerator ProcessCard(GameObject cardGO)
    {
        if (!cardGO) yield break;

        int difficulty = GetCardDifficulty(cardGO);
        Card cardScript = cardGO.GetComponentInChildren<Card>();

        SelectQuestion(difficulty);
        cardScript.ChangeColor(Card.CardColor.Yellow);

        _playerHasAnswered = false;
        _timeRanOut = false;

        StartCoroutine(CountdownTimer(timePerQuestion));
        yield return new WaitUntil(() => _playerHasAnswered || _timeRanOut);

        HandleAnswerResult(cardScript, cardGO, difficulty);

        yield return new WaitForSeconds(1f);
    }

    private int GetCardDifficulty(GameObject cardGO)
    {
        string numberText = cardGO.GetComponentInChildren<TextMeshProUGUI>().text.Substring(1);
        return int.Parse(numberText);
    }

    private void HandleAnswerResult(Card card, GameObject cardGO, int difficulty)
    {
        if (!card) return;

        if (_playerAnswersCorrectly)
        {
            card.ChangeColor(Card.CardColor.Green);
            gameManager.GetPlayer().AddStats(cardGO, difficulty);
        }
        else
        {
            card.ChangeColor(Card.CardColor.Red);
        }
    }

    public void OnAnswerSelected(Button clickedButton)
    {
        string selectedText = clickedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        _playerAnswersCorrectly = CheckAnswer(selectedText);
        _playerHasAnswered = true;
    }

    private bool CheckAnswer(string selectedText)
    {
        return selectedText == _selectedQuestion.GetCorrectAnswer();
    }

    private IEnumerator CountdownTimer(float duration)
    {
        float timeLeft = duration;

        while (timeLeft > 0f && !_playerHasAnswered)
        {
            gameManager.GetUIManager().UpdateTimer(Mathf.CeilToInt(timeLeft).ToString());
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        if (!_playerHasAnswered)
        {
            _timeRanOut = true;
            _playerAnswersCorrectly = false;
        }

        gameManager.GetUIManager().UpdateTimer("");
    }

    private void LoadQuestions()
    {
        allQuestions = Resources.LoadAll<QuestionData>("Questions");

        foreach (var q in allQuestions)
        {
            switch (q.difficulty)
            {
                case QuestionData.Difficulty.Easy:
                    _easy.Add(q);
                    break;
                case QuestionData.Difficulty.Medium:
                    _medium.Add(q);
                    break;
                case QuestionData.Difficulty.Hard:
                    _hard.Add(q);
                    break;
            }
        }
    }

    private void SelectQuestion(int difficulty)
    {
        List<QuestionData> listToUse = null;

        switch (difficulty)
        {
            case 1:
                listToUse = _easy;
                break;
            case 2:
                listToUse = _medium;
                break;
            case 3:
                listToUse = _hard;
                break;
        }

        if (listToUse != null && listToUse.Count > 0)
        {
            int i = Random.Range(0, listToUse.Count);
            _selectedQuestion = listToUse[i];
            listToUse.RemoveAt(i);
        }

        gameManager.GetUIManager().ShowQuestion(_selectedQuestion);
    }
}
