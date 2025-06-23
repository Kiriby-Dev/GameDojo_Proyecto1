using UnityEngine;

public class PlayersHand : MonoBehaviour
{
    public GameManager gameManager;
    
    private int _cardsCount;
    private float _cardSize = 2.5f;
    private float _startPosition;

    private Card grabbedCard = null; // Carta que está agarrada

    void Start()
    {
        Recalculate();
    }

    public void SetGrabbedCard(Card card)
    {
        grabbedCard = card;
        Recalculate();
    }

    public void ClearGrabbedCard()
    {
        grabbedCard = null;
        Recalculate();
    }

    public void Recalculate()
    {
        _cardsCount = gameManager.CardsCountInHand();

        SetCardsOrder();
        SetPositions();
    }

    private void SetCardsOrder()
    {
        int i = 0;
        foreach (Transform cardTransform in transform)
        {
            Card card = cardTransform.GetComponent<Card>();
            if (card == grabbedCard)
                continue; // Saltar carta agarrada

            card.SetCardOrder(i * 2);
            i++;
        }
    }

    private void SetPositions()
    {
        int i = 0;
        int visibleCardsCount = grabbedCard ? _cardsCount - 1 : _cardsCount;

        if (visibleCardsCount == 0)
            return;

        bool isEven = visibleCardsCount % 2 == 0;
        float halfCount = Mathf.Floor(visibleCardsCount * 0.5f);

        if (!isEven)
            _startPosition = -1 * (_cardSize * halfCount);
        else
            _startPosition = -1 * (_cardSize * halfCount - _cardSize / 2f);

        foreach (Transform cardTransform in transform)
        {
            Card card = cardTransform.GetComponent<Card>();
            if (card == grabbedCard)
                continue; // No mover carta agarrada

            Vector3 cardPos = cardTransform.position;
            cardTransform.position = new Vector3(_startPosition + (i * _cardSize), cardPos.y, cardPos.z);

            card.SetCardStartPosition();
            i++;
        }
    }
}
