using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private Canvas boardCanvas;
    [SerializeField] private Canvas battleCanvas;
    [SerializeField] private Canvas gameCanvas;
    
    [Header("Level Decoration")]
    [SerializeField] private Image background;
    [SerializeField] private Image boardBackground;
    [SerializeField] private Image battleBackground;
    [SerializeField] private Image table;
    [SerializeField] private Image border;
    [SerializeField] private Sprite[] backgrounds;
    [SerializeField] private Sprite[] tables;
    
    [Header("Board")]
    [SerializeField] private TextMeshProUGUI boardAttackText;
    [SerializeField] private TextMeshProUGUI boardDefenseText;
    [SerializeField] private CardBoard[] boardCards;
    
    [Header("Question")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] answerButtons;
    
    private int _cantCardsPlaced;
    private Button _correctAnswerButton;
    
    private TransitionManager _transitionManager;

    private void Awake()
    {
        ActionZone.OnCardPlaced += UpdateBoardCards;
        QuestionManager.OnAnswer += ChangeAnswersColors;
        LevelsManager.OnSubjectChosen += ChangeLevelDecoration;
        Player.OnPlayerStatsChanged += UpdateBoardStats;
    }

    private void OnDestroy()
    {
        ActionZone.OnCardPlaced -= UpdateBoardCards;
        QuestionManager.OnAnswer -= ChangeAnswersColors;
        LevelsManager.OnSubjectChosen -= ChangeLevelDecoration;
        Player.OnPlayerStatsChanged -= UpdateBoardStats;
    }

    private void Start()
    {
        _transitionManager = GameManager.Instance.GetTransitionManager();
    }

    public void ResetVisuals()
    {
        ResetBoardCardsColor();
        ResetAnswersColors();
        _cantCardsPlaced = 0;
        gameCanvas.enabled = true;
        boardCanvas.enabled = false;
        battleCanvas.enabled = false;
    }

    public void UpdatePhaseText(PhaseManager.GamePhase phase)
    {
        switch (phase)
        {
            case PhaseManager.GamePhase.Discard:
                phaseText.text = "DESCARTE 2 CARTAS";
                break;
            case PhaseManager.GamePhase.Colocation:
                phaseText.text = "COLOQUE 3 CARTAS";
                break;
            case PhaseManager.GamePhase.Draw:
                phaseText.text = "ROBANDO CARTAS";
                break;
        }
    }

    private void UpdateBoardStats(int currentAttack, int currentDefense)
    {
        boardAttackText.text = currentAttack.ToString();
        boardDefenseText.text = currentDefense.ToString();
    }

    private void UpdateBoardCards(Sprite sprite, int cardValue, ActionZone.ZoneType cardType)
    {
        CardBoard card = boardCards[_cantCardsPlaced];
        
        card.ChangeImage(sprite);
        card.ChangeText(cardValue);
        card.ChangeCardType(cardType);
        
        _cantCardsPlaced++;
    }

    public void ShowQuestion(QuestionData selectedQuestion)
    {
        ToggleButtonsInteraction(true);
        
        questionText.text = selectedQuestion.GetQuestion();

        List<string> allOptions = new List<string>(selectedQuestion.GetOptions());
        allOptions.Add(selectedQuestion.GetCorrectAnswer());

        // Mezclar opciones
        for (int i = 0; i < allOptions.Count; i++)
        {
            string temp = allOptions[i];
            int randomIndex = Random.Range(i, allOptions.Count);
            allOptions[i] = allOptions[randomIndex];
            allOptions[randomIndex] = temp;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            TextMeshProUGUI answerText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            answerText.text = allOptions[i];
            
            if (allOptions[i] == selectedQuestion.GetCorrectAnswer())
            {
                _correctAnswerButton = answerButtons[i];
            }
        }
    }
    
    private void ChangeAnswersColors(bool playerAnswersCorrectly, Button selectedAnswer)
    {
        ToggleButtonsInteraction(false);
        
        ChangeButtonColor(_correctAnswerButton, CardBoard.CardColor.Green);
        if (!playerAnswersCorrectly)
            ChangeButtonColor(selectedAnswer, CardBoard.CardColor.Red);
    }

    private void ChangeLevelDecoration(QuestionData.Subject subject)
    {
        background.sprite = backgrounds[(int)subject];
        boardBackground.sprite = backgrounds[(int)subject];
        battleBackground.sprite = backgrounds[(int)subject];
        table.sprite = tables[(int)subject];
        if (subject == QuestionData.Subject.Principal)
            border.enabled = true;
        else
            border.enabled = false;
    }

    #region Modes
    // Entra en el modo preguntas, se hace la transición y se muestra el pizarrón agrandado.
    public IEnumerator QuestionMode()
    {
        yield return new WaitForSeconds(1f);
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(0.3f);
        
        GameManager.Instance.DeactivateGameObjects(true);
        boardCanvas.enabled = true;
        gameCanvas.enabled = false;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.GetAudioManager().PlayAudio(AudioManager.AudioList.PaperBreaking);
    }

    public IEnumerator BattleMode()
    {
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(0.3f);
        
        boardCanvas.enabled = false;
        battleCanvas.enabled = true;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.GetAudioManager().PlayAudio(AudioManager.AudioList.PaperBreaking);
        yield return new WaitForSeconds(2f);
    }

    // Vuelve al modo normal, se hace la transición y se muestra el pizarrón en su estado original.
    public IEnumerator NormalMode()
    {
        _transitionManager.PlayTransition("Paper", "TransitionIn");
        yield return new WaitForSeconds(0.3f);
        
        GameManager.Instance.DeactivateGameObjects(false);
        battleCanvas.enabled = false;
        gameCanvas.enabled = true;
        
        _transitionManager.PlayTransition("Paper", "TransitionOut");
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.GetAudioManager().PlayAudio(AudioManager.AudioList.PaperBreaking);
        yield return new WaitForSeconds(2f);
    }
    #endregion
    
    #region Utilities
    private void ResetBoardCardsColor()
    {
        foreach (CardBoard card in boardCards)
        {
            card.ChangeColor();
        }
    }
    
    private void ToggleButtonsInteraction(bool interactable)
    {
        foreach (Button button in answerButtons)
        {
            button.interactable = interactable;
        }
    }

    public void ResetAnswersColors()
    {
        foreach (Button button in answerButtons)
        {
            ChangeButtonColor(button);
        }
    }
    
    private void ChangeButtonColor(Button answerButton, CardBoard.CardColor color = CardBoard.CardColor.White)
    {
        TextMeshProUGUI answerText = answerButton.GetComponentInChildren<TextMeshProUGUI>();
        switch (color)
        {
            case CardBoard.CardColor.Green:
                answerButton.image.color = Color.green;
                answerText.color = Color.green;
                break;
            case CardBoard.CardColor.Red:
                answerButton.image.color = Color.red;
                answerText.color = Color.red;
                break;
            default:
                answerButton.image.color = Color.white;
                answerText.color = Color.white;
                break;
        }
    }

    #endregion

    #region Getters
    public CardBoard[] GetBoardCards() => boardCards;
    #endregion
}
