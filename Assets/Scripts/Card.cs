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
    
    private bool _isReturning = false;
    private bool _isDragging = false;
    private Vector3 _startPosition;
    private PlayersHand _playersHand;

    private void Awake()
    {
        _camera = Camera.main;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _playersHand = FindAnyObjectByType<PlayersHand>();
    }

    private void Start()
    {
        ChangeColor();
        ChangeSprite();
    }

    private void Update()
    {
        if (_isReturning)
        {
            ReturnStartPosition();
        }

        if (_isDragging)
        {
            FollowMousePosition();
        }
    }

    private void OnMouseDown()
    {
        SetCardStartPosition();
        
        _isDragging = true;
        _isReturning = false;
        
        _playersHand.SetGrabbedCard(this);
    }

    private void OnMouseUp()
    {
        _isReturning = true;
        _isDragging = false;
        
        _playersHand.ClearGrabbedCard();
    }

    private void FollowMousePosition()
    {
        Vector3 mousePoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;
        transform.position = mousePoint;
    }

    public void SetCardStartPosition()
    {
        if (!_isDragging && !_isReturning)
        {
            _startPosition = transform.position;
        }
    }

    public void SetCardOrder(int cardOrder)
    {
        _spriteRenderer.sortingOrder = cardOrder;
        textCanvas.sortingOrder = cardOrder + 1;
    }

    private void ReturnStartPosition()
    {
        transform.position = Vector3.Lerp(transform.position, _startPosition, returnSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _startPosition) < 0.01f)
        {
            transform.position = _startPosition;
            _isReturning = false;
        }
    }
    
    public void PutCardInSlot(Transform slot)
    {
        DisableInteraction();
        transform.position = slot.position;
        transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        transform.SetParent(slot);
        if (_playersHand)
            _playersHand.Recalculate();
    }
    
    private void DisableInteraction()
    {
        _isDragging = false;
        _isReturning = false;
        GetComponent<Collider2D>().enabled = false;
        transform.Find("DropZone").gameObject.SetActive(false);
    }

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
    }

    public void GenerateCardValue()
    {
        TextMeshProUGUI cardText = textCanvas.GetComponentInChildren<TextMeshProUGUI>();
        int number = Random.Range(1,4);
        cardText.text = "+" + number;
    }
}
