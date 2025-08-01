using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    public static event Action<int, int> OnEnemyStatsChanged;
    public static event Action OnDeath;

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

    public void GenerateStats(int minAttack, int maxAttack, int minDefense, int maxDefense)
    {
        CurrentAttack = Random.Range(minAttack, maxAttack + 1);
        CurrentDefense = Random.Range(minDefense, maxDefense + 1);
        OnEnemyStatsChanged?.Invoke(CurrentAttack, CurrentDefense);
    }
    
    private void EnemyDie()
    {
        OnDeath?.Invoke();
    }
}
