using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource audioSource;
    public List<AudioClip> audioClips;

    public AudioSource musicSource;
    private bool isMuted = false;

    public void PlaySound(string clipName)
    {
        AudioClip clip = audioClips.Find(c => c.name == clipName);

        if (clip != null)
        {
            // Check if the audio source is already playing this specific clip
            if (!audioSource.isPlaying || audioSource.clip != clip)
            {
                audioSource.clip = clip; // Set the clip (optional, depending on your setup)
                audioSource.PlayOneShot(clip); // Play the sound
            }
            else
            {
                Debug.Log("Clip is already playing: " + clipName);
            }
        }
        else
        {
            Debug.LogWarning("Audio clip not found: " + clipName);
        }
    }

    public void PlayMusic(string musicName)
    {
        AudioClip musicClip = audioClips.Find(c => c.name == musicName);
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music clip not found: " + musicName);
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    public float GetFXVolume()
    {
        return audioSource.volume;
    }

    public void SetFXVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    public float GetMusicVolume()
    {
        return musicSource.volume;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void Mute()
    {
        isMuted = true;
        audioSource.mute = true;
        musicSource.mute = true;
    }

    public void Unmute()
    {
        isMuted = false;
        audioSource.mute = false;
        musicSource.mute = false;
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    public void PauseSound()
    {
        audioSource.Pause();
    }

    public void UnpauseSound()
    {
        audioSource.UnPause();
    }

    public void CrossfadeMusic(string newMusicName, float duration)
    {
        StartCoroutine(CrossfadeMusicCoroutine(newMusicName, duration));
    }

    private IEnumerator CrossfadeMusicCoroutine(string newMusicName, float duration)
    {
        AudioClip newClip = audioClips.Find(c => c.name == newMusicName);
        if (newClip == null)
        {
            Debug.LogWarning("Music clip not found: " + newMusicName);
            yield break;
        }

        float startVolume = musicSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / duration);
            yield return null;
        }

        musicSource.volume = startVolume;
    }


}
