using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public static event Action<int, int> OnPlayerStatsChanged;
    public static event Action OnDeath;

    protected override void Awake()
    {
        base.Awake(); // Llama al Awake de Character
        
        DiscardPoints.OnFullPoints += HealCharacter;
        CombatManager.OnPlayerHealthChanged += ChangeLife;
        GameFlowManager.OnGameStarted += ResetLife;
        HPBar.OnDeath += PlayerDie;
    }

    private void OnDestroy()
    {
        DiscardPoints.OnFullPoints -= HealCharacter;
        CombatManager.OnPlayerHealthChanged -= ChangeLife;
        GameFlowManager.OnGameStarted -= ResetLife;
        HPBar.OnDeath -= PlayerDie;
    }

    private void PlayerDie()
    {
        OnDeath?.Invoke();
    }

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

    private void ChangeLife(int value)
    {
        if (value < 0)
            TakeDamage(value);
        else
            HealCharacter(value);
    }
    
    private void HealCharacter(int value)
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
        gameManager.GetAudioManager().PlayAudio(AudioManager.AudioList.Heal);
    }
    
    #endregion
}
