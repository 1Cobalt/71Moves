using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Timer : MonoBehaviour
{
    public Text timerTxt;

    private float time;

    void Update()
    {
        time += Time.deltaTime;

        int minutes = (int)(time / 60); 
        int seconds = (int)(time % 60);

        //update the label value
        timerTxt.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
}
