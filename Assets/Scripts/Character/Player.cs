using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{

    //Le añade ataque o defensa al jugador con el valor correspondiente.
    public void AddStats(ActionZone.ZoneType cardType, int value)
    {
        switch (cardType)
        {
            case ActionZone.ZoneType.Attack:
                AddAttack(value);
                break;
            case ActionZone.ZoneType.Defense:
                AddDefense(value);
                break;
        }
        gameManager.GetUIManager().UpdateStats(CurrentAttack, CurrentDefense, "Player");
    }

    #region Utilities
    public void ResetStats()
    {
        CurrentAttack = 0;
        CurrentDefense = 0;
        gameManager.GetUIManager().UpdateStats(CurrentAttack, CurrentDefense, "Player");
    }
    
    private void AddAttack(int value)
    {
        CurrentAttack += value;
    }

    private void AddDefense(int value)
    {
        CurrentDefense += value;
    }
    #endregion
}
