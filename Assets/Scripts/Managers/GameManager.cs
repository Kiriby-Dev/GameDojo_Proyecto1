using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public QuestionManager questionManager;
    public PhaseManager phaseManager;
    public UIManager uiManager;
    public TransitionManager transitionManager;
    
    [Header("Game Objects")]
    public GameObject playersHand;
    public GameObject card;
    public Player player;
    public Enemy enemy;
    
    [Header("ActionZones")]
    public ActionZone discardZone;
    public ActionZone attackZone;
    public ActionZone defenseZone;
    public ActionZone cardsZone;
    
    private bool _gameOver;
    private PlayersHand _playersHandScript;
    private Card _cardScript;
    private int _cantCardsInHand;
    private int _cardsPlayed;
    private bool _gameStarted;

    private void Awake()
    {
        _playersHandScript = playersHand.GetComponent<PlayersHand>();
        _cardScript = card.GetComponent<Card>();
    }

    private void Update()
    {
        if (!_gameStarted) return;
        CheckEndGame();
    }

    public void StartGame()
    {
        _gameStarted = true;
        phaseManager.StartPhases();
    }

    //Se habilitan y deshabilitan las zonas de accion dependiendo la fase en la que estamos.
    public void UpdateZones(string gamePhase)
    {
        bool active = (gamePhase == "Discard");
        
        discardZone.enabled = active;
        attackZone.enabled = !active;
        defenseZone.enabled = !active;
    }

    //Se instancian las 5 cartas con valores random en la mano del jugador.
    public void DrawCards()
    {
        for (int i = 1; i <= 5; i++)
        {
            GameObject go = Instantiate(card, playersHand.transform);
            go.GetComponent<Card>().GenerateCardValue();
            go.name = "Card" + i;
            AddCardToHand();
        }
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

    public int CantCardsInHandObject()
    {
        return playersHand.transform.childCount;
    }

    public int CantCardsInHand() => _cantCardsInHand;
    public void RemoveCardFromHand() => _cantCardsInHand--;
    public void AddCardToHand() => _cantCardsInHand++;

    public GameObject GetActualCardFromBoard()
    {
        GameObject card = cardsZone.transform.GetChild(_cardsPlayed).gameObject;
        _cardsPlayed++;
        return card;
    }

    public bool GameStarted() => _gameStarted;

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
    public Card GetCard() => _cardScript;
    #endregion
}
