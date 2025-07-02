using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    public enum CardColor { Red, Yellow, Green, White }
    
    public float returnSpeed;
    public Canvas textCanvas;
    public Sprite[] cardSprites;
    
    private Camera _camera;
    private SpriteRenderer _spriteRenderer;
    private PlayersHand _playersHand;
    
    private bool _isReturning = false;
    private bool _isDragging = false;
    private Vector3 _startPosition;
    private int _cardIndex;
    private bool _isCardActive;

    private void Awake()
    {
        _camera = Camera.main;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _playersHand = FindAnyObjectByType<PlayersHand>();
    }

    private void Start()
    {
        ChangeColor(); //Setea la carta en su color original
    }

    private void Update()
    {
        if (_isReturning)
            ReturnStartPosition();

        if (_isDragging)
            FollowMousePosition();
    }

    #region Card Movement
    private void OnMouseDown()
    {
        if (!_isCardActive) return;
        SetCardOrder(99);
        SetCardStartPosition();
        
        _isDragging = true;
        _isReturning = false;

        _cardIndex = _playersHand.GetCardIndex(transform);
        transform.parent = null;
        _playersHand.ToggleCardsEnable(false);
        _playersHand.Recalculate();
    }

    private void OnMouseUp()
    {
        _isReturning = true;
        _isDragging = false;
    }

    private void FollowMousePosition()
    {
        Vector3 mousePoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;
        transform.position = mousePoint;
    }
    
    private void ReturnStartPosition()
    {
        transform.position = Vector3.Lerp(transform.position, _startPosition, returnSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, _startPosition) < 0.03f)
        {
            transform.position = _startPosition;
            _playersHand.ToggleCardsEnable(true);
            _isReturning = false;
            transform.parent = _playersHand.transform;
            transform.SetSiblingIndex(_cardIndex);
            _playersHand.Recalculate();
        }
    }
    
    //Setea la sorting layer de la carta y pone el texto en 1 sorting layer mas arriba
    public void SetCardOrder(int cardOrder)
    {
        _spriteRenderer.sortingOrder = cardOrder;
        textCanvas.sortingOrder = cardOrder + 1;
    }

    public void SetCardStartPosition()
    {
        if (!_isDragging && !_isReturning)
            _startPosition = transform.position;
    }
    #endregion
    
    public void PutCardInSlot(Transform slot)
    {
        DisableInteraction();
        transform.position = slot.position;
        transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        transform.SetParent(slot);
    }
    
    private void DisableInteraction()
    {
        _isDragging = false;
        _isReturning = false;
        GetComponent<Collider2D>().enabled = false;
        transform.Find("DropZone").gameObject.SetActive(false);
    }

    public void GenerateCardValue()
    {
        TextMeshProUGUI cardText = textCanvas.GetComponentInChildren<TextMeshProUGUI>();
        int number = Random.Range(1,4);
        cardText.text = "+" + number;
    }

    #region Utilities
    public void ChangeColor(CardColor color = CardColor.White)
    {
        switch (color)
        {
            case CardColor.Red: 
                _spriteRenderer.color = Color.red; 
                break;
            case CardColor.Yellow: 
                _spriteRenderer.color = Color.yellow; 
                break;
            case CardColor.Green: 
                _spriteRenderer.color = Color.green; 
                break;
            case CardColor.White: 
                _spriteRenderer.color = Color.white; 
                break;
        }
    }

    public void ChangeSprite(int i = 0)
    {
        _spriteRenderer.sprite = cardSprites[i];
        Debug.Log(gameObject.name);
        Debug.Log(i);
    }
    
    public void SetCardActive(bool active) => _isCardActive = active;
    #endregion
}
