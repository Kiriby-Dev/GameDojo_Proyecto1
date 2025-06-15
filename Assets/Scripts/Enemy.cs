using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;

    public void GenerateStats()
    {
        int attack = Random.Range(1, 4);
        int defense = Random.Range(1, 4);
        attackText.text = attack.ToString();
        defenseText.text = defense.ToString();
    }
}
