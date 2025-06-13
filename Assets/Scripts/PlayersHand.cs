using UnityEngine;

public class PlayersHand : MonoBehaviour
{
    private int _cardsCount;
    private float _cardSize = 2.5f;
    private float _startPosition;
    
    void Start()
    {
        Recalculate();
    }

    public void Recalculate()
    {
        _cardsCount = transform.childCount;

        SetCardsOrder();
        SetPositions();
    }

    private void SetCardsOrder()
    {
        int i = 0;
        foreach (var card in transform)
        {
            Transform currentCard = (Transform) card;
            currentCard.GetComponent<Card>().SetCardOrder(i * 2); 
            /*Ordenamos las cartas solo en los pares pues los layers impares estan reservados para los textos
            (la carta en el lugar 0, tiene su texto en el lugar 1, la carta en el lugar 2 tiene su texto en el lugar 3 y asi)*/
            i++;
        }
    }

    private void SetPositions()
    {
        int i = 0;
        bool isEven = _cardsCount % 2 == 0;

        float halfCount = Mathf.Floor(_cardsCount * 0.5f);

        if (!isEven)
        {
            _startPosition = -1 * (_cardSize * halfCount);
        }
        else
        {
            _startPosition = -1 * (_cardSize * halfCount - _cardSize / 2f);
        }

        foreach (var card in transform)
        {
            Transform currentCard = (Transform) card;
            
            Vector3 cardPos = currentCard.position;
            currentCard.position = new Vector3(_startPosition + (i * _cardSize), cardPos.y, cardPos.z);
            
            currentCard.GetComponent<Card>().SetCardStartPosition();
            i++;
        }
    }

    private void DiscardCard()
    {
        
    }
}
