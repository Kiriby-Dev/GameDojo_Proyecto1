using System;
using TMPro;
using UnityEngine;

public class InputName : MonoBehaviour
{
    public static event Action<string> OnPlayerNameChanged;

    [SerializeField] private TextMeshProUGUI playerNameText;
    
    public void UpdatePlayerName()
    {
        string playerName = playerNameText.text;
        OnPlayerNameChanged?.Invoke(playerName);
    }
}
