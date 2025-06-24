using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI phaseText;
    public Canvas winCanvas;
    public Canvas loseCanvas;
    
    [Header("Player Stats")]
    public TextMeshProUGUI playerAttackText;
    public TextMeshProUGUI playerDefenseText;
    
    [Header("Enemy Stats")]
    public TextMeshProUGUI enemyAttackText;
    public TextMeshProUGUI enemyDefenseText;
    
    [Header("Questions")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI answerText1;
    public TextMeshProUGUI answerText2;
    public TextMeshProUGUI answerText3;
    public TextMeshProUGUI answerText4;
    public TextMeshProUGUI timerText;

    public void ResetVisuals()
    {
        winCanvas.gameObject.SetActive(false);
        loseCanvas.gameObject.SetActive(false);
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
    
    public void UpdateTimer(string timeText)
    {
        timerText.text = timeText;
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
}
