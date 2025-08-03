using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    public static event Action<int, int> OnEnemyStatsChanged;
    public static event Action OnDeath;

    private int _minAttack;
    private int _maxAttack;
    private int _minDefense;
    private int _maxDefense;

    protected override void Awake()
    {
        base.Awake(); // Llama al Awake de Character
        
        CombatManager.OnEnemyHealthChanged += TakeDamage;
        GameFlowManager.OnGameStarted += ResetLife;
        HPBar.OnDeath += EnemyDie;
    }

    private void OnDestroy()
    {
        CombatManager.OnEnemyHealthChanged -= TakeDamage;
        GameFlowManager.OnGameStarted -= ResetLife;
        HPBar.OnDeath -= EnemyDie;
    }

    public void GenerateStats()
    {
        CurrentAttack = Random.Range(_minAttack, _maxAttack + 1);
        print(CurrentAttack);
        print(CurrentDefense);
        CurrentDefense = Random.Range(_minDefense, _maxDefense + 1);
        OnEnemyStatsChanged?.Invoke(CurrentAttack, CurrentDefense);
    }

    public void SetEnemyStats(int minAttack, int maxAttack, int minDefense, int maxDefense)
    {
        _minAttack = minAttack;
        _maxAttack = maxAttack;
        _minDefense = minDefense;
        _maxDefense = maxDefense;
    }

    private void EnemyDie()
    {
        OnDeath?.Invoke();
    }
}
