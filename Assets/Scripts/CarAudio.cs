using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource skidAudioSource;

    public List<AudioClip> skidAudioClips = new List<AudioClip>();

    public float minPitch = 0.05f;
    public float pitchFromCar;
    public float divider;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.pitch = minPitch;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.pitch = minPitch;
            audioSource.Play();
        }
        if (HCFW.GameManager.Instance)
        {
            if (HCFW.GameManager.Instance.tcc != null)
            {
                pitchFromCar = HCFW.GameManager.Instance.tcc.body.velocity.magnitude / divider;
            }
            if (pitchFromCar < minPitch)
            {
                audioSource.pitch = minPitch;

            }
            else
            {
                audioSource.pitch = pitchFromCar;
            }
        }

    }

    public void PlaySkidAudio()
    {
        if (!skidAudioSource.isPlaying)
        {
            skidAudioSource.clip = skidAudioClips[Random.Range(0, skidAudioClips.Count)];
            skidAudioSource.pitch = Random.Range(0.85f, 1.15f);
            skidAudioSource.Play();
        }
    }


}
