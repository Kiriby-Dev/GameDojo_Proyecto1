using System;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gameManager;
    
    private int _currentAttack;
    private int _currentDefense;
    private HPBar _hpBar;

    private void Awake()
    {
        _hpBar = GetComponentInChildren<HPBar>();
    }

    private void Start()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        _currentAttack = 0;
        _currentDefense = 0;
        gameManager.GetUIManager().UpdatePlayerStats(_currentAttack, _currentDefense);
    }

    public void AddStats(GameObject card, int value)
    {
        ActionZone.ZoneType zoneType = card.transform.parent.GetComponent<ActionZone>().GetZoneType();
        switch (zoneType)
        {
            case ActionZone.ZoneType.Attack:
                AddAttack(value);
                break;
            case ActionZone.ZoneType.Defense:
                AddDefense(value);
                break;
        }
        gameManager.GetUIManager().UpdatePlayerStats(_currentAttack, _currentDefense);
    }
    
    private void AddAttack(int value)
    {
        _currentAttack += value;
    }

    private void AddDefense(int value)
    {
        _currentDefense += value;
    }

    public void TakeDamage(int value)
    {
        _hpBar.Damage(-value);
    }

    public bool IsDead()
    {
        return _hpBar.IsDead();
    }

    public int GetAttack() => _currentAttack;
    public int GetDefense() => _currentDefense;
}
