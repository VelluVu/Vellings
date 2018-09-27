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
    public Text vellingEscapeCount;
    public Text vellingDeadCount;

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
    public GameObject winPopUp;
    public GameObject losePopUp;
    public Text wintext;
    public InputField inputLinkField;
    
    void Start()
    {
        Cursor.visible = false;
        canvas.SetActive(true);
        loading.SetActive(false);
        loadTextureUI.SetActive(false);
        winPopUp.SetActive(false);
        losePopUp.SetActive(false);
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

        if(button.ability == Ability.speedy)
        {

            GameControl.singleton.isSpeed = !GameControl.singleton.isSpeed;

            return;
        }

        if (curButton)
        {
            curButton.buttonImage.color = defaultColor;
        }

        curButton = button;
        defaultColor = curButton.buttonImage.color;
        curButton.buttonImage.color = selectTint;
        targetAbility = curButton.ability;
    }

    //Manipulating counters texts in UI

    public void ResetVellingCounter(int count)
    {
        vellingCount.text = "" + count;
    }

    public void ResetVellingEscape(int count)
    {
        vellingEscapeCount.text = "" + count;
    }

    public void ResetVellingDead(int count)
    {
        vellingDeadCount.text = "" + count;
    }
    
    //GameState SetUps

    public void MainMenuUISetUp()
    {
        gameUI.SetActive(false);
        levelEditor.SetActive(false);
        miniMap.SetActive(false);
        levelSelection.SetActive(true);
        mainMenu.SetActive(true);
        backButton.SetActive(false);
        winPopUp.SetActive(false);
        losePopUp.SetActive(false);
    }

    public void LevelEditorSetUp()
    {
        gameUI.SetActive(false);
        levelEditor.SetActive(true);
        miniMap.SetActive(true);
        levelSelection.SetActive(true);
        mainMenu.SetActive(false);
        backButton.SetActive(true);
    }

    public void GameUISetUp()
    {
        gameUI.SetActive(true);
        levelEditor.SetActive(false);
        miniMap.SetActive(true);
        levelSelection.SetActive(false);
        mainMenu.SetActive(false);
        backButton.SetActive(true);
    }

    public static UiControl singleton;

    void Awake()
    {   
        singleton = this;
    }
}
