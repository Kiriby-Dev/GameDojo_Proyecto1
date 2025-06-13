using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionZone : MonoBehaviour
{
    private GameObject[] _cardsInZone;
    private int _cardsInZoneCount = 0;
    private bool _isDropping;
    private bool _activeZone;
    private Card _selectedCard;
    
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
        if(_cardsInZoneCount >= _cardsInZone.Length) return;
        if (!_isDropping) return;
        _activeZone = false;
        
        if(_selectedCard && _selectedCard.gameObject)
        {
            PutCardInZone();
        }
    }

    private void PutCardInZone()
    {
        _selectedCard.SetReturning(false);
        _selectedCard.gameObject.GetComponent<Collider2D>().enabled = false;
        
        Transform card = _selectedCard.gameObject.transform;
        card.Find("DropZone").gameObject.SetActive(false);
        card.position = _cardsInZone[_cardsInZoneCount].transform.position;
        card.localScale = new Vector3(0.6f, 0.6f, 0);
        _cardsInZoneCount++;
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

    private void GoThroughCards()
    {
        
    }
}
