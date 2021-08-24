using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAfterController : MonoBehaviour
{
    [SerializeField] private Transform box;

    private void OnEnable()
    {
        box.localScale = Vector2.zero;
        box.LeanScale(Vector2.one, 0.3f).setEaseOutBack();
    }

    public void Confirm()
    {
        NoteManager.instance.SaveLevelAfter();
        box.LeanScale(Vector2.zero, 0.3f).setEaseInBack().setOnComplete(OnClose);
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
    }
}
