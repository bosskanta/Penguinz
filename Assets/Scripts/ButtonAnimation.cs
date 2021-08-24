using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    public Transform button;
    private float scale = 0.7f;
    private float delay = 0.07f;

    public void OnClick()
    {
        button.LeanScale(new Vector3(scale, scale), delay);
        button.LeanScale(Vector3.one, delay).delay = delay;
    }
}
