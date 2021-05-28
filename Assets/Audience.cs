using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audience : MonoBehaviour
{

    public List<GameObject> npcList = new List<GameObject>();

    public GameObject activeNPC;
    public Animation animation;

    public List<string> animNames;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnEnable()
    {
        EnableRandomNPC();
        PlayAnim();
    }

    [NaughtyAttributes.Button]
    public void PlayAnim()
    {
        animation.Play(animNames[Random.Range(0, animNames.Count)]);
    }

    public void EnableRandomNPC()
    {
        foreach (GameObject npc in npcList)
        {
            npc.SetActive(false);
        }

        int rnd = Random.Range(0, npcList.Count);
        npcList[rnd].SetActive(true);
        activeNPC = npcList[rnd];

        animation = activeNPC.GetComponent<Animation>();
    }
}
