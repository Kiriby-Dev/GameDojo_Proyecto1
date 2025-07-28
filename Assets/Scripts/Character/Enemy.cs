using UnityEngine;

public class Enemy : Character
{
    public void GenerateStats(int minAttack, int maxAttack, int minDefense, int maxDefense)
    {
        CurrentAttack = Random.Range(minAttack, maxAttack + 1);
        CurrentDefense = Random.Range(minDefense, maxDefense + 1);
        gameManager.GetUIManager().UpdateStats(CurrentAttack , CurrentDefense, "Enemy");
    }
}
