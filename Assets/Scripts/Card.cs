using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    public float returnSpeed;
    public Canvas textCanvas;
    public SpriteRenderer spriteRenderer;
    
    private Camera _camera;
    
    private bool _isReturning = false;
    private bool _isDragging = false;
    private Vector3 _startPosition;

    private void Awake()
    {
        _camera = Camera.main;
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
        spriteRenderer.sortingOrder = cardOrder;
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

    public void ChangeColor(bool correct)
    {
        if (correct)
            spriteRenderer.color = Color.green;
        else
            spriteRenderer.color = Color.red;
    }

    public void GenerateCardValue()
    {
        TextMeshProUGUI cardText = textCanvas.GetComponentInChildren<TextMeshProUGUI>();
        int number = Random.Range(1,4);
        cardText.text = "+" + number;
    }
}
