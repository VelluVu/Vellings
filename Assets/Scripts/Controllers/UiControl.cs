using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UiControl : MonoBehaviour {

    public Transform mTrans;
    public Image mouse;
    public Sprite cTarget1;
    public Sprite cTarget2;
    public Sprite cSelection;

    public bool overUnit;
    public bool chooseAbility;
    public Ability targetAbility;

    public ButtonControl curButton;

    public Color selectTint;
    Color defaultColor;

    public GameObject canvas;

    void Start()
    {
        Cursor.visible = false;
        canvas.SetActive(true);
    }

    public void FrameTick()
    {
        mTrans.transform.position = Input.mousePosition;
        
        if (overUnit)
        {
            mouse.sprite = cSelection;
        } else
        {
            mouse.sprite = cTarget1;
        }
    }

    public void PressAbilityButton(ButtonControl button)
    {
        if (curButton)
        {
            curButton.buttonImage.color = defaultColor;
        }

        curButton = button;
        defaultColor = curButton.buttonImage.color;
        curButton.buttonImage.color = selectTint;
        targetAbility = curButton.ability;

    }

    public static UiControl singleton;

    void Awake()
    {
        singleton = this;
    }
}
