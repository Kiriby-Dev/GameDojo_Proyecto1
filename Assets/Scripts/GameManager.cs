using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject playersHand;
    public GameObject card;
    public QuestionManager questionManager;
    public Player player;
    public Enemy enemy;

    [Header("UI")] 
    public TextMeshProUGUI phaseText;
    
    [Header("ActionZones")]
    public GameObject discardZone;
    public GameObject attackZone;
    public GameObject defenseZone;

    private ActionZone _discardActionZone;
    private ActionZone _attackActionZone;
    private ActionZone _defenseActionZone;
    private PlayersHand _playersHandScript;
    private bool _waitingForQuestions = false;
    private bool _waitingForResults = false;
    
    private bool _isTurnOver = true;
    private string _phase;

    private void Awake()
    {
        _discardActionZone = discardZone.GetComponent<ActionZone>();
        _attackActionZone = attackZone.GetComponent<ActionZone>();
        _defenseActionZone = defenseZone.GetComponent<ActionZone>();
        _playersHandScript = playersHand.GetComponent<PlayersHand>();
    }

    private void Update()
    {
        ChangePhase();
    }

    public string CheckPhase()
    {
        return _phase;
    }

    public void SetPhase(string phase)
    {
        _phase = phase;
        phaseText.text = _phase;
    }

    private void ChangePhase()
    {
        if (_waitingForQuestions) return;
        if (_waitingForResults) return;
        
        int cantCards = CardsCountInHand();
        
        if (cantCards <= 0 && !_isTurnOver)
        {
            SetPhase("Questions");
            _waitingForQuestions = true;
            StartCoroutine(WaitQuestionsPhase());
        }
        
        if (cantCards <= 0 && _isTurnOver)
        {
            ResetVariables();
            SetPhase("Draw");
            for (int i = 1; i <= 5; i++)
            {
                GameObject go = Instantiate(card, playersHand.transform);
                go.GetComponent<Card>().GenerateCardValue();
            }
            _playersHandScript.Recalculate();
            _isTurnOver = false;
        }

        if (cantCards <= 5 && cantCards > 3)
        {
            SetPhase("Discard");
            _discardActionZone.enabled = true;
            _attackActionZone.enabled = false;
            _defenseActionZone.enabled = false;
        }

        if (cantCards <= 3 && cantCards > 0)
        {
            SetPhase("Colocation");
            _discardActionZone.enabled = false;
            _attackActionZone.enabled = true;
            _defenseActionZone.enabled = true;
        }
    }

    private void ResetVariables()
    {
        player.ResetStats();
        enemy.GenerateStats();
        questionManager.ResetBoard();
        _attackActionZone.ResetZone();
        _defenseActionZone.ResetZone();
    }

    private IEnumerator WaitQuestionsPhase()
    {
        yield return StartCoroutine(questionManager.StartQuestions());
        _waitingForQuestions = false;
        
        SetPhase("Resolution");
        _waitingForResults = true;
        StartCoroutine(WaitResolutionPhase());
    }
    
    private IEnumerator WaitResolutionPhase()
    {
        yield return StartCoroutine(ResolveStats());
        _waitingForResults = false;
        _isTurnOver = true;
    }

    private IEnumerator ResolveStats()
    {
        int damageDealed = enemy.GetDefense() - player.GetAttack();
        int damageTaken = player.GetDefense() - enemy.GetAttack();
        
        if (damageTaken < 0)
            player.TakeDamage(damageTaken);
        if (damageDealed < 0)
            enemy.TakeDamage(damageDealed);
        
        yield return new WaitForSeconds(2.0f);
    }

    public int CardsCountInHand()
    {
        return playersHand.transform.childCount;
    }
    
    public PlayersHand GetPlayersHand() => _playersHandScript;
    public ActionZone GetAttackZone() => _attackActionZone;
    public ActionZone GetDefenseZone() => _defenseActionZone;
    public Player GetPlayer() => player;
}
