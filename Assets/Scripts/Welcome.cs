using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Welcome : MonoBehaviour
{

    public Texture welcomeBackgroundTexture;
    public Texture winBackgroundTexture;
    public Texture lossBackgroundTexture;

    string instruction = "Welcome\n\n" +
        "Use arrow keys to move the runner, Space-Bar to jump, C to crouch.\n\n" +
        "Use Ctrl + arrow keys to rotate camera.\n\n" +
        "Use Plus and Minus keys to move camera forward and backward.\n\n" +
        "Use Page-Up, Page-Down, Home and End keys to move camera up, down, left and right.";
    string playText = "Play";

    int buttonHeight = 40;
    Rect backgroundRect, playRect, playFromStartRect, exitRect;

    void Start()
    {
        if (!PlayerPrefs.HasKey("Level"))
        {
            StartFromBeginning();
        }

        backgroundRect = new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2);
        playRect = new Rect(backgroundRect.xMin + 10, backgroundRect.yMax - (buttonHeight + 10), backgroundRect.width / 3, buttonHeight);
        playFromStartRect = new Rect(playRect.xMax + 10, playRect.yMin, playRect.width, buttonHeight);
        exitRect = new Rect(backgroundRect.xMax - (buttonHeight + 10), playRect.yMin, buttonHeight, buttonHeight);
    }

    void OnGUI()
    {
        var gameCompleted = false;

        if (Game.GameState == GameStates.Win)
        {
            GUI.DrawTexture(backgroundRect, winBackgroundTexture);
            playText = string.Format("Move to Level {0}", PlayerPrefs.GetInt("Level"));

            gameCompleted = PlayerPrefs.GetInt("Level") == Game.EndLevel;
        }
        else if (Game.GameState == GameStates.Loss)
        {
            GUI.DrawTexture(backgroundRect, lossBackgroundTexture);
            playText = string.Format("Try Level {0} Again", PlayerPrefs.GetInt("Level"));
        }
        else
        {
            GUI.DrawTexture(backgroundRect, welcomeBackgroundTexture);
            //GUI.Label(new Rect(10, 10, 600, 600), instruction);
            playText = string.Format("Play Level {0}", PlayerPrefs.GetInt("Level"));

            gameCompleted = PlayerPrefs.GetInt("Level") == Game.EndLevel;
        }

        if (!gameCompleted)
        {
            if (GUI.Button(playRect, playText))
            {
                Game.LoadLevel(PlayerPrefs.GetInt("Level"));
            }
        }
        else
        {
            GUI.Label(playRect, "You have completed the game!");
        }

        if (GUI.Button(playFromStartRect, "Play From Beginning"))
        {
            StartFromBeginning();
            Game.LoadLevel(PlayerPrefs.GetInt("Level"));
        }

        if (GUI.Button(exitRect, "Exit"))
        {
            Application.Quit();
        }
    }

    void StartFromBeginning()
    {
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("Lives", 5);
        PlayerPrefs.SetInt("Score", 0);
    }
}