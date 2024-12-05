using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour
{
    public RectTransform slideMenuObj;
    public int currentPage = 0;
    public int pageCount;
    public Vector2 changeValue;
    public float smoothnnessValue;
    public GameObject[] pagePointers;

    private void Start()
    {
        currentPage = PlayerPrefs.GetInt("page", 1);
        StartCoroutine(changeSlidePositionI(slideMenuObj.anchoredPosition + (changeValue * -(currentPage - 1))));
        for (int i = 0; i < pagePointers.Length; i++)
        {
            if (i == currentPage - 1)
            {
                pagePointers[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            else
            {
                pagePointers[i].GetComponent<Image>().color = new Color32(255, 255, 255, 125);
            }
        }
    }
    public void loadLevel(int index)
    {
        if (index != -1)
        {
            SceneManager.LoadScene(index);
        }
        
    }
    public void slideMenu(int value)
    {
        print(currentPage + value);
        print(currentPage + value != pageCount + 1);
        if (currentPage + value <= pageCount && currentPage + value >= 1)
        {
            currentPage += value;
            PlayerPrefs.SetInt("page", currentPage);
            for (int i = 0; i < pagePointers.Length; i++)
            {
                if (i == currentPage - 1)
                {
                    pagePointers[i].GetComponent<Image>().color = new Color32(255,255, 255, 255);
                }else
                {
                    pagePointers[i].GetComponent<Image>().color = new Color32(255, 255, 255, 125);
                }
            }
            StartCoroutine(changeSlidePositionI(slideMenuObj.anchoredPosition + (changeValue * -value)));
        }
    }
    public IEnumerator changeSlidePositionI (Vector2 newPosition)
    {
        while (slideMenuObj.anchoredPosition != newPosition)
        {
            slideMenuObj.anchoredPosition = Vector2.MoveTowards(slideMenuObj.anchoredPosition, newPosition, Time.deltaTime * smoothnnessValue);
            yield return new WaitForEndOfFrame();
        }
    }
}
