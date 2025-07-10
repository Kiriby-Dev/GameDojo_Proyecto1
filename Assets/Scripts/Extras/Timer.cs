using System;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public GameManager gameManager;
    public Image timerFill;
    public float duration;
    public float lerpSpeed;

    private float _remainingTime;
    private float _targetFill;
    private bool _isRunning;
    private QuestionManager _questionManager;

    private void Start()
    {
        _questionManager = gameManager.GetQuestionManager() ;
    }

    private void Update()
    {
        if (_questionManager.PlayerHasAnswered() || !_isRunning) return;
        TimerUpdate();
    }

    public void StartTimer()
    {
        _remainingTime = duration;
        _targetFill = 1f;
        timerFill.fillAmount = 1f;
        _isRunning = true;
        ChangeTimerColor();
    }

    private void TimerUpdate()
    {
        if (!_isRunning) return;

        _remainingTime -= Time.deltaTime;
        _remainingTime = Mathf.Clamp(_remainingTime, 0, duration);

        ChangeTimerColor();

        // Calculamos el valor objetivo (target) del fill
        _targetFill = _remainingTime / duration;

        // Interpolamos suavemente hacia ese valor
        timerFill.fillAmount = Mathf.Lerp(timerFill.fillAmount, _targetFill, Time.deltaTime * lerpSpeed);

        if (_remainingTime <= 0.01f)
        {
            TimerFinished();
        }
    }
    
    private void TimerFinished()
    {
        _isRunning = false;
        _questionManager.TimeRanOut(true);
        _questionManager.PlayerAnswersCorrectly(false);
    }

    private void ChangeTimerColor()
    {
        if (_remainingTime <= duration * 0.3f)
            timerFill.color = Color.red;
        else if (_remainingTime <= duration * 0.6f)
            timerFill.color = Color.yellow;
        else
            timerFill.color = Color.green;
    }
}
