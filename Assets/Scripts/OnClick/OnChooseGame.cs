using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnChooseGame : MonoBehaviour
{
    public Transform gameBox;
    public Button playButton;

    private float scale = 0.8f, delay = 0.07f;

    private void Awake()
    {
        GameFileManager.instance.GameName = "";
        playButton.gameObject.SetActive(false);
    }

    public void ChooseGame()
    {
        if (GameFileManager.instance.GameName == "")
        {
            playButton.gameObject.SetActive(true);
            playButton.transform.LeanScale(Vector3.one, 0.5f).setEaseOutBack();

            playButton.image.color = MyColors.Angry;
            playButton.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = MyColors.White;
        }

        GameFileManager.instance.GameName = gameObject.name;

        gameBox.LeanScale(new Vector3(scale, scale), delay);
        gameBox.LeanScale(Vector3.one, delay).delay = delay;

        Transform border;

        if (gameObject.name.Equals("ice"))
        {
            border = GameObject.Find("ice_border").transform;
        }
        else
        {
            border = GameObject.Find("breath_border").transform;
        }

        border.LeanScale(new Vector3(scale, scale), delay);
        border.LeanScale(Vector3.one, delay).delay = delay;
    }

    private void Update()
    {
        Image border;

        if (gameObject.name.Equals("ice"))
        {
            border = GameObject.Find("ice_border").GetComponent<Image>();
        }
        else
        {
            border = GameObject.Find("breath_border").GetComponent<Image>();
        }
        // If click level, set color
        if (GameFileManager.instance.GameName.Equals(gameObject.name))
        {
            if (gameObject.name.Equals("ice") && !border.color.Equals(MyColors.IceGameBorder))
            {
                border.color = MyColors.IceGameBorder;
                gameObject.GetComponent<Image>().color = MyColors.White;
            }
            else if (gameObject.name.Equals("breath") && !border.color.Equals(MyColors.BreathGameBorder))
            {
                border.color = MyColors.BreathGameBorder;
                gameObject.GetComponent<Image>().color = MyColors.White;
            }
        }
        else
        {
            if (!border.color.Equals(MyColors.ZeroAlpha))
            {
                border.color = MyColors.ZeroAlpha;
                gameObject.GetComponent<Image>().color = MyColors.ZeroAlpha;
            }
        }
    }
}
