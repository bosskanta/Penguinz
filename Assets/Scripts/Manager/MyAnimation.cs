using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAnimation : MonoBehaviour
{
    public static MyAnimation instance;
    public Animator penguinAnimator;

    private const string OPEN_APP = "OpenApp", INTERACT = "Interact";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        penguinAnimator.SetTrigger(OPEN_APP);
    }

    
    public void Interact()
    {
        penguinAnimator.SetTrigger(INTERACT);
    }
}
