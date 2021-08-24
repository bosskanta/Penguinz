using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickTab : MonoBehaviour
{
    public void ClickTab()
    {
        if (gameObject.name == "NoteTab")
        {
            DayManager.instance.SetTab(true);
        }
        else
        {
            DayManager.instance.SetTab(false);
        }
    }
}
