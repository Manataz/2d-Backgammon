using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public bool show;
    public CanvasGroup canvas;
    
    public delegate void Function();
    public Function[] buttonFunction = new Function[3];
    
    public Text info;
    public GameObject noButton;
    public GameObject yesButton;
    public GameObject confirmButton;

    private void Start()
    {
        canvas.alpha = 0;
    }

    private void Update()
    {
        SetCanvas(canvas, show, 0, 1);
    }
    
    void SetCanvas(CanvasGroup canvas, bool active, float min, float max)
    {
        float speed = 10 * Time.deltaTime;
        
        if (active)
        {
            canvas.gameObject.SetActive(true);

            if (canvas.alpha < max)
                canvas.alpha += speed;
        }else
        {
            if (canvas.alpha > min)
                canvas.alpha -= speed;
            
            if (canvas.alpha <= 0 && min == 0)
                canvas.gameObject.SetActive(false);
        }
    }

    public void OpenPopUp(bool _noButton, bool _yesButton, bool _confirmButton, string _info, Function noButtonFunction, Function yesButtonFunction, Function confirmButtonFunction)
    {
        noButton.SetActive(_noButton);
        yesButton.SetActive(_yesButton);
        confirmButton.SetActive(_confirmButton);
        buttonFunction[0] = noButtonFunction;
        buttonFunction[1] = yesButtonFunction;
        buttonFunction[2] = confirmButtonFunction;
        info.text = _info;
        show = true;
    }

    public void ButtonClick(string type)
    {
        switch (type)
        {
            case "no":
            {
                buttonFunction[0]();
            }
                break;
            
            case "yes":
            {
                buttonFunction[1]();
            }
                break;
            
            case "confirm":
            {
                buttonFunction[2]();
            }
                break;
        }
    }
}
