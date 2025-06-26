using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    
    [Header("Config")]
    public Vector3 enlargedBoardScale;
    public Vector3 newActionZonePosition;
    public Vector3 newActionZoneScale;
    
    [Header("UI Elements")]
    public TextMeshProUGUI phaseText;
    public Canvas winCanvas;
    public Canvas loseCanvas;
    public GameObject board;
    public GameObject actionZones;
    public GameObject questionModeBackground;
    public GameObject answersContainer;
    
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
    
    private Vector3 _boardOriginalScale;
    private Vector3 _actionZonesOriginalPosition;
    private Vector3 _actionZonesOriginalScale;

    private void Start()
    {
        _boardOriginalScale = board.transform.localScale;
        _actionZonesOriginalPosition = actionZones.transform.position;
        _actionZonesOriginalScale = actionZones.transform.localScale;
    }

    public void ResetVisuals()
    {
        NormalMode();
        winCanvas.gameObject.SetActive(false);
        loseCanvas.gameObject.SetActive(false);
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

    #region Utilities
    // Agranda el tablero y posiciona las zonas de acción
    public IEnumerator QuestionMode()
    {
        yield return new WaitForSeconds(1f);
        gameManager.GetTransitionManager().PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        board.transform.localScale = enlargedBoardScale;
        actionZones.transform.position = newActionZonePosition;
        actionZones.transform.localScale = newActionZoneScale;
        ToggleUIItems(true);
        
        gameManager.GetTransitionManager().PlayTransition("Paper", "TransitionOut");
        yield return new WaitForSeconds(1.5f);
    }

    // Vuelve al tamaño original y posiciona las zonas de acción
    public IEnumerator NormalMode()
    {
        gameManager.GetTransitionManager().PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(1f);
        
        board.transform.localScale = _boardOriginalScale;
        actionZones.transform.position = _actionZonesOriginalPosition;
        actionZones.transform.localScale = _actionZonesOriginalScale;
        ToggleUIItems(false);
        
        gameManager.GetTransitionManager().PlayTransition("Paper", "TransitionOut");
        yield return new WaitForSeconds(2f);
    }

    private void ToggleUIItems(bool toggle)
    {
        actionZones.transform.GetChild(2).gameObject.SetActive(!toggle); //Desactivo o activo la discard zone
        questionModeBackground.SetActive(toggle);
        timerText.gameObject.SetActive(toggle);
        answersContainer.gameObject.SetActive(toggle);
    }
    #endregion
}
