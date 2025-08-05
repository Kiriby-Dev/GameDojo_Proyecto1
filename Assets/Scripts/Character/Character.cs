using System;
using UnityEngine;

public class Character : MonoBehaviour
{
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
    }
    
    protected void TakeDamage(int value)
    {
        _hpBar.Damage(Mathf.Abs(value));
    }

    protected void ResetLife()
    {
        _hpBar.RestoreAllLife();
    }
}
