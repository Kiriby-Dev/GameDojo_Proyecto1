using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionZone : MonoBehaviour
{
    public GameManager gameManager;
    public enum ZoneType { Attack, Defense , Discard}
    public ZoneType zoneType;
    
    private GameObject[] _cardsInZone;
    private int _cantCardsInZone = 0;
    private Card _selectedCard;
    private bool _isDropping;
    private bool _activeZone;
    private Animator _animator;
    private PhaseManager.GamePhase _currentPhase;

    public static event Action<int> OnDiscard;
    public static event Action<Sprite, int, ZoneType> OnCardPlaced;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GetZoneChildren();
    }

    //Si la zona esta activa verifico si solté una carta ahí.
    private void Update()
    {
        if(!_activeZone) return;
        
        _isDropping = Input.GetMouseButtonUp(0);
        DropCard();
        
    }

    /*Si suelto una carta mientras está en fase de descarte la elimino.
    Si suelto una carta mientras está en fase de colocación agrego la carta a la zona correspondiente.*/
    private void DropCard()
    {
        if(_cantCardsInZone >= _cardsInZone.Length || !_isDropping || !_selectedCard || !_selectedCard.gameObject) return;
        _activeZone = false;
        
        _currentPhase = gameManager.GetPhaseManager().CurrentPhase;

        switch (_currentPhase)
        {
            case PhaseManager.GamePhase.Discard:
                if (zoneType == ZoneType.Discard)
                    DiscardCard();
                break;

            case PhaseManager.GamePhase.Colocation:
                AddCardInZone();
                break;
        }
    }

    private void DiscardCard()
    {
        int cardValue = _selectedCard.GetCardDifficulty();
        OnDiscard?.Invoke(cardValue);
        Destroy(_selectedCard.gameObject);
        DisableSlotAndRemoveCard();
        gameManager.GetAudioManager().PlayAudio(AudioManager.AudioList.Discard);
    }

    private void DisableSlotAndRemoveCard()
    {
        int actualSlot = _selectedCard.transform.parent.GetSiblingIndex();
        gameManager.GetPlayersHand().DisableSlot(actualSlot);
        gameManager.GetPlayersHand().RemoveCardFromHand();
    }

    private void AddCardInZone()
    {
        DisableSlotAndRemoveCard();
        
        int cardValue = _selectedCard.GetCardDifficulty();
        Sprite cardSprite = null;
        switch (zoneType)
        {
            case ZoneType.Attack: 
                _selectedCard.ChangeSprite(Card.CardSprites.Attack);
                cardSprite = _selectedCard.GetCardSprite();
                OnCardPlaced?.Invoke(cardSprite, cardValue, ZoneType.Attack);
                break;
            case ZoneType.Defense:
                _selectedCard.ChangeSprite(Card.CardSprites.Defense);
                cardSprite = _selectedCard.GetCardSprite();
                OnCardPlaced?.Invoke(cardSprite, cardValue, ZoneType.Defense);
                break;
        }
        Transform slot = _cardsInZone[_cantCardsInZone].transform;
        _selectedCard.PutCardInSlot(slot);
        
        _cantCardsInZone++;
        gameManager.GetAudioManager().PlayAudio(AudioManager.AudioList.CardColocation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _activeZone = true;
        _selectedCard = other.gameObject.GetComponentInParent<Card>();
        _currentPhase = gameManager.GetPhaseManager().CurrentPhase;
        
        /*if (zoneType == ZoneType.Discard && _currentPhase == PhaseManager.GamePhase.Discard)
        {
            _selectedCard.ToggleAnimator(true);
            _selectedCard.PlayDiscardAnimation();
        }*/
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _activeZone = false;
        //_selectedCard.ToggleAnimator(false);
    }

    #region Utilities
    private void GetZoneChildren()
    {
        int childCount = transform.childCount;
        _cardsInZone = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            _cardsInZone[i] = transform.GetChild(i).gameObject;
        }
    }

    public void ResetZone()
    {
        if (zoneType == ZoneType.Discard) return;
        
        for (int i = 0; i < _cantCardsInZone; i++)
        {
            StartCoroutine(DiscardCardsInZone(i));
        }
        _cantCardsInZone = 0;
    }

    private IEnumerator DiscardCardsInZone(int i)
    {
        Card card = _cardsInZone[i].transform.GetChild(0).GetComponent<Card>();
        Vector3 discardZonePosition = gameManager.GetDiscardZone().transform.position;
        gameManager.GetPlayersHand().MoveCardToPosition(card, discardZonePosition, 0.5f);
        yield return new WaitForSeconds(0.51f);
        Destroy(_cardsInZone[i].transform.GetChild(0).gameObject);
    }

    #endregion

    #region Animations
    public void ToggleLightAnimation()
    {
        _animator.SetTrigger("LightWaving");
    }
    #endregion
}
