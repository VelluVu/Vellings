using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInputField : MonoBehaviour {

    public InputField inpField;

	public void Press()
    {
        if ( string.IsNullOrEmpty(inpField.text))
        {
            return;
        }
        LevelEditor.singleton.levelName = inpField.text;
    }
}
