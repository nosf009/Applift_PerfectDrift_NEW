#if SERVICES_MODULE
using GameAnalyticsSDK;
#endif
using DavidJalbert;
using EVP;
using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HCFW
{
    public enum GameState { NotInGame, InGame, InEOL, FinishedEOL }

    public class GameManager : MonoBehaviour
    {

        #region Singleton
        private static GameManager _Instance;
        public static GameManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = FindObjectOfType<GameManager>();
                return _Instance;
            }
        }
        #endregion

        #region References

        private AudioManager am;
        private LevelManager lm;
        private MenuManager mm;
#if SHOP_MODULE
        private ShopManager sm;
#endif

        public AudioManager AudioManager
        {
            get
            {
                return am;
            }
        }

        public LevelManager LevelManager
        {
            get
            {
                return lm;
            }
        }

        public MenuManager MenuManager
        {
            get
            {
                return mm;
            }
        }

#if SHOP_MODULE
        public ShopManager ShopManager
        {
            get
            {
                return sm;
            }
        }
#endif

        public int LevelScore
        {
            get
            {
                if (previousLevelScores.Count == 0)
                {
                    return levelScore + levelBonusScore;
                }
                else
                {
                    int prevScoresTotal = 0;
                    foreach (int ps in previousLevelScores)
                    {
                        prevScoresTotal += ps;
                    }
                    return levelScore + prevScoresTotal + levelBonusScore;
                }
            }
        }

        #endregion

        #region Private Variables
        private float adCooldown;
        #endregion

        #region Public Variables


        [Header("EOL")]
        public bool startedSlowingDown = false;
        public float gasValue = 1f;

        public float firstEOLDelay;
        public float secondEOLDelay;
        public float thirdEOLDelay;

        public bool noMoreTimeTriggerEntered = false;

        [Header("Steering setting")]
        public float defaultSteering = 250f;
        public float driftSteering = 175f;

        [Header("New Turning Feature")]
        public bool isNewTurningEnabled = false; // new feature
        public bool isInTurn = false;
        public bool canPlayerInput = false;
        public bool receivedInputForTurn = false;
        public bool enteredTrigger = false;

        [NaughtyAttributes.ReadOnly]
        public float steerValueThatWasSet = 0f;

        public float startSequenceDelay;
        public float enteringSlowmoTime;
        public float slomoTargetTime = 0.33f;

        public float defaultCameraDistance;
        public float defaultCameraHeight;
        public float defaultCameraViewHeight;
        public float defaultCameraRotationDampening;

        public float onEnterCameraDistance;
        public float onEnterCameraHeight;
        public float onEnterCameraViewHeight;
        public float onEnterCameraRotationDampening;


        public UnityEngine.UI.Slider turnSlider;

        public float percentOfDurationWhenWeSwitchCameraSettingsBackToNormal;

        [Header("Game Data")]
        public float globalTimeScale;
        public float currentDriftValue = 0f;
        public float totalDriftValue = 0f;
        public int countPickups = 0;
        public bool isSkidding = false;
        public float gettingPointsFactor = 1f;
        public LeanCircleJoystick joystick;
        public TinyCarController tcc;
        public TinyCarVisuals tcv;
        public CarAudio carAudio;
        public SteeringWheel steeringWheel;
        public TinyCarSurfaceParameters parametersToSetUponEntering;

        public bool sendDriftValue = false;
        public bool leftDrift = false;

        [Header("Camera FoV change")]
        public Camera camera;
        public bool isFovChangeEnabled = false;
        public float increaseFactor;
        public float decreaseFactor;
        public float clampMin;
        public float clampMax;

        [Header("Camera stuff")]
        public GameObject gameCamera;
        public VehicleCameraController vcc;

        [Header("Global Game Data")]
        public GameState state;
        public string saveKey = "_gameKey_";
        public float general_ShowNewAdCooldown;
        public float firstSession_ShowNewAdCooldown;

        [Header("Data / Game Profile")]
        public bool isFirstSession = true;
        public PlayerGameProfile activeProfile;

        [Header("Level Debug")]
        public int levelScore = 0;
        public int levelBonusScore = 0;
        public int bestScore = 0;
        public List<int> previousLevelScores = new List<int>();

        #endregion

        #region Initialization


        public void OnEnable()
        {
            // Hook into the events we need
            LeanTouch.OnFingerSet += OnFingerDown;
        }

        public void OnDisable()
        {
            // Unhook the events
            LeanTouch.OnFingerSet -= OnFingerDown;
        }

        bool canIncreaseSlider = true;
        void OnFingerDown(LeanFinger finger)
        {
            if (turnSlider == null)
            {
                return;
            }
            if (isInTurn && canIncreaseSlider)
            {
                steerValueThatWasSet = turnSlider.value;
                canIncreaseSlider = false;
                receivedInputForTurn = true;
            }
        }

        private void Awake()
        {
            am = FindObjectOfType<AudioManager>();
            lm = FindObjectOfType<LevelManager>();
            mm = FindObjectOfType<MenuManager>();
#if SHOP_MODULE
            sm = FindObjectOfType<ShopManager>();
#endif
            InitGameData(); // VERY IMPORTANT TO STAY HERE AND IN AWAKE (JSON cache part, which is used instead player prefs)
            Time.timeScale = globalTimeScale;
            //MenuManager.steeringWheelImage.color = Color.clear;


        }

        public List<GameObject> fountainConfetti = new List<GameObject>();
        public List<GameObject> explodeConfetti = new List<GameObject>();

        public void FetchConfetti()
        {
            fountainConfetti.AddRange(GameObject.FindGameObjectsWithTag("ConfettiFountain"));
            explodeConfetti.AddRange(GameObject.FindGameObjectsWithTag("ConfettiFountain"));
        }

        public void EnableDisableFountainConfetti(bool isOn)
        {
            foreach (GameObject confetti in fountainConfetti)
            {
                if (confetti != null)
                {
                    confetti.SetActive(isOn);
                }
            }
        }

        public void EnableDisableExpodeConfetti(bool isOn)
        {
            foreach (GameObject confetti in explodeConfetti)
            {
                if (confetti != null)
                {
                    confetti.SetActive(isOn);
                }
            }
        }


        [Header("Drift boost")]
        public bool driftBoostEnabled = false;
        public void CheckForDriftValue()
        {
            if (driftBoostEnabled == false)
            {
                if (currentDriftValue >= 100f)
                {
                    //Debug.Log("Enable ADDITIONAL BOOST");
                    driftBoostEnabled = true;
                    StartCoroutine(IncreaseBoost());

                }
            }
        }

        public IEnumerator IncreaseBoost()
        {
            tcv.particlesDRIFT.Play();
            tcv.particlesDRIFT.startSize = 3.5f;

            yield return new WaitForSecondsRealtime(1f);
            tcv.particlesDRIFT.Stop();
            tcv.particlesDRIFT.startSize = 1f;
            driftBoostEnabled = false;
        }


        public void OnSetJoystick(Vector2 delta)
        {
            //Debug.Log(delta);
            var fingers = Lean.Touch.LeanTouch.Fingers;
            //Debug.Log("There are currently " + fingers.Count + " fingers touching the screen.");

            if (isInTurn)
            {
                if (fingers.Count > 0)
                {
                    if (fingers[0].Up)
                    {
                        receivedInputForTurn = true;
                    }
                }
            }
            //
        }


        private void Update()
        {

            //Debug.Log("Body Local Rotation Vec3: " + tcc.body.transform.localRotation.eulerAngles);

            if (steeringWheel == null)
            {
                steeringWheel = FindObjectOfType<SteeringWheel>();
            }

            if (turnSlider != null)
            {
                if (isInTurn)
                {
                    if (!canIncreaseSlider) { return; }
                    if (turnSlider.gameObject.activeInHierarchy == false) { turnSlider.gameObject.SetActive(true); }
                    MenuManager.steeringWheelImage.color = Color.clear;
                    turnSlider.value += (Time.unscaledDeltaTime / 2f) * lastTurnDuration;
                    turnSlider.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                    if (lastTurnMultiplier < 0f)
                    {
                        turnSlider.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                    }
                }
                else
                {
                    if (turnSlider.gameObject.activeInHierarchy == true) { turnSlider.gameObject.SetActive(false); }
                    turnSlider.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                    MenuManager.steeringWheelImage.color = Color.white;
                    turnSlider.value = 0f;
                }
            }

           
            if (tcc != null)
            {
                // Displaying drift value and animating text
                if (MenuManager.driftValueText != null)
                {

                    if (isSkidding)
                    {
                        currentDriftValue += (Mathf.Abs(tcc.inputSteering) * Mathf.Abs(tcc.body.velocity.magnitude)) * gettingPointsFactor * Time.deltaTime;
                        MenuManager.dtAnim.DOPlay();
                        MenuManager.driftValueText.fontSize = Mathf.RoundToInt(Mathf.MoveTowards(MenuManager.driftValueText.fontSize, 110f, 60 * Time.deltaTime));
                        carAudio.PlaySkidAudio();
                        if (!driftBoostEnabled)
                        {
                            tcv.particlesBoost.startSize = 2f;
                        }
                        if (isFovChangeEnabled)
                        {
                            camera.fieldOfView += increaseFactor * Time.deltaTime;
                            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, clampMin, clampMax);
                        }
                    }
                    else
                    {
                        MenuManager.dtAnim.DOPause();
                        MenuManager.driftValueText.color = Color.white;
                        MenuManager.driftValueText.fontSize = Mathf.RoundToInt(Mathf.MoveTowards(MenuManager.driftValueText.fontSize, 80f, 70 * Time.deltaTime));
                        MenuManager.driftValueText.text = "";
                        carAudio.skidAudioSource.Stop();
                        if (!driftBoostEnabled)
                        {
                            tcv.particlesBoost.startSize = .35f;
                        }
                        if (isFovChangeEnabled)
                        {
                            camera.fieldOfView -= decreaseFactor * Time.deltaTime;
                            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, clampMin, clampMax);
                        }
                    }

                }

                CheckForDriftValue();

                if (isSkidding)
                {
                    leftDrift = false;
                    sendDriftValue = false;
                    MenuManager.driftValueText.text = currentDriftValue.ToString("F0");
                }
                else
                {
                    leftDrift = true;
                }

                if (leftDrift && sendDriftValue == false)
                {
                    sendDriftValue = true;
                    totalDriftValue += currentDriftValue;
                    MenuManager.driftValueText.text = "";
                    currentDriftValue = 0;
                    Debug.Log("SEND DRIFT VALUE");
                }
                MenuManager.totalDriftValueText.text = totalDriftValue.ToString("F0");

            }
        }


        public void OnEnterTurnTrigger()
        {
            Debug.Log("<Color=Cyan>Player entered turn trigger!</color>");

            if (!enteredTrigger)
            {
                enteredTrigger = true;
                StartCoroutine(OnEnterCoroutine());
            }

        }

        public void OnExitTurnTrigger()
        {
            Debug.Log("<Color=Cyan>Player exited turn trigger!</color>");
            //StartCoroutine(OnExitCoroutine());
        }




        public bool PlayerPickedSteeringAngle()
        {
            if ((canPlayerInput && receivedInputForTurn) || noMoreTimeTriggerEntered)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        float lastTurnDuration = 0f;
        float lastTurnMultiplier = 1f;


        public float timeInCoroutine = 0f;

        public IEnumerator OnEnterCoroutine()
        {
            isInTurn = true;
            float startingRotationDamping = vcc.smoothFollowSettings.rotationDamping;
          
            // move steering wheel & colored parts to the middle of screen
            MenuManager.orangePosition.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            MenuManager.orangePosition.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

            MenuManager.redPosition.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            MenuManager.redPosition.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

            MenuManager.greenPosition.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            MenuManager.greenPosition.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

            MenuManager.steeringWheelImage.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            MenuManager.steeringWheelImage.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

            MenuManager.orangePosition.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
            MenuManager.redPosition.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
            MenuManager.greenPosition.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
            MenuManager.steeringWheelImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);

            yield return new WaitForSecondsRealtime(startSequenceDelay);

            MenuManager.greenPosition.color = Color.green;
            MenuManager.redPosition.color = Color.red;
            MenuManager.orangePosition.color = Color.yellow;

            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, slomoTargetTime, enteringSlowmoTime).SetEase(Ease.OutSine).SetUpdate(true);
            DOTween.To(() => vcc.smoothFollowSettings.distance, x => vcc.smoothFollowSettings.distance = x, onEnterCameraDistance, enteringSlowmoTime).SetUpdate(true);
            DOTween.To(() => vcc.smoothFollowSettings.height, x => vcc.smoothFollowSettings.height = x, onEnterCameraHeight, enteringSlowmoTime).SetUpdate(true);
            DOTween.To(() => vcc.smoothFollowSettings.viewHeightRatio, x => vcc.smoothFollowSettings.viewHeightRatio = x, onEnterCameraViewHeight, enteringSlowmoTime).SetUpdate(true);
            DOTween.To(() => vcc.smoothFollowSettings.rotationDamping, x => vcc.smoothFollowSettings.rotationDamping = x, onEnterCameraRotationDampening, enteringSlowmoTime).SetUpdate(true);

            yield return new WaitForSecondsRealtime(enteringSlowmoTime);
            Debug.Log("Player can now input steering wheel");
            canPlayerInput = true;
            MenuManager.steeringWheelImage.color = Color.white;
            LevelEndManager.Instance.moveToThisTransform = GameObject.FindGameObjectWithTag("DOTweenPath").transform;
            HCFW.GameManager.Instance.tcv.GetComponent<LeanTinyInput>().enabled = false;
            HCFW.GameManager.Instance.tcv.GetComponent<CarAiHelper>().enabled = true; // init self (also activate)

            // waiting for player input
            yield return new WaitUntil(() => PlayerPickedSteeringAngle());

            MenuManager.orangePosition.DOFade(0f, .25f).SetUpdate(true);
            MenuManager.redPosition.DOFade(0f, .25f).SetUpdate(true);
            MenuManager.greenPosition.DOFade(0f, .25f).SetUpdate(true);
            MenuManager.steeringWheelImage.DOFade(0f, .25f).SetUpdate(true);

            Debug.Log("Fading out UI because PlayerPickedSteeringAngle() return true");

            // return timescale to normal
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, globalTimeScale, enteringSlowmoTime).SetUpdate(true);

            MenuManager.greenPosition.color = Color.clear;
            MenuManager.orangePosition.color = Color.clear;
            MenuManager.redPosition.color = Color.clear;

            // get the current steering area player was
            SteerArea pickedSteeringArea = steeringWheel.currentArea;

            // go to EOL with steering area as param
            LevelEndManager.Instance.GoToEOL(pickedSteeringArea);

            /*
            ////////////////////////////////////////////////////// FIRST HALF
            ///
            float newTime = 1f;
            yield return new WaitForSeconds(newTime);

            DOTween.To(() => vcc.smoothFollowSettings.rotationDamping, x => vcc.smoothFollowSettings.rotationDamping = x, defaultCameraRotationDampening, newTime).SetUpdate(true);
            DOTween.To(() => vcc.smoothFollowSettings.distance, x => vcc.smoothFollowSettings.distance = x, defaultCameraDistance, newTime).SetUpdate(true);
            DOTween.To(() => vcc.smoothFollowSettings.height, x => vcc.smoothFollowSettings.height = x, defaultCameraHeight, newTime).SetUpdate(true);
            DOTween.To(() => vcc.smoothFollowSettings.viewHeightRatio, x => vcc.smoothFollowSettings.viewHeightRatio = x, defaultCameraViewHeight, newTime).SetUpdate(true);


            ////////////////////////////////////////////////////// SECOND HALF
            yield return new WaitForSeconds(.5f);
            // reset everything here
            receivedInputForTurn = false;
            canPlayerInput = false;
            isInTurn = false;
            enteredTrigger = false;
            */

         

        }

        public IEnumerator TutorialCoroutine(float duration, TurnTriggerDir dir, float enterDelay)
        {

            if (dir == TurnTriggerDir.Left)
            {
                MenuManager.tutorialGreenImage.transform.DORotate(new Vector3(0F, 0F, 90F), .5F).Complete();
                MenuManager.tutorialGreenImage.fillClockwise = true;
            }
            else
            {
                MenuManager.tutorialGreenImage.transform.DORotate(new Vector3(0F, 0F, -90F), .5F).Complete();
                MenuManager.tutorialGreenImage.fillClockwise = false;
            }


            yield return new WaitForSecondsRealtime(enterDelay);

            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.33f, .5f).SetEase(Ease.OutSine).SetUpdate(true);

            MenuManager.tutorialGreenImage.DOColor(Color.green, .5f).SetUpdate(true); ;
            MenuManager.tutorialText.DOColor(Color.white, .5f).SetUpdate(true); ;

            if (dir == TurnTriggerDir.Left)
            {

                yield return new WaitUntil(() => steeringWheel.zValue >= 65f && steeringWheel.zValue <= 91f);
            }
            else
            {
                yield return new WaitUntil(() => steeringWheel.zValue <= -65f && steeringWheel.zValue >= -91f);
            }

            MenuManager.tutorialText.text = "";
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, globalTimeScale, .1f).SetEase(Ease.OutSine).SetUpdate(true);

            yield return new WaitForSecondsRealtime(.75f);
            MenuManager.tutorialGreenImage.DOColor(Color.clear, .25f);

            if (Time.timeScale <= 0.75f)
            {
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, globalTimeScale, .1f).SetEase(Ease.OutSine).SetUpdate(true);
            }

            /*
            yield return new WaitForSecondsRealtime(1f);

            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.1f, .1f).SetEase(Ease.OutSine).SetUpdate(true);
            MenuManager.tutorialText.text = "COUNTER STEER!";

            MenuManager.tutorialGreenImage.transform.DORotate(new Vector3(0F, 0F, 30F), .5F);

            yield return new WaitUntil(() => steeringWheel.zValue > 30f);

            MenuManager.tutorialGreenImage.DOColor(Color.clear, .5f);
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, globalTimeScale, .1f).SetEase(Ease.OutSine).SetUpdate(true);
            MenuManager.tutorialText.DOFade(0F, .5F);
            */

        }



        public IEnumerator OnExitCoroutine()
        {
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 1f);

            steerValueThatWasSet = -steerValueThatWasSet;

            yield return new WaitForSecondsRealtime(.25f);

            // reset everything here
            isInTurn = false;
            steerValueThatWasSet = 0f;

            MenuManager.steeringWheelImage.color = Color.clear;
            yield return new WaitForSecondsRealtime(1f);
        }


        // Use this for initialization
        void Start()
        {
            CheckFirstSession();
            bestScore = activeProfile.bestScore;
            levelScore = 0;
            MenuManager.UpdateMainMenuLabels();
            PrepGameAtCurrentLevel();
            adCooldown = isFirstSession ? firstSession_ShowNewAdCooldown : general_ShowNewAdCooldown;

            MenuManager.restartButton.onClick.RemoveAllListeners();
            MenuManager.restartButton.onClick.AddListener(ReloadScene);
            tcc = FindObjectOfType<TinyCarController>();
            tcv = FindObjectOfType<TinyCarVisuals>();
            carAudio = FindObjectOfType<CarAudio>();
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            joystick.OnSet.AddListener(OnSetJoystick);


            vcc.smoothFollowSettings.rotationDamping = defaultCameraRotationDampening;
            vcc.smoothFollowSettings.viewHeightRatio = defaultCameraViewHeight;
            vcc.smoothFollowSettings.distance = defaultCameraDistance;
            vcc.smoothFollowSettings.height = defaultCameraHeight;

            steeringWheel = FindObjectOfType<SteeringWheel>();

            MenuManager.greenPosition.color = Color.clear;
            MenuManager.orangePosition.color = Color.clear;
            MenuManager.redPosition.color = Color.clear;

            MenuManager.tutorialGreenImage.color = Color.clear;
            MenuManager.tutorialText.color = Color.clear;

            FetchConfetti();
            EnableDisableFountainConfetti(false);
        }


        void CheckFirstSession()
        {
            if (PlayerPrefs.HasKey("NotFirstSession"))
            {
                isFirstSession = false;
            }
            else
            {
                isFirstSession = true;
                PlayerPrefs.SetInt("NotFirstSession", 1);
            }
        }

        #endregion

        #region Example Controls

        public void AddDummyScore()
        {
            OnAddScore(0);
        }

        public void AddDummyBonus()
        {
            OnAddScore(1, true);
        }

        public void AddDummyCoins()
        {
            OnAddCoins(1000);
        }

        #endregion

        #region Game Management

        public IEnumerator Countdown()
        {
            float timeBetweenLights = 0.3f;
            Debug.Log("<Color=White>Starting game in 3...</color>");
            MenuManager.lightsBG.gameObject.SetActive(true);
            MenuManager.lights1.gameObject.SetActive(true);
            MenuManager.lights2.gameObject.SetActive(false);
            MenuManager.lights3.gameObject.SetActive(false);
            MenuManager.lights4.gameObject.SetActive(false);

            yield return new WaitForSecondsRealtime(timeBetweenLights);
            Debug.Log("<Color=White>Starting game in 2...</color>");

            MenuManager.lights1.gameObject.SetActive(true);
            MenuManager.lights2.gameObject.SetActive(true);
            MenuManager.lights3.gameObject.SetActive(false);
            MenuManager.lights4.gameObject.SetActive(false);

            yield return new WaitForSecondsRealtime(timeBetweenLights);
            Debug.Log("<Color=Yellow>Starting game in 1...</color>");

            MenuManager.lights1.gameObject.SetActive(true);
            MenuManager.lights2.gameObject.SetActive(true);
            MenuManager.lights3.gameObject.SetActive(true);
            MenuManager.lights4.gameObject.SetActive(false);

            yield return new WaitForSecondsRealtime(timeBetweenLights);
            Debug.Log("<Color=Green>GO!!!</color>");
            state = GameState.InGame;
            MenuManager.lights1.gameObject.SetActive(true);
            MenuManager.lights2.gameObject.SetActive(true);
            MenuManager.lights3.gameObject.SetActive(true);
            MenuManager.lights4.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(timeBetweenLights);
            MenuManager.lightsBG.gameObject.SetActive(false);
            MenuManager.lights1.gameObject.SetActive(false);
            MenuManager.lights2.gameObject.SetActive(false);
            MenuManager.lights3.gameObject.SetActive(false);
            MenuManager.lights4.gameObject.SetActive(false);
        }


        public void PrepGameAtCurrentLevel()
        {
            // Preparing just level so it's visible in main menu view
            state = GameState.NotInGame;
            levelScore = 0;
            levelBonusScore = 0;
            LevelManager.current = LevelManager.Current;
            LevelManager.current.gameObject.SetActive(true);
            // main difference is: inGame is still FALSE here

            FindObjectOfType<LeanTinyInput>().InitLeanTinyInput();
            gameCamera.GetComponent<VehicleCameraController>().target = FindObjectOfType<TinyCarVisuals>().vehicleContainer;
        }

        public void StartGameAtCurrentLevel()
        {
            // Starting new game: reset score and bonus score, refresh current level, update start labels and enable view. Then set inGame = true
            levelScore = 0;
            levelBonusScore = 0;
            LevelManager.current = LevelManager.Current;
            MenuManager.UpdateLevelStartLabels();
            MenuManager.EnableView(MenuManager.gameView);
            StartCoroutine(Countdown());
#if SERVICES_MODULE
            if (ServicesManager.Instance)
            {
                ServicesManager.Instance.GaProggressionEvent(GameAnalyticsSDK.GAProgressionStatus.Start, LevelManager.currentLevel.ToString());
                ServicesManager.Instance.ReloadInterstitial(); // prepare the interstitial
            }
#endif
        }

        [NaughtyAttributes.Button]
        public void ResetCarToPoint()
        {
            StartCoroutine(ResetCoroutine());
        }

        public IEnumerator ResetCoroutine()
        {
            state = GameState.NotInGame;
            HCFW.GameManager.Instance.tcc.enabled = false;
            HCFW.GameManager.Instance.tcv.enabled = false;
            Transform t = LevelManager.Current.GetClosestTransformToPlayer();
            tcv.carController.transform.DOMove(t.transform.position + new Vector3(0f, 5f, 0f), .5f);
            tcv.carController.transform.DORotate(t.transform.rotation.eulerAngles, .5f);
            //tcv.gameObject.transform.rotation = Quaternion.identity;
            yield return new WaitForSecondsRealtime(.5f);
            HCFW.GameManager.Instance.tcc.enabled = true;
            HCFW.GameManager.Instance.tcv.enabled = true;
            state = GameState.InGame;

        }

        public void FailGameAtCurrentLevel()
        {
            // Game Fail: set inGame = false, update labels, handle best score (at fail it writes to disk if it's new best score), clear all scores in memeory and enable fail view
            state = GameState.NotInGame;
            // Also hide CURRENT level gameObjects or similar if it fits game concept, e.g: LevelManager.Current.levelDataContainer.setActive(false);
            MenuManager.UpdateLevelFailLabels();
            HandleBestScore();
            previousLevelScores.Clear();
            MenuManager.EnableView(MenuManager.failView);
#if SERVICES_MODULE
            if (ServicesManager.Instance)
            {
                ServicesManager.Instance.GaProggressionEvent(GameAnalyticsSDK.GAProgressionStatus.Fail, LevelManager.currentLevel.ToString());
            }
#endif
        }

        public void WinGameAtCurrentLevel()
        {
#if SERVICES_MODULE
            if (ServicesManager.Instance)
            {
                // BEFORE we increase level :)
                ServicesManager.Instance.GaProggressionEvent(GameAnalyticsSDK.GAProgressionStatus.Complete, LevelManager.currentLevel.ToString());
            }
#endif
            // Game Win: inGame = false, add last level score to previous level scores, update labels, increase current level and show winView
            state = GameState.NotInGame;
            //To go to a next random level we must delete the last saved random levle index
            if (PlayerPrefs.HasKey("lastRandomLevelIndex"))
            {
                PlayerPrefs.DeleteKey("lastRandomLevelIndex");
            }
            // Also hide CURRENT level gameObjects or similar before increasing current level, e.g: LevelManager.Current.levelDataContainer.setActive(false);
            previousLevelScores.Add(levelScore + levelBonusScore);
            MenuManager.UpdateLevelWinLabels();
            LevelManager.IncreaseCurrentLevel();
            //MenuManager.EnableView(MenuManager.winView);

        }

        public void OnAddScore(int modifier, bool addBonus = false, bool combineBoth = false)
        {
            // OnAddScore can take single score update or score with bonus.
            // NOTE: adding bonus score does not affect target level score in progression manner
            // Example: If 10 taps are required to win a level of a dummy game, and bonus of 2 is sent at first tap, there is 9 taps remaining still but score is 2 already. 

            // This can be modified any way it suits each game, this is just a dummy set up

            if (addBonus)
            {
                levelBonusScore += 1 * modifier;
                if (!combineBoth)
                {
                    levelScore += 1;
                }
            }
            else
            {
                levelScore += 1;
            }
            if (levelScore >= LevelManager.Current.targetScore)
            {
                WinGameAtCurrentLevel();
            }
            MenuManager.UpdateInGameLabels();
        }

        // Small helper method to add coins from dummy method
        public void OnAddCoins(int amount)
        {
            activeProfile.coins += amount;
            DataManager.PlayerGameProfileData = activeProfile; // save
        }

        void HandleBestScore()
        {
            if (LevelScore > bestScore)
            {
                bestScore = LevelScore; // local reference, may not be needed in general even, activeProfile.bestScore can be used instead
                activeProfile.bestScore = LevelScore; // active profile reference
                DataManager.PlayerGameProfileData = activeProfile; // save to JSON file, only best score is updated, current score is persistent in scene during runtime, along with past level scores
                                                                   // there is no score property in PlayerGameProfile anyway
            }
            MenuManager.UpdateMainMenuLabels();
        }

        public void ReloadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        #endregion

        #region Services (Ads, etc)

        // for simplicity, here is the GameManager -> ServicesManager relation to show ads only from WinView and FailView
        public void ShowInterstitialAtWinOrFail()
        {
            if (LevelManager.currentLevel < 5)
            {
                return;
            }
            if ((Time.time / activeProfile.currentAdCycle) >= adCooldown)
            {
#if SERVICES_MODULE
                if (ServicesManager.Instance)
                {
                    ServicesManager.Instance.ShowInterstitial();
                }
#endif
                activeProfile.currentAdCycle++;
                DataManager.PlayerGameProfileData = GameManager.Instance.activeProfile;
            }
        }

        // Dummy example of RV flow use to skip a level
        public void ShowRewardedToSkipLevel()
        {
#if SERVICES_MODULE
            if (ServicesManager.Instance)
            {
                // basically, when RV is completed, WinGameAtCurrentLevel() is called
                ServicesManager.Instance.ShowRewardedVideo(WinGameAtCurrentLevel);
            }
#endif
        }

        #endregion

        #region Other Helper Functions

        public bool CanBuy(int amount)
        {
            if (amount < activeProfile.coins)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Do NOT Modify

        // This is mandatory part, gets called in Awake to load up JSON cache/config
        // Instead of player prefs, JSON is used

        void InitGameData()
        {
            PlayerGameProfile currentGameData = null; // set as null first
            currentGameData = DataManager.PlayerGameProfileData;
            if (currentGameData != null)
            {
                // if it exists from before, load up values
                Debug.Log("PlayerGameProfile found, loading values...");
                activeProfile = currentGameData;
            }
            else
            {
                activeProfile = new PlayerGameProfile();
                Debug.Log("PlayerGameProfile not found, creating one...");
                activeProfile.currentLevelsCycleData = LevelManager.GetCycleByCycleIndex(0); // 1 should always be anyway set manually
                DataManager.PlayerGameProfileData = activeProfile; // save immediately
            }
        }

        #endregion

    }
}


