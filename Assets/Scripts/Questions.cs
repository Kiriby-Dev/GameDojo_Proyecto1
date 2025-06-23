using UnityEngine;

public class Questions : MonoBehaviour
{
    private string _question;
    private string[] _options;
    private string _correctAnswer;

    public void SetData(string question, string[] opts, string correct)
    {
        _question = question;
        _options = opts;
        _correctAnswer = correct;
    }

    public string GetQuestion() => _question;
    public string[] GetOptions() => _options;
    public string GetCorrectAnswer() => _correctAnswer;
}