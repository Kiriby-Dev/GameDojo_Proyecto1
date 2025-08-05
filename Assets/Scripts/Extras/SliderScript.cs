using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    private Slider _slider;
    private AudioManager _audioManager;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void Start()
    {
        _audioManager = GameManager.Instance.GetAudioManager();
        
        _slider.onValueChanged.AddListener((value) =>
        {
            _audioManager.SetMasterVolume(value);
        });
    }
}
