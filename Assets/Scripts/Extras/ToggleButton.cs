using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private Sprite imageEnabled;
    [SerializeField] private Sprite imageDisabled;
    
    private Toggle _toggle;
    private Image _checkMarkImage;

    private void Awake()
    {
        _toggle = GetComponentInParent<Toggle>();
        _checkMarkImage = gameObject.GetComponent<Image>();
    }

    public void ChangeSprite()
    {
        if (!_toggle.isOn)
        {
            _checkMarkImage.sprite = imageDisabled;
            _checkMarkImage.color = Color.red;
        }
        else
        {
            _checkMarkImage.sprite = imageEnabled;
            _checkMarkImage.color = Color.green;
        }
    }
}
