using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] clips;
    private AudioSource audioSource;

    #region DefaultMethods
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    #endregion

    #region Static

    public static void Play_Sound(int code)
    {
        FindObjectOfType<AudioManager>().PlaySound(code);
    }

    #endregion

    #region Control

    public void PlaySound(int code)
    {
        audioSource.PlayOneShot(clips[code]);
    }
    #endregion
}
