using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAudioManager : MonoBehaviour
{
    //This script will be incharge of all unit based audio
    //Place a copy of this script on all units in game along with an audio source
    //Get any Overall Settings from the audio manager

    public AudioSource audioSource;
    public List<AudioClip> audioClips;

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


}
