using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestionManager : MonoBehaviour
{
    public GameManager gameManager;
    public Timer timer;
    
    private QuestionData[] allQuestions;
    private List<QuestionData> _historyEasy;
    private List<QuestionData> _historyMedium;
    private List<QuestionData> _historyHard;
    
    private List<QuestionData> _scienceEasy;
    private List<QuestionData> _scienceMedium;
    private List<QuestionData> _scienceHard;
    
    private List<QuestionData> _entertainmentEasy;
    private List<QuestionData> _entertainmentMedium;
    private List<QuestionData> _entertainmentHard;
    
    private List<QuestionData> _geographyEasy;
    private List<QuestionData> _geographyMedium;
    private List<QuestionData> _geographyHard;
    
    private QuestionData _selectedQuestion;
    private bool _playerHasAnswered;
    private bool _playerAnswersCorrectly;
    private bool _timeRanOut;
    private int _actualCardIndex;

    private void Start()
    {
        _historyEasy = new List<QuestionData>();
        _historyMedium = new List<QuestionData>();
        _historyHard = new List<QuestionData>();

        _scienceEasy = new List<QuestionData>();
        _scienceMedium = new List<QuestionData>();
        _scienceHard = new List<QuestionData>();

        _entertainmentEasy = new List<QuestionData>();
        _entertainmentMedium = new List<QuestionData>();
        _entertainmentHard = new List<QuestionData>();
        
        _geographyEasy = new List<QuestionData>();
        _geographyMedium = new List<QuestionData>();
        _geographyHard = new List<QuestionData>();

        LoadQuestions();
    }
    
    public IEnumerator StartQuestions()
    {
        _actualCardIndex = 0;
        yield return StartCoroutine(GoThroughCards()); //Espera a que termine la corrutina antes de seguir.
    }

    //Recorre todas las cartas jugadas en la fase de colocación.
    private IEnumerator GoThroughCards()
    {
        ActionZone cardsZone = gameManager.GetCardsZone();
        
        for (int i = 1; i < 6; i += 2)
        {
            GameObject cardGo = cardsZone.GetActualCardZone(i);
            yield return ProcessCard(cardGo);
            gameManager.GetUIManager().ToggleButtonsInteraction(true);
        }
    }

    #region ProcessCard

    /*Se procesa la carta, esto conlleva muchas cosas:
    - Selecciona una pregunta aleatoria de la dificultad asociada.
    - Indica que la carta está seleccionada (la pinta de amarillo).
    - Inicia el timer.
    - Espera a que el jugador responda o el tiempo se acabe.
    - Verifica la respuesta seleccionada.*/
    private IEnumerator ProcessCard(GameObject cardGo)
    {
        if (!cardGo) yield break;

        int difficulty = GetCardDifficulty(cardGo);
        Card cardScript = cardGo.GetComponentInChildren<Card>();

        SelectQuestion(difficulty);
        cardScript.ChangeColor(Card.CardColor.Yellow);

        _playerHasAnswered = false;
        _timeRanOut = false;

        timer.StartTimer();
        yield return new WaitUntil(() => _playerHasAnswered || _timeRanOut);

        HandleAnswerResult(cardScript, difficulty);

        yield return new WaitForSeconds(2f);
        
        gameManager.GetUIManager().ResetAnswerColor();
    }
    
    //Selecciona una pregunta dada una dificultad y la elimina de la lista correspondiente para no repetirla.
    private void SelectQuestion(int difficulty)
    {
        QuestionData.Subject subject = gameManager.GetLevelsManager().GetActualSubject();
        
        if (subject == QuestionData.Subject.Principal)
        {
            // Obtener todos los valores posibles del enum
            var values = Enum.GetValues(typeof(QuestionData.Subject)).Cast<QuestionData.Subject>().ToList();

            // Eliminar el valor 'Principal'
            values.Remove(QuestionData.Subject.Principal);

            // Elegir uno al azar
            subject = values[Random.Range(0, values.Count)];
        }
                
        List<QuestionData> listToUse = null;

        switch (subject)
        {
            case QuestionData.Subject.History:
                listToUse = GetListByDifficulty(_historyEasy, _historyMedium, _historyHard, difficulty);
                break;
            case QuestionData.Subject.Science:
                listToUse = GetListByDifficulty(_scienceEasy, _scienceMedium, _scienceHard, difficulty);
                break;
            case QuestionData.Subject.Entertainment:
                listToUse = GetListByDifficulty(_entertainmentEasy, _entertainmentMedium, _entertainmentHard, difficulty);
                break;
            case QuestionData.Subject.Geography:
                listToUse = GetListByDifficulty(_geographyEasy, _geographyMedium, _geographyHard, difficulty);
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

    private List<QuestionData> GetListByDifficulty(List<QuestionData> easy, List<QuestionData> medium, List<QuestionData> hard, int difficulty)
    {
        switch (difficulty)
        {
            case 1: return easy;
            case 2: return medium;
            case 3: return hard;
            default: return null;
        }
    }
    
    //Indica que el jugador selecciono una respuesta.
    public void OnAnswerSelected(Button clickedButton)
    {
        gameManager.GetUIManager().ToggleButtonsInteraction(false);
        string selectedText = clickedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        _playerAnswersCorrectly = CheckAnswer(selectedText);
        _playerHasAnswered = true;
        MarkSelectedAnswer(selectedText);
    }

    private void MarkSelectedAnswer(string selectedText)
    {
        if (!_playerAnswersCorrectly)
            gameManager.GetUIManager().ShowSelectedAnswer(selectedText);
        
        gameManager.GetUIManager().ShowCorrectAnswer(_selectedQuestion.GetCorrectAnswer());
    }

    //Verifica si la respuesta es correcta (pinta la carta de verde) o incorrecta (pinta la carta de rojo).
    private void HandleAnswerResult(Card card, int difficulty)
    {
        if (!card) return;

        if (_playerAnswersCorrectly)
        {
            card.ChangeColor(Card.CardColor.Green);
            ActionZone.ZoneType cardType = gameManager.GetCardType(_actualCardIndex);
            gameManager.GetPlayer().AddStats(cardType, difficulty);
            gameManager.GetAudioManager().PlayAudio(AudioManager.AudioList.RightAnswer);
        }
        else
        {
            card.ChangeColor(Card.CardColor.Red);
            gameManager.GetAudioManager().PlayAudio(AudioManager.AudioList.WrongAnswer);
        }

        _actualCardIndex++;
    }

    #endregion
    
    /*Toma todos los scriptable objects (preguntas) y los carga en la lista general.
    Luego recorre esta lista y la separa en 3 distintas: fácil, medio y difícil*/
    private void LoadQuestions()
    {
        allQuestions = Resources.LoadAll<QuestionData>("Questions");

        foreach (var q in allQuestions)
        {
            if (q.difficulty == QuestionData.Difficulty.Easy && q.subject == QuestionData.Subject.History)
                _historyEasy.Add(q);
            else if (q.difficulty == QuestionData.Difficulty.Medium && q.subject == QuestionData.Subject.History)
                _historyMedium.Add(q);
            else if (q.difficulty == QuestionData.Difficulty.Hard && q.subject == QuestionData.Subject.History)
                _historyHard.Add(q);

            else if (q.difficulty == QuestionData.Difficulty.Easy && q.subject == QuestionData.Subject.Science)
                _scienceEasy.Add(q);
            else if (q.difficulty == QuestionData.Difficulty.Medium && q.subject == QuestionData.Subject.Science)
                _scienceMedium.Add(q);
            else if (q.difficulty == QuestionData.Difficulty.Hard && q.subject == QuestionData.Subject.Science)
                _scienceHard.Add(q);

            else if (q.difficulty == QuestionData.Difficulty.Easy && q.subject == QuestionData.Subject.Entertainment)
                _entertainmentEasy.Add(q);
            else if (q.difficulty == QuestionData.Difficulty.Medium && q.subject == QuestionData.Subject.Entertainment)
                _entertainmentMedium.Add(q);
            else if (q.difficulty == QuestionData.Difficulty.Hard && q.subject == QuestionData.Subject.Entertainment)
                _entertainmentHard.Add(q);
            
            else if (q.difficulty == QuestionData.Difficulty.Easy && q.subject == QuestionData.Subject.Geography)
                _geographyEasy.Add(q);
            else if (q.difficulty == QuestionData.Difficulty.Medium && q.subject == QuestionData.Subject.Geography)
                _geographyMedium.Add(q);
            else if (q.difficulty == QuestionData.Difficulty.Hard && q.subject == QuestionData.Subject.Geography)
                _geographyHard.Add(q);
        }
    }

    #region Utilities

    private bool CheckAnswer(string selectedText)
    {
        return selectedText == _selectedQuestion.GetCorrectAnswer();
    }
    
    private int GetCardDifficulty(GameObject cardGo)
    {
        string numberText = cardGo.GetComponentInChildren<TextMeshProUGUI>().text.Substring(1);
        return int.Parse(numberText);
    }

    public bool PlayerHasAnswered() => _playerHasAnswered;
    public void PlayerAnswersCorrectly(bool correct) => _playerAnswersCorrectly = correct;
    public void TimeRanOut(bool ranOut) => _timeRanOut = ranOut;

    #endregion
}
