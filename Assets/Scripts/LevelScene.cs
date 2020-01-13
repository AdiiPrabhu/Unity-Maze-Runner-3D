using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
using System.Threading;

public class LevelScene : MonoBehaviour {

    public Texture pauseBackgroundTexture;

    public Texture pause;
    public Texture play;

    public Texture cameraMoveUp;
    public Texture cameraMoveLeft;
    public Texture cameraMoveRight;
    public Texture cameraMoveDown;

    public Texture cameraMoveForward;
    public Texture cameraMoveBack;

    public Texture cameraRotateUp;
    public Texture cameraRotateLeft;
    public Texture cameraRotateRight;
    public Texture cameraRotateDown;

    public Texture playerMoveUp;
    public Texture playerMoveLeft;
    public Texture playerMoveRight;
    public Texture playerMoveDown;

    public GameObject floor;
    public GameObject wall;
    public GameObject runner;

    public int level;

    public float scale = 1;
    public int rows = 10;
    public int columns = 10;

    public float translateRate = 3;
    public float rotateRate = 2;

    //*********************
    ThirdPersonCharacter runnerChar;

    int buttonWidth = 25;
    int buttonHeight = 25;
    int textWidth = 100;
    int textHeight = 25;

    float moveUp = 0.05f;
    float moveDown = 0.05f;
    float moveLeft = 0.05f;
    float moveRight = 0.05f;

    // Use this for initialization
    void Start()
    {
        PlayerPrefs.SetInt("Level", level);
        Game.SetLevelStats(PlayerPrefs.GetInt("Level"));

        GameObject floor = (GameObject)GameObject.Instantiate(this.floor);
        //floor.transform.localScale = new Vector3(scale, scale, scale);
        floor.transform.localScale = new Vector3(Game.MazeScale, Game.MazeScale, Game.MazeScale);

        //transform.position = new Vector3(0, 11 * scale, 0);
        transform.position = new Vector3(0, 11 * Game.MazeScale, 0);

        //Maze maze = new Maze(rows, columns, scale);
        Maze maze = new Maze(Game.MazeRows, Game.MazeColumns, Game.MazeScale);
        maze.Initialize();
        maze.Draw(runner, wall);

        ResumeLevel();
    }

    void Update()
    {
        if (Game.GameState == GameStates.InPlay)
        {
            if (GetSecondsLeft() <= 0) 
            {
                Game.GameState = GameStates.Loss;
                var currentLives = PlayerPrefs.GetInt("Lives");
                currentLives--;
                PlayerPrefs.SetInt("Lives", currentLives);
            }

            if (/*(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && */Input.GetKey(KeyCode.F4))
            {
                //ResetCameraLocation();
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) //rotate
            {
                RotateUp(CrossPlatformInputManager.GetAxis("Vertical"));
                RotateRight(CrossPlatformInputManager.GetAxis("Horizontal"));
            }
            //else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) //forward/back
            //{
            //    MoveForward(CrossPlatformInputManager.GetAxis("Vertical"));
            //}
            else if (((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.Plus)) 
                || Input.GetKey(KeyCode.KeypadPlus))
            {
                //camera move forward
                MoveForward(1);
            }
            else if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
            {
                //camera move back
                MoveForward(-1);
            }
            //else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) //up/down, left/right
            //{
            //    MoveUp(CrossPlatformInputManager.GetAxis("Vertical"));
            //    MoveRight(CrossPlatformInputManager.GetAxis("Horizontal"));
            //}
            else if (Input.GetKey(KeyCode.PageUp))
            {
                //camera move up
                MoveUp(1);
            }
            else if (Input.GetKey(KeyCode.Home))
            {
                //camera move left
                MoveRight(-1);
            }
            else if (Input.GetKey(KeyCode.End))
            {
                //camera move right
                MoveRight(1);
            }
            else if (Input.GetKey(KeyCode.PageDown))
            {
                //camera move down
                MoveUp(-1);
            }
        }
        else if (Game.GameState == GameStates.Win) 
        {
            var currentLevel = PlayerPrefs.GetInt("Level");
            currentLevel++;
            PlayerPrefs.SetInt("Level", currentLevel);
            Thread.Sleep(1000); //just for good user experience
            Game.LoadWelcomeLevel();
        }
        else if (Game.GameState == GameStates.Loss)
        {
            Thread.Sleep(1000); //just for good user experience
            Game.LoadWelcomeLevel();
        }
    }

