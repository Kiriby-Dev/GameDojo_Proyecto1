using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private SliderType sliderType;
    
    private Slider _slider;
    private AudioManager _audioManager;
    
    private enum SliderType
    {
        Music,
        SFX
    }

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _audioManager = GameManager.Instance.GetAudioManager();
    }

    private void Start()
    {
        float savedValue = PlayerPrefs.GetFloat(sliderType.ToString(), 1f);
        _slider.SetValueWithoutNotify(savedValue);
        SetVolume(savedValue);

        _slider.onValueChanged.AddListener(SetVolume);
    }
    
    private void SetVolume(float value)
    {
        // Convierte de escala lineal (0-1) a decibelios (-80 a 0)
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        
        if (sliderType == SliderType.Music)
            _audioManager.SetMusicVolume(dB);
        else if (sliderType == SliderType.SFX)
            _audioManager.SetSFXVolume(dB);

        // Guarda el valor para la próxima vez
        PlayerPrefs.SetFloat(sliderType.ToString(), value);
    }
}
