using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    
    [Header("UI Elements")]
    public TextMeshProUGUI phaseText;
    public Canvas winCanvas;
    public Canvas loseCanvas;
    public Canvas boardCanvas;
    public Canvas gameCanvas;
    
    [Header("Player Stats On Board")]
    public TextMeshProUGUI playerAttackText;
    public TextMeshProUGUI playerDefenseText;
    
    [Header("Player Stats")]
    public TextMeshProUGUI playerAttack;
    public TextMeshProUGUI playerDefense;
    
    [Header("Enemy Stats")]
    public TextMeshProUGUI enemyAttackText;
    public TextMeshProUGUI enemyDefenseText;
    
    [Header("Questions")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI answerText1;
    public TextMeshProUGUI answerText2;
    public TextMeshProUGUI answerText3;
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
        playerAttack.text = "0";
        playerDefense.text = "0";
        questionText.text = "";
        answerText1.text = "";
        answerText2.text = "";
        answerText3.text = "";
        answerText4.text = "";
    }

    public void UpdatePhaseText(string phase)
    {
        phaseText.text = phase;
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

    #region Utilities
    // Entra en el modo preguntas, se hace la transición y se muestra el pizarrón agrandado.
    public IEnumerator QuestionMode()
    {
        yield return new WaitForSeconds(1f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        ToggleUIItems(true);
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
        yield return new WaitForSeconds(1.5f);
    }

    // Vuelve al modo normal, se hace la transición y se muestra el pizarrón en su estado original.
    public IEnumerator NormalMode()
    {
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        ToggleUIItems(false);
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
    #endregion
}
