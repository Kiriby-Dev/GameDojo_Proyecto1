using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject playersHand;
    public GameObject card;
    
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

    public bool IsTurnOver()
    {
        return _isTurnOver;
    }

    public string CheckPhase()
    {
        return _phase;
    }

    public void SetPhase(string phase)
    {
        _phase = phase;
    }

    private void ChangePhase()
    {
        int cantCards = CardsCountInHand();
        if (cantCards <= 0 && IsTurnOver())
        {
            SetPhase("Draw");
            for (int i = 1; i <= 5; i++)
            {
                GameObject go = Instantiate(card, playersHand.transform);
                go.GetComponent<Card>().GenerateCardValue();
            }
            playersHandScript.Recalculate();
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

    public int CardsCountInHand()
    {
        return playersHand.transform.childCount;
    }
}
