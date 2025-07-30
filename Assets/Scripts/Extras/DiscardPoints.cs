using System;
using TMPro;
using UnityEngine;

public class DiscardPoints : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private int[] neededPoints;
    [SerializeField] private int healValue;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI discardText;
    
    private int _index;
    private int _actualPoints;
    private bool _isFull;

    private void Awake()
    {
        ActionZone.OnDiscard += UpdatePoints;
    }

    private void Update()
    {
        if (!_isFull) return;
        ResetPoints();
    }

    private void UpdatePoints(int value = 0)
    {
        if (!_isFull)
        {
            _actualPoints += value;
            if (_actualPoints >= neededPoints[_index])
            {
                _actualPoints = neededPoints[_index];
                _isFull = true;
            }
            discardText.text = _actualPoints + "/" + neededPoints[_index];
        }
    }
    
    private void ResetPoints()
    {
        _actualPoints = 0;
        _isFull = false;
        _index++;
        UpdatePoints();
        OnPlayerHealthChanged?.Invoke(healValue);
        _audioManager.PlayAudio(AudioManager.AudioList.Heal);
    }
    
    public void Reset()
    {
        _isFull = false;
        _actualPoints = 0;
        _index = 0;
    }
}
