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
    
    private PlayersHand _playersHandScript;
    private Player _playerScript;
    private Enemy _enemyScript;

    private void Awake()
    {
        GameFlowManager.OnLevelStart += StartGame;
        GameFlowManager.OnLevelOver += EndGame;
        
        _playersHandScript = playersHand.GetComponent<PlayersHand>();
        _playerScript = player.GetComponent<Player>();
        _enemyScript = enemy.GetComponent<Enemy>();
    }

    private void OnDestroy()
    {
        GameFlowManager.OnLevelStart -= StartGame;
        GameFlowManager.OnLevelOver -= EndGame;
    }

    #region LevelLoop
    private void StartGame()
    {
        _enemyScript.GenerateStats();
        uiManager.ResetVisuals();
    }
    
    private void EndGame()
    {
        _enemyScript.ResetLife();
        menuManager.MenuLevelsButton();
        levelsManager.AdvanceLevel();
    }

    public void EndTurn()
    {
        attackZone.ResetZone();
        defenseZone.ResetZone();
        _playerScript.ResetStats();
        _enemyScript.GenerateStats();
        uiManager.ResetVisuals();
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

    public void DeactivateGameObjects(bool toggle)
    {
        discardZone.gameObject.SetActive(!toggle);
        attackZone.gameObject.SetActive(!toggle);
        defenseZone.gameObject.SetActive(!toggle);
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
    public ActionZone GetDiscardZone() => discardZone;
    public Player GetPlayer() => _playerScript;
    public Enemy GetEnemy() => _enemyScript;
    #endregion
}
