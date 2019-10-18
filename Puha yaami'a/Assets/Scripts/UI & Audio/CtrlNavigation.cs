using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CtrlNavigation : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelection;

    private void OnEnable()
    {
        //if (defaultSelection.CompareTag("UI_Element"))
        //    defaultSelection.GetComponent<Selectable>().Select();
    }
}
