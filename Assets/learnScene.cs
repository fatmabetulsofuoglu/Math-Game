using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class learnScene : MonoBehaviour
{
    public Sprite[] images;
    public Image photo;
    public int currentNo;
    public void clickPhoto()
    {
        if (currentNo < images.Length - 1)
        {
            currentNo++;
            photo.sprite = images[currentNo];
        }
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
