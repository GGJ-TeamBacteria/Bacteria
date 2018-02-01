using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideFromEvent : MonoBehaviour
{
    public GameObject[] targets;

    public void ShowHide()
    {
        foreach (var target in targets)
        {
            target.SetActive(!target.activeInHierarchy);
        }
    }
}
