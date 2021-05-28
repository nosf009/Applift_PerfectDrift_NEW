using DG.Tweening;
using HCFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    public GameObject meshObject;
    public bool isPickedUp = false;

    public GameObject gameObjectParticles;

    [Header("Tweening")]
    public Tween constantRotationTween;
    public float rotationAmount = 50f;

    // Start is called before the first frame update
    void Start()
    {
        gameObjectParticles.SetActive(false);
        constantRotationTween = transform.DORotate(new Vector3(0, rotationAmount, 0f), 1f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isPickedUp)
            {
                isPickedUp = true;
                OnTriggerEntered();
            }
        }
    }
    private void OnDestroy()
    {
        if (constantRotationTween != null)
        {
            constantRotationTween.Kill();
        }
    }

    public void OnTriggerEntered()
    {
        meshObject.SetActive(false);
        gameObjectParticles.SetActive(true);
        HCFW.GameManager.Instance.countPickups++;
        GameManager.Instance.activeProfile.coins++;
        DataManager.PlayerGameProfileData = GameManager.Instance.activeProfile;
        HCFW.GameManager.Instance.MenuManager.UpdateCollectableTexts();
    }

    public IEnumerator PickupCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.1f);
    }
}
