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
    public bool smoothFollow = false;
    public Transform moveEnd;
    private float dampen;

    public void OnPlayerEnteredWinTrigger()
    {
        if (GameManager.Instance.state != HCFW.GameState.InGame) return;
        ActivateSteeringWheelSpecial(); // activate the steering wheel & slowmo
    }

    // this goes to the EOL coroutine, where the player car drifts etc
    public void GoToEOL(SteerArea pickedSteerArea)
    {
        GameManager.Instance.WinGameAtCurrentLevel();
        GameManager.Instance.state = GameState.InEOL;
        HCFW.GameManager.Instance.tcv.GetComponent<CarAiHelper>().InitSelf(pickedSteerArea);
        StartCoroutine(StartEOL(pickedSteerArea));
    }


    public void ActivateSteeringWheelSpecial()
    {

        // generating the green position and the others, either left or right
        if (UnityEngine.Random.value < 0.5f)
        {
            HCFW.GameManager.Instance.MenuManager.PickGreenPosition(true);
        }
        else
        {
            HCFW.GameManager.Instance.MenuManager.PickGreenPosition(false);
        }
        // start the coroutine for the steering wheel to appear
        HCFW.GameManager.Instance.OnEnterTurnTrigger();
    }

    public IEnumerator StartEOL(SteerArea pickedSteerArea)
    {

        Debug.Log("::<Color=White> Player picked " + pickedSteerArea.ToString() + " ::</color>");
        /*
        // do something with according to picked steer area
        /////////////////////////////////////////////////
        switch (pickedSteerArea)
        {
            case SteerArea.Green:
                break;
            case SteerArea.Orange:
                break;
            case SteerArea.Red:
                break;
            case SteerArea.Invalid:
                break;
            default:
                break;
        }
        ////////////////////////////////////////////////
        */

        HCFW.GameManager.Instance.MenuManager.eolTextEnd.DOColor(Color.clear, .5F).SetDelay(2.5F);
        dampen = 8f;
        CanvasGroup cgGameView = HCFW.GameManager.Instance.MenuManager.gameView.gameObject.AddComponent<CanvasGroup>();
        cgGameView.alpha = 1f;
        cgGameView.DOFade(0f, 1f);
        //moveToThisTransform = GameObject.FindGameObjectWithTag("DOTweenPath").transform;
        //HCFW.GameManager.Instance.tcv.GetComponent<LeanTinyInput>().enabled = false;
        //HCFW.GameManager.Instance.tcv.GetComponent<CarAiHelper>().InitSelf(pickedSteerArea); // init self (also activate)
        DOTween.To(() => GameManager.Instance.vcc.smoothFollowSettings.distance, x => GameManager.Instance.vcc.smoothFollowSettings.distance = x, 60f, 1f).SetUpdate(true);
        DOTween.To(() => GameManager.Instance.vcc.smoothFollowSettings.height, x => GameManager.Instance.vcc.smoothFollowSettings.height = x, 30f, 1f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(1f);

        moveEnd = GameObject.FindGameObjectWithTag("EndCam").transform;
        yield return new WaitForSecondsRealtime(1f);
        GameManager.Instance.vcc.enabled = false;
        smoothFollow = true;
        //GameManager.Instance.gameCamera.transform.DOMove(moveEnd.transform.position, .8f).SetUpdate(true).SetEase(Ease.InOutSine);
        List<Vector3> path = new List<Vector3>();
        path.Add(GameManager.Instance.gameCamera.transform.position /*- new Vector3(0f, 1f, 0f)*/);
        //path.Add(GameManager.Instance.tcv.vehicleBody.position + new Vector3(0f, 3f, 0f));
        path.Add(moveEnd.position);
        GameManager.Instance.gameCamera.transform.DOPath(path.ToArray(), 2.5f, PathType.CatmullRom, PathMode.Full3D).SetUpdate(true).SetEase(Ease.OutSine);
        yield return new WaitForSecondsRealtime(.3f);
        smoothRot = true;
        yield return new WaitForSecondsRealtime(.7f);
        GameManager.Instance.state = GameState.FinishedEOL;
        smoothRot = false;
        GameManager.Instance.gameCamera.transform.DORotate(moveEnd.transform.rotation.eulerAngles, 1.75f).SetUpdate(true).SetEase(Ease.OutSine);
        /*
        yield return new WaitForSecondsRealtime(GameManager.Instance.thirdEOLDelay);
        GameManager.Instance.MenuManager.EnableView(GameManager.Instance.MenuManager.winView);
        */
    }



    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
     
    }

    private void FixedUpdate()
    {
        var rotation = Quaternion.LookRotation(HCFW.GameManager.Instance.vcc.target.position - HCFW.GameManager.Instance.gameCamera.transform.position);
        var rotation2 = Quaternion.Slerp(HCFW.GameManager.Instance.gameCamera.transform.rotation, rotation, Time.deltaTime * dampen);
        if (smoothRot)
        {
            GameManager.Instance.gameCamera.transform.rotation = rotation2;
        }
    }

    public void LateUpdate()
    {
       



    }
}
