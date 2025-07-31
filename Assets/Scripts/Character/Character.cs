using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    public GameManager gameManager;
    
    protected HPBar _hpBar;
    protected int CurrentAttack;
    protected int CurrentDefense;
    
    public enum CharacterType
    {
        Player,
        Enemy
    }
    
    protected virtual void Awake()
    {
        _hpBar = GetComponentInChildren<HPBar>();
        gameManager = FindAnyObjectByType<GameManager>();
    }
    
    protected void TakeDamage(int value)
    {
        _hpBar.Damage(Mathf.Abs(value));
    }

    public void ResetLife()
    {
        _hpBar.RestoreAllLife();
    }

    public bool IsDead()
    {
        return _hpBar.IsDead();
    }
}
