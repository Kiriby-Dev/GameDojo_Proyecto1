using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public QuestionManager questionManager;
    public PhaseManager phaseManager;
    public UIManager uiManager;
    
    [Header("Game Objects")]
    public GameObject playersHand;
    public GameObject card;
    public Player player;
    public Enemy enemy;
    
    [Header("ActionZones")]
    public ActionZone discardZone;
    public ActionZone attackZone;
    public ActionZone defenseZone;
    
    private bool _gameOver = false;
    private PlayersHand _playersHandScript;
    private Card _cardScript;

    private void Awake()
    {
        _playersHandScript = playersHand.GetComponent<PlayersHand>();
        _cardScript = card.GetComponent<Card>();
    }

    private void Update()
    {
        CheckEndGame();
    }

    public void UpdateZones(string gamePhase)
    {
        bool active = (gamePhase == "Discard");
        
        discardZone.enabled = active;
        attackZone.enabled = !active;
        defenseZone.enabled = !active;
    }

    public void DrawCards()
    {
        for (int i = 1; i <= 5; i++)
        {
            GameObject go = Instantiate(card, playersHand.transform);
            go.GetComponent<Card>().GenerateCardValue();
        }
    }

    private void CheckEndGame()
    {
        if (player.IsDead())
            GameOver(false);
        if (enemy.IsDead())
            GameOver(true);
    }

    private void GameOver(bool win)
    {
        _gameOver = true;
        uiManager.GameOverCanvas(win);
    }

    public void ResetVariables()
    {
        player.ResetStats();
        enemy.GenerateStats();
        attackZone.ResetZone();
        defenseZone.ResetZone();
        uiManager.ResetVisuals();
    }
    
    public void ResolveCombat()
    {
        int damageDealed = enemy.GetDefense() - player.GetAttack();
        int damageTaken = player.GetDefense() - enemy.GetAttack();
        
        if (damageTaken < 0)
            player.TakeDamage(damageTaken);
        if (damageDealed < 0)
            enemy.TakeDamage(damageDealed);
    }

    public int CardsCountInHand()
    {
        return playersHand.transform.childCount;
    }

    public PhaseManager GetPhaseManager() => phaseManager;
    public QuestionManager GetQuestionManager() => questionManager;
    public UIManager GetUIManager() => uiManager;
    public PlayersHand GetPlayersHand() => _playersHandScript;
    public ActionZone GetAttackZone() => attackZone;
    public ActionZone GetDefenseZone() => defenseZone;
    public Player GetPlayer() => player;
    public bool IsGameOver() => _gameOver;
    public Card GetCard() => _cardScript;
}
