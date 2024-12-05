using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterPooling : MonoBehaviour
{
    public GameObject waterGrain;
    public int poolAmount;
    public bool willGrow;

    private List<GameObject> waterPools = new List<GameObject>();
    void Start()
    {
        for (int i = 0; i < poolAmount; i++)
        {
            GameObject obj = Instantiate(waterGrain);
            obj.SetActive(false);
            waterPools.Add(obj);    
        }
    }

    public GameObject getWaterGrain()
    {
        for (int i = 0; i < waterPools.Count; i++)
        {
            if (!waterPools[i].activeInHierarchy)
            {
                return waterPools[i];
            }
        }
        if (willGrow)
        {
            GameObject obj = Instantiate(waterGrain);
            waterPools.Add(obj);
            return obj;
        }
        return null;
    }
}
