using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonIceGame : MonoBehaviour
{
    public Animator backHomeTransition, restartTransition;

    private float waitTime = 1.65f;

    public void Restart()
    {
        StartCoroutine(LoadRestart());
    }

    IEnumerator LoadRestart()
    {
        restartTransition.gameObject.SetActive(true);
        restartTransition.SetTrigger("Start");

        yield return new WaitForSeconds(waitTime);

        IceGame.instance.ResetGame();

        restartTransition.gameObject.SetActive(false);
    }

    public void BackHome()
    {
        if (IceGame.instance.PlayTimes > 1)
        {
            GameFileManager.instance.UpdateLastGamePlayTimes(IceGame.instance.PlayTimes);
        }
        StartCoroutine(LoadHome());
    }

    IEnumerator LoadHome()
    {
        backHomeTransition.gameObject.SetActive(true);

        // Play animation
        backHomeTransition.SetTrigger("BackHome");

        // Wait
        yield return new WaitForSeconds(waitTime);

        GameFileManager.instance.IncreasetId();

        // Load scene
        SceneManager.LoadScene("HomeScene");
    }
}
