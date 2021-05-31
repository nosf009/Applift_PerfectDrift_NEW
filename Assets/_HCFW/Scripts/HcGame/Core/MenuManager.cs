using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HCFW
{
    public class MenuManager : MonoBehaviour
    {
        // References to canvas views
        [Header("Views")]
        public Transform mainView;
        public Transform gameView;
        public Transform winView;
        public Transform failView;
        public Transform persistentView;
        public Transform settingsView;
        public Transform shopView;

        // references to various labels
        [Header("Labels")]
        public Text currentLevelProgLabel;
        public Text nextLevelProgLabel;
        public Text currentLevelLabel;
        public Text bestScoreLabel;
        public Text currentScoreLabel;
        public Slider levelProgressLabel;
        public Text failScoreLabel;

        public Image steeringWheelImage;

        [Header("Buttons")]
        public Button restartButton;
        public Lean.Touch.LeanCircleJoystick joystick;

        [Header("Start counter")]
        public Image lightsBG;
        public Image lights1;
        public Image lights2;
        public Image lights3;
        public Image lights4;


        [Header("Collectable count text")]
        public Text collectableCountTextMainView;
        public Text collectableCountTextGameView;

        [Header("Drift Value")]
        public Text driftValueText;
        public DOTweenAnimation dtAnim;
        public Text totalDriftValueText;

        [Header("Steering wheel position")]
        public Image greenPosition;
        public Image orangePosition;
        public Image redPosition;

        public float rndZ = 0f;
        public float greenLowerBoundaryZ = 180f;
        public float greenHigherBoundaryZ = 180f;

        public float lowerBoundaryOrangeZ = 180f;
        public float higherBoundaryOrangeZ = 180f;

        public float lowerBoundaryRedZ = 180f;
        public float higherBoundaryRedZ = 180f;

        public float valueForCalculatingBoundary;

        [Header("Tutorial stuff")]
        public Text tutorialText;
        public Image tutorialGreenImage;

        [Header("EOL Text")]
        public Text eolTextEnd;
        public Text eolTimerText;
        public Text donutsCountsText;

        [NaughtyAttributes.Button]
        public void PickGreenPosition(bool isLeft)
        {
            orangePosition.transform.rotation = Quaternion.identity;
            redPosition.transform.rotation = Quaternion.identity;
            greenPosition.transform.rotation = Quaternion.identity;
            if (isLeft)
            {
                // left part of the wheel

                greenPosition.fillClockwise = false;
                orangePosition.fillClockwise = false;
                redPosition.fillClockwise = false;
            }
            else
            {
                // on the right part of wheel
                greenPosition.fillClockwise = true;
                orangePosition.fillClockwise = true;
                redPosition.fillClockwise = true;
            }


            if (isLeft)
            {
                float offset = Random.Range(20f, 35f);
                GenerateBoundaries(offset, 18f, .00000001f);
            }
            else
            {
                float offset = Random.Range(-35f, -20f);
                GenerateBoundaries(offset, -18f, .00000001f);
            }
        }


        [Header("Win particles FX")]
        public Sprite Firework;
        public Color[] colorsFX;
        public Transform parent;

        [Button]
        public void FallingConfettiFX(int howManyParticles = 70)
        {
            RectTransform component = parent.GetComponent<RectTransform>();
            for (int i = 0; i < howManyParticles; i++)
            {
                Image image = CreateImage(this.Firework, component);
                image.color = colorsFX[UnityEngine.Random.Range(0, colorsFX.Length)];
                image.transform.eulerAngles = new Vector3(0f, 0f, (float)UnityEngine.Random.Range(0, 360));
                image.transform.localScale = new Vector2(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.3f, 1f));
                image.rectTransform.localPosition = new Vector2(UnityEngine.Random.Range(-component.rect.width / 2f, component.rect.width / 2f), component.rect.y + component.rect.height + 20f);
                image.gameObject.SetActive(false);
                float duration = UnityEngine.Random.Range(1f, 1.5f);
                float delay = UnityEngine.Random.Range(0f, 1f);
                image.transform.DORotate(new Vector3(0f, 0f, (float)UnityEngine.Random.Range(0, 360)), duration, RotateMode.Fast).SetEase(Ease.Linear).SetDelay(delay).OnStart(delegate
                {
                    image.gameObject.SetActive(true);
                });
                image.transform.DOLocalMoveY(component.rect.y - 20f, duration, false).SetDelay(delay).SetEase(Ease.Linear).OnComplete(delegate
                {
                    UnityEngine.Object.Destroy(image.gameObject);
                });
            }
        }

        public Image CreateImage(Sprite sprite, Transform parent)
        {
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<Image>();
            gameObject.transform.SetParent(parent, false);
            Image component = gameObject.GetComponent<Image>();
            component.sprite = sprite;
            component.SetNativeSize();
            component.transform.localScale = Vector3.one;
            return component;
        }


        /// <summary>
        /// Generate boundaries for the steering wheel
        /// </summary>
        /// <param name="offset">How big is the offset from zero. This moves the orange/green/red part "further" into the wheel. Use negative values for the right side.</param>
        /// <param name="partSizeValue">PartSizeValue is the value of the fillValue of the circle part. So if we use the fill value of 0.05f, then 360° * 0.05f, we get 18f, we use 18f</param>
        /// <param name="boundaryTinyValue">Very small value for if checks if we are inside boundary</param>
        public void GenerateBoundaries(float offset, float partSizeValue, float boundaryTinyValue)
        {

            // green boundaries are generated first, we take the part size value and add the offset, thats the lower boundary
            greenLowerBoundaryZ = partSizeValue + offset;
            greenHigherBoundaryZ = greenLowerBoundaryZ + partSizeValue; // higher boundary is calculated by lower boundary adding up the partSizeValue

            // orange boundaries calculated from GREEN boundaries & offset
            higherBoundaryOrangeZ = greenLowerBoundaryZ - boundaryTinyValue;
            lowerBoundaryOrangeZ = greenLowerBoundaryZ - greenLowerBoundaryZ + offset;

            // red boundaries calculated from GREEN boundaries & offset
            higherBoundaryRedZ = greenHigherBoundaryZ + greenLowerBoundaryZ - offset;
            lowerBoundaryRedZ = greenHigherBoundaryZ + boundaryTinyValue;

            // ROTATION OF THE VISUAL PARTS
            // rotating the green part
            greenPosition.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, greenLowerBoundaryZ));

            // for new  red
            Vector3 newRotationRed = greenPosition.transform.rotation.eulerAngles + new Vector3(0f, 0f, greenLowerBoundaryZ - offset);
            redPosition.transform.rotation = Quaternion.Euler(newRotationRed);

            // for new orange 
            Vector3 newRotationOrange = greenPosition.transform.rotation.eulerAngles + new Vector3(0f, 0f, -greenLowerBoundaryZ + offset);
            orangePosition.transform.rotation = Quaternion.Euler(newRotationOrange);
        }


        private void Update()
        {
            //Debug.Log(steeringWheelImage.transform.rotation.eulerAngles);

        }

        public void Start()
        {
            HCFW.GameManager.Instance.MenuManager.eolTextEnd.DOColor(Color.clear, 0F).Complete();
            GameManager.Instance.MenuManager.donutsCountsText.color = Color.clear;
        }

        public void EnableView(Transform view) // simple enable/disable view. Due to it's simplicity it allows for various animation usage on menu gameObjects in hierarchy
        {
            // disable all
            mainView.gameObject.SetActive(false);
            gameView.gameObject.SetActive(false);
            winView.gameObject.SetActive(false);
            failView.gameObject.SetActive(false);
            settingsView.gameObject.SetActive(false);


            if (shopView != null)
            {
                shopView.gameObject.SetActive(false);
            }

            //enable targeted
            view.gameObject.SetActive(true);
        }

        public void UpdateCollectableTexts()
        {
            collectableCountTextMainView.text = HCFW.GameManager.Instance.activeProfile.coins.ToString();
            collectableCountTextGameView.text = HCFW.GameManager.Instance.activeProfile.coins.ToString();
        }

        public void UpdateLevelStartLabels()
        {
            // Update relevant labels at each level start, such as progress bar, current and next level, score or similar
            currentLevelLabel.text = GameManager.Instance.LevelManager.currentLevel.ToString();
            currentScoreLabel.text = "SCORE: " + GameManager.Instance.LevelScore.ToString();
            levelProgressLabel.value = 0;
            levelProgressLabel.maxValue = GameManager.Instance.LevelManager.current.targetScore;
            currentLevelProgLabel.text = GameManager.Instance.LevelManager.currentLevel.ToString();
            nextLevelProgLabel.text = (GameManager.Instance.LevelManager.currentLevel + 1).ToString();
            UpdateCollectableTexts();
        }

        public void UpdateLevelFailLabels()
        {
            // this can be used or not, depending on the game
            if (GameManager.Instance.levelScore > GameManager.Instance.bestScore)
            {
                failScoreLabel.text = "NEW BEST: " + GameManager.Instance.LevelScore.ToString();
            }
            else
            {
                failScoreLabel.text = "SCORE: " + GameManager.Instance.LevelScore.ToString();
            }
            UpdateCollectableTexts();
        }

        public void UpdateLevelWinLabels()
        {
            // right now not used, but called by GameManager on Win, if any dynamic labels or UI components are used on win screen, they can be controlled/updated here
        }

        public void UpdateInGameLabels()
        {
            // Used to refresh labels during level play at some event. Such as changing score value, progress etc
            currentScoreLabel.text = "SCORE: " + GameManager.Instance.LevelScore.ToString();
            currentLevelProgLabel.text = GameManager.Instance.LevelManager.currentLevel.ToString();
            nextLevelProgLabel.text = (GameManager.Instance.LevelManager.currentLevel + 1).ToString();
            levelProgressLabel.value = GameManager.Instance.levelScore;
        }

        public void UpdateMainMenuLabels()
        {
            // Called after fail/at start of app to refreesh relevant main menu labels with latest data
            bestScoreLabel.text = "BEST: " + GameManager.Instance.bestScore.ToString();
            UpdateCollectableTexts();
        }

    }
}


