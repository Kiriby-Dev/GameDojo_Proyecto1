using System;
using TMPro;
using UnityEngine;

public class PlayerNameTransition : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;

    private void Awake()
    {
        InputName.OnPlayerNameChanged += UpdatePlayerName;
    }

    private void OnDestroy()
    {
        InputName.OnPlayerNameChanged -= UpdatePlayerName;
    }

    private void UpdatePlayerName(string playerNameString)
    {
        if (!playerName) return;
        playerName.text = playerNameString;
    }
}
