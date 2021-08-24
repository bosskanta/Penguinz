using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickPlayGame : MonoBehaviour
{
    public Animator iceTransition, breathTransition;

    public void StartGame()
    {
        if (GameFileManager.instance.GameName.Equals("ice"))
        {
            iceTransition.gameObject.SetActive(true);
            iceTransition.SetTrigger("Start");
        }
        else if (GameFileManager.instance.GameName.Equals("breath"))
        {
            breathTransition.gameObject.SetActive(true);
            breathTransition.SetTrigger("Start");
        }
    }
}
