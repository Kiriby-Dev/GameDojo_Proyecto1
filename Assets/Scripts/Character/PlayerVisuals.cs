using TMPro;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI[] playerDialogs;
    [SerializeField] private TextMeshProUGUI playerAttackText;
    [SerializeField] private TextMeshProUGUI playerDefenseText;
    [SerializeField] private ParticleSystem healEffect;
    
    private Animator _animator;

    private void Awake()
    {
        Player.OnPlayerStatsChanged += UpdatePlayerStats;
        DiscardPoints.OnFullPoints += PlayHealAnimation;
        InputName.OnPlayerNameChanged += UpdatePlayerName;
        
        _animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        Player.OnPlayerStatsChanged -= UpdatePlayerStats;
        DiscardPoints.OnFullPoints -= PlayHealAnimation;
        InputName.OnPlayerNameChanged -= UpdatePlayerName;
    }

    private void UpdatePlayerName(string playerNameString)
    {
        if (!playerName) return;
        playerName.text = playerNameString;
    }

    private void UpdatePlayerStats(int currentAttack, int currentDefense)
    {
        playerAttackText.text = currentAttack.ToString();
        playerDefenseText.text = currentDefense.ToString();
    }

    private void PlayHealAnimation(int value)
    {
        _animator.SetTrigger("Heal");
    }

    private void PlayHealEffect()
    {
        healEffect.Play();
    }
}
