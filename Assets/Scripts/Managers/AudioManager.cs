using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public enum AudioList
    {

    }

    [SerializeField] private List<AudioClip> audioClips;
    [SerializeField] private Sprite imageEnabled;
    [SerializeField] private Sprite imageDisabled;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioMixer mainMixer;

    private bool _masterMuted;
    private bool _sfxMuted;
    private bool _musicMuted;
    private bool _isEnabled;

    public void PlayAudio(AudioList clipName, float pitch = 1.0f, bool oneShoot = true)
    {
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(audioClips[(int)clipName]);
    }

    public void PlayAudioPitched(AudioList clipName, bool oneShoot = true)
    {
        float randomPitch = sfxSource.pitch = Random.Range(0.9f, 1.1f);
        PlayAudio(clipName, randomPitch, oneShoot);
    }

    public void ToggleMusic()
    {
        _musicMuted = !_musicMuted;
        SetMixerState("MusicVolume", _musicMuted);
    }

    public void ToggleSFX()
    {
        _sfxMuted = !_sfxMuted;
        SetMixerState("SFXVolume", _sfxMuted);
    }

    public void SetMasterVolume(float volume)
    {
        mainMixer.SetFloat("MasterVolume", volume);
    }

    public void ToggleMaster()
    {
        _masterMuted = !_masterMuted;
        SetMixerState("MasterVolume", _masterMuted);
    }

    private void SetMixerState(string parameter, bool muted)
    {
        mainMixer.SetFloat(parameter, (muted) ? -80.0f : 0.0f);
    }

    public void ChangeSprite(Button button)
    {
        Image img = button.gameObject.GetComponent<Image>();

        if (img.sprite.name == imageEnabled.name)
        {
            img.sprite = imageDisabled;
            img.color = Color.red;
        }
        else
        {
            img.sprite = imageEnabled;
            img.color = Color.green;
        }
    }
}
