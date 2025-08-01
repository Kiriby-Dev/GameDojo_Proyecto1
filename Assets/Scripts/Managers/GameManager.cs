using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private QuestionManager questionManager;
    [SerializeField] private PhaseManager phaseManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TransitionManager transitionManager;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private LevelsManager levelsManager;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private GameFlowManager gameFlowManager;
    
    [Header("Game Objects")]
    [SerializeField] private GameObject playersHand;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    
    
    [Header("ActionZones")]
    [SerializeField] private ActionZone discardZone;
    [SerializeField] private ActionZone attackZone;
    [SerializeField] private ActionZone defenseZone;
    [SerializeField] private ActionZone cardsZone;
    
    
    private int _actualBoardcard;
   
    private PlayersHand _playersHandScript;
    private ActionZone.ZoneType[] _cardTypes;
    private Player _playerScript;
    private Enemy _enemyScript;
    private int _cardTypesIndex = 0;
    
    private bool _turnEnded = false;

    private void Awake()
    {
        _playersHandScript = playersHand.GetComponent<PlayersHand>();
        _playerScript = player.GetComponent<Player>();
        _enemyScript = enemy.GetComponent<Enemy>();
    }

    private void Start()
    {
        _cardTypes = new ActionZone.ZoneType[3];
    }
    
    #region GameLoop
    public void StartGame()
    {
        ResetVariables();
    }

    public void ResetVariables()
    {
        ResetCardTypes();
        _actualBoardcard = 1;
    }

    public void EndGame()
    {
        phaseManager.CurrentPhase = PhaseManager.GamePhase.Draw;
        ResetVariables();
        //_cantCardsInHand = 0;
        _enemyScript.ResetLife();
        //_enemyFightScript.ResetLife();
        
    }

    public void EndTurn()
    {
        attackZone.ResetZone();
        defenseZone.ResetZone();
        uiManager.ResetVisuals();
    }

    #endregion

    #region Cards
    public GameObject GetActualCardForQuestion()
    {
        GameObject card = cardsZone.transform.GetChild(_actualBoardcard).gameObject;
        _actualBoardcard += 2;
        return card;
    }
    
    #endregion

    #region  Utilities
    //Se habilitan y deshabilitan las zonas de accion dependiendo la fase en la que estamos.
    public void UpdateZones(string gamePhase)
    {
        bool active = (gamePhase == "Discard");
        
        discardZone.enabled = active;
        attackZone.enabled = !active;
        defenseZone.enabled = !active;
    }
    
    public void SaveCardType(ActionZone.ZoneType type)
    {
        _cardTypes[_cardTypesIndex] = type;
        _cardTypesIndex++;
    }

    public ActionZone.ZoneType GetCardType(int index)
    {
        return _cardTypes[index];
    }

    private void ResetCardTypes()
    {
        _cardTypesIndex = 0;
    }
    #endregion
    
    #region Getters
    public PhaseManager GetPhaseManager() => phaseManager;
    public QuestionManager GetQuestionManager() => questionManager;
    public UIManager GetUIManager() => uiManager;
    public TransitionManager GetTransitionManager() => transitionManager;
    public AudioManager GetAudioManager() => audioManager;
    public MenuManager GetMenuManager() => menuManager;
    public LevelsManager GetLevelsManager() => levelsManager;
    public CombatManager GetCombatManager() => combatManager;
    public GameFlowManager GetGameFlowManager() => gameFlowManager;
    public PlayersHand GetPlayersHand() => _playersHandScript;
    public ActionZone GetAttackZone() => attackZone;
    public ActionZone GetDefenseZone() => defenseZone;
    public ActionZone GetCardsZone() => cardsZone;
    public ActionZone GetDiscardZone() => discardZone;
    public Player GetPlayer() => _playerScript;
    public Enemy GetEnemy() => _enemyScript;
    #endregion
}
