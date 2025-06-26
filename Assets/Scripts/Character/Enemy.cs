using UnityEngine;

public class Enemy : Character
{
    [Header("Stats")] 
    public int maxAttack;
    public int minAttack;
    public int maxDefense;
    public int minDefense;
    
    public void GenerateStats()
    {
        CurrentAttack = Random.Range(minAttack, maxAttack + 1);
        CurrentDefense = Random.Range(minDefense, maxDefense + 1);
        gameManager.GetUIManager().UpdateStats(CurrentAttack , CurrentDefense, "Enemy");
    }
}
