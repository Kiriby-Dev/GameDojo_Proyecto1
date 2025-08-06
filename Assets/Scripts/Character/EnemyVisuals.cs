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
        string enemyText = "";
        switch (subject)
        {
            case QuestionData.Subject.History:
                enemyText = "Srta. Arqueloca";
                enemySprite.sprite = enemySprites[0];
                break;
            case QuestionData.Subject.Science:
                enemyText = "Dr. Newtonazo";
                enemySprite.sprite = enemySprites[1];
                break;
            case QuestionData.Subject.Entertainment:
                enemyText = "Lic. Chistemalo";
                enemySprite.sprite = enemySprites[2];
                break;
            case QuestionData.Subject.Geography:
                enemyText = "Sr. Tierraplana";
                enemySprite.sprite = enemySprites[3];
                break;
            case QuestionData.Subject.Principal:
                enemyText = "Dir. Sabelotodo";
                enemySprite.sprite = enemySprites[4];
                break;
        }

        if (enemyName)
            enemyName.text = enemyText;
    }

    private void UpdateEnemyStats(int currentAttack, int currentDefense)
    {
        enemyAttackText.text = currentAttack.ToString();
        enemyDefenseText.text = currentDefense.ToString();
    }
}
