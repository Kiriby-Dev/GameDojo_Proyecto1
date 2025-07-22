using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegenerationBar : MonoBehaviour
{
    [Header("Config")] 
    public float barSpeed;
    public int[] neededPoints;
    
    [Header("UI")] 
    public Image imageBar;
    //public TextMeshProUGUI textBar;
    
    private int _currentFill;
    private float _targetFill;
    private bool _isFull;
    private bool _cardDiscarded;
    private int _index;

    private void Start()
    {
        _currentFill = 0;
        _targetFill = 0f;
        imageBar.fillAmount = 0f;
        _index = 0;
    }

    private void Update()
    {
        
        if (!_cardDiscarded || IsFull()) return;
        
        imageBar.fillAmount = Mathf.Lerp(imageBar.fillAmount, _targetFill, barSpeed * Time.deltaTime);
        //textBar.text = _currentFill + "/" + neededPoints[_index];
    }

    public void Fill(int cardValue)
    {
        if (_isFull) return;
        
        _cardDiscarded = true;
        _currentFill -= cardValue;

        if (_currentFill >= neededPoints[_index])
        {
            _isFull = true;
            return;
        }
        
        _targetFill = Mathf.Clamp01((float)_currentFill / neededPoints[_index]);
    }

    private bool IsFull()
    {
        return _isFull;
    }
}
