//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class OnClickLevelManager : MonoBehaviour
//{
//    public Transform levelBox;

//    private float scale = 0.8f, delay = 0.07f;
//    private Color32 defaultColor;

//    private void Awake()
//    {
//        Image boxColor = gameObject.transform.GetChild(0).GetComponent<Image>();
//        defaultColor = boxColor.color;
//    }

//    public void OnClickLevel()
//    {
//        if (NoteManager.instance.Level == gameObject.name)
//        {
//            NoteManager.instance.Level = "";
//        }
//        else
//        {
//            // Animate level box
//            levelBox.LeanScale(new Vector3(scale, scale), delay);
//            levelBox.LeanScale(Vector3.one, delay).delay = delay;

//            NoteManager.instance.Level = gameObject.name;
//        }
//    }

//    private void Update()
//    {
//        // If click level, set color
//        if (NoteManager.instance.Level == gameObject.name)
//        {
//            if (!gameObject.GetComponent<Image>().color.Equals(MyColors.OnChooseBox))
//            {
//                gameObject.GetComponent<Image>().color = MyColors.OnChooseBox;
//            }
//            NoteManager.instance.whatLevelTMP.color = MyColors.NoteTitle;
//            gameObject.transform.GetChild(0).GetComponent<Image>().color = MyColors.White;
//        }
//        else
//        {
//            gameObject.GetComponent<Image>().color = MyColors.ZeroAlpha;
//            gameObject.transform.GetChild(0).GetComponent<Image>().color = defaultColor;
//        }
//    }
//}
