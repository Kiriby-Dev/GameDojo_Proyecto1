using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestionManager : MonoBehaviour
{
    public GameManager gameManager;
    public int timePerQuestion;
    
    private QuestionData[] allQuestions;
    private List<QuestionData> _easy;
    private List<QuestionData> _medium;
    private List<QuestionData> _hard;
    
    private QuestionData _selectedQuestion = null;
    private bool _playerHasAnswered = false;
    private bool _playerAnswersCorrectly;
    private bool _timeRanOut = false;

    private void Start()
    {
        _easy = new List<QuestionData>();
        _medium = new List<QuestionData>();
        _hard = new List<QuestionData>();
        LoadQuestions();
    }

    //Recorro todas las cartas de la zona de ataque y cuando termino recien ahi recorro las de la zona de defensa.
    public IEnumerator StartQuestions()
    {
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
    - Indica que la carta esta seleccionada (la pinta de amarillo).
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

        StartCoroutine(Timer(timePerQuestion));
        yield return new WaitUntil(() => _playerHasAnswered || _timeRanOut);

        HandleAnswerResult(cardScript, cardGo, difficulty);

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
    
    private IEnumerator Timer(float duration)
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
    
    //Indica que el jugador selecciono una respuesta.
    public void OnAnswerSelected(Button clickedButton)
    {
        string selectedText = clickedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        _playerAnswersCorrectly = CheckAnswer(selectedText);
        _playerHasAnswered = true;
    }

    //Verifica si la respuesta es correcta (pinta la carta de verde) o incorrecta (pinta la carta de rojo).
    private void HandleAnswerResult(Card card, GameObject cardGo, int difficulty)
    {
        if (!card) return;

        if (_playerAnswersCorrectly)
        {
            card.ChangeColor(Card.CardColor.Green);
            gameManager.GetPlayer().AddStats(cardGo, difficulty);
        }
        else
        {
            card.ChangeColor(Card.CardColor.Red);
        }
    }

    #endregion
    
    /*Toma todos los scriptable objects (preguntas) y los carga en la lista general.
    Luego recorre esta lista y la separa en 3 distintas: facil, medio y dificil*/
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

    #endregion
}
