using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    private void Start()
    {
        ResetStats();
    }

    //Le añade ataque o defensa al jugador con el valor correspondiente.
    public void AddStats(GameObject card, int value)
    {
        Image cardImage = card.transform.GetComponentInChildren<Image>();
        switch (cardImage.sprite.name)
        {
            case "CartaV3.4_0": //Este chequeo es un DESASTRE hay que arreglarlo después :)
                AddAttack(value);
                break;
            case "CartaV3.5_0":
                AddDefense(value);
                break;
        }
        gameManager.GetUIManager().UpdateStats(CurrentAttack, CurrentDefense, "Player");
    }

    #region Utilities
    public void ResetStats()
    {
        CurrentAttack = 0;
        CurrentDefense = 0;
        gameManager.GetUIManager().UpdateStats(CurrentAttack, CurrentDefense, "Player");
    }
    
    private void AddAttack(int value)
    {
        CurrentAttack += value;
    }

    private void AddDefense(int value)
    {
        CurrentDefense += value;
    }
    #endregion
}