    void OnGUI()
    {
        if (Game.GameState == GameStates.Pause) 
        {
            GUI.DrawTexture(new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), pauseBackgroundTexture);

            //**********PLAY
            if (GUI.Button(new Rect(Screen.width - (buttonWidth * 1 + 10), 10, buttonWidth, buttonHeight), play))
            {
                Game.GameState = GameStates.InPlay;
                ResumeLevel();
            }
        }
        else if (Game.GameState == GameStates.InPlay)
        {
            //**********STATS
            GUI.Label(new Rect(10, 10, 150, 50), "Time: " + GetSecondsLeft());
            GUI.Label(new Rect(10, 40, 150, 50), "Level:     " + PlayerPrefs.GetInt("Level"));
            GUI.Label(new Rect(10, 70, 150, 50), "Lives:     " + PlayerPrefs.GetInt("Lives"));

            //**********PAUSE
            if (GUI.Button(new Rect(Screen.width - (buttonWidth * 1 + 10), 10, buttonWidth, buttonHeight), pause))
            {
                Game.GameState = GameStates.Pause;
                PauseLevel();
            }

            //**********PLAYER
            GUI.Label(new Rect(Screen.width - textWidth + 10, Screen.height - (buttonHeight * 4 + 10), textWidth, textHeight), "Move Player");

            if (GUI.RepeatButton(new Rect(Screen.width - (buttonWidth * 2 + 10), Screen.height - (buttonHeight * 3 + 10), buttonWidth, buttonHeight), playerMoveUp))
            {
                moveUp += 0.07f;
                moveDown = 0.05f;
                moveLeft = 0.05f;
                moveRight = 0.05f;
                //player move up
                MovePlayerForward(-moveUp);
            }
            if (GUI.RepeatButton(new Rect(Screen.width - (buttonWidth * 3 + 10), Screen.height - (buttonHeight * 2 + 10), buttonWidth, buttonHeight), playerMoveLeft))
            {
                moveUp = 0.05f;
                moveDown = 0.05f;
                moveLeft += 0.07f;
                moveRight = 0.05f;
                //player move left
                MovePlayerRight(-moveLeft);
            }
            if (GUI.RepeatButton(new Rect(Screen.width - (buttonWidth * 1 + 10), Screen.height - (buttonHeight * 2 + 10), buttonWidth, buttonHeight), playerMoveRight))
            {
                moveUp = 0.05f;
                moveDown = 0.05f;
                moveLeft = 0.05f;
                moveRight += 0.07f;
                //player move right
                MovePlayerRight(moveRight);
            }
            if (GUI.RepeatButton(new Rect(Screen.width - (buttonWidth * 2 + 10), Screen.height - (buttonHeight + 10), buttonWidth, buttonHeight), playerMoveDown))
            {
                moveUp = 0.05f;
                moveDown += 0.07f;
                moveLeft = 0.05f;
                moveRight = 0.05f;
                //player move down
                MovePlayerForward(moveDown);
            }

            //*********CAMERA

            GUI.Label(new Rect(10, Screen.height - (buttonHeight * 10 + 10), textWidth, textHeight), "Rotate Camera");

            if (GUI.RepeatButton(new Rect(buttonWidth + 10, Screen.height - (buttonHeight * 9 + 10), buttonWidth, buttonHeight), cameraRotateUp))
            {
                //camera rotate up
                RotateUp(0.1f);
            }
            if (GUI.RepeatButton(new Rect(10, Screen.height - (buttonHeight * 8 + 10), buttonWidth, buttonHeight), cameraRotateLeft))
            {
                //camera rotate left
                RotateRight(-0.1f);
            }
            if (GUI.RepeatButton(new Rect(buttonWidth * 2 + 10, Screen.height - (buttonHeight * 8 + 10), buttonWidth, buttonHeight), cameraRotateRight))
            {
                //camera rotate right
                RotateRight(0.1f);
            }
            if (GUI.RepeatButton(new Rect(buttonWidth + 10, Screen.height - (buttonHeight * 7 + 10), buttonWidth, buttonHeight), cameraRotateDown))
            {
                //camera rotate down
                RotateUp(-0.1f);
            }

            GUI.Label(new Rect(textWidth + 20, Screen.height - (buttonHeight * 5 + 10), textWidth, textHeight * 2), "Move Camera\nZ-axis");

            if (GUI.RepeatButton(new Rect(buttonWidth * 5 + 20, Screen.height - (buttonHeight * 3 + 10), buttonWidth, buttonHeight), cameraMoveForward))
            {
                //camera move forward
                MoveForward(1);
            }
            if (GUI.RepeatButton(new Rect(buttonWidth * 5 + 20, Screen.height - (buttonHeight + 10), buttonWidth, buttonHeight), cameraMoveBack))
            {
                //camera move back
                MoveForward(-1);
            }

            GUI.Label(new Rect(10, Screen.height - (buttonHeight * 5 + 10), textWidth, textHeight * 2), "Move Camera\nXY-axes");

            if (GUI.RepeatButton(new Rect(buttonWidth + 10, Screen.height - (buttonHeight * 3 + 10), buttonWidth, buttonHeight), cameraMoveUp))
            {
                //camera move up
                MoveUp(1);
            }
            if (GUI.RepeatButton(new Rect(10, Screen.height - (buttonHeight * 2 + 10), buttonWidth, buttonHeight), cameraMoveLeft))
            {
                //camera move left
                MoveRight(-1);
            }
            if (GUI.RepeatButton(new Rect(buttonWidth * 2 + 10, Screen.height - (buttonHeight * 2 + 10), buttonWidth, buttonHeight), cameraMoveRight))
            {
                //camera move right
                MoveRight(1);
            }
            if (GUI.RepeatButton(new Rect(buttonWidth + 10, Screen.height - (buttonHeight + 10), buttonWidth, buttonHeight), cameraMoveDown))
            {
                //camera move down
                MoveUp(-1);
            }
        }
    }

    void MoveForward(float axis)
    {
        var forwardMoveAmount = axis * translateRate;
        transform.position += transform.forward * forwardMoveAmount * Time.deltaTime;
    }
    void MoveRight(float axis)
    {
        var rightMoveAmount = axis * translateRate;
        transform.position += transform.right * rightMoveAmount * Time.deltaTime;
    }
    void MoveUp(float axis)
    {
        var forwardMoveAmount = axis * translateRate;
        transform.position += transform.up * forwardMoveAmount * Time.deltaTime;
    }
    void RotateRight(float axis)
    {
        var rightTurnForce = axis * rotateRate;
        transform.Rotate(0, -rightTurnForce, 0);
    }
    void RotateUp(float axis)
    {
        var forwardTurnForce = axis * rotateRate;
        transform.Rotate(forwardTurnForce, 0, 0);
    }

    void MovePlayerForward(float axis)
    {
        //var m_CamForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
        //var m_Move = axis * m_CamForward;
        var m_Move = axis * Vector3.forward;
        ThirdPersonUserControl.ThirdPersonCharacter.Move(m_Move, false, false);

        ResumeLevel();
    }
    void MovePlayerRight(float axis)
    {
        //var m_CamForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
        var m_Move = axis * transform.right;
        //var m_Move = axis * Vector3.right;
        ThirdPersonUserControl.ThirdPersonCharacter.Move(m_Move, false, false);

        //ThirdPersonUserControl.ThirdPersonCharacter.GhostMode(true);
        PauseLevel();
    }

    float GetSecondsLeft() 
    {
        var ellapsed = Time.timeSinceLevelLoad;
        var timeLeft = Game.Duration - ellapsed;
        return timeLeft;
    }
    void ResumeLevel()
    {
        Time.timeScale = 1;
    }

    void PauseLevel()
    {
        Time.timeScale = 0;
        //var mainCam = GameObject.FindGameObjectsWithTag("MainCamera")[0];
        //mainCam.GetComponent<Camera>().backgroundColor = new Color(0, 0, 0);
    }
}
