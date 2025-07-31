using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public enum AudioList
    {
        RightAnswer,
        WrongAnswer,
        GameOver,
        GameWin,
        Blocked,
        Attack,
        Click,
        CardColocation,
        Discard,
        Heal,
        Hurt
    }

    [SerializeField] private List<AudioClip> audioClips;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioMixer mainMixer;

    private bool _masterMuted;
    private bool _sfxMuted;
    private bool _musicMuted;

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
}
