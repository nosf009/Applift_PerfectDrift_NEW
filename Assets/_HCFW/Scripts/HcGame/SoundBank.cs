using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBank : MonoBehaviour
{
    #region Singleton
    private static SoundBank _instance;
    public static SoundBank Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SoundBank>();
            return _instance;
        }
    }
    #endregion

    #region AudioClips

    public AudioClip winLevelSound;
    public AudioClip loseLevelSound;

    //Rest of the clips are defined game spesific according to the needs.

    #endregion
}
