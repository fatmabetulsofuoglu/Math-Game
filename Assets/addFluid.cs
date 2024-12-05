using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class addFluid : MonoBehaviour
{
    [System.Serializable]
    public class waterGrains
    {
        public List<GameObject> grains = new List<GameObject>();
    }

    public Transform liquidPourPoint;
    public GameObject pourCup;
    public Vector3[] cupPos;
    public float pourSpeed;
    public waterGrains[] Grains;
    public float amountMultiplier;
    public bool isPouring;
    public float removeWaterSmoothness;

    public waterPooling waterPooling;

    public IEnumerator pourLiquid(float amountInput,int index,Sprite spr)
    {
        isPouring = true;
        pourCup.GetComponent<SpriteRenderer>().sprite = spr;
        pourCup.transform.position = cupPos[index];
        pourCup.SetActive(true);
        int amount = Mathf.CeilToInt(amountInput * amountMultiplier);
        while (pourCup.transform.rotation != Quaternion.Euler(0,0,50f)) {
            pourCup.transform.rotation = Quaternion.LerpUnclamped(pourCup.transform.rotation, Quaternion.Euler(0, 0, 50f), Time.deltaTime * pourSpeed);
            int oran = Random.Range(1,Mathf.Abs(Mathf.CeilToInt(pourCup.transform.rotation.eulerAngles.z) - 40));
            int dropAmountMax = Mathf.CeilToInt((amount / Mathf.Max(Mathf.Abs(Mathf.CeilToInt(pourCup.transform.rotation.eulerAngles.z) - 40),1)));
            int dropAmount = Random.Range(1, dropAmountMax);
            if (oran == 1 && amount > 0)
            {
                for (int i = 0; i < dropAmount; i++)
                {
                    amount--;
                    GameObject liquid = waterPooling.getWaterGrain();
                    Grains[index].grains.Add(liquid);
                    liquid.transform.position = liquidPourPoint.position;
                    liquid.SetActive(true);
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        if (amount > 0)
        {
            while (amount > 0)
            {
                amount--;
                GameObject liquid = waterPooling.getWaterGrain();
                Grains[index].grains.Add(liquid);
                liquid.transform.position = liquidPourPoint.position;
                liquid.SetActive(true);
                yield return new WaitForEndOfFrame();
            }
        }
        while (pourCup.transform.rotation != Quaternion.Euler(0, 0, 0)){
            pourCup.transform.rotation = Quaternion.RotateTowards(pourCup.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * pourSpeed * 100);
            yield return new WaitForEndOfFrame();
        }
        pourCup.SetActive(false);
        isPouring = false;
    }
    public IEnumerator pourLiquidOldStyle(float amountInput, int index, Sprite spr,Image cupLiquid,float defaultAmount)
    {
        isPouring = true;
        pourCup.GetComponent<SpriteRenderer>().sprite = spr;
        pourCup.transform.position = cupPos[index];
        pourCup.SetActive(true);
        int amount = Mathf.CeilToInt(amountInput);
        float smoothCount = 10f;
        for (int i = 0; i < smoothCount; i++)
        {
            pourCup.transform.rotation = Quaternion.Euler(0, 0, (50f / smoothCount) * i);
            cupLiquid.fillAmount = (defaultAmount + ((amount / smoothCount) * i)) / 110;
            yield return new WaitForSeconds(0.1f);
        }
        while (pourCup.transform.rotation != Quaternion.Euler(0, 0, 0))
        {
            pourCup.transform.rotation = Quaternion.RotateTowards(pourCup.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * pourSpeed * 100);
            yield return new WaitForEndOfFrame();
        }
        pourCup.SetActive(false);
        isPouring = false;
    }
    public void removeGrains(int index,float amount)
    {
        float maxAmount = 0;
        if (amount > Grains[index].grains.Count)
        {
            maxAmount = Mathf.Min(Grains[index].grains.Count, amount);
        }else
        {
            maxAmount = Mathf.Min(Grains[index].grains.Count, amount) * amountMultiplier;
        }
        for (int i = 0; i < maxAmount; i++)
        {
            Grains[index].grains[0].gameObject.SetActive(false);
            Grains[index].grains.RemoveAt(0);
        }
        for (int i = 0; i < Grains[index].grains.Count; i++)
        {
            Grains[index].grains[i].SetActive(false);
            Grains[index].grains[i].SetActive(true);
        }
    }
    public IEnumerator removeWaterOldStyle(float amountInput,Image cupLiquid)
    {
        print(amountInput);
        float targetAmount = amountInput / 110f;
        while (cupLiquid.fillAmount != targetAmount)
        {
            print("remove old style");
            cupLiquid.fillAmount = Mathf.MoveTowards(cupLiquid.fillAmount,targetAmount,Time.deltaTime * removeWaterSmoothness);
            yield return new WaitForSeconds(0.1f);
            print(cupLiquid.fillAmount);
        }
    }
}
