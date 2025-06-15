using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionZone : MonoBehaviour
{
    public GameManager gameManager;
    public QuestionManager questionManager;
    
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
        if(_cantCardsInZone >= _cardsInZone.Length) return;
        if (!_isDropping) return;
        _activeZone = false;
        
        if(_selectedCard && _selectedCard.gameObject)
        {
            string _phase = gameManager.CheckPhase();
            if (_phase == "Discard")
            {
                Destroy(_selectedCard.gameObject);
                gameManager.GetPlayersHand().Recalculate();
            }

            if (_phase == "Colocation")
                AddCardInZone();
        }
    }

    private void AddCardInZone()
    {
        _cardsInZone = _selectedCard.PutCardInZone(_cardsInZone, _cantCardsInZone);
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
    public GameObject GetActualCard(int i) => _cardsInZone[i];
}
