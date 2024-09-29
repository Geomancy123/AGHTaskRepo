using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    //AudioSource
    [SerializeField]private AudioSource audioSource;

    //Audio clips
    [SerializeField] private AudioClip upAudio;
    [SerializeField] private AudioClip downAudio;
    [SerializeField] private AudioClip leftAudio;
    [SerializeField] private AudioClip rightAudio;
    [SerializeField] private AudioClip movingAudio;

    private List<AudioClip> audioClipSequence;

    private Coroutine audioSequenceCoruotine;
    private bool bPlayingAudio = false;

    private void Awake()
    {
        //Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    //Will be called by Player
    public void PlayMovementAudio(E_Direction _direction)
    {
        //stop the current audio
        audioSource.Stop();

        //renewing the audio list
        audioClipSequence = new List<AudioClip>();
        audioClipSequence.Add(movingAudio);

        //set the directional audio
        switch (_direction)
        {
            case E_Direction.LEFT:
                audioClipSequence.Add(leftAudio);
                break;
            case E_Direction.RIGHT:
                audioClipSequence.Add(rightAudio);
                break;
            case E_Direction.UP:
                audioClipSequence.Add(upAudio);
                break;
            case E_Direction.DOWN:
                audioClipSequence.Add(downAudio);
                break;

            default:
                Debug.Log("enum not valid @AudioManager->PlayAudioClip");
                break;
        }

        //if audio is currently playing, stop the current audo coroutine
        if(bPlayingAudio)
        {
            StopCoroutine(audioSequenceCoruotine);
        }

        //play the audio sequence
        audioSequenceCoruotine = StartCoroutine(PlayAudioSequence());
    }

    //play the audio sequence
    IEnumerator PlayAudioSequence()
    {
        //Set playing audio bool
        bPlayingAudio = true;

        //Play every audio clip int he sequence
        for(int i = 0; i < audioClipSequence.Count; i++)
        {
            audioSource.clip = audioClipSequence[i];

            audioSource.Play();

            while(audioSource.isPlaying)
            {
                yield return null;
            }
        }

        //Set playing audio bool
        bPlayingAudio = false;
    }
    
    public void FadeAudioOut()
    {

    }
    
}
