using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVisuals : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemyName;
    [SerializeField] private Image enemySprite;
    [SerializeField] private Sprite[] enemySprites;
    [SerializeField] private TextMeshProUGUI[] enemyDialogs;
    [SerializeField] private TextMeshProUGUI enemyAttackText;
    [SerializeField] private TextMeshProUGUI enemyDefenseText;

    private void Awake()
    {
        LevelsManager.OnSubjectChosen += ChangeEnemy;
        Enemy.OnEnemyStatsChanged += UpdateEnemyStats;
    }

    private void OnDestroy()
    {
        LevelsManager.OnSubjectChosen -= ChangeEnemy;
        Enemy.OnEnemyStatsChanged -= UpdateEnemyStats;
    }

    private void ChangeEnemy(QuestionData.Subject subject)
    {
        if (!enemyName) return;
        
        switch (subject)
        {
            case QuestionData.Subject.History:
                enemyName.text = "Srta. Anecdota";
                enemySprite.sprite = enemySprites[0];
                break;
            case QuestionData.Subject.Science:
                enemyName.text = "Dr. Chispa";
                enemySprite.sprite = enemySprites[1];
                break;
            case QuestionData.Subject.Entertainment:
                enemyName.text = "Sr. Trailer";
                enemySprite.sprite = enemySprites[2];
                break;
            case QuestionData.Subject.Geography:
                enemyName.text = "Srta. Brujula";
                enemySprite.sprite = enemySprites[3];
                break;
            case QuestionData.Subject.Principal:
                enemyName.text = "Dir. Sabelotodo";
                enemySprite.sprite = enemySprites[4];
                break;
        }
    }

    private void UpdateEnemyStats(int currentAttack, int currentDefense)
    {
        enemyAttackText.text = currentAttack.ToString();
        enemyDefenseText.text = currentDefense.ToString();
    }
}
