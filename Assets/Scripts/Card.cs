using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    public float returnSpeed;
    public Canvas textCanvas;
    
    public Sprite[] cardSprites;
    
    private Camera _camera;
    private SpriteRenderer _spriteRenderer;
    
    private bool _isReturning = false;
    private bool _isDragging = false;
    private Vector3 _startPosition;

    private void Awake()
    {
        _camera = Camera.main;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
        _isDragging = true;
        _isReturning = false;
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

    public void SetCardStartPosition()
    {
        _startPosition = transform.position;
    }

    public void SetCardOrder(int cardOrder)
    {
        _spriteRenderer.sortingOrder = cardOrder;
        textCanvas.sortingOrder = cardOrder + 1;
    }

    private void ReturnStartPosition()
    {
        transform.position = Vector3.Lerp(transform.position, _startPosition, returnSpeed * Time.deltaTime);
        if (transform.position == _startPosition)
        {
            _isReturning = false;
        }
    }
    
    public GameObject[] PutCardInZone(GameObject[] cardsInZone, int cardsInZoneIndex)
    {
        _isReturning = false;
        GetComponent<Collider2D>().enabled = false;
        
        transform.Find("DropZone").gameObject.SetActive(false);
        transform.position = cardsInZone[cardsInZoneIndex].transform.position;
        transform.localScale = new Vector3(0.6f, 0.6f, 0);
        transform.SetParent(cardsInZone[cardsInZoneIndex].transform);
        return cardsInZone;
    }

    public void ChangeColor(string color = "white")
    {
        switch (color)
        {
            case "red":
                _spriteRenderer.color = Color.red;
                break;
            case "yellow":
                _spriteRenderer.color = Color.yellow;
                break;
            case "green":
                _spriteRenderer.color = Color.green;
                break;
            case "white":
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
