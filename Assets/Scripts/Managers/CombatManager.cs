using System;
using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    [Header("Characters")]
    [SerializeField] private Animator playerFight;
    [SerializeField] private Animator enemyFight;

    private int _playerAttack;
    private int _playerDefense;
    private int _enemyAttack;
    private int _enemyDefense;
    private int _damageDealt;
    private int _damageTaken;
    private bool _enemyIsDead;
    
    private AudioManager _audioManager;

    public enum Actions
    {
        Attack,
        BlockAttack,
        Hurt,
    }

    public static event Action<int> OnPlayerHealthChanged;
    public static event Action<int> OnEnemyHealthChanged;

    private void Awake()
    {
        Player.OnPlayerStatsChanged += PlayerStats;
        Enemy.OnEnemyStatsChanged += EnemyStats;
        Enemy.OnEnemyDeath += EnemyIsDead;
        _audioManager = gameManager.GetAudioManager();
    }

    public IEnumerator ResolveCombat()
    {
        CalculateDamage();
        
        DoAction(Character.CharacterType.Player, Actions.Attack);
        yield return new WaitForSeconds(0.35f);
        
        ManageReaction(_damageDealt, Character.CharacterType.Enemy);
        
        yield return new WaitForSeconds(2f);

        if (!_enemyIsDead)
        {
            DoAction(Character.CharacterType.Enemy, Actions.Attack);
            yield return new WaitForSeconds(0.35f);
            
            ManageReaction(_damageTaken, Character.CharacterType.Player);
            
            yield return new WaitForSeconds(2f);
        }
    }

    private void ManageReaction(int damage, Character.CharacterType characterType)
    {
        if (damage < 0)
        {
            if (characterType == Character.CharacterType.Player)
                OnPlayerHealthChanged?.Invoke(damage);
            else
                OnEnemyHealthChanged?.Invoke(damage);
            
            DoAction(characterType, Actions.Hurt);
        }
        else
            DoAction(characterType, Actions.BlockAttack);
    }

    private void DoAction(Character.CharacterType characterType, Actions action)
    {
        if (characterType == Character.CharacterType.Player)
            playerFight.SetTrigger(action.ToString());

        if (characterType == Character.CharacterType.Enemy)
            enemyFight.SetTrigger(action.ToString());

        switch (action)
        {
            case Actions.Attack:
                _audioManager.PlayAudio(AudioManager.AudioList.Attack);
                break;
            case Actions.BlockAttack:
                _audioManager.PlayAudio(AudioManager.AudioList.Blocked);
                break;
            case Actions.Hurt:
                _audioManager.PlayAudio(AudioManager.AudioList.Hurt);
                break;
        }
    }

    #region Utilities
    private void EnemyIsDead()
    {
        _enemyIsDead = true;
    }

    private void PlayerStats(int currentAttack, int currentDefense)
    {
        _playerAttack = currentAttack;
        _playerDefense = currentDefense;
    }
    
    private void EnemyStats(int currentAttack, int currentDefense)
    {
        _enemyAttack = currentAttack;
        _enemyDefense = currentDefense;
    }

    private void CalculateDamage()
    {
        _damageDealt = _enemyDefense - _playerAttack;
        _damageTaken = _playerDefense - _enemyAttack;
    }
    
    public void Reset()
    {
        _enemyIsDead = false;
    }
    #endregion
}
