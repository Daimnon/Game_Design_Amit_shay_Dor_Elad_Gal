using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class PowrGrid : MonoBehaviour, IPointerDownHandler
{
    public LightGrid LightGridScript;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (LightGridScript.IsGridOn)
        {
            LightGridScript.TurnOn();
            Debug.Log("Grid false");
        }
        else
        {
            LightGridScript.TurnOff();
            Debug.Log("Grid true");
        }
    }
}
