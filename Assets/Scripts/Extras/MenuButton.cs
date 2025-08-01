using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _hoverImage;
    [SerializeField] private Button _button;

    public static event Action OnLevelButtonClicked; 
    
    private void Start()
    {
        _hoverImage.enabled = false;
        if (_button)
            _button.onClick.AddListener(() => OnLevelButtonClicked?.Invoke());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hoverImage)
            _hoverImage.enabled = true;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_hoverImage)
            _hoverImage.enabled = false;
    }
}
