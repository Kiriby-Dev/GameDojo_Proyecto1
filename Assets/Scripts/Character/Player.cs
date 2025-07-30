using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public static event Action<int, int> OnPlayerStatsChanged;

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
    }

    #region Utilities
    public void ResetStats()
    {
        CurrentAttack = 0;
        CurrentDefense = 0;
        OnPlayerStatsChanged?.Invoke(CurrentAttack, CurrentDefense);
    }
    
    private void AddAttack(int value)
    {
        CurrentAttack += value;
        OnPlayerStatsChanged?.Invoke(CurrentAttack, CurrentDefense);
    }

    private void AddDefense(int value)
    {
        CurrentDefense += value;
        OnPlayerStatsChanged?.Invoke(CurrentAttack, CurrentDefense);
    }
    
    public void HealCharacter(int value)
    {
        int maxHp = _hpBar.GetMaxHp();
        int currentHp = _hpBar.GetCurrentHp();
        
        if ((currentHp + value) > maxHp)
        {
            AddDefense((currentHp + value) - maxHp);
            _hpBar.Heal(maxHp - currentHp);
        }
        else
        {
            _hpBar.Heal(value);
        }
    }
    
    #endregion
}
