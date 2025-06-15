using System;
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
    private PlayersHand playersHandScript;
    
    private bool _isTurnOver = true;
    private string _phase;

    private void Awake()
    {
        _discardActionZone = discardZone.GetComponent<ActionZone>();
        _attackActionZone = attackZone.GetComponent<ActionZone>();
        _defenseActionZone = defenseZone.GetComponent<ActionZone>();
        playersHandScript = playersHand.GetComponent<PlayersHand>();
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
        int cantCards = CardsCountInHand();
        if (cantCards <= 0 && _isTurnOver)
        {
            SetPhase("Draw");
            for (int i = 1; i <= 5; i++)
            {
                GameObject go = Instantiate(card, playersHand.transform);
                go.GetComponent<Card>().GenerateCardValue();
            }
            enemy.GenerateStats();
            playersHandScript.Recalculate();
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

        if (cantCards <= 0 && !_isTurnOver)
        {
            SetPhase("Questions");
            StartCoroutine(questionManager.StartQuestions());
        }
    }

    public void FinishTurn()
    {
        _isTurnOver = true;
    }

    public int CardsCountInHand()
    {
        return playersHand.transform.childCount;
    }
    
    public PlayersHand GetPlayersHand() => playersHandScript;
    public ActionZone GetAttackZone() => _attackActionZone;
    public ActionZone GetDefenseZone() => _defenseActionZone;
    public Player GetPlayer() => player;
}
