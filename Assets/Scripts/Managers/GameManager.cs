using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public QuestionManager questionManager;
    public PhaseManager phaseManager;
    public UIManager uiManager;
    public TransitionManager transitionManager;
    public MenuManager menuManager;
    
    [Header("Game Objects")]
    public GameObject playersHand;
    public GameObject card;
    public GameObject cardSlot;
    public Player player;
    public Enemy enemy;
    
    [Header("ActionZones")]
    public ActionZone discardZone;
    public ActionZone attackZone;
    public ActionZone defenseZone;
    public ActionZone cardsZone;
    
    private int _cantCardsInHand;
    private int _cardsPlayed;
    private bool _gameStarted;
    private bool _gameOver;
    private PlayersHand _playersHandScript;

    private void Awake()
    {
        _playersHandScript = playersHand.GetComponent<PlayersHand>();
    }

    private void Update()
    {
        if (!_gameStarted) return;
        CheckEndGame();
    }

    #region GameLoop
    public void StartGame()
    {
        _gameStarted = true;
        ResetVariables();
        phaseManager.StartPhases();
    }
    
    //Verificamos si alguno de los personajes murio y en ese caso terminamos el juego.
    private void CheckEndGame()
    {
        if (player.IsDead())
            GameOver(false);//Como murio el jugador termina el juego y perdemos
        if (enemy.IsDead())
            GameOver(true);//Como murio el enemigo termina el juego y ganamos
    }

    private void GameOver(bool win)
    {
        _gameOver = true;
        uiManager.UpdateGameOverCanvas(win);
    }

    public void ResetVariables()
    {
        player.ResetStats();
        enemy.GenerateStats();
        attackZone.ResetZone();
        defenseZone.ResetZone();
        uiManager.ResetVisuals();
        _cardsPlayed = 0;
    }
    #endregion

    #region Cards
    //Se instancian las 5 cartas con valores random en la mano del jugador.
    public void DrawCards()
    {
        for (int i = 0; i <= 4; i++)
        {
            GameObject go = Instantiate(card, playersHand.transform.GetChild(i));
            go.GetComponent<Card>().GenerateCardValue();
            go.name = "Card" + i;
            AddCardToHand();
        }
    }
    
    public GameObject GetActualCardForQuestion()
    {
        GameObject card = cardsZone.transform.GetChild(_cardsPlayed).gameObject;
        _cardsPlayed++;
        return card;
    }
    
    public int CantCardsInHand() => _cantCardsInHand;

    public void RemoveCardFromHand()
    {
        _cantCardsInHand--;
        _playersHandScript.Recalculate();
    }
    private void AddCardToHand() => _cantCardsInHand++;
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
    
    //Se resuelve la fase de combate haciendo daño a los personajes con los valores generados anteriormente.
    public void ResolveCombat()
    {
        int damageDealed = enemy.GetDefense() - player.GetAttack();
        int damageTaken = player.GetDefense() - enemy.GetAttack();
        
        if (damageTaken < 0)
            player.TakeDamage(damageTaken);
        if (damageDealed < 0)
            enemy.TakeDamage(damageDealed);
    }
    #endregion
    
    #region Getters
    public PhaseManager GetPhaseManager() => phaseManager;
    public QuestionManager GetQuestionManager() => questionManager;
    public UIManager GetUIManager() => uiManager;
    public TransitionManager GetTransitionManager() => transitionManager;
    public PlayersHand GetPlayersHand() => _playersHandScript;
    public ActionZone GetAttackZone() => attackZone;
    public ActionZone GetDefenseZone() => defenseZone;
    public ActionZone GetCardsZone() => cardsZone;
    public ActionZone GetDiscardZone() => discardZone;
    public Player GetPlayer() => player;
    public bool IsGameOver() => _gameOver;
    #endregion
}
