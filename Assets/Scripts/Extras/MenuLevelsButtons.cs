using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuLevelsButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _hoverImage;

    private void Start()
    {
        _hoverImage.enabled = false;
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
