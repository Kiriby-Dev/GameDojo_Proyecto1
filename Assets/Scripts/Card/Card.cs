using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    public enum CardColor { Red, Yellow, Green, White }
    
    [SerializeField] private float followSmoothness = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float maxTiltAngle = 15f;
    public float returnSpeed;
    public Canvas textCanvas;
    public Sprite[] cardSprites;
    
    private bool _isReturning;
    private bool _isDragging;
    private int _cardLayer;
    
    private Vector3 _lastMousePosition;
    private float _currentRotationZ;
    
    private Camera _camera;
    private SpriteRenderer _spriteRenderer;
    private GameManager _gameManager;
    private PlayersHand _playersHand;

    private void Awake()
    {
        _camera = Camera.main;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _playersHand = _gameManager.GetPlayersHand();
    }

    private void Update()
    {
        if (_isReturning)
            ReturnStartPosition();

        if (_isDragging)
        {
            FollowMousePosition();
        }
    }

    #region Card Movement
    private void OnMouseDown()
    {
        SetCardOrder(99);
        
        _isDragging = true;
        _isReturning = false;

        _playersHand.SelectedCard(this);
    }

    private void OnMouseUp()
    {
        int slotLayer = transform.GetComponentInParent<SpriteRenderer>().sortingOrder;
        SetCardOrder(slotLayer);
        _isReturning = true;
        _isDragging = false;
        
        _playersHand.SelectedCard(null);
    }

    private void FollowMousePosition()
    {
        // Obtener posición del mouse
        Vector3 mousePoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;

        // Calcular delta de movimiento
        Vector3 mouseDelta = mousePoint - _lastMousePosition;
        _lastMousePosition = mousePoint;

        // Seguir al mouse con delay
        transform.position = Vector3.Lerp(transform.position, mousePoint, Time.deltaTime * followSmoothness);

        // Calcular rotación deseada en Z
        float targetRotationZ = Mathf.Clamp(-mouseDelta.x * 100f, -maxTiltAngle, maxTiltAngle);

        // Suavizar rotación actual hacia la deseada
        _currentRotationZ = Mathf.Lerp(_currentRotationZ, targetRotationZ, Time.deltaTime * rotationSpeed);

        // Aplicar rotación en eulerAngles
        transform.eulerAngles = new Vector3(0, 0, _currentRotationZ);
    }
    
    private void ReturnStartPosition()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, returnSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.localPosition, Vector3.zero) < 0.01f)
        {
            transform.localPosition = Vector3.zero;
            _isReturning = false;
        }
    }
    
    //Setea la sorting layer de la carta y pone el texto en 1 sorting layer mas arriba
    public void SetCardOrder(int cardOrder)
    {
        _spriteRenderer.sortingOrder = cardOrder;
        textCanvas.sortingOrder = cardOrder + 1;
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
        Image cardImage = gameObject.GetComponent<Image>();
        switch (color)
        {
            case CardColor.Red: 
                cardImage.color = Color.red; 
                break;
            case CardColor.Yellow: 
                cardImage.color = Color.yellow; 
                break;
            case CardColor.Green: 
                cardImage.color = Color.green; 
                break;
            case CardColor.White: 
                cardImage.color = Color.white; 
                break;
        }
    }

    public void ChangeSprite(int i = 0)
    {
        _spriteRenderer.sprite = cardSprites[i];
    }
    
    public int GetParentIndex()
    {
        return transform.parent.GetSiblingIndex();
    }
    #endregion
}
