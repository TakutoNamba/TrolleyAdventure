using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTreasures : MonoBehaviour
{
    [SerializeField] GameObject[] treasures;
    public int tresuresNum;
    public Vector3 range;
    public float scale;


    void Start()
    {
        for(int i=0; i<tresuresNum; i++)
        {
            int r = Random.Range(0, treasures.Length);
            float x = Random.Range(range.x - scale, range.x + scale);
            float y = Random.Range(range.y - scale, range.y + scale);
            float z = Random.Range(range.z - scale, range.z + scale);
            GameObject g = Instantiate(treasures[r], new Vector3(x,y,z), treasures[r].transform.rotation);
        　　 g.transform.localScale = new Vector3(.2f, .2f, .2f);        
        }
    }

    void Update()
    {
        
    }
}
