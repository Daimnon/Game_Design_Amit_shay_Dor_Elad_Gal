using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGrid : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> LightSpotObjects;

    public bool IsGridOn;

    private void Update()
    {
        if (IsGridOn)
            foreach (GameObject lightSpot in LightSpotObjects)
                lightSpot.SetActive(true);
        else
            foreach (GameObject lightSpot in LightSpotObjects)
                lightSpot.SetActive(false);

    }

    public void TurnOn()
    {
        IsGridOn = true;
    }

    public void TurnOff()
    {
        IsGridOn = false;
    }
}
