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
    
    public void Awake()
    {
        _hpBar = GetComponentInChildren<HPBar>();
    }
    
    public void TakeDamage(int value)
    {
        _hpBar.Damage(-1 * value);
    }

    public void ResetLife()
    {
        _hpBar.RestoreAllLife();
    }

    public bool IsDead()
    {
        return _hpBar.IsDead();
    }

    #region Getters
    public int GetAttack() => CurrentAttack;
    public int GetDefense() => CurrentDefense;
    #endregion
}
