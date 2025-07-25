using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    public enum CardColor { Red, Yellow, Green, White }
    
    [SerializeField] private float followSmoothness;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxTiltAngle;
    
    [SerializeField] private SpriteRenderer cardSprite;
    [SerializeField] private SpriteRenderer shadowSprite;
    [SerializeField] private ParticleSystem chalkEffect;
    
    public float returnSpeed;
    public Canvas textCanvas;

    public enum CardSprites
    {
        Base,
        Attack,
        Defense,
        HistoryEasy,
        HistoryMedium,
        HistoryHard,
        ScienceEasy,
        ScienceMedium,
        ScienceHard,
        EntertainmentEasy,
        EntertainmentMedium,
        EntertainmentHard,
        GeographyEasy,
        GeographyMedium,
        GeographyHard,
    };
    
    [SerializeField] private List<Sprite> sprites;

    private bool _isReturning;
    private bool _isDragging;
    private int _cardLayer;
    
    private Vector3 _lastMousePosition;
    private float _currentRotationZ;
    
    private Camera _camera;
    private GameManager _gameManager;
    private PlayersHand _playersHand;
    private Animator _animator;

    private void Awake()
    {
        _camera = Camera.main;
        _gameManager = FindAnyObjectByType<GameManager>();
        _playersHand = _gameManager.GetPlayersHand();
        _animator = GetComponent<Animator>();
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
        ToggleAnimator(false);
        
        _isDragging = true;
        _isReturning = false;

        _playersHand.SelectedCard(this);
        shadowSprite.transform.localPosition = new Vector3(shadowSprite.transform.localPosition.x, -0.3f, shadowSprite.transform.localPosition.z);
        transform.localPosition = new Vector3(transform.localPosition.x, 0.3f, transform.localPosition.z);
        ChangeSize(1.2f);
    }

    private void OnMouseUp()
    {
        int slotLayer = transform.GetComponentInParent<SpriteRenderer>().sortingOrder;
        SetCardOrder(slotLayer);
        _isReturning = true;
        _isDragging = false;
        
        _playersHand.SelectedCard(null);
        shadowSprite.transform.localPosition = new Vector3(shadowSprite.transform.localPosition.x, -0.137f, shadowSprite.transform.localPosition.z);
        ChangeSize(1f);
    }

    private void FollowMousePosition()
    {
        // Obtener posición del mouse
        Vector3 mousePoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;

        // Calcular delta de movimiento
        Vector3 mouseDelta = (mousePoint - _lastMousePosition).normalized;
        _lastMousePosition = mousePoint;

        // Seguir al mouse con delay
        transform.position = Vector3.Lerp(transform.position, mousePoint, Time.deltaTime * followSmoothness);

        // Calcular rotación deseada en Z
        float targetRotationZ = Mathf.Clamp(-mouseDelta.x * 100f, -maxTiltAngle, maxTiltAngle);
        
        LerpRotation(targetRotationZ);
    }

    private void ReturnStartPosition()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, returnSpeed * Time.deltaTime);
        LerpRotation(0);
        
        if (Vector3.Distance(transform.localPosition, Vector3.zero) < 0.01f)
        {
            transform.localPosition = Vector3.zero;
            ToggleAnimator(true);
            _isReturning = false;
        }
    }
    
    //Setea la sorting layer de la carta y pone el texto en 1 sorting layer mas arriba
    public void SetCardOrder(int cardOrder)
    {
        shadowSprite.sortingOrder = cardOrder - 1;
        cardSprite.sortingOrder = cardOrder;
        textCanvas.sortingOrder = cardOrder + 1;
    }
    #endregion
    
    public void PutCardInSlot(Transform slot)
    {
        DisableInteraction();
        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        transform.localScale = new Vector3(0.4f, 0.3f, 1f);
        chalkEffect.Play();
    }
    
    private void DisableInteraction()
    {
        _isDragging = false;
        _isReturning = false;
        GetComponent<Collider2D>().enabled = false;
        transform.Find("DropZone").gameObject.SetActive(false);
    }

    public int GenerateCardValue()
    {
        TextMeshProUGUI cardText = textCanvas.GetComponentInChildren<TextMeshProUGUI>();
        int number = Random.Range(1,4);
        cardText.text = "+" + number;
        return number;
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

    private void ChangeSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);
    }

    public void ChangeSprite(CardSprites sprite)
    {
        cardSprite.sprite = sprites[(int)sprite];
    }
    
    public int GetParentIndex()
    {
        return transform.parent.GetSiblingIndex();
    }
    
    private void LerpRotation(float targetRotationZ)
    {
        // Suavizar rotación actual hacia 0
        _currentRotationZ = Mathf.Lerp(_currentRotationZ, targetRotationZ, Time.deltaTime * rotationSpeed);

        // Aplicar rotación en eulerAngles
        transform.eulerAngles = new Vector3(0, 0, _currentRotationZ);
    }
    #endregion

    #region Animation
    public void PlayMoveAnimation(float direction)
    {
        if (direction <= 0)
            _animator.SetTrigger("MoveRight");
        else
            _animator.SetTrigger("MoveLeft");
    }
    
    public void ToggleAnimator(bool state)
    {
        _animator.enabled = state;
    }
    #endregion
}
