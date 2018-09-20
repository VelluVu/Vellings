using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UiControl : MonoBehaviour {

    public Transform mTrans;
    public Image mouse;
    public Image miniMapRenderer;
    public Sprite cTarget1;
    public Sprite cTarget2;
    public Sprite cSelection;
    public Text vellingCount;

    public bool isLoading;
    public bool isTextureUI;
    public bool overUnit;
    public bool chooseAbility;
    public Ability targetAbility;

    public ButtonControl curButton;
    public Transform levelGrid;
    public Color selectTint;
    Color defaultColor;

    public GameObject canvas;
    public GameObject mainMenu;
    public GameObject gameUI;
    public GameObject levelEditor;
    public GameObject miniMap;
    public GameObject levelSelection;
    public GameObject backButton;
    public GameObject loading;
    public GameObject loadTextureUI;
    public InputField inputLinkField;
    
    void Start()
    {
        Cursor.visible = false;
        canvas.SetActive(true);
        loading.SetActive(false);
        loadTextureUI.SetActive(false);
    }

    //Replaces Update, this is updated on GameControl class.
    public void Tick()
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

    public void OpenLoadTextureUI()
    {
        loadTextureUI.SetActive(true);
        isTextureUI = true;
    }

    public void CloseLoadTextureUI()
    {
        loadTextureUI.SetActive(false);
        isTextureUI = false;
    }

    public void LoadTextureFromWWW ()
    {
        isLoading = true;
        GameControl.singleton.LoadTextureFromWWW(inputLinkField.text);
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

    public void IncrementVellingCounter(int count)
    {
        vellingCount.text = "" + count;

    }

    public void ResetVellingCounter(int count)
    {
        vellingCount.text = "" + count;
    }

    void Awake()
    {
        singleton = this;
    }
}
