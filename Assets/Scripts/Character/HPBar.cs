using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public event Action<bool> OnDeath;
    
    [Header("Config")] 
    public int maxHp;
    public float hpBarSpeed;

    [Header("UI")] 
    public Image imageBar;
    public TextMeshProUGUI textBar;
    
    private int _currentHp;
    private float _targetFill;
    private bool _isDead;
    private bool _healthChanged;

    private void Start()
    {
        _currentHp = maxHp;
        _targetFill = 1f;
        imageBar.fillAmount = 1f;
    }

    private void Update()
    {
        if (!_healthChanged) return;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        imageBar.fillAmount = Mathf.Lerp(imageBar.fillAmount, _targetFill, hpBarSpeed * Time.deltaTime);
        textBar.text = _currentHp + "/" + maxHp;
        
        if (Mathf.Abs(imageBar.fillAmount - _targetFill) < 0.001f)
        {
            imageBar.fillAmount = _targetFill; // Fuerza el valor exacto
            _healthChanged = false;
        }
    }

    public void Damage(int damage)
    {
        if (_isDead) return;
        
        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            _isDead = true;
            _currentHp = 0;
            OnDeath?.Invoke(true);
        }
        
        _targetFill = Mathf.Clamp01((float)_currentHp / maxHp);
        _healthChanged = true;
    }

    public void Heal(int heal)
    {
        if (_isDead) return;
        _currentHp += heal;
        _targetFill = Mathf.Clamp01((float)_currentHp / maxHp);
        _healthChanged = true;
    }

    public void RestoreAllLife()
    {
        _isDead = false;
        _currentHp = maxHp;
        imageBar.fillAmount = 1f;
        _targetFill = 1f;
        _healthChanged = true;
        OnDeath?.Invoke(false);
    }

    #region Getters
    public int GetCurrentHp() => _currentHp;
    public int GetMaxHp() => maxHp;
    public bool IsDead() => _isDead;
    #endregion
}
