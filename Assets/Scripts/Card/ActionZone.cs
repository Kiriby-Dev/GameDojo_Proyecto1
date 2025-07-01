using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
                    gameManager.RemoveCardFromHand();
                }
                break;

            case PhaseManager.GamePhase.Colocation:
                AddCardInZone();
                gameManager.RemoveCardFromHand();
                break;
        }
    }
    
    private void AddCardInZone()
    {
        int spriteIndex = 0;
        switch (zoneType)
        {
            case ZoneType.Attack: 
                _selectedCard.ChangeSprite(1);
                spriteIndex = 1;
                break;
            case ZoneType.Defense:
                _selectedCard.ChangeSprite(2);
                spriteIndex = 2;
                break;
        }
        _selectedCard.PutCardInSlot(_cardsInZone[_cantCardsInZone].transform);
        CopyCardToBoard(spriteIndex);
        _cantCardsInZone++;
    }

    private void CopyCardToBoard(int spriteIndex)
    {
        Transform zone = gameManager.GetActualZoneFromBoard();
        Card cardClone = Instantiate(_selectedCard, zone);
        cardClone.ChangeSprite(spriteIndex);
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
            Destroy(_cardsInZone[i].transform.GetChild(0).gameObject);
        }
        _cantCardsInZone = 0;
    }
    #endregion

    #region Getters
    public GameObject GetActualCardZone(int i) => _cardsInZone[i];
    public ZoneType GetZoneType() => zoneType;
    #endregion
}
