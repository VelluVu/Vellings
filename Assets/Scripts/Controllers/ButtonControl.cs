using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour {

    public Ability ability;

    public Image buttonImage;

    public void Press()
    {
        UiControl.singleton.PressAbilityButton(this);
    }
	
}
