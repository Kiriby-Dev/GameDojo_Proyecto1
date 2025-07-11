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
    private ZoneType[] _cardsInBoard;
    private int _cardsPlayed;
    
    private void Start()
    {
        GetZoneChildren();
        _cardsInBoard = new ZoneType[3];
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
        _cardsPlayed = gameManager.GetCantPlayedCards();
        print(_cardsPlayed);
        switch (zoneType)
        {
            case ZoneType.Attack: 
                _selectedCard.ChangeSprite(1);
                _cardsInBoard[_cardsPlayed] = ZoneType.Attack;
                print(_cardsInBoard[_cardsPlayed].ToString());
                break;
            case ZoneType.Defense:
                _selectedCard.ChangeSprite(2);
                _cardsInBoard[_cardsPlayed] = ZoneType.Defense;
                print(_cardsInBoard[_cardsPlayed].ToString());
                break;
        }
        _selectedCard.PutCardInSlot(_cardsInZone[_cantCardsInZone].transform);
        CopyCardToBoard();
        _cantCardsInZone++;
        gameManager.PlayCard();
    }

    public ZoneType GetCardType(int position)
    {
        return _cardsInBoard[position];
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
            Destroy(_cardsInZone[i].transform.GetChild(0).gameObject);
        }
        _cantCardsInZone = 0;
        _cardsPlayed = 0;
    }
    #endregion

    #region Getters
    public GameObject GetActualCardZone(int i) => _cardsInZone[i];
    #endregion
}
