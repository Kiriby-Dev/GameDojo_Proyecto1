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

    public static event Action<int> OnFullPoints;

    private void Awake()
    {
        ActionZone.OnDiscard += UpdatePoints;
    }

    private void Start()
    {
        UpdatePoints();
    }

    private void Update()
    {
        if (!_isFull) return;
        AdvanceNeededPoints();
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
                OnFullPoints?.Invoke(healValue);
            }
            discardText.text = _actualPoints + "/" + neededPoints[_index];
        }
    }
    
    private void AdvanceNeededPoints()
    {
        _actualPoints = 0;
        _isFull = false;
        _index++;
        if (_index > neededPoints.Length - 1)
            _index--;
        UpdatePoints();
    }
    
    public void Reset()
    {
        _isFull = false;
        _actualPoints = 0;
        _index = 0;
    }
}
