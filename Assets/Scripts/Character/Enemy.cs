using System;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    public static event Action<int, int> OnEnemyStatsChanged;
    
    public void GenerateStats(int minAttack, int maxAttack, int minDefense, int maxDefense)
    {
        CurrentAttack = Random.Range(minAttack, maxAttack + 1);
        CurrentDefense = Random.Range(minDefense, maxDefense + 1);
        OnEnemyStatsChanged?.Invoke(CurrentAttack, CurrentDefense);
    }
}
