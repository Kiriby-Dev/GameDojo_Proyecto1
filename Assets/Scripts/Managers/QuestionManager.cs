using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestionManager : MonoBehaviour
{
    public GameManager gameManager;
    public Timer timer;
    
    private QuestionData[] allQuestions;
    private List<QuestionData> _easy;
    private List<QuestionData> _medium;
    private List<QuestionData> _hard;
    
    private QuestionData _selectedQuestion;
    private bool _playerHasAnswered;
    private bool _playerAnswersCorrectly;
    private bool _timeRanOut;
    private int _actualCardIndex;

    private void Start()
    {
        _easy = new List<QuestionData>();
        _medium = new List<QuestionData>();
        _hard = new List<QuestionData>();
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
        
        for (int i = 0; i < 3; i++)
        {
            GameObject cardGo = cardsZone.GetActualCardZone(i);
            yield return ProcessCard(cardGo);
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

        yield return new WaitForSeconds(1f);
    }
    
    //Selecciona una pregunta dada una dificultad y la elimina de la lista correspondiente para no repetirla.
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
    
    //Indica que el jugador selecciono una respuesta.
    public void OnAnswerSelected(Button clickedButton)
    {
        string selectedText = clickedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        _playerAnswersCorrectly = CheckAnswer(selectedText);
        _playerHasAnswered = true;
    }

    //Verifica si la respuesta es correcta (pinta la carta de verde) o incorrecta (pinta la carta de rojo).
    private void HandleAnswerResult(Card card, int difficulty)
    {
        if (!card) return;

        if (_playerAnswersCorrectly)
        {
            card.ChangeColor(Card.CardColor.Green);
            ActionZone.ZoneType cardType = gameManager.GetCardsZone().GetCardType(_actualCardIndex);
            print(_actualCardIndex);
            print(cardType.ToString());
            gameManager.GetPlayer().AddStats(cardType, difficulty);
        }
        else
        {
            card.ChangeColor(Card.CardColor.Red);
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
