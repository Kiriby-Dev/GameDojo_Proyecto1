using System;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public int maxHp;

    [Header("UI")] 
    public Image imageBar;
    
    private int _currentHp;
    private bool _isDead = false;

    private void Awake()
    {
        _currentHp = maxHp;
    }

    public void Damage(int damage)
    {
        if (_isDead) return;
        
        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            _isDead = true;
            return;
        }
        
        float fill = (float)_currentHp / (float)maxHp;
        imageBar.fillAmount = fill;
    }
}
