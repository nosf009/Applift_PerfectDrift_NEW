using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HCFW
{
    public class AudioManager : MonoBehaviour
    {

        public Image home_SoundButtonImage;
        public Sprite soundOffSprite;
        public Sprite soundOnSprite;

        private AudioSource audioPlayer;

        public bool AudioPaused()
        {
            return AudioListener.pause;
        }

        // Use this for initialization
        void Start()
        {
            audioPlayer = transform.GetComponentInChildren<AudioSource>();

            if (PlayerPrefs.HasKey(GameManager.Instance.saveKey + "sounds"))
            {
                int state = PlayerPrefs.GetInt(GameManager.Instance.saveKey + "sounds");
                //Debug.Log(state);
                bool audioPaused = false;
                if (state == 1)
                {
                    audioPaused = true;
                }
                ToggleAudioMute(audioPaused, true);
            }
            else
            {
                PlayerPrefs.SetInt(GameManager.Instance.saveKey + "sounds", 0); // IS PAUSED
            }
        }

        public void ToggleSounds()
        {
            StartCoroutine(ToggleSoundBitDelayed());
        }

        IEnumerator ToggleSoundBitDelayed()
        {

            int state = 0;
            if (AudioPaused())
            {
                yield return new WaitForSeconds(0.15f);
                ToggleAudioMute(false);
            }
            else
            {
                yield return new WaitForSeconds(0.15f);
                ToggleAudioMute(true);
                state = 1;
            }

            PlayerPrefs.SetInt(GameManager.Instance.saveKey + "sounds", state);
        }

        public void ToggleAudioMute(bool state, bool isStart = false)
        {

            if (state == true)
            {
                if (home_SoundButtonImage)
                {
                    home_SoundButtonImage.sprite = soundOffSprite;
                }

                AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
                foreach (AudioSource audioS in allAudioSources)
                {
                    audioS.Stop();
                }

            }
            else
            {

                AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
                foreach (AudioSource audioS in allAudioSources)
                {
                    audioS.Stop();
                }

                if (home_SoundButtonImage)
                {
                    home_SoundButtonImage.sprite = soundOnSprite;
                }
            }

            StartCoroutine(ToggleMuteDelayed(state));

        }

        IEnumerator ToggleMuteDelayed(bool state)
        {
            yield return new WaitForSeconds(0.1f);
            AudioListener.pause = state;
        }

        public void PlayRandomisedSound(List<AudioClip> al, float volume = 0.5f, float pitch = 1f)
        {
            if (al == null || al.Count == 0)
                return;

            audioPlayer.volume = volume;
            audioPlayer.pitch = pitch;
            audioPlayer.PlayOneShot(al[Random.Range(0, al.Count)]);
        }

        public void PlaySound(AudioClip a, float volume = 0.5f, float pitch = 1f)
        {
            if (a == null)
                return;

            audioPlayer.volume = volume;
            audioPlayer.pitch = pitch;
            audioPlayer.PlayOneShot(a);
        }

    }
}


