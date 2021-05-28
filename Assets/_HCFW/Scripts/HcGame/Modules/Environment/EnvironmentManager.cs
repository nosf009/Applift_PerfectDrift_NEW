using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{

    #region Singleton
    private static EnvironmentManager _Instance;
    public static EnvironmentManager Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<EnvironmentManager>();
            return _Instance;
        }
    }
    #endregion

    [Header("Levels per theme")]
    public int themeCycleLenght = 5;

    [Header("Available Themes")]
    public List<EnvironmentTheme> themes = new List<EnvironmentTheme>();

    //runtime objects
    GameObject spawnedThemePrefab = null;
    [Header("Current Theme Debug")]
    public EnvironmentTheme currentTheme;

    //[Header("Road References")]
    //public Material roadUpperMaterial;
    //public Material roadSideMaterial;

    [Header("Debug")]
    public int themeIndex = 0;

    void Start()
    {
        // not having this class auto init with start, instead via Init() which will be called by GameManager
        // because we need to have theme sorted in PrepGameAtCurrentLevel already
    }

    public void Init()
    {
        CalculateThemeIndex();
        SetTheme();
    }

    void CalculateThemeIndex()
    {
        int curLevelIndex = HCFW.GameManager.Instance.LevelManager.currentLevel - 1;
        int maxLevelsInThemeCyclesIndex = (themeCycleLenght * themes.Count) - 1;
        if (curLevelIndex > maxLevelsInThemeCyclesIndex)
        {
            curLevelIndex = (curLevelIndex % maxLevelsInThemeCyclesIndex) - 1;
        }
        themeIndex = Mathf.FloorToInt(curLevelIndex / themeCycleLenght);
        if (themeIndex < 0) { themeIndex = 0; }
        if (themeIndex >= themes.Count)
        {
            themeIndex = themes.Count - 1; // if we end up in case there is no themes for a level (config WIP or so) - use last theme
        }
        currentTheme = themes[themeIndex];
    }

    public void SetRandomTheme()
    {
        themeIndex = Random.Range(0, themes.Count);
        ApplyTheme(themeIndex);
    }

    public void SetTheme(int index = -1)
    {
        if (index != -1)
        {
            themeIndex = index;
        }
        ClearEnvironment();
        ApplyTheme(themeIndex);
    }

    public void ApplyTheme(int themeIndex)
    {
        // Set new theme code
        return; // do custom code here
        //GameObject clec = GameObject.FindGameObjectWithTag("CurLevEnv");
        //if (clec != null)
        //{
        //    currentTheme.themeContainer = clec.transform;
        //}
        ClearEnvironment();

        spawnedThemePrefab = Instantiate(currentTheme.environmentPrefab, currentTheme.themeContainer);
        RenderSettings.fogColor = currentTheme.fogColor;
        RenderSettings.fogStartDistance = currentTheme.fogStartDistance;
        RenderSettings.fogEndDistance = currentTheme.fogEndDistance;
        RenderSettings.skybox = currentTheme.skyBoxMaterial;
        //roadUpperMaterial.mainTexture = currentTheme.roadTexture;
        //roadSideMaterial.SetColor("_Color", currentTheme.roadSideColor);
    }

    private void ClearEnvironment()
    {
        int counter = currentTheme.themeContainer.childCount;
        for (int i = counter - 1; i > 0; i--)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }

}


[System.Serializable]
public class CycleData
{
    // like, a teme ID or so
    public int themeID;
    public string themeName;
}

[System.Serializable]
public class EnvironmentTheme
{
    [Header("Theme UI Display")]
    public string themeName;
    public Sprite themeIcon;

    [Header("Env Container & Prefab")]
    public Transform themeContainer;
    public GameObject environmentPrefab;

    //[Header("Road")]
    //public Texture roadTexture;
    //public Color roadSideColor;

    [Header("Sky & Fog")]
    public Material skyBoxMaterial;
    public Color fogColor;
    public float fogStartDistance;
    public float fogEndDistance;
    // and any optional / needed variables
}
