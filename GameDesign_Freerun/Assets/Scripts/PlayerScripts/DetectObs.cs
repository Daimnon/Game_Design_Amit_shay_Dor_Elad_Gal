using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectObs : MonoBehaviour
{
    [SerializeField]
    private string _objectTagName = "";

    [SerializeField]
    public bool Obstruction;

    private GameObject _object;
    private Collider _currentCol;

    void OnTriggerStay(Collider col)
    {
        if (_objectTagName != "" && !Obstruction)
        {
            if (col.GetComponent<CustomTag>().IsEnabled)
            {
                if (col != null && !col.isTrigger && col.GetComponent<CustomTag>().HasTag(_objectTagName)) // checks if the object has the right tag
                {
                    Obstruction = true;
                    _object = col.gameObject;
                    _currentCol = col;
                }
            }
        }


        if (_objectTagName == "" && !Obstruction)
        {
            if (col != null && !col.isTrigger)
            {
                Obstruction = true;
                _currentCol = col;
            }
        }
    }

    private void Update()
    {
        
        if (_object == null || !_currentCol.enabled)
            Obstruction = false;

        if (_object != null)
            if (!_object.activeInHierarchy)
                Obstruction = false;
    }

    void OnTriggerExit(Collider col)
    {
        if (col == _currentCol)
            Obstruction = false;
    }
}
