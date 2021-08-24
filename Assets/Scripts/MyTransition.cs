using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyTransition : MonoBehaviour
{
    public void LoadIceScene()
    {
        SceneManager.LoadScene("IceScene");
    }

    public void LoadBreathScene()
    {
        SceneManager.LoadScene("BreathScene");
    }
}
