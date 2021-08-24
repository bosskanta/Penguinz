using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickPlayButton : MonoBehaviour
{
    public GameObject angerCheck;

    public void ClickPlay()
    {
        angerCheck.SetActive(true);
    }
}
