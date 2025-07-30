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
        _audioManager = gameManager.GetAudioManager();
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
        _damageDealt = _playerAttack - _enemyDefense;
        _damageTaken = _enemyAttack - _playerDefense;
    }

    public IEnumerator ResolveCombat()
    {
        CalculateDamage();
        
        DoAction(Character.CharacterType.Player, Actions.Attack);
        yield return new WaitForSeconds(0.35f);
        
        if (_damageDealt > 0)
        {
            OnEnemyHealthChanged?.Invoke(_damageDealt);
            DoAction(Character.CharacterType.Enemy, Actions.Hurt);
        }
        else
            DoAction(Character.CharacterType.Enemy, Actions.BlockAttack);
        
        yield return new WaitForSeconds(2f);

        if (!_enemyScript.IsDead())
        {
            DoAction(Character.CharacterType.Enemy, Actions.Attack);
            yield return new WaitForSeconds(0.35f);
            
            if (_damageTaken > 0)
            {
                OnPlayerHealthChanged?.Invoke(_damageTaken);
                DoAction(Character.CharacterType.Player, Actions.Hurt);
            }
            else
                DoAction(Character.CharacterType.Player, Actions.BlockAttack);
            
            yield return new WaitForSeconds(2f);
        }
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
                _audioManager.PlayAudio(AudioManager.AudioList.Heal);
                break;
        }
    }
}
