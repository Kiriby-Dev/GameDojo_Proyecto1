using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI phaseText;
    public Canvas winCanvas;
    public Canvas loseCanvas;
    
    [Header("Player Stats")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    
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

    public void GameOverCanvas(bool win)
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

    public void UpdatePlayerStats(int currentAttack, int currentDefense)
    {
        attackText.text = currentAttack.ToString();
        defenseText.text = currentDefense.ToString();
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
