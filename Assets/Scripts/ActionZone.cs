using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    
    private void Start()
    {
        GetZoneChildren();
    }
    
    private void Update()
    {
        if(!_activeZone) return;
        
        _isDropping = Input.GetMouseButtonUp(0);
        DropCard();
        
    }

    private void GetZoneChildren()
    {
        int childCount = transform.childCount;
        _cardsInZone = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            _cardsInZone[i] = transform.GetChild(i).gameObject;
        }
    }

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
                    StartCoroutine(RecalculateNextFrame());
                }
                break;

            case PhaseManager.GamePhase.Colocation:
                AddCardInZone();
                break;
        }
    }
    
    private IEnumerator RecalculateNextFrame()
    {
        yield return new WaitForEndOfFrame();
        gameManager.GetPlayersHand().Recalculate();
    }

    public void ResetZone()
    {
        for (int i = 0; i < _cantCardsInZone; i++)
        {
            Destroy(_cardsInZone[i].transform.GetChild(0).gameObject);
        }
        _cantCardsInZone = 0;
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
        _cantCardsInZone++;
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
    
    public int GetCantCardsInZone() => _cantCardsInZone;
    public GameObject GetActualCardZone(int i) => _cardsInZone[i];
    public ZoneType GetZoneType() => zoneType;
}
