using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    
    [Header("UI Elements")]
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI discardText;
    public Canvas winCanvas;
    public Canvas loseCanvas;
    public Canvas boardCanvas;
    public Canvas battleCanvas;
    public Canvas gameCanvas;
    
    [Header("Player Stats On Board")]
    public TextMeshProUGUI playerAttackText;
    public TextMeshProUGUI playerDefenseText;
    
    [Header("Player Stats")]
    public TextMeshProUGUI playerAttack;
    public TextMeshProUGUI playerDefense;
    public TextMeshProUGUI playerFightAttackText;
    public TextMeshProUGUI playerFightDefenseText;
    
    [Header("Enemy Stats")]
    public TextMeshProUGUI enemyAttackText;
    public TextMeshProUGUI enemyDefenseText;
    public TextMeshProUGUI enemyFightAttackText;
    public TextMeshProUGUI enemyFightDefenseText;
    
    [Header("Question")]
    public TextMeshProUGUI questionText;
    public Button answerButton1;
    public TextMeshProUGUI answerText1;
    public Button answerButton2;
    public TextMeshProUGUI answerText2;
    public Button answerButton3;
    public TextMeshProUGUI answerText3;
    public Button answerButton4;
    public TextMeshProUGUI answerText4;
    
    private TransitionManager _transitionManager;

    private void Start()
    {
        _transitionManager = gameManager.GetTransitionManager();
    }

    public void ResetVisuals()
    {
        ResetBoardCardsColor();
        winCanvas.gameObject.SetActive(false);
        loseCanvas.gameObject.SetActive(false);
        boardCanvas.gameObject.SetActive(false);
        battleCanvas.gameObject.SetActive(false);
        playerAttack.text = "0";
        playerDefense.text = "0";
        questionText.text = "";
        answerText1.text = "";
        answerText2.text = "";
        answerText3.text = "";
        answerText4.text = "";
    }

    public void UpdatePhaseText(PhaseManager.GamePhase phase)
    {
        switch (phase)
        {
            case PhaseManager.GamePhase.Discard:
                phaseText.text = "DESCARTE";
                break;
            case PhaseManager.GamePhase.Colocation:
                phaseText.text = "COLOCACIÓN";
                break;
            case PhaseManager.GamePhase.Draw:
                phaseText.text = "ROBANDO";
                break;
            default:
                break;
        }
    }

    public void UpdateDiscardText(int value)
    {
        discardText.text = value + "/" + "2";
    }

    public void UpdateGameOverCanvas(bool win)
    {
        if (win)
            winCanvas.gameObject.SetActive(true);
        else
            loseCanvas.gameObject.SetActive(true);
    }

    public void UpdateStats(int currentAttack, int currentDefense, string character)
    {
        switch (character)
        {
            case "Player":
                playerAttackText.text = currentAttack.ToString();
                playerDefenseText.text = currentDefense.ToString();
                break;
            case "Enemy":
                enemyAttackText.text = currentAttack.ToString();
                enemyDefenseText.text = currentDefense.ToString();
                break;
        }
    }

    public void ShowQuestion(QuestionData selectedQuestion)
    {
        questionText.text = selectedQuestion.GetQuestion();

        List<string> allOptions = new List<string>(selectedQuestion.GetOptions());
        allOptions.Add(selectedQuestion.GetCorrectAnswer());
        
        for (int i = 0; i < allOptions.Count; i++)
        {
            string temp = allOptions[i];
            int randomIndex = Random.Range(i, allOptions.Count);
            allOptions[i] = allOptions[randomIndex];
            allOptions[randomIndex] = temp;
        }
        
        answerText1.text = allOptions[0];
        answerText2.text = allOptions[1];
        answerText3.text = allOptions[2];
        answerText4.text = allOptions[3];
    }
    
    //estas 3 funciones hay que cambiarlas para poner los valores en un arreglo y recorrerlo
    public void ShowCorrectAnswer(string correctAnswer)
    {
        if (correctAnswer == answerText1.text)
            ChangeAnswerColor(answerText1, answerButton1, "Green");
        if (correctAnswer == answerText2.text)
            ChangeAnswerColor(answerText2, answerButton2, "Green");
        if (correctAnswer == answerText3.text)
            ChangeAnswerColor(answerText3, answerButton3, "Green");
        if (correctAnswer == answerText4.text)
            ChangeAnswerColor(answerText4, answerButton4, "Green");
    }

    public void ShowSelectedAnswer(string selectedText)
    {
        if (selectedText == answerText1.text)
            ChangeAnswerColor(answerText1, answerButton1, "Red");
        if (selectedText == answerText2.text)
            ChangeAnswerColor(answerText2, answerButton2, "Red");
        if (selectedText == answerText3.text)
            ChangeAnswerColor(answerText3, answerButton3, "Red");
        if (selectedText == answerText4.text)
            ChangeAnswerColor(answerText4, answerButton4, "Red");
    }

    public void ResetAnswerColor()
    {
        ChangeAnswerColor(answerText1, answerButton1, "");
        ChangeAnswerColor(answerText2, answerButton2, "");
        ChangeAnswerColor(answerText3, answerButton3, "");
        ChangeAnswerColor(answerText4, answerButton4, "");
    }

    #region Utilities
    // Entra en el modo preguntas, se hace la transición y se muestra el pizarrón agrandado.
    public IEnumerator QuestionMode()
    {
        yield return new WaitForSeconds(1f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        ToggleUIItems(true);
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
    }

    public IEnumerator BattleMode()
    {
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        boardCanvas.gameObject.SetActive(false);
        battleCanvas.gameObject.SetActive(true);
        playerFightAttackText.text = playerAttackText.text;
        playerFightDefenseText.text = playerDefenseText.text;
        enemyFightAttackText.text = enemyAttackText.text;
        enemyFightDefenseText.text = enemyDefenseText.text;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
        yield return new WaitForSeconds(2f);
    }

    // Vuelve al modo normal, se hace la transición y se muestra el pizarrón en su estado original.
    public IEnumerator NormalMode()
    {
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        ToggleUIItems(false);
        battleCanvas.gameObject.SetActive(false);
        playerAttack.text = playerAttackText.text;
        playerDefense.text = playerDefenseText.text;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
        yield return new WaitForSeconds(2f);
    }

    private void ToggleUIItems(bool toggle)
    {
        gameManager.GetDiscardZone().gameObject.SetActive(!toggle); //Desactivo o activo la discard zone
        gameManager.GetAttackZone().gameObject.SetActive(!toggle);
        gameManager.GetDefenseZone().gameObject.SetActive(!toggle);
        gameCanvas.gameObject.SetActive(!toggle);
        boardCanvas.gameObject.SetActive(toggle);
    }
    
    private void ResetBoardCardsColor()
    {
        foreach (Card card in gameManager.GetCardsZone().gameObject.GetComponentsInChildren<Card>())
        {
            card.ChangeColor();
        }
    }

    private void ChangeAnswerColor(TextMeshProUGUI answerText, Button answerButton, string color)
    {
        switch (color)
        {
            case "Green":
                answerText.color = Color.green;
                answerButton.image.color = Color.green;
                break;
            case "Red":
                answerText.color = Color.red;
                answerButton.image.color = Color.red;
                break;
            default:
                answerText.color = Color.white;
                answerButton.image.color = Color.white;
                break;
        }
    }

    #endregion
}
