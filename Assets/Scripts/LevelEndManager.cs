using HCFW;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelEndManager : MonoBehaviour
{

    #region Singleton
    private static LevelEndManager _Instance;
    public static LevelEndManager Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<LevelEndManager>();
            return _Instance;
        }
    }
    #endregion

    public Transform moveToThisTransform;
    public bool smoothRot = false;
    private float dampen;

    public void OnPlayerEnteredWinTrigger()
    {
        if (GameManager.Instance.state != HCFW.GameState.InGame) return;
        GameManager.Instance.WinGameAtCurrentLevel();
        GameManager.Instance.state = GameState.InEOL;
        StartCoroutine(StartEOL());
    }

    public IEnumerator StartEOL()
    {
        dampen = 15f;
        CanvasGroup cgGameView = HCFW.GameManager.Instance.MenuManager.gameView.gameObject.AddComponent<CanvasGroup>();
        cgGameView.alpha = 1f;
        cgGameView.DOFade(0f, 1f);
        moveToThisTransform = GameObject.FindGameObjectWithTag("DOTweenPath").transform;
        HCFW.GameManager.Instance.tcv.GetComponent<LeanTinyInput>().enabled = false;
        HCFW.GameManager.Instance.tcv.GetComponent<CarAiHelper>().enabled = true;
        DOTween.To(() => GameManager.Instance.vcc.smoothFollowSettings.distance, x => GameManager.Instance.vcc.smoothFollowSettings.distance = x, 55f, 2f).SetUpdate(true);
        DOTween.To(() => GameManager.Instance.vcc.smoothFollowSettings.height, x => GameManager.Instance.vcc.smoothFollowSettings.height = x, 25f, 2f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(GameManager.Instance.firstEOLDelay);
        Transform moveEnd = GameObject.FindGameObjectWithTag("EndCam").transform;
        yield return new WaitForSecondsRealtime(1f);
        GameManager.Instance.vcc.enabled = false;
        smoothRot = true;
        GameManager.Instance.gameCamera.transform.DOMove(moveEnd.transform.position, 1f).SetUpdate(true).SetEase(Ease.InOutSine);
        yield return new WaitForSecondsRealtime(1f);
        GameManager.Instance.state = GameState.FinishedEOL;
        smoothRot = false;
        GameManager.Instance.gameCamera.transform.DORotate(moveEnd.transform.rotation.eulerAngles, 2f).SetUpdate(true).SetEase(Ease.OutSine);
        yield return new WaitForSecondsRealtime(GameManager.Instance.thirdEOLDelay);
        GameManager.Instance.MenuManager.EnableView(GameManager.Instance.MenuManager.winView);
    }



    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }

    public void LateUpdate()
    {
        var rotation = Quaternion.LookRotation(HCFW.GameManager.Instance.vcc.target.position - HCFW.GameManager.Instance.gameCamera.transform.position);
        var rotation2 = Quaternion.Slerp(HCFW.GameManager.Instance.gameCamera.transform.rotation, rotation, Time.deltaTime * dampen);
        if (smoothRot)
        {
            GameManager.Instance.gameCamera.transform.rotation = rotation2;
        }
    }
}
