using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionZone : MonoBehaviour
{
    public GameManager gameManager;
    public enum ZoneType { Attack, Defense , Discard, Cards}
    public ZoneType zoneType;
    
    private GameObject[] _cardsInZone;
    private int _cantCardsInZone = 0;
    private Card _selectedCard;
    private bool _isDropping;
    private bool _activeZone;
    private int _cardsDiscarded;
    private Animator _animator;

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
        
        var currentPhase = gameManager.GetPhaseManager().CurrentPhase;

        switch (currentPhase)
        {
            case PhaseManager.GamePhase.Discard:
                if (zoneType == ZoneType.Discard)
                {
                    Destroy(_selectedCard.gameObject);
                    _cardsDiscarded++;
                    gameManager.GetUIManager().UpdateDiscardText(_cardsDiscarded);
                    DisableSlotAndRemoveCard();
                }
                break;

            case PhaseManager.GamePhase.Colocation:
                DisableSlotAndRemoveCard();
                AddCardInZone();
                break;
        }
    }

    private void DisableSlotAndRemoveCard()
    {
        gameManager.GetPlayersHand().DisableSlot(_selectedCard.transform.parent.GetSiblingIndex());
        gameManager.RemoveCardFromHand();
    }

    private void AddCardInZone()
    {
        switch (zoneType)
        {
            case ZoneType.Attack: 
                _selectedCard.ChangeSprite(1);
                break;
            case ZoneType.Defense:
                _selectedCard.ChangeSprite(2);
                break;
        }
        _selectedCard.PutCardInSlot(_cardsInZone[_cantCardsInZone].transform);
        CopyCardToBoard();
        _cantCardsInZone++;
        gameManager.SaveCardType(zoneType);
        gameManager.PlayCard();
    }

    private void CopyCardToBoard()
    {
        GameObject card = gameManager.GetActualCardForQuestion();
        Image cardImage = card.GetComponent<Image>();
        TextMeshProUGUI cardText = card.GetComponentInChildren<TextMeshProUGUI>();
        cardImage.sprite = _selectedCard.GetComponentInChildren<SpriteRenderer>().sprite;
        cardText.text = _selectedCard.GetComponentInChildren<TextMeshProUGUI>().text;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _activeZone = true;
        _selectedCard = other.gameObject.GetComponentInParent<Card>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _activeZone = false;
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
        for (int i = 0; i < _cantCardsInZone; i++)
        {
            StartCoroutine(DiscardCardInZone(i));
        }
        _cantCardsInZone = 0;
    }

    private IEnumerator DiscardCardInZone(int i)
    {
        Card card = _cardsInZone[i].transform.GetChild(0).GetComponent<Card>();
        Vector3 discardZonePosition = gameManager.GetDiscardZone().transform.position;
        gameManager.GetPlayersHand().MoveCardToPosition(card, discardZonePosition, 0.5f);
        yield return new WaitForSeconds(0.51f);
        Destroy(_cardsInZone[i].transform.GetChild(0).gameObject);
    }

    #endregion

    #region Getters
    public GameObject GetActualCardZone(int i) => _cardsInZone[i];

    public void ResetDiscardedCards()
    { 
        _cardsDiscarded = 0;
        gameManager.GetUIManager().UpdateDiscardText(_cardsDiscarded);
    }
    #endregion

    #region Animations

    public void PlayDiscardAnimation()
    {
        
    }

    public void ToggleLightAnimation()
    {
        _animator.SetTrigger("LightWaving");
    }

    #endregion
}
