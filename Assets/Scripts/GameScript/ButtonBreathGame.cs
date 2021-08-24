using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBreathGame : MonoBehaviour
{
    public Animator backHomeTransition, restartTransition;

    private float waitTime = 1.4f;

    public void Restart()
    {
        StartCoroutine(LoadRestart());
    }

    IEnumerator LoadRestart()
    {
        restartTransition.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.2f);

        BreathGame.instance.ResetGame();

        restartTransition.gameObject.SetActive(false);
    }
    public void BackHome()
    {
        if (BreathGame.instance.PlayTimes > 1)
        {
            GameFileManager.instance.UpdateLastGamePlayTimes(BreathGame.instance.PlayTimes);
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
