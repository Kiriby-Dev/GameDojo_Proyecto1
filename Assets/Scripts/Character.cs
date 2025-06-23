using UnityEngine;

public class Character : MonoBehaviour
{
    public GameManager gameManager;
    
    protected int CurrentAttack;
    protected int CurrentDefense;
    private HPBar _hpBar;
    
    public void Awake()
    {
        _hpBar = GetComponentInChildren<HPBar>();
    }
    
    public void TakeDamage(int value)
    {
        _hpBar.Damage(-1 * value);
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
