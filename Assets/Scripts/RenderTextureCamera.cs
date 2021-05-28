using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureCamera : MonoBehaviour
{

    #region Singleton
    private static RenderTextureCamera _Instance;
    public static RenderTextureCamera Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<RenderTextureCamera>();
            return _Instance;
        }
    }
    #endregion

    public Camera rtCamera;
    public Transform spawnPos;
    public GameObject spawnedObjToRender;

    public List<GameObject> spawnedObjects = new List<GameObject>();
    public List<Sprite> createdSpritesFromGameObjects = new List<Sprite>();

    private void Start()
    {
        StartCreating();
    }

    [NaughtyAttributes.Button]
    public void StartCreating()
    {
        StartCoroutine(CreateSprites(HCFW.GameManager.Instance.tcv.carMeshParents));
    }

    public IEnumerator CreateSprites(List<GameObject> listOfObjects)
    {
        createdSpritesFromGameObjects.Clear();
        rtCamera.gameObject.SetActive(true);
        foreach (GameObject g in listOfObjects)
        {
            RenderTextureCamera.Instance.SpawnToRender(g);
            yield return new WaitForSecondsRealtime(0.00001f);
            Sprite sprite = Sprite.Create(RenderTextureCamera.Instance.SnapshotImage(), new Rect(0.0f, 0.0f, 256f, 256f), new Vector2(0.5f, 0.5f), 100.0f);
            createdSpritesFromGameObjects.Add(sprite);
        }
        rtCamera.gameObject.SetActive(false);

        foreach (GameObject g in spawnedObjects)
        {
            if (g != null)
            {
                Destroy(g);
            }
        }
        spawnedObjects.Clear();
        spawnedObjToRender = null;
    }

    public void SpawnToRender(GameObject objToRender)
    {
        if (spawnedObjToRender != null)
        {
            Destroy(spawnedObjToRender);
            spawnedObjToRender = Instantiate(objToRender, spawnPos.transform.position, Quaternion.identity, spawnPos);
            spawnedObjToRender.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            spawnedObjToRender.SetActive(true);
        }
        else
        {
            spawnedObjToRender = Instantiate(objToRender, spawnPos.transform.position, Quaternion.identity, spawnPos);
            spawnedObjToRender.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            spawnedObjToRender.SetActive(true);
        }
        spawnedObjects.Add(spawnedObjToRender);
    }

    public Texture2D SnapshotImage()
    { // The camera must has a renderTexture target
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rtCamera.targetTexture;
        rtCamera.Render();
        Texture2D image = new Texture2D(rtCamera.targetTexture.width, rtCamera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, rtCamera.targetTexture.width, rtCamera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = currentRT;
        return image;
    }
}
