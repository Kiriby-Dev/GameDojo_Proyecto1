using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [Header("Config")] 
    public int maxHp;
    public float hpBarSpeed;

    [Header("UI")] 
    public Image imageBar;
    public TextMeshProUGUI textBar;
    
    private int _currentHp;
    private float _targetFill;
    private bool _isDead;
    private bool _takesDamage;

    private void Start()
    {
        _currentHp = maxHp;
        _targetFill = 1f;
        imageBar.fillAmount = 1f;
    }

    private void Update()
    {
        if (_isDead)
        {
            imageBar.fillAmount = Mathf.Lerp(imageBar.fillAmount, 0, hpBarSpeed * Time.deltaTime);
            textBar.text = 0 + "/" + maxHp;
        }
        
        if (!_takesDamage || _isDead) return;
        
        imageBar.fillAmount = Mathf.Lerp(imageBar.fillAmount, _targetFill, hpBarSpeed * Time.deltaTime);
        textBar.text = _currentHp + "/" + maxHp;
    }

    public void Damage(int damage)
    {
        if (_isDead) return;
        
        _takesDamage = true;
        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            _isDead = true;
            return;
        }
        
        _targetFill = Mathf.Clamp01((float)_currentHp / maxHp);
    }

    public void Heal(int heal)
    {
        if (_isDead) return;
        _currentHp += heal;
        _targetFill = Mathf.Clamp01((float)_currentHp / maxHp);
    }

    public int GetCurrentHp()
    {
        return _currentHp;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    public bool IsDead()
    {
        return _isDead;
    }

    public void RestoreAllLife()
    {
        _isDead = false;
        _currentHp = maxHp;
        imageBar.fillAmount = 1f;
        _targetFill = 1F;
    }
}
