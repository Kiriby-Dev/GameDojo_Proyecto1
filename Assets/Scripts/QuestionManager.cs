using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI answerText1;
    public TextMeshProUGUI answerText2;
    public TextMeshProUGUI answerText3;
    public TextMeshProUGUI answerText4;
    
    [Header("GameObjects")]
    public GameManager gameManager;
    
    private Questions[] _hard;
    private Questions[] _medium;
    private Questions[] _easy;
    
    private bool _playerHasAnswered = false;
    private Questions _selectedQuestion = null;
    private bool _playerAnswersCorrectly;
    
    private void Awake()
    {
        SetQuestionsArrays();
        LoadQuestions();
    }

    public IEnumerator StartQuestions()
    {
        ActionZone attackZone = gameManager.GetAttackZone();
        ActionZone defenseZone = gameManager.GetDefenseZone();
        yield return StartCoroutine(GoThroughCards(attackZone));
        yield return StartCoroutine(GoThroughCards(defenseZone));
    }

    private IEnumerator GoThroughCards(ActionZone actionZone)
    {
        for (int i = 0; i < actionZone.GetCantCardsInZone(); i++)
        {
            GameObject card = actionZone.GetActualCardZone(i);
            if (card)
            {
                int difficulty = int.Parse(card.GetComponentInChildren<TextMeshProUGUI>().text.Substring(1));
                SelectQuestion(difficulty);
                yield return new WaitUntil(() => PlayerSelectAnswer());
                
                Card cardScript = card.GetComponentInChildren<Card>();
                if (cardScript)
                {
                    if (_playerAnswersCorrectly)
                    {
                        cardScript.ChangeColor(true);
                        AddStats(card, difficulty);
                    }
                    else
                        cardScript.ChangeColor(false);
                }

                _playerHasAnswered = false;
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private void AddStats(GameObject card, int value)
    {
        Player player = gameManager.GetPlayer();
        Transform zone = card.transform.parent;
        if (zone.name == "AttackZone")
            player.AddAttack(value);
        if (zone.name == "DefenseZone")
            player.AddDefense(value);
    }

    public void OnAnswerSelected(Button clickedButton)
    {
        string selectedText = clickedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        _playerAnswersCorrectly = CheckAnswer(selectedText);
        _playerHasAnswered = true;
    }

    public bool PlayerSelectAnswer()
    {
        return _playerHasAnswered;
    }

    private bool CheckAnswer(string selectedText)
    {
        return selectedText == _selectedQuestion.GetCorrectAnswer();
    }

    private void SetQuestionsArrays()
    {
        _easy = new Questions[4];
        _medium = new Questions[4];
        _hard = new Questions[4];

        for (int i = 0; i < 4; i++)
        {
            _easy[i] = gameObject.AddComponent<Questions>();
            _medium[i] = gameObject.AddComponent<Questions>();
            _hard[i] = gameObject.AddComponent<Questions>();
        }
    }

    public void SelectQuestion(int difficulty)
    {
        int i = Random.Range(0, 4);
        switch (difficulty)
        {
            case 1: _selectedQuestion = _easy[i];
                break;
            case 2: _selectedQuestion = _medium[i];
                break;
            case 3: _selectedQuestion = _hard[i];
                break;
            default: 
                break;
        }
        ShowQuestion(_selectedQuestion);
    }

    private void ShowQuestion(Questions selectedQuestion)
    {
        questionText.text = selectedQuestion.GetQuestion();
        answerText1.text = selectedQuestion.GetCorrectAnswer();
        answerText2.text = selectedQuestion.GetOptions()[0];
        answerText3.text = selectedQuestion.GetOptions()[1];
        answerText4.text = selectedQuestion.GetOptions()[2];
    }

    private void LoadQuestions()
    {
        _easy[0].SetData("¿De qué color es el cielo en un día despejado?", new string[]{"Rojo", "Verde", "Negro"}, "Azul");
        _easy[1].SetData("¿Cuánto es 5 + 3?", new string[]{"6", "7", "9"}, "8");
        _easy[2].SetData("¿Qué animal dice 'miau'?", new string[]{"Perro", "Vaca", "Gallina"}, "Gato");
        _easy[3].SetData("¿Qué día viene después del lunes?", new string[]{"Miércoles", "Jueves", "Viernes"}, "Martes");
        
        _medium[0].SetData("¿Quién pintó la Mona Lisa?", new string[]{"Picasso", "Van Gogh", "Rembrandt"}, "Leonardo da Vinci");
        _medium[1].SetData("¿Cuál es el país con mayor población del mundo?", new string[]{"Estados Unidos", "Brasil", "Indonesia"}, "China");
        _medium[2].SetData("¿Cuántos lados tiene un hexágono?", new string[]{"5", "7", "8"}, "6");
        _medium[3].SetData("¿En qué continente está Egipto?", new string[]{"Asia", "Europa", "Oceanía"}, "África");
        
        _hard[0].SetData("¿En qué año comenzó la Segunda Guerra Mundial?", new string[]{"1937", "1940", "1942"}, "1939");
        _hard[1].SetData("¿Qué elemento químico tiene el símbolo 'Fe'?", new string[]{"Flúor", "Francio", "Fermio"}, "Hierro");
        _hard[2].SetData("¿Cuál es la capital de Finlandia?", new string[]{"Oslo", "Copenhague", "Estocolmo"}, "Helsinki");
        _hard[3].SetData("¿Quién escribió 'La Odisea'?", new string[]{"Sófocles", "Virgilio", "Platón"}, "Homero");
    }
}
