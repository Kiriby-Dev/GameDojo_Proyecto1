using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;

    private int _attack;
    private int _defense;
    private HPBar hpBar;

    private void Awake()
    {
        hpBar = GetComponentInChildren<HPBar>();
    }
    
    public void GenerateStats()
    {
        _attack = Random.Range(1, 4);
        _defense = Random.Range(1, 4);
        attackText.text = _attack.ToString();
        defenseText.text = _defense.ToString();
    }
    
    public void TakeDamage(int value)
    {
        hpBar.Damage(-1 * value);
    }
    
    public int GetAttack() => _attack;
    public int GetDefense() => _defense;
}
