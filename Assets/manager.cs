using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static manager;

public class manager : MonoBehaviour
{
    public enum GameMode
    {
        compare,
        comparePutSymbol,
        putElementsBySymbol,
        cubeMatch,
        cupMatch,
        cupMatchTutorial,
        cupMatchAdd,
        cupMatchRemove,
        scale,
        scaleLearn,
        scaleMatchAdd,
        scaleMatchRemove,
        MultipleCupAmountMatch,
        cupMatchAddNRemove,
        scaleAddEquation,
        cupMatchAddNRemoveNew
    }

    public Transform followObj;
    public Transform selectedArea;
    public element firstValue,secondValue,thirdValue;
    public element[] valuesForElements;
    public GameObject[] objectsForElements;
    public int tour = 0;
    public GameObject firstValueObject, secondValueObject,lowerBarObject;
    public Sprite greatSprite, equelSprite, lessSprite,nullSprite;
    public Image symbolObj;
    public GameMode gameMode;
    private List<GameObject> weightsList = new List<GameObject>();
    public GameObject popupObj;
    public Image popupElement1, popupElement2;
    public TextMeshProUGUI popupText;
    public GameObject popupRestartButton, popupNextLevelButton, popupExitButton;
    public GameObject rightSymbolObj;
    private int currentSymbol;
    public Sprite rightIcon, wrongIcon;
    public Image cup1Image, cup2Image;
    public List<element> scale1, scale2;
    public GameObject[] scaleItemPlacesL, scaleItemPlacesR;
    private Vector2 defaultPosScale;
    public Vector2 scaleRiseLevel, scaleFallLevel;
    public Sprite[] scaleSprites;
    public Image scalePartObj;
    public element nullElement;
    public addFluid addFluid;
    private List<element> elements = new List<element>();
    public TextMeshProUGUI scaleText,scaleTextCups;
    public Image[] scaleImages;
    public GameObject scaleObj;
    public int[] liquidStartAmount;
    public GameObject[] inputs;
    public int cupMaxAmountLimit;
    public bool showScaleText = true;
    public GameObject[] slotParts;
    private Vector2 standartSize = new Vector2(250, 250);
    public Image[] cupLiquidShower;
    public float cupLiquidShowerSmoothness;
    private void Start()
    {
        var list = GameObject.FindGameObjectsWithTag("element");
        weightsList.AddRange(list);
        if (gameMode == GameMode.comparePutSymbol)
        {
            changeValues();
        }else if (gameMode == GameMode.putElementsBySymbol)
        {
            putSymbol();
        }
        else if (gameMode == GameMode.scale || gameMode == GameMode.scaleLearn || gameMode == GameMode.scaleMatchAdd || gameMode == GameMode.scaleMatchRemove)
        {
            defaultPosScale = scaleItemPlacesL[1].GetComponent<RectTransform>().anchoredPosition;
            if (gameMode == GameMode.scaleMatchAdd)
            {
                while (true)
                {
                    GameObject[] elementsArray = GameObject.FindGameObjectsWithTag("element");
                    for (int i = 0; i < elementsArray.Length; i++)
                    {
                        elements.Add(elementsArray[i].GetComponent<element>());
                    }
                    scale1.Clear();
                    scale2.Clear();
                    int randomElement = UnityEngine.Random.Range(0, elements.Count);
                    element newElement = Instantiate(elements[randomElement]);
                    newElement.transform.SetParent(GameObject.Find("Canvas").transform);
                    scale1.Add(newElement);
                    elements.RemoveAt(randomElement);
                    randomElement = UnityEngine.Random.Range(0, elements.Count);
                    element newElement2 = Instantiate(elements[randomElement]);
                    newElement2.transform.SetParent(GameObject.Find("Canvas").transform);
                    scale2.Add(newElement2);
                    elements.RemoveAt(randomElement);

                    float totalScale1 = 0;
                    float totalScale2 = 0;
                    for (int i = 0; i < scale1.Count; i++)
                    {
                        if (scale1[i] != null)
                        {
                            totalScale1 += scale1[i].weight;
                        }
                    }
                    for (int i = 0; i < scale2.Count; i++)
                    {
                        if (scale2[i] != null)
                        {
                            totalScale2 += scale2[i].weight;
                        }
                    }
                    bool acceptable = false;
                    for (int i = 0; i < elements.Count; i++)
                    {
                        if (totalScale1 == totalScale2 + elements[i].weight)
                        {
                            acceptable = true;
                        }
                    }
                    if (!acceptable)
                    {
                        continue;
                    }
                    else
                    {
                        for (int i = 0; i < scale1.Count; i++)
                        {
                            scale1[i].GetComponent<Image>().raycastTarget = false;
                        }
                        for (int i = 0; i < scale2.Count; i++)
                        {
                            scale2[i].GetComponent<Image>().raycastTarget = false;
                        }
                        break;
                    }
                }
                scaleView();
            }
            else if (gameMode == GameMode.scaleMatchRemove)
            {
                GameObject[] elementsArray = GameObject.FindGameObjectsWithTag("element");
                for (int i = 0; i < elementsArray.Length; i++)
                {
                    elements.Add(elementsArray[i].GetComponent<element>());
                }
                int randomv = UnityEngine.Random.Range(0, elements.Count);
                scale1.Add(elements[randomv]);
                elements.RemoveAt(randomv);
                randomv = UnityEngine.Random.Range(0, elements.Count);
                scale1.Add(elements[randomv]);
                elements.RemoveAt(randomv);
                for (int i = 0; i < 2; i++)
                {
                    int randomb = UnityEngine.Random.Range(0, elements.Count);
                    if (elements[randomb].weight != scale1[scale1.Count - 1].weight)
                    {
                        scale2.Add(elements[randomb]);
                        elements.RemoveAt(randomb);
                        break;
                    }
                    else
                    {
                        elements.RemoveAt(randomb);
                    }
                }
                scaleView();
            }
        }
        else if (gameMode == GameMode.cupMatchAdd || gameMode == GameMode.cupMatchRemove || gameMode == GameMode.cupMatchAddNRemove)
        {
            var cupsValue = new List<float>();
            GameObject[] elementsArray = GameObject.FindGameObjectsWithTag("element");
            for (int i = 0; i < elementsArray.Length; i++)
            {
                elements.Add(elementsArray[i].GetComponent<element>());
            }
            for (int i = 0; i < weightsList.Count; i++)
            {
                cupsValue.Add(weightsList[i].GetComponent<element>().weight);
            }
            float averigeWeight = 0f;
            for (int i = 0; i < elements.Count; i++)
            {
                averigeWeight = (averigeWeight + elements[i].weight) / 2;
            }
            var elementsUp = new List<element>();
            var elementsDown = new List<element>();
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].weight > averigeWeight)
                {
                    elementsUp.Add(elements[i]);
                }else
                {
                    elementsDown.Add(elements[i]);
                }
            }
            int randomIndex = UnityEngine.Random.Range(0, elementsUp.Count);
            
            scale1.Add(elementsUp[randomIndex]);
            randomIndex = UnityEngine.Random.Range(0,elementsDown.Count);
            scale2.Add(elementsDown[randomIndex]);
            if (gameMode == GameMode.cupMatchAddNRemove)
            {
                liquidStartAmount[0] = Mathf.RoundToInt(scale1[0].weight);
                liquidStartAmount[1] = Mathf.RoundToInt(scale2[0].weight);
            }
            StartCoroutine(startLoadCups());
            scaleTextShow();
            MultiScaleTextView();
        }
        else if (gameMode == GameMode.scaleAddEquation)
        {
            defaultPosScale = scaleItemPlacesL[1].GetComponent<RectTransform>().anchoredPosition;
            scaleEquationUI();
        }
    }
    private IEnumerator startLoadCups()
    {
        yield return StartCoroutine(addFluid.pourLiquid(scale1[0].weight, 0, scale1[0].GetComponent<Image>().sprite));
        StartCoroutine(addFluid.pourLiquid(scale2[0].weight, 1, scale2[0].GetComponent<Image>().sprite));

    }
    void Update()
    {
        if (followObj != null)
        {
            if (Input.touchCount > 0)
            {
                followObj.position = Input.touches[0].position;
            }
        }
        
    }

    public void changeValues()
    {
        if (weightsList.Count < 2)
        {
            weightsList.Clear();
            var list = GameObject.FindGameObjectsWithTag("element");
            weightsList.AddRange(list);
        }
        firstValue = weightsList[UnityEngine.Random.Range(0,weightsList.Count)].GetComponent<element>();
        weightsList.Remove(firstValue.gameObject);
        secondValue = weightsList[UnityEngine.Random.Range(0, weightsList.Count)].GetComponent<element>();
        weightsList.Remove(secondValue.gameObject);
        firstValueObject.GetComponent<Image>().sprite = firstValue.GetComponent<Image>().sprite;
        secondValueObject.GetComponent<Image>().sprite = secondValue.GetComponent<Image>().sprite;
        firstValueObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300 * (firstValue.weight / 10), 300 * (firstValue.weight / 10));
        secondValueObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300 * (secondValue.weight / 10), 300 * (secondValue.weight / 10));
    }

    public void putSymbol()
    {
        currentSymbol = UnityEngine.Random.Range(-1, 1);
        if (currentSymbol == 1)
        {
            symbolObj.sprite = greatSprite;
        }else if (currentSymbol == 0)
        {
            symbolObj.sprite = equelSprite;
        }else
        {
            symbolObj.sprite = lessSprite;
        }
    }

    public void elementDown(GameObject obj)
    {
        followObj = obj.transform;
        followObj.GetComponent<Image>().raycastTarget = false;
        //followObj.transform.SetAsLastSibling();

    }

    public void elementUp()
    {
        followObj.GetComponent<Image>().raycastTarget = true;

        if (selectedArea != null)
        {
            if (gameMode == GameMode.compare || gameMode == GameMode.putElementsBySymbol || gameMode == GameMode.cupMatchAddNRemove || gameMode == GameMode.cupMatchAddNRemoveNew)
            {
                element placed = null;
                if (firstValue == followObj.GetComponent<element>())
                {
                    firstValue = null;
                }
                if (secondValue == followObj.GetComponent<element>())
                {
                    secondValue = null;
                }
                if (selectedArea.gameObject == firstValueObject)
                {
                    if (firstValue == null)
                    {
                        firstValue = followObj.GetComponent<element>();
                        placed = firstValue;
                    }
                }
                else if (selectedArea.gameObject == secondValueObject)
                {
                    if (secondValue == null)
                    {
                        secondValue = followObj.GetComponent<element>();
                        placed = secondValue;
                    }
                }
                followObj.transform.position = selectedArea.transform.position;
            }
            else if (gameMode == GameMode.comparePutSymbol)
            {
                followObj.transform.position = selectedArea.transform.position;
                thirdValue = followObj.GetComponent<element>();
            }
            else if (gameMode == GameMode.cubeMatch)
            {
                GameObject placedObj = null;
                for (int i = 0; i < valuesForElements.Length; i++)
                {
                    if (followObj.GetComponent<element>() == valuesForElements[i])
                    {
                        valuesForElements[i] = null;
                    }
                }
                for (int i = 0; i < objectsForElements.Length; i++)
                {
                    if (selectedArea.gameObject == objectsForElements[i])
                    {
                        print("bulundu");
                        valuesForElements[i] = followObj.GetComponent<element>();
                        placedObj = objectsForElements[i];
                    }
                }
                followObj.transform.position = placedObj.transform.position;
            }
            else if (gameMode == GameMode.cupMatchTutorial || gameMode == GameMode.cupMatch)
            {
                element placed = null;

                if (firstValue == followObj.GetComponent<element>() && !followObj.GetComponent<element>().symbol)
                {
                    firstValue = null;
                }
                if (secondValue == followObj.GetComponent<element>() && !followObj.GetComponent<element>().symbol)
                {
                    secondValue = null;
                }
                if (selectedArea.gameObject == firstValueObject && !followObj.GetComponent<element>().symbol)
                {
                    if (firstValue == null)
                    {
                        firstValue = followObj.GetComponent<element>();
                        placed = firstValue;

                    }
                }
                else if (selectedArea.gameObject == secondValueObject && !followObj.GetComponent<element>().symbol)
                {
                    if (secondValue == null)
                    {
                        secondValue = followObj.GetComponent<element>();
                        placed = secondValue;

                    }
                }
                else if (selectedArea.gameObject == symbolObj.gameObject && followObj.GetComponent<element>().symbol)
                {
                    if (thirdValue == null)
                    {
                        thirdValue = followObj.GetComponent<element>();
                        placed = thirdValue;
                    }
                }
                if (placed != null)
                {
                    GameObject placedObj = null;
                    if (placed == firstValue)
                    {
                        placedObj = firstValueObject;
                    }
                    else if (placed == secondValue)
                    {
                        placedObj = secondValueObject;
                    }
                    else if (placed == thirdValue)
                    {
                        placedObj = symbolObj.gameObject;
                    }
                    followObj.transform.position = placedObj.transform.position;
                }
                if (gameMode == GameMode.cupMatchTutorial && showScaleText)
                {
                    if (firstValue != null && secondValue != null)
                    {
                        scaleObj.SetActive(true);
                        var v = Math.Clamp(firstValue.weight - secondValue.weight, -1, 1);
                        if (v == 0)
                        {
                            scaleText.text = "=";
                        }
                        else if (v == 1)
                        {
                            scaleText.text = ">";
                        }
                        else if (v == -1)
                        {
                            scaleText.text = "<";
                        }
                        scaleImages[0].sprite = firstValue.GetComponent<Image>().sprite;
                        scaleImages[1].sprite = secondValue.GetComponent<Image>().sprite;
                    }
                    else
                    {
                        scaleObj.SetActive(false);
                    }
                }
            }
            else if (gameMode == GameMode.cupMatchAdd)
            {
                element placed = null;

                if (firstValue == followObj.GetComponent<element>() && !followObj.GetComponent<element>().symbol && selectedArea.gameObject != firstValueObject)
                {
                    firstValue = null;
                }
                else if (firstValue == followObj.GetComponent<element>() && !followObj.GetComponent<element>().symbol && selectedArea.gameObject == firstValueObject)
                {
                    followObj.GetComponent<RectTransform>().anchoredPosition = firstValueObject.GetComponent<RectTransform>().anchoredPosition;
                }
                if (secondValue == followObj.GetComponent<element>() && !followObj.GetComponent<element>().symbol && selectedArea.gameObject != secondValueObject)
                {
                    secondValue = null;
                }
                else if (secondValue == followObj.GetComponent<element>() && !followObj.GetComponent<element>().symbol && selectedArea.gameObject != secondValueObject)
                {
                    followObj.GetComponent<RectTransform>().anchoredPosition = secondValueObject.GetComponent<RectTransform>().anchoredPosition;
                }
                if (selectedArea.gameObject == firstValueObject && !followObj.GetComponent<element>().symbol)
                {
                    if (firstValue == null)
                    {
                        firstValue = followObj.GetComponent<element>();
                        placed = firstValue;
                    }
                }
                else if (selectedArea.gameObject == secondValueObject && !followObj.GetComponent<element>().symbol)
                {
                    if (secondValue == null)
                    {
                        secondValue = followObj.GetComponent<element>();
                        placed = secondValue;
                    }
                }
                else if (selectedArea.gameObject == symbolObj.gameObject && followObj.GetComponent<element>().symbol)
                {
                    if (thirdValue == null)
                    {
                        thirdValue = followObj.GetComponent<element>();
                        placed = thirdValue;
                    }
                }
                if (placed != null)
                {
                    GameObject placedObj = null;
                    if (placed == firstValue)
                    {
                        placedObj = firstValueObject;
                    }
                    else if (placed == secondValue)
                    {
                        placedObj = secondValueObject;
                    }
                    else if (placed == thirdValue)
                    {
                        placedObj = symbolObj.gameObject;
                    }
                    followObj.transform.position = placedObj.transform.position;
                }
                scaleTextShow();
            }
            else if (gameMode == GameMode.cupMatchRemove)
            {
                element placed = null;

                if (firstValue == followObj.GetComponent<element>() && !followObj.GetComponent<element>().symbol && selectedArea.gameObject != firstValueObject)
                {
                    firstValue = null;
                }
                else if (firstValue == followObj.GetComponent<element>() && !followObj.GetComponent<element>().symbol && selectedArea.gameObject == firstValueObject)
                {
                    followObj.GetComponent<RectTransform>().anchoredPosition = firstValueObject.GetComponent<RectTransform>().anchoredPosition;
                }
                if (secondValue == followObj.GetComponent<element>() && !followObj.GetComponent<element>().symbol && selectedArea.gameObject != secondValueObject)
                {
                    secondValue = null;
                }
                else if (secondValue == followObj.GetComponent<element>() && !followObj.GetComponent<element>().symbol && selectedArea.gameObject != secondValueObject)
                {
                    followObj.GetComponent<RectTransform>().anchoredPosition = secondValueObject.GetComponent<RectTransform>().anchoredPosition;
                }
                if (selectedArea.gameObject == firstValueObject && !followObj.GetComponent<element>().symbol)
                {
                    if (firstValue == null)
                    {
                        firstValue = followObj.GetComponent<element>();
                        placed = firstValue;

                    }
                }
                else if (selectedArea.gameObject == secondValueObject && !followObj.GetComponent<element>().symbol)
                {
                    if (secondValue == null)
                    {
                        secondValue = followObj.GetComponent<element>();
                        placed = secondValue;
                    }
                }
                else if (selectedArea.gameObject == symbolObj.gameObject && followObj.GetComponent<element>().symbol)
                {
                    if (thirdValue == null)
                    {
                        thirdValue = followObj.GetComponent<element>();
                        placed = thirdValue;
                    }
                }
                if (placed != null)
                {
                    GameObject placedObj = null;
                    if (placed == firstValue)
                    {
                        placedObj = firstValueObject;
                    }
                    else if (placed == secondValue)
                    {
                        placedObj = secondValueObject;
                    }
                    else if (placed == thirdValue)
                    {
                        placedObj = symbolObj.gameObject;
                    }
                    followObj.transform.position = placedObj.transform.position;
                }
                scaleTextShow();
            }
            else if (gameMode == GameMode.scale || gameMode == GameMode.scaleLearn || gameMode == GameMode.scaleMatchRemove)
            {
                if (firstValue == followObj.GetComponent<element>() || firstValue == nullElement)
                {
                    firstValue = null;
                }
                if (secondValue == followObj.GetComponent<element>() || secondValue == nullElement)
                {
                    secondValue = null;
                }
                if (selectedArea.gameObject == firstValueObject)
                {
                    if (firstValue == null)
                    {
                        firstValue = followObj.GetComponent<element>();
                        followObj.transform.position = selectedArea.transform.position;

                    }
                }
                else if (selectedArea.gameObject == secondValueObject)
                {
                    if (secondValue == null)
                    {
                        secondValue = followObj.GetComponent<element>();
                        followObj.transform.position = selectedArea.transform.position;

                    }
                }

                scaleView();
            }
            else if (gameMode == GameMode.scaleAddEquation)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (selectedArea.gameObject == inputs[i])
                    {
                        if (i < 2)
                        {
                            if (scale1[i] == null)
                            {
                                scale1[i] = followObj.GetComponent<element>();
                                followObj.transform.position = inputs[i].transform.position;
                            }

                        }
                        else
                        {
                            if (scale2[i - 2] == null)
                            {
                                scale2[i - 2] = followObj.GetComponent<element>();
                                followObj.transform.position = inputs[i].transform.position;
                            }

                        }
                    }
                }

                scaleView();
            }
            else if (gameMode == GameMode.scaleMatchAdd)
            {
                if (firstValue == followObj.GetComponent<element>() || firstValue == nullElement)
                {
                    firstValue = null;
                }
                if (secondValue == followObj.GetComponent<element>() || secondValue == nullElement)
                {
                    secondValue = null;
                }
                if (selectedArea.gameObject == firstValueObject)
                {
                    if (firstValue == null)
                    {
                        firstValue = followObj.GetComponent<element>();
                        followObj.transform.position = selectedArea.transform.position;
                        ScaleDynamicRemoveElementFix(firstValue);

                    }
                }
                else if (selectedArea.gameObject == secondValueObject)
                {
                    if (secondValue == null)
                    {
                        secondValue = followObj.GetComponent<element>();
                        followObj.transform.position = selectedArea.transform.position;

                    }
                }
                else if (selectedArea.gameObject == lowerBarObject)
                {
                    if (scale1.Count > 0)
                    {
                        for (int i = 0; i < scale1.Count; i++)
                        {
                            if (scale1[i] == followObj.GetComponent<element>())
                            {
                                scale1.RemoveAt(i);
                            }
                        }
                    }
                    if (scale2.Count > 0)
                    {
                        for (int i = 0; i < scale2.Count; i++)
                        {
                            if (scale2[i] == followObj.GetComponent<element>())
                            {
                                scale2.RemoveAt(i);
                            }
                        }
                    }
                    followObj.GetComponent<RectTransform>().anchoredPosition = followObj.GetComponent<element>().startPos;
                    followObj.GetComponent<RectTransform>().sizeDelta = followObj.GetComponent<element>().defaultSize;
                    print("s?f?rlama");
                }
                scaleView();
            }
            else if (gameMode == GameMode.MultipleCupAmountMatch)
            {
                if (!addFluid.isPouring)
                {
                    Debug.Log("ba?lad?");
                    GameObject placedObj = null;
                    for (int i = 0; i < valuesForElements.Length; i++)
                    {
                        if (followObj.GetComponent<element>() == valuesForElements[i])
                        {
                            valuesForElements[i] = null;
                            StartCoroutine(addFluid.removeWaterOldStyle(0, cupLiquidShower[i]));
                        }
                    }
                    for (int i = 0; i < objectsForElements.Length; i++)
                    {
                        if (selectedArea.gameObject == objectsForElements[i])
                        {
                            print("bulundu");
                            valuesForElements[i] = followObj.GetComponent<element>();
                            placedObj = objectsForElements[i];
                            StartCoroutine(addFluid.pourLiquidOldStyle(valuesForElements[i].weight, i, valuesForElements[i].GetComponent<Image>().sprite, cupLiquidShower[i], 0));
                        }
                    }
                    followObj.transform.position = placedObj.transform.position;
                }
                else
                {
                    followObj.GetComponent<RectTransform>().anchoredPosition = followObj.GetComponent<element>().startPos;
                }
            }
        }
        else
        {
            followObj.GetComponent<RectTransform>().anchoredPosition = followObj.GetComponent<element>().startPos;
            followObj.GetComponent<RectTransform>().sizeDelta = followObj.GetComponent<element>().defaultSize;
            if (gameMode == GameMode.comparePutSymbol || gameMode == GameMode.compare || gameMode == GameMode.putElementsBySymbol || gameMode == GameMode.cupMatch || gameMode == GameMode.scaleLearn || gameMode == GameMode.cupMatchTutorial || gameMode == GameMode.scaleMatchAdd || gameMode == GameMode.scaleMatchRemove || gameMode == GameMode.cupMatchAddNRemove || gameMode == GameMode.cupMatchAddNRemoveNew)
            {
                if (firstValue == followObj.GetComponent<element>())
                {
                    firstValue = null;
                }
                if (secondValue == followObj.GetComponent<element>())
                {
                    secondValue = null;
                }
                if (thirdValue == followObj.GetComponent<element>())
                {
                    thirdValue = null;
                }
            }
            else if (gameMode == GameMode.cubeMatch)
            {
                for (int i = 0; i < valuesForElements.Length; i++)
                {
                    if (followObj.GetComponent<element>() == valuesForElements[i])
                    {
                        objectsForElements[i].transform.parent.GetChild(1).gameObject.SetActive(false);
                        valuesForElements[i] = null;
                    }
                }
            }
            else if (gameMode == GameMode.MultipleCupAmountMatch)
            {
                for (int i = 0; i < valuesForElements.Length; i++)
                {
                    if (followObj.GetComponent<element>() == valuesForElements[i])
                    {
                        //addFluid.removeGrains(i, valuesForElements[i].weight);
                        objectsForElements[i].transform.parent.GetChild(1).gameObject.SetActive(false);
                        valuesForElements[i] = null;
                        StartCoroutine(addFluid.removeWaterOldStyle(0, cupLiquidShower[i]));
                    }
                }
            }
            else if (gameMode == GameMode.scaleAddEquation)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (i < 2)
                    {
                        if (scale1[i] == followObj.GetComponent<element>())
                        {
                            scale1[i] = null;
                        }

                    }
                    else
                    {
                        if (scale2[i - 2] == followObj.GetComponent<element>())
                        {
                            scale2[i - 2] = null;
                        }

                    }
                }

                scaleView();
            }
            else if (gameMode == GameMode.cupMatchAdd)
            {
                if (firstValue == followObj.GetComponent<element>())
                {
                    if (firstValue.used)
                    {
                        addFluid.removeGrains(0, firstValue.weight);
                    }
                    firstValue.used = false;
                    if (scale1.Count > 0)
                    {
                        for (int i = 0; i < scale1.Count; i++)
                        {
                            if (scale1[i] == followObj.GetComponent<element>())
                            {
                                scale1.RemoveAt(i);
                            }
                        }
                    }
                    firstValue = null;

                }
                if (secondValue == followObj.GetComponent<element>())
                {
                    if (secondValue.used)
                    {
                        addFluid.removeGrains(1, secondValue.weight);
                    }
                    secondValue.used = false;
                    if (scale2.Count > 0)
                    {
                        for (int i = 0; i < scale2.Count; i++)
                        {
                            if (scale2[i] == followObj.GetComponent<element>())
                            {
                                scale2.RemoveAt(i);
                            }
                        }
                    }
                    secondValue = null;

                }
                if (thirdValue == followObj.GetComponent<element>())
                {
                    thirdValue = null;
                }
                
                MultiScaleTextView();

                scaleTextShow();
            }
            else if (gameMode == GameMode.cupMatchRemove)
            {
                if (firstValue == followObj.GetComponent<element>())
                {
                    if (firstValue.used)
                    {
                        StartCoroutine(addFluid.pourLiquid(firstValue.weight, 0, firstValue.GetComponent<Image>().sprite));
                    }
                    firstValue.used = false;
                    if (scale1.Count > 0)
                    {
                        for (int i = 0; i < scale1.Count; i++)
                        {
                            if (scale1[i] == followObj.GetComponent<element>())
                            {
                                scale1.RemoveAt(i);
                            }
                        }
                    }
                    firstValue = null;

                }
                if (secondValue == followObj.GetComponent<element>())
                {
                    if (secondValue.used)
                    {
                        StartCoroutine(addFluid.pourLiquid(secondValue.weight, 1, secondValue.GetComponent<Image>().sprite));
                    }
                    secondValue.used = false;
                    if (scale2.Count > 0)
                    {
                        for (int i = 0; i < scale2.Count; i++)
                        {
                            if (scale2[i] == followObj.GetComponent<element>())
                            {
                                scale2.RemoveAt(i);
                            }
                        }
                    }
                    secondValue = null;

                }
                if (thirdValue == followObj.GetComponent<element>())
                {
                    thirdValue = null;
                }


                float totalScale1 = 0;
                float totalScale2 = 0;
                for (int i = 0; i < scale1.Count; i++)
                {
                    totalScale1 += scale1[i].weight;
                }
                for (int i = 0; i < scale2.Count; i++)
                {
                    totalScale2 += scale2[i].weight;
                }
                scaleObj.SetActive(true);
                var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
                if (v == 0)
                {
                    scaleText.text = "=";
                }
                else if (v == 1)
                {
                    scaleText.text = ">";
                }
                else if (v == -1)
                {
                    scaleText.text = "<";
                }
                for (int i = 0; i < scaleImages.Length / 2; i++)
                {
                    if (i < scale1.Count)
                    {
                        if (scale1[i] != null)
                        {
                            scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                            scaleImages[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            scaleImages[i].gameObject.SetActive(false);

                        }

                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);
                    }
                }
                for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
                {
                    int a = i - 3;
                    if (a < scale2.Count)
                    {
                        if (scale2[a] != null)
                        {
                            scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                            scaleImages[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            scaleImages[i].gameObject.SetActive(false);

                        }

                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);
                    }
                }
                scaleTextShow();
            }

        }

        // 2. Oyun - Haydi Sen Dene
        if (gameMode == GameMode.compare)
        {
            if (firstValue != null && secondValue != null)
            {
                if (firstValue.weight > secondValue.weight)
                {
                    symbolObj.sprite = greatSprite;
                }
                else if (firstValue.weight == secondValue.weight)
                {
                    symbolObj.sprite = equelSprite;
                }
                else if (firstValue.weight < secondValue.weight)
                {
                    symbolObj.sprite = lessSprite;
                }
            }
            else
            {
                symbolObj.sprite = nullSprite;
            }
        }

        // 3. Oyun - ??areti Sen Koy
        else if (gameMode == GameMode.comparePutSymbol)
        {
            if (firstValue != null && secondValue != null && thirdValue != null)
            {
                var v = Math.Clamp(firstValue.weight - secondValue.weight, -1, 1);
                popupObj.SetActive(true);
                popupElement1.sprite = firstValue.GetComponent<Image>().sprite;
                popupElement2.sprite = secondValue.GetComponent<Image>().sprite;

                if (popupElement1.sprite == null || popupElement2.sprite == null)
                {
                    Debug.LogWarning("Resim y?klenemedi, sprite null!");
                    return; // Resim yoksa i?lemi sonland?r
                }

                // A??rl?k oran?n? hesaplama
                float totalWeight = firstValue.weight + secondValue.weight;
                float scaleFactor1 = totalWeight > 0 ? firstValue.weight / totalWeight : 0; // 0'dan ka??nmak i?in
                float scaleFactor2 = totalWeight > 0 ? secondValue.weight / totalWeight : 0;

                RectTransform rect1 = popupElement1.GetComponent<RectTransform>();
                RectTransform rect2 = popupElement2.GetComponent<RectTransform>();

                // Boyutlar? ayarlama (?rnek boyut 100)
                float baseSize = 100;
                rect1.sizeDelta = new Vector2(baseSize * scaleFactor1 * 7, baseSize * scaleFactor1 * 7); 
                rect2.sizeDelta = new Vector2(baseSize * scaleFactor2 * 7, baseSize * scaleFactor2 * 7);

                LayoutRebuilder.ForceRebuildLayoutImmediate(popupObj.GetComponent<RectTransform>());

                if (thirdValue.weight == v)
                {
                    // Ba?ar?l?
                    Debug.Log("ba?ar?l?");
                    if (v == 0)
                    {
                        popupText.text = "=";
                    }
                    else if (v == 1)
                    {
                        popupText.text = ">";
                    }
                    else if (v == -1)
                    {
                        popupText.text = "<";
                    }
                    popupNextLevelButton.SetActive(true);
                    popupRestartButton.SetActive(false);
                    rightSymbolObj.SetActive(true);
                }
                else
                {
                    Debug.Log("ba?ar?s?z");
                    popupText.text = "?";
                    popupRestartButton.SetActive(true);
                    popupNextLevelButton.SetActive(false);
                    rightSymbolObj.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("firstValue veya secondValue null!");
            }
        }

        // 4. Oyun - ??arete G?re Nesne Bul
        else if (gameMode == GameMode.putElementsBySymbol)
        {
            if (firstValue != null && secondValue != null)
            {
                var v = Math.Clamp(firstValue.weight - secondValue.weight, -1, 1);
                popupObj.SetActive(true);
                popupElement1.sprite = firstValue.GetComponent<Image>().sprite;
                popupElement2.sprite = secondValue.GetComponent<Image>().sprite;

                // A??rl?k oran?na g?re boyut ayarlama
                float scaleFactor1 = firstValue.weight / (firstValue.weight + secondValue.weight);
                float scaleFactor2 = secondValue.weight / (firstValue.weight + secondValue.weight);
                popupElement1.rectTransform.localScale = new Vector3(scaleFactor1, scaleFactor1, 1);
                popupElement2.rectTransform.localScale = new Vector3(scaleFactor2, scaleFactor2, 1);

                if (v == currentSymbol)
                {
                    Debug.Log("ba?ar?l?");
                    popupText.text = (v == 0) ? "=" : (v == 1) ? ">" : "<";
                    popupNextLevelButton.SetActive(true);
                    popupRestartButton.SetActive(false);
                    rightSymbolObj.SetActive(true);
                }
                else
                {
                    Debug.Log("ba?ar?s?z");
                    popupText.text = "?";
                    popupRestartButton.SetActive(true);
                    popupNextLevelButton.SetActive(false);
                    rightSymbolObj.SetActive(false);
                }

                // Exit butonuna t?klama i?lemi - GameObject olarak tan?mlanan popupExitButton'un Button bile?enine eri?iyoruz
                popupExitButton.GetComponent<Button>().onClick.RemoveAllListeners(); // ?nceki eventleri kald?r
                popupExitButton.GetComponent<Button>().onClick.AddListener(ClosePopup); // Yeni fonksiyonu ata
            }
        }


        else if (gameMode == GameMode.cubeMatch)
        {
            int rightCount = 0;
            for (int i = 0; i < valuesForElements.Length; i += 2)
            {
                if (valuesForElements[i] != null && valuesForElements[i + 1] != null)
                {
                    objectsForElements[i].transform.parent.GetChild(1).gameObject.SetActive(true);
                    if (valuesForElements[i].weight == valuesForElements[i + 1].weight)
                    {
                        objectsForElements[i].transform.parent.GetChild(1).GetComponent<Image>().sprite = rightIcon;
                        rightCount++;
                    }
                    else
                    {
                        objectsForElements[i].transform.parent.GetChild(1).GetComponent<Image>().sprite = wrongIcon;
                    }
                }
            }
            if (rightCount == 4)
            {
                popupObj.SetActive(true);
            }
        }
        else if (gameMode == GameMode.cupMatch)
        {
            if (selectedArea == null)
            {
                selectedArea = this.gameObject.transform;
            }
            if (firstValue != null && selectedArea.gameObject == firstValueObject)
            {
                StartCoroutine(addFluid.pourLiquid(firstValue.weight, 0, firstValue.GetComponent<Image>().sprite));
            }
            else if (firstValue == null)
            {
                addFluid.removeGrains(0, 999999);
            }
            if (secondValue != null && selectedArea.gameObject == secondValueObject)
            {
                StartCoroutine(addFluid.pourLiquid(secondValue.weight, 1, secondValue.GetComponent<Image>().sprite));
            }
            else if (secondValue == null)
            {
                addFluid.removeGrains(1, 999999);
            }
            if (thirdValue != null)
            {
                var v = Math.Clamp(firstValue.weight - secondValue.weight, -1, 1);
                if (v != thirdValue.weight)
                {
                    popupText.text = "Yanl?? yapt?n?z! Tekrar deneyebilirsin.";
                    rightSymbolObj.GetComponent<Image>().sprite = wrongIcon;
                }
                popupObj.SetActive(true);
            }
        }
        else if (gameMode == GameMode.MultipleCupAmountMatch)
        {
            for (int i = 0; i < objectsForElements.Length; i++)
            {
                if (selectedArea == null)
                {
                    selectedArea = this.gameObject.transform;
                }
                if (valuesForElements[i] != null && selectedArea.gameObject == objectsForElements[i])
                {
                    //StartCoroutine(addFluid.pourLiquid(valuesForElements[i].weight, i, valuesForElements[i].GetComponent<Image>().sprite));
                }
            }
            int rightCount = 0;
            for (int i = 0; i < valuesForElements.Length; i += 2)
            {
                if (valuesForElements[i] != null && valuesForElements[i + 1] != null)
                {
                    objectsForElements[i].transform.parent.GetChild(1).gameObject.SetActive(true);
                    if (valuesForElements[i].weight == valuesForElements[i + 1].weight)
                    {
                        objectsForElements[i].transform.parent.GetChild(1).GetComponent<Image>().sprite = rightIcon;
                        rightCount++;
                        objectsForElements[i].transform.parent.GetChild(4).GetComponent<TextMeshProUGUI>().text = "=";
                    }
                    else if (valuesForElements[i].weight > valuesForElements[i + 1].weight)
                    {
                        objectsForElements[i].transform.parent.GetChild(4).GetComponent<TextMeshProUGUI>().text = ">";
                        objectsForElements[i].transform.parent.GetChild(1).GetComponent<Image>().sprite = wrongIcon;
                    }
                    else if (valuesForElements[i].weight < valuesForElements[i + 1].weight)
                    {
                        objectsForElements[i].transform.parent.GetChild(4).GetComponent<TextMeshProUGUI>().text = "<";
                        objectsForElements[i].transform.parent.GetChild(1).GetComponent<Image>().sprite = wrongIcon;
                    }
                }
            }
            if (rightCount == 3)
            {
                StartCoroutine(PopupAnim());
            }
        }
        else if (gameMode == GameMode.scaleLearn || gameMode == GameMode.scaleMatchAdd || gameMode == GameMode.scaleAddEquation)
        {
            scaleView();
        }
        else if (gameMode == GameMode.cupMatchTutorial)
        {
            if (selectedArea == null)
            {
                selectedArea = this.gameObject.transform;
            }
            if (firstValue != null && selectedArea.gameObject == firstValueObject)
            {
                StartCoroutine(addFluid.pourLiquid(firstValue.weight, 0, firstValue.GetComponent<Image>().sprite));
            }
            else if (firstValue == null)
            {
                addFluid.removeGrains(0, 99999);
            }
            if (secondValue != null && selectedArea.gameObject == secondValueObject)
            {
                StartCoroutine(addFluid.pourLiquid(secondValue.weight, 1, secondValue.GetComponent<Image>().sprite));
            }
            else if (secondValue == null)
            {
                addFluid.removeGrains(1, 999999);
            }
            if (firstValue != null && secondValue != null)
            {
                if (firstValue.weight > secondValue.weight)
                {
                    symbolObj.sprite = greatSprite;
                }
                else if (firstValue.weight == secondValue.weight)
                {
                    symbolObj.sprite = equelSprite;
                }
                else if (firstValue.weight < secondValue.weight)
                {
                    symbolObj.sprite = lessSprite;
                }
            }
            else
            {
                symbolObj.sprite = nullSprite;
            }
        }
        else if (gameMode == GameMode.cupMatchAdd || gameMode == GameMode.cupMatchRemove)
        {
            if (!addFluid.isPouring)
            {
                if (selectedArea == null)
                {
                    selectedArea = this.gameObject.transform;
                }
                if (firstValue != null && selectedArea.gameObject == firstValueObject)
                {
                    //StartCoroutine(addFluid.pourLiquid(firstValue.weight, 0, firstValue.GetComponent<Image>().sprite));
                }
                if (secondValue != null && selectedArea.gameObject == secondValueObject)
                {
                    //StartCoroutine(addFluid.pourLiquid(secondValue.weight, 1, secondValue.GetComponent<Image>().sprite));
                }
                if (thirdValue != null)
                {
                    var v = Math.Clamp(firstValue.weight - secondValue.weight, -1, 1);
                    if (v != thirdValue.weight)
                    {
                        popupText.text = "Yanl?? yapt?n?z! Tekrar deneyebilirsin.";
                        rightSymbolObj.GetComponent<Image>().sprite = wrongIcon;
                    }
                }
            }
            else
            {
                if (followObj.GetComponent<element>() == firstValue)
                {
                    followObj.GetComponent<RectTransform>().anchoredPosition = firstValueObject.GetComponent<RectTransform>().anchoredPosition;
                }
                else if (followObj.GetComponent<element>() == secondValue)
                {
                    followObj.GetComponent<RectTransform>().anchoredPosition = secondValueObject.GetComponent<RectTransform>().anchoredPosition;
                }
                else
                {
                    followObj.GetComponent<RectTransform>().anchoredPosition = followObj.GetComponent<element>().startPos;

                }

            }
            if (gameMode == GameMode.cupMatchAdd)
            {
                float totalScale1 = 0;
                float totalScale2 = 0;
                for (int i = 0; i < scale1.Count; i++)
                {
                    totalScale1 += scale1[i].weight;
                }
                for (int i = 0; i < scale2.Count; i++)
                {
                    totalScale2 += scale2[i].weight;
                }

                if (totalScale1 == totalScale2)
                {
                    print("e?it");
                    StartCoroutine(PopupAnim());
                }
            }
            else if (gameMode == GameMode.cupMatchRemove)
            {
                float totalScale1 = 0;
                float totalScale2 = 0;
                for (int i = 0; i < scale1.Count; i++)
                {
                    totalScale1 -= scale1[i].weight;
                }
                for (int i = 0; i < scale2.Count; i++)
                {
                    totalScale2 -= scale2[i].weight;
                }

                if (totalScale1 == totalScale2)
                {
                    print("e?it");
                    StartCoroutine(PopupAnim());
                }
            }
        }
        else if (gameMode == GameMode.cupMatchAddNRemove)
        {
            if (!addFluid.isPouring)
            {
                if (selectedArea == null)
                {
                    selectedArea = this.gameObject.transform;
                }
                if (firstValue != null && selectedArea.gameObject == firstValueObject)
                {
                    //StartCoroutine(addFluid.pourLiquid(firstValue.weight, 0, firstValue.GetComponent<Image>().sprite));
                }
                if (secondValue != null && selectedArea.gameObject == secondValueObject)
                {
                    //StartCoroutine(addFluid.pourLiquid(secondValue.weight, 1, secondValue.GetComponent<Image>().sprite));
                }
                if (thirdValue != null)
                {
                    var v = Math.Clamp(firstValue.weight - secondValue.weight, -1, 1);
                    if (v != thirdValue.weight)
                    {
                        popupText.text = "Yanl?? yapt?n?z! Tekrar deneyebilirsin.";
                        rightSymbolObj.GetComponent<Image>().sprite = wrongIcon;
                    }
                }
            }
            else
            {
                if (followObj.GetComponent<element>() == firstValue)
                {
                    followObj.GetComponent<RectTransform>().anchoredPosition = firstValueObject.GetComponent<RectTransform>().anchoredPosition;
                }
                else if (followObj.GetComponent<element>() == secondValue)
                {
                    followObj.GetComponent<RectTransform>().anchoredPosition = secondValueObject.GetComponent<RectTransform>().anchoredPosition;
                }
                else
                {
                    followObj.GetComponent<RectTransform>().anchoredPosition = followObj.GetComponent<element>().startPos;

                }

            }
            float totalScale1 = liquidStartAmount[0];
            float totalScale2 = liquidStartAmount[1];

            if (totalScale1 == totalScale2)
            {
                print("e?it");
                StartCoroutine(PopupAnim());
            }
        }
        followObj = null;
    }

    private void ScaleDynamicRemoveElementFix(element element)
    {
        
    }

    private void MultiScaleTextView()
    {

        if (scale1.Count != 0 || scale2.Count != 0)
        {
            float totalScale1 = 0;
            float totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }
            scaleObj.SetActive(true);
            var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
            if (v == 0)
            {
                scaleText.text = "=";
            }
            else if (v == 1)
            {
                scaleText.text = ">";
            }
            else if (v == -1)
            {
                scaleText.text = "<";
            }
            for (int i = 0; i < scaleImages.Length / 2; i++)
            {
                if (i < scale1.Count)
                {
                    if (scale1[i] != null)
                    {
                        scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
            {
                int a = i - 3;
                if (a < scale2.Count)
                {
                    if (scale2[a] != null)
                    {
                        scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            scaleObj.SetActive(false);
        }
    }

    public void scaleTextShow()
    {
        float totalScale1 = liquidStartAmount[0];
        float totalScale2 = liquidStartAmount[1];
        for (int i = 0; i < scale1.Count; i++)
        {
            totalScale1 += scale1[i].weight;
        }
        for (int i = 0; i < scale2.Count; i++)
        {
            totalScale2 += scale2[i].weight;
        }
        var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
        if (v == 0)
        {
            scaleTextCups.text = "=";
        }
        else if (v == 1)
        {
            scaleTextCups.text = ">";
        }
        else if (v == -1)
        {
            scaleTextCups.text = "<";
        }
    }

    public void addWeight(int index)
    {
        if (gameMode == GameMode.scaleMatchAdd)
        {
            if (index == 0)
            {
                if (firstValue != null && scale1.Count < 2)
                {
                    scale1.Add(firstValue);
                    firstValue.GetComponent<RectTransform>().anchoredPosition = firstValue.startPos;
                    firstValue = null;
                }
            }
            else
            {
                if (secondValue != null && scale2.Count < 2)
                {
                    scale2.Add(secondValue);
                    secondValue.GetComponent<RectTransform>().anchoredPosition = secondValue.startPos;
                    secondValue = null;
                }
            }
            
            scaleView();
        }
        else if (gameMode == GameMode.cupMatchAddNRemove)
        {
            if (index == 0)
            {
                if (firstValue != null)
                {
                    if (liquidStartAmount[index] + firstValue.weight <= cupMaxAmountLimit)
                    {
                        liquidStartAmount[index] = Mathf.Clamp(liquidStartAmount[index] + Mathf.RoundToInt(firstValue.weight), 0, 200);
                        StartCoroutine(addFluid.pourLiquid(firstValue.weight, 0, firstValue.gameObject.GetComponent<Image>().sprite));
                        firstValue.GetComponent<RectTransform>().anchoredPosition = firstValue.startPos;
                        scale1.Add(firstValue);
                        firstValue = null;
                    }   
                }
            }
            else
            {
                if (secondValue != null)
                {
                    if (liquidStartAmount[index] + secondValue.weight <= cupMaxAmountLimit)
                    {
                        liquidStartAmount[index] = Mathf.Clamp(liquidStartAmount[index] + Mathf.RoundToInt(secondValue.weight), 0, 200);
                        StartCoroutine(addFluid.pourLiquid(secondValue.weight, 1, secondValue.gameObject.GetComponent<Image>().sprite));
                        secondValue.GetComponent<RectTransform>().anchoredPosition = secondValue.startPos;
                        scale2.Add(secondValue);
                        secondValue = null;
                    }
                }
            }
            float totalScale1 = liquidStartAmount[0];
            float totalScale2 = liquidStartAmount[1];

            if (totalScale1 == totalScale2)
            {
                print("e?it");
                StartCoroutine(PopupAnim());
            }
            scaleObj.SetActive(true);
            var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
            if (v == 0)
            {
                scaleText.text = "=";
            }
            else if (v == 1)
            {
                scaleText.text = ">";
            }
            else if (v == -1)
            {
                scaleText.text = "<";
            }
            for (int i = 0; i < scaleImages.Length / 2; i++)
            {
                if (i < scale1.Count)
                {
                    if (scale1[i] != null)
                    {
                        scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
            {
                int a = i - 3;
                if (a < scale2.Count)
                {
                    if (scale2[a] != null)
                    {
                        scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            scaleTextShow();
        }
        else if (gameMode == GameMode.cupMatchAddNRemoveNew)
        {
            float totalScale1 = 0;
            float totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }
            if (index == 0)
            {
                if (firstValue != null)
                {
                    if (totalScale1 + firstValue.weight <= cupMaxAmountLimit)
                    {
                        firstValue.GetComponent<RectTransform>().anchoredPosition = firstValue.startPos;
                        scale1.Add(firstValue);
                        StartCoroutine(addFluid.pourLiquidOldStyle(firstValue.weight, 0, firstValue.gameObject.GetComponent<Image>().sprite, cupLiquidShower[0],totalScale1));
                        firstValue = null;
                    }
                }
            }
            else
            {
                if (secondValue != null)
                {
                    if (totalScale2 + secondValue.weight <= cupMaxAmountLimit)
                    {
                        secondValue.GetComponent<RectTransform>().anchoredPosition = secondValue.startPos;
                        scale2.Add(secondValue);
                        StartCoroutine(addFluid.pourLiquidOldStyle(secondValue.weight, 1, secondValue.gameObject.GetComponent<Image>().sprite, cupLiquidShower[1], totalScale2));
                        secondValue = null;
                    }
                }
            }
            totalScale1 = 0;
            totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }

            if (totalScale1 == totalScale2)
            {
                print("e?it");
                StartCoroutine(PopupAnim());
            }
            scaleObj.SetActive(true);
            var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
            if (v == 0)
            {
                scaleText.text = "=";
            }
            else if (v == 1)
            {
                scaleText.text = ">";
            }
            else if (v == -1)
            {
                scaleText.text = "<";
            }
            for (int i = 0; i < scaleImages.Length / 2; i++)
            {
                if (i < scale1.Count)
                {
                    if (scale1[i] != null)
                    {
                        scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
            {
                int a = i - 3;
                if (a < scale2.Count)
                {
                    if (scale2[a] != null)
                    {
                        scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            scaleTextShow();
        }
        else if (gameMode == GameMode.cupMatchAdd)
        {
            float totalScale1 = 0;
            float totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }
            if (firstValue != null && index == 0)
            {
                if (totalScale1 + firstValue.weight <= cupMaxAmountLimit)
                {
                    scale1.Add(firstValue);
                    StartCoroutine(addFluid.pourLiquid(firstValue.weight, 0, firstValue.GetComponent<Image>().sprite));
                    scale1[scale1.Count - 1].used = true;
                }
            }else if (secondValue != null && index == 1)
            {
                if (totalScale2 + secondValue.weight <= cupMaxAmountLimit)
                {
                    scale2.Add(secondValue);
                    StartCoroutine(addFluid.pourLiquid(secondValue.weight, 1, secondValue.GetComponent<Image>().sprite));
                    scale2[scale2.Count - 1].used = true;
                }
            }
            totalScale1 = 0;
            totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }
            scaleObj.SetActive(true);
            var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
            if (v == 0)
            {
                scaleText.text = "=";
            }
            else if (v == 1)
            {
                scaleText.text = ">";
            }
            else if (v == -1)
            {
                scaleText.text = "<";
            }
            for (int i = 0; i < scaleImages.Length / 2; i++)
            {
                if (i < scale1.Count)
                {
                    if (scale1[i] != null)
                    {
                        scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
            {
                int a = i - 3;
                if (a < scale2.Count)
                {
                    if (scale2[a] != null)
                    {
                        scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            totalScale1 = 0;
            totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }

            if (totalScale1 == totalScale2)
            {
                print("e?it");
                StartCoroutine(PopupAnim());
            }
        }
        else
        {
            if (index == 0)
            {
                if (firstValue != null && scale1.Count < 2)
                {
                    scale1.Add(firstValue);
                    firstValue.GetComponent<RectTransform>().anchoredPosition = firstValue.startPos;
                    firstValue = null;
                }
            }
            else
            {
                if (secondValue != null && scale2.Count < 2)
                {
                    scale2.Add(secondValue);
                    secondValue.GetComponent<RectTransform>().anchoredPosition = secondValue.startPos;
                    secondValue = null;
                }
            }
            scaleView();
        }
        
    }

    public void removeWeight(int index)
    {
        if (gameMode == GameMode.scale || gameMode == GameMode.scaleLearn)
        {
            if (index == 0)
            {
                if (scale1.Count >= 1)
                {
                    try
                    {
                        scale1.Remove(firstValue);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    firstValue.GetComponent<RectTransform>().anchoredPosition = firstValue.startPos;
                    firstValue = null;
                }
            }
            else
            {
                if (scale2.Count >= 1)
                {
                    try
                    {
                        scale2.Remove(secondValue);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    secondValue.GetComponent<RectTransform>().anchoredPosition = secondValue.startPos;
                    secondValue = null;
                }
            }
            scaleView();
        }
        else if (gameMode == GameMode.scaleMatchRemove)
        {
            if (index == 0)
            {
                if (firstValue != null)
                {
                    try
                    {
                        firstValue.GetComponent<RectTransform>().anchoredPosition = firstValue.startPos;

                        for (int i = 0; i < scale1.Count; i++)
                        {
                            if (scale1[i] == firstValue)
                            {
                                scale1.RemoveAt(i);
                            }
                        }
                        firstValue = null;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }else if (index == 1)
            {
                if (secondValue != null)
                {
                    try
                    {
                        secondValue.GetComponent<RectTransform>().anchoredPosition = secondValue.startPos;

                        for (int i = 0; i < scale2.Count; i++)
                        {
                            if (scale2[i] == secondValue)
                            {
                                scale2.RemoveAt(i);
                            }
                        }
                        secondValue = null;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            scaleView();
        }
        else if (gameMode == GameMode.cupMatchAddNRemove)
        {
            if (index == 0)
            {
                if (firstValue != null)
                {
                    if (liquidStartAmount[index] >= firstValue.weight)
                    {
                        liquidStartAmount[index] = Mathf.Clamp(liquidStartAmount[index] - Mathf.RoundToInt(firstValue.weight), 0, 200);
                        addFluid.removeGrains(0, firstValue.weight);
                        firstValue.GetComponent<RectTransform>().anchoredPosition = firstValue.startPos;
                        scale2.Add(firstValue);
                        firstValue = null;
                    }
                }
            }
            else
            {
                if (secondValue != null)
                {
                    if (liquidStartAmount[index] >= secondValue.weight)
                    {
                        liquidStartAmount[index] = Mathf.Clamp(liquidStartAmount[index] - Mathf.RoundToInt(secondValue.weight), 0, 200);
                        addFluid.removeGrains(1, secondValue.weight);
                        secondValue.GetComponent<RectTransform>().anchoredPosition = secondValue.startPos;
                        scale1.Add(secondValue);
                        secondValue = null;
                    } 
                }
            }
            float totalScale1 = liquidStartAmount[0];
            float totalScale2 = liquidStartAmount[1];

            if (totalScale1 == totalScale2)
            {
                print("e?it");
                StartCoroutine(PopupAnim());
            }
            scaleObj.SetActive(true);
            var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
            if (v == 0)
            {
                scaleText.text = "=";
            }
            else if (v == 1)
            {
                scaleText.text = ">";
            }
            else if (v == -1)
            {
                scaleText.text = "<";
            }
            for (int i = 0; i < scaleImages.Length / 2; i++)
            {
                if (i < scale1.Count)
                {
                    if (scale1[i] != null)
                    {
                        scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
            {
                int a = i - 3;
                if (a < scale2.Count)
                {
                    if (scale2[a] != null)
                    {
                        scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            scaleTextShow();
        }
        else if (gameMode == GameMode.cupMatchAddNRemoveNew)
        {
            float totalScale1 = 0;
            float totalScale2 = 0;
            if (index == 0)
            {
                if (firstValue != null)
                {
                    try
                    {
                        firstValue.GetComponent<RectTransform>().anchoredPosition = firstValue.startPos;

                        for (int i = 0; i < scale1.Count; i++)
                        {
                            if (scale1[i] == firstValue)
                            {
                                scale1.RemoveAt(i);
                                totalScale1 = 0;
                                for (int a = 0; a < scale1.Count; a++)
                                {
                                    totalScale1 += scale1[a].weight;
                                }
                                StartCoroutine(addFluid.removeWaterOldStyle(totalScale1, cupLiquidShower[0]));
                            }
                        }
                        firstValue = null;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            else if (index == 1)
            {
                if (secondValue != null)
                {
                    try
                    {
                        secondValue.GetComponent<RectTransform>().anchoredPosition = secondValue.startPos;

                        for (int i = 0; i < scale2.Count; i++)
                        {
                            if (scale2[i] == secondValue)
                            {
                                scale2.RemoveAt(i);
                                totalScale2 = 0;
                                for (int a = 0; a < scale2.Count; a++)
                                {
                                    totalScale2 += scale2[a].weight;
                                }
                                StartCoroutine(addFluid.removeWaterOldStyle(totalScale2,cupLiquidShower[1]));
                            }
                        }
                        secondValue = null;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            totalScale1 = 0;
            totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }
            if (totalScale1 == totalScale2 && totalScale2 != 0)
            {
                print("e?it");
                StartCoroutine(PopupAnim());
            }
            scaleObj.SetActive(true);
            var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
            if (v == 0)
            {
                scaleText.text = "=";
            }
            else if (v == 1)
            {
                scaleText.text = ">";
            }
            else if (v == -1)
            {
                scaleText.text = "<";
            }
            for (int i = 0; i < scaleImages.Length / 2; i++)
            {
                if (i < scale1.Count)
                {
                    if (scale1[i] != null)
                    {
                        scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
            {
                int a = i - 3;
                if (a < scale2.Count)
                {
                    if (scale2[a] != null)
                    {
                        scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            scaleTextShow();
        }
        else if (gameMode == GameMode.cupMatchRemove)
        {
            float totalScale1 = 0;
            float totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }
            if (firstValue != null && index == 0)
            {
                bool found = false;
                for (int i = 0; i < scale1.Count; i++)
                {
                    if (scale1[i] == firstValue)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    if (totalScale1 >= firstValue.weight)
                    {
                        scale1.Add(firstValue);
                        firstValue.used = true;
                        addFluid.removeGrains(0, firstValue.weight);
                    }  
                }
            }
            else if (secondValue != null && index == 1)
            {
                bool found = false;
                for (int i = 0; i < scale2.Count; i++)
                {
                    if (scale2[i] == secondValue)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    if (totalScale2 >= secondValue.weight)
                    {
                        scale2.Add(secondValue);
                        secondValue.used = true;
                        addFluid.removeGrains(1, secondValue.weight);
                    }
                }
            }
            totalScale1 = 0;
            totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }
            scaleObj.SetActive(true);
            var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
            if (v == 0)
            {
                scaleText.text = "=";
            }
            else if (v == 1)
            {
                scaleText.text = ">";
            }
            else if (v == -1)
            {
                scaleText.text = "<";
            }
            for (int i = 0; i < scaleImages.Length / 2; i++)
            {
                if (i < scale1.Count)
                {
                    print(i);
                    if (scale1[i] != null)
                    {
                        scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
            {
                int a = i - 3;
                if (a < scale2.Count)
                {
                    print(i);
                    if (scale2[a] != null)
                    {
                        scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
        }
        
    }

    public void scaleView()
    {
        if (gameMode == GameMode.scaleLearn)
        {
            for (int i = 0; i < scaleItemPlacesL.Length; i++)
            {
                scaleItemPlacesL[i].gameObject.SetActive(false);
                scaleItemPlacesR[i].gameObject.SetActive(false);
            }
            if (firstValue != null && firstValue != nullElement)
            {
                scaleItemPlacesL[1].gameObject.SetActive(true);
                if (firstValue != nullElement) {
                    scaleItemPlacesL[1].GetComponent<Image>().sprite = firstValue.GetComponent<Image>().sprite;
                }
            }
            if (secondValue != null && secondValue != nullElement)
            {
                scaleItemPlacesR[1].gameObject.SetActive(true);
                scaleItemPlacesR[1].GetComponent<Image>().sprite = secondValue.GetComponent<Image>().sprite;
            }
            if (firstValue == null)
            {
                firstValue = nullElement;
            }
            if (secondValue == null)
            {
                secondValue = nullElement;
            }
            if (firstValue.weight > secondValue.weight)
            {
                symbolObj.GetComponent<Image>().sprite = greatSprite;
                scalePartObj.sprite = scaleSprites[2];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, scaleFallLevel.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, scaleRiseLevel.y);
                }
            }
            else if (firstValue.weight < secondValue.weight)
            {
                symbolObj.GetComponent<Image>().sprite = lessSprite;
                scalePartObj.sprite = scaleSprites[1];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, scaleRiseLevel.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, scaleFallLevel.y);
                }
            }
            else
            {
                symbolObj.GetComponent<Image>().sprite = equelSprite;
                scalePartObj.sprite = scaleSprites[0];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, defaultPosScale.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, defaultPosScale.y);
                }
            }
            if (showScaleText)
            {
                if (firstValue != nullElement && secondValue != nullElement)
                {
                    scaleObj.SetActive(true);
                    var v = Math.Clamp(firstValue.weight - secondValue.weight, -1, 1);
                    if (v == 0)
                    {
                        scaleText.text = "=";
                    }
                    else if (v == 1)
                    {
                        scaleText.text = ">";
                    }
                    else if (v == -1)
                    {
                        scaleText.text = "<";
                    }
                    scaleImages[0].sprite = firstValue.GetComponent<Image>().sprite;
                    scaleImages[1].sprite = secondValue.GetComponent<Image>().sprite;
                }
                else
                {
                    scaleObj.SetActive(false);
                }
            }
        }
        else if (gameMode == GameMode.scaleAddEquation)
        {
            float totalScale1 = 0;
            float totalScale2 = 0;
            var visibleScales1 = new List<element>();
            var visibleScales2 = new List<element>();
            for (int i = 0; i < 2; i++)
            {
                if (scale1[i] != null)
                {
                    totalScale1 += scale1[i].weight;
                    visibleScales1.Add(scale1[i]);
                }
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                if (scale2[i] != null)
                {
                    totalScale2 += scale2[i].weight;
                    visibleScales2.Add(scale2[i]);
                }
            }
            for (int i = 0; i < scaleItemPlacesL.Length; i++)
            {
                scaleItemPlacesL[i].gameObject.SetActive(false);
                scaleItemPlacesR[i].gameObject.SetActive(false);
            }
            if (visibleScales1.Count == 1)
            {
                scaleItemPlacesL[1].gameObject.SetActive(true);
                scaleItemPlacesL[1].GetComponent<Image>().sprite = visibleScales1[0].GetComponent<Image>().sprite;
            }
            else if (visibleScales1.Count == 2)
            {
                scaleItemPlacesL[2].gameObject.SetActive(true);
                scaleItemPlacesL[0].gameObject.SetActive(true);
                scaleItemPlacesL[2].GetComponent<Image>().sprite = visibleScales1[0].GetComponent<Image>().sprite;
                scaleItemPlacesL[0].GetComponent<Image>().sprite = visibleScales1[1].GetComponent<Image>().sprite;
            }
            if (visibleScales2.Count == 1)
            {
                scaleItemPlacesR[1].gameObject.SetActive(true);
                scaleItemPlacesR[1].GetComponent<Image>().sprite = visibleScales2[0].GetComponent<Image>().sprite;
            }
            else if (visibleScales2.Count == 2)
            {
                scaleItemPlacesR[0].GetComponent<Image>().gameObject.SetActive(true);
                scaleItemPlacesR[2].gameObject.SetActive(true);
                scaleItemPlacesR[0].GetComponent<Image>().sprite = visibleScales2[0].GetComponent<Image>().sprite;
                scaleItemPlacesR[2].GetComponent<Image>().sprite = visibleScales2[1].GetComponent<Image>().sprite;
            }
            if (totalScale1 > totalScale2)
            {
                symbolObj.GetComponent<Image>().sprite = greatSprite;
                scalePartObj.sprite = scaleSprites[2];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, scaleFallLevel.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, scaleRiseLevel.y);
                }
            }
            else if (totalScale2 > totalScale1)
            {
                symbolObj.GetComponent<Image>().sprite = lessSprite;
                scalePartObj.sprite = scaleSprites[1];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, scaleRiseLevel.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, scaleFallLevel.y);
                }
            }
            else
            {
                symbolObj.GetComponent<Image>().sprite = equelSprite;
                scalePartObj.sprite = scaleSprites[0];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, defaultPosScale.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, defaultPosScale.y);
                }
                StartCoroutine(PopupAnim());
            }
            scaleObj.SetActive(true);
            var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
            if (v == 0)
            {
                scaleText.text = "=";
            }
            else if (v == 1)
            {
                scaleText.text = ">";
            }
            else if (v == -1)
            {
                scaleText.text = "<";
            }
            for (int i = 0; i < scaleImages.Length / 2; i++)
            {
                if (i < scale1.Count)
                {
                    if (scale1[i] != null)
                    {
                        scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
            {
                int a = i - 3;
                if (a < scale2.Count)
                {
                    if (scale2[a] != null)
                    {
                        scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
        }
        else if (gameMode == GameMode.scale)
        {
            float totalScale1 = 0;
            float totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }
            for (int i = 0; i < scaleItemPlacesL.Length; i++)
            {
                scaleItemPlacesL[i].gameObject.SetActive(false);
                scaleItemPlacesR[i].gameObject.SetActive(false);
            }
            if (scale1.Count == 1)
            {
                scaleItemPlacesL[1].gameObject.SetActive(true);
                scaleItemPlacesL[1].GetComponent<Image>().sprite = scale1[0].GetComponent<Image>().sprite;
            }
            else if (scale1.Count == 2)
            {
                scaleItemPlacesL[2].gameObject.SetActive(true);
                scaleItemPlacesL[0].gameObject.SetActive(true);
                scaleItemPlacesL[2].GetComponent<Image>().sprite = scale1[0].GetComponent<Image>().sprite;
                scaleItemPlacesL[0].GetComponent<Image>().sprite = scale1[1].GetComponent<Image>().sprite;
            }
            if (scale2.Count == 1)
            {
                scaleItemPlacesR[1].gameObject.SetActive(true);
                scaleItemPlacesR[1].GetComponent<Image>().sprite = scale2[0].GetComponent<Image>().sprite;
            }
            else if (scale2.Count == 2)
            {
                scaleItemPlacesR[0].GetComponent<Image>().gameObject.SetActive(true);
                scaleItemPlacesR[2].gameObject.SetActive(true);
                scaleItemPlacesR[0].GetComponent<Image>().sprite = scale2[0].GetComponent<Image>().sprite;
                scaleItemPlacesR[2].GetComponent<Image>().sprite = scale2[1].GetComponent<Image>().sprite;
            }
            if (totalScale1 > totalScale2)
            {
                symbolObj.GetComponent<Image>().sprite = greatSprite;
                scalePartObj.sprite = scaleSprites[2];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, scaleFallLevel.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, scaleRiseLevel.y);
                }
            }
            else if (totalScale2 > totalScale1)
            {
                symbolObj.GetComponent<Image>().sprite = lessSprite;
                scalePartObj.sprite = scaleSprites[1];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, scaleRiseLevel.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, scaleFallLevel.y);
                }
            }
            else
            {
                symbolObj.GetComponent<Image>().sprite = equelSprite;
                scalePartObj.sprite = scaleSprites[0];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, defaultPosScale.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, defaultPosScale.y);
                }
            }

            scaleObj.SetActive(true);
            var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
            if (v == 0)
            {
                scaleText.text = "=";
            }
            else if (v == 1)
            {
                scaleText.text = ">";
            }
            else if (v == -1)
            {
                scaleText.text = "<";
            }
            for (int i = 0; i < scaleImages.Length / 2; i++)
            {
                if (i < scale1.Count)
                {
                    if (scale1[i] != null)
                    {
                        scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
            {
                int a = i - 3;
                if (a < scale2.Count)
                {
                    if (scale2[a] != null)
                    {
                        scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);

                    }

                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
        }
        else if (gameMode == GameMode.scaleMatchAdd)
        {
            float totalScale1 = 0;
            float totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }
            if (totalScale1 > totalScale2)
            {
                symbolObj.GetComponent<Image>().sprite = greatSprite;
                scalePartObj.sprite = scaleSprites[2];
                scaleDynamicElementPlace(0);
            }
            else if (totalScale2 > totalScale1)
            {
                symbolObj.GetComponent<Image>().sprite = lessSprite;
                scalePartObj.sprite = scaleSprites[1];
                scaleDynamicElementPlace(1);
            }
            else
            {
                symbolObj.GetComponent<Image>().sprite = equelSprite;
                scalePartObj.sprite = scaleSprites[0];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, defaultPosScale.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, defaultPosScale.y);
                }
                scaleDynamicElementPlace(2);
                StartCoroutine(PopupAnim());
                
            }
            for (int i = 0; i < scale1.Count; i++)
            {
                scale1[i].gameObject.GetComponent<RectTransform>().anchoredPosition = scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                scale2[i].gameObject.GetComponent<RectTransform>().anchoredPosition = scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition;
            }
            if (showScaleText)
            {
                scaleObj.SetActive(true);
                var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
                if (v == 0)
                {
                    scaleText.text = "=";
                }
                else if (v == 1)
                {
                    scaleText.text = ">";
                }
                else if (v == -1)
                {
                    scaleText.text = "<";
                }
                for (int i = 0; i < scaleImages.Length / 2; i++)
                {
                    if (i < scale1.Count)
                    {
                        scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);
                    }
                }
                for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
                {
                    int a = i - 3;
                    if (a < scale2.Count)
                    {
                        scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                        scaleImages[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        scaleImages[i].gameObject.SetActive(false);
                    }
                }
            }
            
        }
        else if (gameMode == GameMode.scaleMatchRemove)
        {
            float totalScale1 = 0;
            float totalScale2 = 0;
            for (int i = 0; i < scale1.Count; i++)
            {
                totalScale1 += scale1[i].weight;
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                totalScale2 += scale2[i].weight;
            }
            for (int i = 0; i < scaleItemPlacesL.Length; i++)
            {
                scaleItemPlacesL[i].gameObject.SetActive(false);
                scaleItemPlacesR[i].gameObject.SetActive(false);
            }
            if (scale1.Count == 1)
            {
                scaleItemPlacesL[1].gameObject.SetActive(true);
                scaleItemPlacesL[1].GetComponent<Image>().sprite = scale1[0].GetComponent<Image>().sprite;
            }
            else if (scale1.Count == 2)
            {
                scaleItemPlacesL[2].gameObject.SetActive(true);
                scaleItemPlacesL[0].gameObject.SetActive(true);
                scaleItemPlacesL[2].GetComponent<Image>().sprite = scale1[0].GetComponent<Image>().sprite;
                scaleItemPlacesL[0].GetComponent<Image>().sprite = scale1[1].GetComponent<Image>().sprite;
            }
            if (scale2.Count == 1)
            {
                scaleItemPlacesR[1].gameObject.SetActive(true);
                scaleItemPlacesR[1].GetComponent<Image>().sprite = scale2[0].GetComponent<Image>().sprite;
            }
            else if (scale2.Count == 2)
            {
                scaleItemPlacesR[0].GetComponent<Image>().gameObject.SetActive(true);
                scaleItemPlacesR[2].gameObject.SetActive(true);
                scaleItemPlacesR[0].GetComponent<Image>().sprite = scale2[0].GetComponent<Image>().sprite;
                scaleItemPlacesR[2].GetComponent<Image>().sprite = scale2[1].GetComponent<Image>().sprite;
            }
            if (totalScale1 > totalScale2)
            {
                symbolObj.GetComponent<Image>().sprite = greatSprite;
                scalePartObj.sprite = scaleSprites[2];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, scaleFallLevel.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, scaleRiseLevel.y);
                }
            }
            else if (totalScale2 > totalScale1)
            {
                symbolObj.GetComponent<Image>().sprite = lessSprite;
                scalePartObj.sprite = scaleSprites[1];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, scaleRiseLevel.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, scaleFallLevel.y);
                }
            }
            else
            {
                symbolObj.GetComponent<Image>().sprite = equelSprite;
                scalePartObj.sprite = scaleSprites[0];
                for (int i = 0; i < scaleItemPlacesL.Length; i++)
                {
                    scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, defaultPosScale.y);
                }
                for (int i = 0; i < scaleItemPlacesR.Length; i++)
                {
                    scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, defaultPosScale.y);
                }
                StartCoroutine(PopupAnim());
            }
            scaleObj.SetActive(true);
            var v = Math.Clamp(totalScale1 - totalScale2, -1, 1);
            if (v == 0)
            {
                scaleText.text = "=";
            }
            else if (v == 1)
            {
                scaleText.text = ">";
            }
            else if (v == -1)
            {
                scaleText.text = "<";
            }
            for (int i = 0; i < scaleImages.Length / 2; i++)
            {
                if (i < scale1.Count)
                {
                    scaleImages[i].sprite = scale1[i].GetComponent<Image>().sprite;
                    scaleImages[i].gameObject.SetActive(true);
                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
            for (int i = scaleImages.Length / 2; i < scaleImages.Length; i++)
            {
                int a = i - 3;
                if (a < scale2.Count)
                {
                    scaleImages[i].sprite = scale2[a].GetComponent<Image>().sprite;
                    scaleImages[i].gameObject.SetActive(true);
                }
                else
                {
                    scaleImages[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void scaleDynamicElementPlace(int id)
    {
        float yPos1 = 0f;
        float yPos2 = 0f;
        switch (id)
        {
            case 0:
                yPos1 = scaleFallLevel.y;
                yPos2 = scaleRiseLevel.y;
                break;
            case 1:
                yPos1 = scaleRiseLevel.y;
                yPos2 = scaleFallLevel.y;
                break;
            case 2:
                yPos1 = defaultPosScale.y;
                yPos2 = defaultPosScale.y;
                break;
            default:
                break;
        }
        print("scale Index:" + scalePartObj.transform.GetSiblingIndex());
        for (int i = 0; i < scaleItemPlacesL.Length; i++)
        {
            if (i < scale1.Count)
            {
                Vector2 targetPos = new Vector2(scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition.x, yPos1 - Mathf.Clamp((standartSize.y - scale1[i].gameObject.GetComponent<RectTransform>().sizeDelta.y) / 1f, -20, 50));
                scaleItemPlacesL[i].GetComponent<RectTransform>().anchoredPosition = targetPos;
                //scale1[i].transform.SetSiblingIndex(scalePartObj.transform.GetSiblingIndex() - 1);
            }
        }
        for (int i = 0; i < scaleItemPlacesR.Length; i++)
        {
            if (i < scale2.Count)
            {
                Vector2 targetPos = new Vector2(scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition.x, yPos2 - Mathf.Clamp((standartSize.y - scale2[i].gameObject.GetComponent<RectTransform>().sizeDelta.y) / 1f, -20, 50));
                scaleItemPlacesR[i].GetComponent<RectTransform>().anchoredPosition = targetPos;
                //scale2[i].transform.SetSiblingIndex(scalePartObj.transform.GetSiblingIndex() - 1);
            }
        }
    }

    public IEnumerator PopupAnim()
    {
        yield return new WaitForSeconds(1);
        if (gameMode == GameMode.scaleMatchAdd || gameMode == GameMode.scaleMatchRemove)
        {
            scalePartObj.transform.SetParent(popupObj.transform);
            scalePartObj.GetComponent<RectTransform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
            scalePartObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
            popupObj.SetActive(true);
        }else if (gameMode == GameMode.cupMatchAdd || gameMode == GameMode.cupMatchRemove || gameMode == GameMode.cupMatchAddNRemove ||gameMode == GameMode.scaleAddEquation || gameMode == GameMode.cupMatchAddNRemoveNew)
        {
            popupObj.SetActive(true);
        }
        else if (gameMode == GameMode.MultipleCupAmountMatch)
        {
            while (true)
            {
                if (!addFluid.isPouring)
                {
                    popupObj.SetActive(true);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void areaEnter(GameObject obj)
    {
        selectedArea = obj.transform;
    }

    public void areaExit()
    {
        selectedArea=null;
    }

    public void restartLevel()
    {
        if (gameMode == GameMode.comparePutSymbol)
        {
            thirdValue.gameObject.GetComponent<RectTransform>().anchoredPosition = thirdValue.startPos;
            thirdValue = null;
        }
        else if (gameMode == GameMode.putElementsBySymbol)
        {
            firstValue.GetComponent<RectTransform>().anchoredPosition = firstValue.startPos;
            secondValue.GetComponent<RectTransform>().anchoredPosition = secondValue.startPos;
            firstValue = null;
            secondValue = null;
        }
        else if (gameMode == GameMode.scaleMatchRemove || gameMode == GameMode.scaleMatchAdd || gameMode == GameMode.cupMatch || gameMode == GameMode.cubeMatch || gameMode == GameMode.MultipleCupAmountMatch || gameMode == GameMode.cupMatchAdd || gameMode == GameMode.cupMatchRemove || gameMode == GameMode.cupMatchAddNRemove || gameMode == GameMode.cupMatchAddNRemoveNew)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        popupObj.SetActive(false);
    }

    public void scaleEquationUI()
    {
        if (tour == 0)
        {
            slotParts[1].SetActive(true);
            slotParts[0].SetActive(false);
            while (true)
            {
                GameObject[] elementsArray = GameObject.FindGameObjectsWithTag("element");
                for (int i = 0; i < elementsArray.Length; i++)
                {
                    elements.Add(elementsArray[i].GetComponent<element>());
                }
                for (int i = 0; i < scale1.Count; i++)
                {
                    scale1[i] = null;
                }
                for (int i = 0; i < scale2.Count; i++)
                {
                    scale2[i] = null;
                }
                int randomElement = UnityEngine.Random.Range(0, elements.Count);
                scale1[0] = elements[randomElement];
                elements.RemoveAt(randomElement);
                randomElement = UnityEngine.Random.Range(0, elements.Count);
                scale2[0] = elements[randomElement];
                elements.RemoveAt(randomElement);

                float totalScale1 = 0;
                float totalScale2 = 0;
                for (int i = 0; i < scale1.Count; i++)
                {
                    if (scale1[i] != null)
                    {
                        totalScale1 += scale1[i].weight;
                    }
                }
                for (int i = 0; i < scale2.Count; i++)
                {
                    if (scale2[i] != null)
                    {
                        totalScale2 += scale2[i].weight;
                    }
                }
                bool acceptable = false;
                for (int i = 0; i < elements.Count; i++)
                {
                    if (totalScale1 == totalScale2 + elements[i].weight)
                    {
                        acceptable = true;
                    }
                }
                if (!acceptable)
                {
                    continue;
                }
                else
                {
                    for (int i = 0; i < scale1.Count; i++)
                    {
                        if (scale1[i] != null)
                        {
                            scale1[i].transform.position = inputs[i].transform.position;
                            scale1[i].GetComponent<Image>().raycastTarget = false;
                        }
                    }
                    scale2[0].transform.position = inputs[2].transform.position;
                    scale2[0].GetComponent<Image>().raycastTarget = false;
                    break;
                }
            }

        }
        else if (tour == 1)
        {
            slotParts[1].SetActive(true);
            slotParts[0].SetActive(false);
            while (true)
            {
                GameObject[] elementsArray = GameObject.FindGameObjectsWithTag("element");
                for (int i = 0; i < elementsArray.Length; i++)
                {
                    elements.Add(elementsArray[i].GetComponent<element>());
                }
                for (int i = 0; i < scale1.Count; i++)
                {
                    scale1[i] = null;
                }
                for (int i = 0; i < scale2.Count; i++)
                {
                    scale2[i] = null;
                }
                int randomElement = UnityEngine.Random.Range(0, elements.Count);
                scale1[0] = elements[randomElement];
                elements.RemoveAt(randomElement);
                randomElement = UnityEngine.Random.Range(0, elements.Count);
                scale2[1] = elements[randomElement];
                elements.RemoveAt(randomElement);

                float totalScale1 = 0;
                float totalScale2 = 0;
                for (int i = 0; i < scale1.Count; i++)
                {
                    if (scale1[i] != null)
                    {
                        totalScale1 += scale1[i].weight;
                    }
                }
                for (int i = 0; i < scale2.Count; i++)
                {
                    if (scale2[i] != null)
                    {
                        totalScale2 += scale2[i].weight;
                    }
                }
                bool acceptable = false;
                for (int i = 0; i < elements.Count; i++)
                {
                    if (totalScale1 == totalScale2 + elements[i].weight)
                    {
                        acceptable = true;
                    }
                }
                if (!acceptable)
                {
                    continue;
                }
                else
                {
                    for (int i = 0; i < scale1.Count; i++)
                    {
                        if (scale1[i] != null)
                        {
                            scale1[i].transform.position = inputs[i].transform.position;
                            scale1[i].GetComponent<Image>().raycastTarget = false;
                        }
                    }
                    scale2[1].transform.position = inputs[3].transform.position;
                    scale2[1].GetComponent<Image>().raycastTarget = false;
                    break;
                }
            }

        }else if (tour == 2)
        {
            slotParts[0].SetActive(true);
            slotParts[1].SetActive(false);
            while (true)
            {
                GameObject[] elementsArray = GameObject.FindGameObjectsWithTag("element");
                for (int i = 0; i < elementsArray.Length; i++)
                {
                    elements.Add(elementsArray[i].GetComponent<element>());
                }
                for (int i = 0; i < scale1.Count; i++)
                {
                    scale1[i] = null;
                }
                for (int i = 0; i < scale2.Count; i++)
                {
                    scale2[i] = null;
                }
                int randomElement = UnityEngine.Random.Range(0, elements.Count);
                scale1[0] = elements[randomElement];
                elements.RemoveAt(randomElement);
                randomElement = UnityEngine.Random.Range(0, elements.Count);
                scale1[1] = elements[randomElement];
                elements.RemoveAt(randomElement);

                float totalScale1 = 0;
                for (int i = 0; i < scale1.Count; i++)
                {
                    if (scale1[i] != null)
                    {
                        totalScale1 += scale1[i].weight;
                    }
                }
                bool acceptable = false;
                for (int i = 0; i < elements.Count; i++)
                {
                    if (totalScale1 == elements[i].weight)
                    {
                        acceptable = true;
                    }
                }
                if (!acceptable)
                {
                    continue;
                }
                else
                {
                    for (int i = 0; i < scale1.Count; i++)
                    {
                        if (scale1[i] != null)
                        {
                            scale1[i].transform.position = inputs[i].transform.position;
                            scale1[i].GetComponent<Image>().raycastTarget = false;
                        }
                    }
                    break;
                }
            }
        }else if (tour == 3)
        {
            slotParts[1].SetActive(true);
            slotParts[0].SetActive(false);
            while (true)
            {
                GameObject[] elementsArray = GameObject.FindGameObjectsWithTag("element");
                for (int i = 0; i < elementsArray.Length; i++)
                {
                    elements.Add(elementsArray[i].GetComponent<element>());
                }
                for (int i = 0; i < scale1.Count; i++)
                {
                    scale1[i] = null;
                }
                for (int i = 0; i < scale2.Count; i++)
                {
                    scale2[i] = null;
                }
                int randomElement = UnityEngine.Random.Range(0, elements.Count);
                scale2[0] = elements[randomElement];
                elements.RemoveAt(randomElement);
                randomElement = UnityEngine.Random.Range(0, elements.Count);
                scale2[1] = elements[randomElement];
                elements.RemoveAt(randomElement);

                float totalScale2 = 0;
                for (int i = 0; i < scale2.Count; i++)
                {
                    if (scale2[i] != null)
                    {
                        totalScale2 += scale2[i].weight;
                    }
                }
                bool acceptable = false;
                for (int i = 0; i < elements.Count; i++)
                {
                    if (totalScale2 == elements[i].weight)
                    {
                        acceptable = true;
                    }
                }
                if (!acceptable)
                {
                    continue;
                }
                else
                {
                    for (int i = 0; i < scale2.Count; i++)
                    {
                        if (scale2[i] != null)
                        {
                            scale2[i].transform.position = inputs[i + 2].transform.position;
                            scale2[i].GetComponent<Image>().raycastTarget = false;
                        }
                    }
                    break;
                }
            }
        }else
        {
            slotParts[1].SetActive(true);
            slotParts[0].SetActive(true);
            while (true)
            {
                GameObject[] elementsArray = GameObject.FindGameObjectsWithTag("element");
                for (int i = 0; i < elementsArray.Length; i++)
                {
                    elements.Add(elementsArray[i].GetComponent<element>());
                }
                for (int i = 0; i < scale1.Count; i++)
                {
                    scale1[i] = null;
                }
                for (int i = 0; i < scale2.Count; i++)
                {
                    scale2[i] = null;
                }
                int randomElement = UnityEngine.Random.Range(0, elements.Count);
                scale1[0] = elements[randomElement];
                elements.RemoveAt(randomElement);
                randomElement = UnityEngine.Random.Range(0, elements.Count);
                scale2[0] = elements[randomElement];
                elements.RemoveAt(randomElement);

                float totalScale1 = 0;
                float totalScale2 = 0;
                for (int i = 0; i < scale1.Count; i++)
                {
                    if (scale1[i] != null)
                    {
                        totalScale1 += scale1[i].weight;
                    }
                }
                for (int i = 0; i < scale2.Count; i++)
                {
                    if (scale2[i] != null)
                    {
                        totalScale2 += scale2[i].weight;
                    }
                }
                bool acceptable = false;
                for (int i = 0; i < elements.Count; i++)
                {
                    for (int a = 0; a < elements.Count; a++)
                    {
                        if (totalScale1 + elements[i].weight == totalScale2 + elements[a].weight && elements[i] != elements[a] && totalScale1 != totalScale2)
                        {
                            acceptable = true;
                        }
                    }       
                }
                if (!acceptable)
                {
                    continue;
                }
                else
                {
                    scale1[0].transform.position = inputs[0].transform.position;
                    scale1[0].GetComponent<Image>().raycastTarget = false;
                    scale2[0].transform.position = inputs[2].transform.position;
                    scale2[0].GetComponent<Image>().raycastTarget = false;
                    break;
                }
            }
        }
        
        scaleView();
    }

    public void nextLevel()
    {
        if (gameMode == GameMode.comparePutSymbol) {
            thirdValue.gameObject.GetComponent<RectTransform>().anchoredPosition = thirdValue.startPos;
            thirdValue = null;
            changeValues();
        }else if (gameMode== GameMode.putElementsBySymbol)
        {
            firstValue.GetComponent<RectTransform>().anchoredPosition = firstValue.startPos;
            secondValue.GetComponent<RectTransform>().anchoredPosition = secondValue.startPos;
            firstValue = null;
            secondValue = null;
            putSymbol();
        }else if (gameMode == GameMode.scaleAddEquation)
        {
            tour++;
            for (int i = 0; i < scale1.Count; i++)
            {
                if (scale1[i] != null)
                {
                    scale1[i].GetComponent<RectTransform>().anchoredPosition = scale1[i].startPos;
                    scale1[i].GetComponent<Image>().raycastTarget = true;
                    scale1[i] = null;
                }
                
            }
            for (int i = 0; i < scale2.Count; i++)
            {
                if (scale2[i] != null)
                {
                    scale2[i].GetComponent<RectTransform>().anchoredPosition = scale2[i].startPos;
                    scale2[i].GetComponent<Image>().raycastTarget = true;
                    scale2[i] = null;
                }
                
            }
            scaleEquationUI();
        }
        
        popupObj.SetActive(false);
    }

    // Pop-up kapatma fonksiyonu
    void ClosePopup()
    {
        popupObj.SetActive(false);  // Pop-up'? kapat
        ClearBoxes();               // Kutular? temizle
    }

    // Kutular? temizleme fonksiyonu
    void ClearBoxes()
    {
        firstValue = null;
        secondValue = null;
        popupElement1.sprite = null;
        popupElement2.sprite = null;
        popupText.text = "";
        popupNextLevelButton.SetActive(false);
        popupRestartButton.SetActive(false);
        rightSymbolObj.SetActive(false);
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
