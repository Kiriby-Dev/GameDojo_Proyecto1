using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

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
        Debug.Log("Fin del turno");
        gameManager.FinishTurn();
    }

    private IEnumerator GoThroughCards(ActionZone actionZone)
    {
        for (int i = 0; i < actionZone.GetCantCardsInZone(); i++)
        {
            GameObject card = actionZone.GetActualCard(i);
            if (card)
            {
                string difficulty = card.GetComponentInChildren<TextMeshProUGUI>().text;
                SelectQuestion(int.Parse(difficulty.Substring(1)));
                yield return new WaitUntil(() => PlayerSelectAnswer());
                _playerHasAnswered = false;
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public void OnAnswerSelected()
    {
        _playerHasAnswered = true;
        CheckAnswer();
    }

    public bool PlayerSelectAnswer()
    {
        return _playerHasAnswered;
    }

    private bool CheckAnswer()
    {
        return true;
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
        Questions selectedQuestion = null;
        int i = Random.Range(0, 4);
        switch (difficulty)
        {
            case 1: selectedQuestion = _easy[i];
                break;
            case 2: selectedQuestion = _medium[i];
                break;
            case 3: selectedQuestion = _hard[i];
                break;
            default: 
                break;
        }
        ShowQuestion(selectedQuestion);
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
