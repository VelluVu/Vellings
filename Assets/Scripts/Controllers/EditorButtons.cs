using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorButtons : MonoBehaviour {


    public EditButtonType bType;
	
    public void Press()
    {
        UiControl uiControl = UiControl.singleton;

        if (uiControl.isLoading)
        {
            return;
        }
        

        switch (bType)
        {
            case EditButtonType.erase:
                if(uiControl.isTextureUI)
                {
                    return;
                   

                }
                LevelEditorControl.singleton.editState = LevelEditorControl.EditState.remove;
                Color c = Color.white;
                c.a = 0;
                LevelEditorControl.singleton.targetColor = c;
                break;

            case EditButtonType.save:
                if (uiControl.isTextureUI)
                {
                    return;
                }
                LevelEditorControl.singleton.SaveLevel();
                break;

            case EditButtonType.load:
                if (uiControl.isTextureUI)
                {
                    return;
                }
                LevelEditorControl.singleton.LoadLevel();
                break;

            case EditButtonType.clearall:
                if (uiControl.isTextureUI)
                {
                    return;
                }
                GameControl.singleton.InitLevelEditor();
                break;

            case EditButtonType.startgame:
                if (uiControl.isTextureUI)
                {
                    return;
                }
                GameControl.singleton.ChangeState(GameState.playGame);
                LevelEditorControl.singleton.editState = LevelEditorControl.EditState.paint;
                break;

            case EditButtonType.leveleditor:
                if (uiControl.isTextureUI)
                {
                    return;
                }
                GameControl.singleton.ChangeState(GameState.levelEditor);
                LevelEditorControl.singleton.editState = LevelEditorControl.EditState.paint;
                break;

            case EditButtonType.mainmenu:
                if (uiControl.isTextureUI)
                {
                    return;
                }
                GameControl.singleton.ChangeState(GameState.mainMenu);
                break;

            case EditButtonType.backbutton:
                if (uiControl.isTextureUI)
                {
                    return;
                }
                GameControl.singleton.BackButton();
                LevelEditorControl.singleton.editState = LevelEditorControl.EditState.paint;
                break;

            case EditButtonType.closeloadtextureui:
                uiControl.CloseLoadTextureUI();
                break;

            case EditButtonType.openloadtextureui:
                if (uiControl.isTextureUI)
                {
                    return;
                }
                uiControl.OpenLoadTextureUI();
                break;

            case EditButtonType.loadtexturefromwww:
                uiControl.LoadTextureFromWWW();
                break;
            case EditButtonType.createspawn:
                LevelEditorControl.singleton.editState = LevelEditorControl.EditState.spawnpoint;
                break;
            case EditButtonType.createexit:
                LevelEditorControl.singleton.editState = LevelEditorControl.EditState.exitpoint;
                break;
            case EditButtonType.exitgame:               
                Application.Quit();
                break;
            default:
                break;
        }
    }

}
public enum EditButtonType
{
    erase, save, load, clearall, startgame, leveleditor, mainmenu, exitgame, backbutton, closeloadtextureui, openloadtextureui,loadtexturefromwww,createspawn,createexit
}
