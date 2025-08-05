using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float followSmoothness;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxTiltAngle;
    
    [Header("Card Visuals")]
    [SerializeField] private SpriteRenderer shadowSprite;
    [SerializeField] private ParticleSystem chalkEffect;
    [SerializeField] private Canvas textCanvas;
    [SerializeField] private SpriteRenderer cardSprite;
    [SerializeField] private List<Sprite> sprites;
    
    [Header("FX")]
    [SerializeField] private GameObject paperDiscardEffect;
    private Animator _paperDiscardAnimator;

    public enum CardSprites
    {
        Base,
        Attack, Defense,
        HistoryEasy, HistoryMedium, HistoryHard,
        ScienceEasy, ScienceMedium, ScienceHard,
        EntertainmentEasy, EntertainmentMedium, EntertainmentHard,
        GeographyEasy, GeographyMedium, GeographyHard,
    };

    private bool _isReturning;
    private bool _isDragging;
    private int _cardLayer;
    
    private Vector3 _lastMousePosition;
    private float _currentRotationZ;
    private TextMeshProUGUI _cardDifficulty;
    
    private Camera _camera;
    private GameManager _gameManager;
    private PlayersHand _playersHand;
    private CardSprites _lastSprite;
    private Animator _animator;

    private void Awake()
    {
        _camera = Camera.main;
        _gameManager = FindAnyObjectByType<GameManager>();
        _playersHand = _gameManager.GetPlayersHand();
        _animator = GetComponent<Animator>();
        _cardDifficulty = textCanvas.GetComponentInChildren<TextMeshProUGUI>();
        
        _paperDiscardAnimator = paperDiscardEffect.GetComponent<Animator>();
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
        DisableInteraction(true);
        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        transform.localScale = new Vector3(0.4f, 0.3f, 1f);
        chalkEffect.Play();
    }
    
    public void DisableInteraction(bool value)
    {
        if (value)
        {
            _isDragging = false;
            _isReturning = false;
            GetComponent<Collider2D>().enabled = false;
            transform.Find("DropZone").gameObject.SetActive(false);
        }
        else
            GetComponent<Collider2D>().enabled = true;
    }

    public int GenerateCardValue()
    {
        int number = Random.Range(1,4);
        _cardDifficulty.text = "+" + number;
        return number;
    }

    #region Utilities

    private void ChangeSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);
    }

    public void ChangeSprite(CardSprites sprite)
    {
        cardSprite.sprite = sprites[(int)sprite];
        _lastSprite = sprite;
    }

    public int GetParentIndex()
    {
        return transform.parent.GetSiblingIndex();
    }

    public Sprite GetCardSprite()
    {
        return cardSprite.sprite;
    }

    public int GetCardDifficulty()
    {
        return int.Parse(_cardDifficulty.text);
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

	public void PlayDiscardAnimation(bool value)
    {
        //_animator.SetBool("Discard", value);
        print("Play Discard Animation");
        StartCoroutine(nameof(DiscardCoroutine), value);
    }

    private IEnumerator DiscardCoroutine(bool value)
    {
        if (value)
        {
            cardSprite.gameObject.SetActive(false);
            textCanvas.gameObject.SetActive(false);
            shadowSprite.enabled = false;
        
            paperDiscardEffect.SetActive(true);
            _paperDiscardAnimator.Play("Discard_Independant");
        }
        else
        {
            _paperDiscardAnimator.Play("DiscardOut_Independant");
            yield return new WaitForSeconds(0.3f);
            paperDiscardEffect.SetActive(false);
            cardSprite.gameObject.SetActive(true);
            textCanvas.gameObject.SetActive(true);
            shadowSprite.enabled = true;
        }
    }
    

    public void OnDiscardOutFinished()
    {
        print("On Discard Out Finished!");
        _paperDiscardAnimator.speed = -1;
        _paperDiscardAnimator.Play("Discard_Independant");
        //ToggleAnimator(false);
        //ChangeSprite(_lastSprite);
    }

    public void OnDiscardStart()
    {
        ChangeSprite(_lastSprite);
    }

    public void ToggleAnimator(bool state)
    {
        _animator.enabled = state;
    }
    #endregion
}
