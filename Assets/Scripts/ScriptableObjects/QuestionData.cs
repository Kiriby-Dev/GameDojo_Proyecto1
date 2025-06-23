using UnityEngine;

[CreateAssetMenu(fileName = "QuestionData", menuName = "Scriptable Objects/QuestionData")]
public class QuestionData : ScriptableObject
{
    public enum Difficulty { Easy, Medium, Hard }

    public string question;
    public string correctAnswer;
    public string[] wrongAnswers;
    public Difficulty difficulty;
    
    public string GetCorrectAnswer() => correctAnswer;
    public string GetQuestion() => question;
    public string[] GetOptions() => wrongAnswers;
}
