using System;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;

    private int _currentAttack;
    private int _currentDefense;
    private HPBar hpBar;

    private void Awake()
    {
        hpBar = GetComponentInChildren<HPBar>();
    }

    private void Start()
    {
        ResetStats();
    }

    private void UpdateStats()
    {
        attackText.text = _currentAttack.ToString();
        defenseText.text = _currentDefense.ToString();
    }

    public void ResetStats()
    {
        _currentAttack = 0;
        _currentDefense = 0;
        UpdateStats();
    }

    public void AddAttack(int value)
    {
        _currentAttack += value;
        UpdateStats();
    }

    public void AddDefense(int value)
    {
        _currentDefense += value;
        UpdateStats();
    }

    public void TakeDamage(int value)
    {
        hpBar.Damage(-value);
    }

    public int GetAttack() => _currentAttack;
    public int GetDefense() => _currentDefense;
}
