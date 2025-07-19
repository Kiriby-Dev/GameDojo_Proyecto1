using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public QuestionManager questionManager;
    public PhaseManager phaseManager;
    public UIManager uiManager;
    public TransitionManager transitionManager;
    public MenuManager menuManager;
    public AudioManager audioManager;
    
    [Header("Game Objects")]
    public GameObject playersHand;
    public GameObject card;
    public GameObject player;
    public GameObject enemy;
    public GameObject playerFight;
    public GameObject enemyFight;
    
    [Header("ActionZones")]
    public ActionZone discardZone;
    public ActionZone attackZone;
    public ActionZone defenseZone;
    public ActionZone cardsZone;
    
    private int _cantCardsInHand;
    private int _actualBoardcard;
    private int _cardsPlayed;
    private bool _gameStarted;
    private bool _gameOver;
    private PlayersHand _playersHandScript;
    private ActionZone.ZoneType[] _cardTypes;
    private Player _playerScript;
    private Enemy _enemyScript;
    private int _cardTypesIndex = 0;
    private Animator _playerFightAnimator;
    private Animator _enemyFightAnimator;

    private void Awake()
    {
        _playersHandScript = playersHand.GetComponent<PlayersHand>();
        _playerScript = player.GetComponent<Player>();
        _enemyScript = enemy.GetComponent<Enemy>();
        _playerFightAnimator = playerFight.GetComponent<Animator>();
        _enemyFightAnimator = enemyFight.GetComponent<Animator>();
    }

    private void Start()
    {
        _cardTypes = new ActionZone.ZoneType[3];
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
        if (_playerScript.IsDead())
            GameOver(false);//Como murio el jugador termina el juego y perdemos
        if (_enemyScript.IsDead())
            GameOver(true);//Como murio el enemigo termina el juego y ganamos
    }

    private void GameOver(bool win)
    {
        _gameOver = true;
        uiManager.UpdateGameOverCanvas(win);
    }

    public void ResetVariables()
    {
        _playerScript.ResetStats();
        _enemyScript.GenerateStats();
        attackZone.ResetZone();
        defenseZone.ResetZone();
        discardZone.ResetDiscardedCards();
        uiManager.ResetVisuals();
        ResetCardTypes();
        _actualBoardcard = 1;
    }
    #endregion

    #region Cards
    //Se instancian las 5 cartas con valores random en la mano del jugador.
    public void DrawCards()
    {
        _playersHandScript.EnableAllSlots();
        for (int i = 0; i <= 4; i++)
        {
            GameObject go = Instantiate(card, playersHand.transform.GetChild(i));
            Card actualCard = go.GetComponent<Card>();
            actualCard.GenerateCardValue();
            actualCard.ToggleAnimator(true);
            go.name = "Card" + i;
            AddCardToHand();
        }

        StartCoroutine(MovePlayersHand(new Vector3(0, -3.3f, 0)));
    }

    private IEnumerator MovePlayersHand(Vector3 targetPosition, float duration = 0.5f)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = playersHand.transform.position;

        while (elapsedTime < duration)
        {
            playersHand.transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playersHand.transform.position = targetPosition; // Asegura que termina exactamente en destino
    }

    public GameObject GetActualCardForQuestion()
    {
        GameObject card = cardsZone.transform.GetChild(_actualBoardcard).gameObject;
        _actualBoardcard += 2;
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
    
    //Se resuelve la fase de combate haciendo daño a los personajes con los valores generados anteriormente.
    public IEnumerator ResolveCombat()
    {
        int damageDealed = _enemyScript.GetDefense() - _playerScript.GetAttack();
        int damageTaken = _playerScript.GetDefense() - _enemyScript.GetAttack();
        
        _playerFightAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.35f);
        if (damageDealed < 0)
        {
            _enemyScript.TakeDamage(damageDealed);
            enemyFight.GetComponent<Enemy>().TakeDamage(damageDealed);
            _enemyFightAnimator.SetTrigger("Hurt");
        }
        else
        {
            _enemyFightAnimator.SetTrigger("BlockAttack");
        }
        yield return new WaitForSeconds(2f);
        
        _enemyFightAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.35f);
        if (damageTaken < 0)
        {
            _playerScript.TakeDamage(damageTaken);
            playerFight.GetComponent<Player>().TakeDamage(damageTaken);
            _playerFightAnimator.SetTrigger("Hurt");
        }
        else
        {
            _playerFightAnimator.SetTrigger("BlockAttack");
        }
        yield return new WaitForSeconds(1f);
    }

    public void PlayCard() => _cardsPlayed++;
    public int GetCantPlayedCards() => _cardsPlayed;
    #endregion
    
    #region Getters
    public PhaseManager GetPhaseManager() => phaseManager;
    public QuestionManager GetQuestionManager() => questionManager;
    public UIManager GetUIManager() => uiManager;
    public TransitionManager GetTransitionManager() => transitionManager;
    public AudioManager GetAudioManager() => audioManager;
    public PlayersHand GetPlayersHand() => _playersHandScript;
    public ActionZone GetAttackZone() => attackZone;
    public ActionZone GetDefenseZone() => defenseZone;
    public ActionZone GetCardsZone() => cardsZone;
    public ActionZone GetDiscardZone() => discardZone;
    public Player GetPlayer() => _playerScript;
    public bool IsGameOver() => _gameOver;
    #endregion
}
