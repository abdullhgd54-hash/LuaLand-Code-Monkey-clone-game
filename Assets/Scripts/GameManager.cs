using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static int currentLevelNumber = 1;
    private static int totalScore = 0; public int TotalScore => totalScore;

    public static void ResetStaticData()
    {
        currentLevelNumber = 1;
        totalScore = 0;
    } 
    
    public int CurrentLevelNumber => currentLevelNumber;
    [SerializeField] private List<GameLevel> gameLevels;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private GameObject statusUI;
    private int score;
    private float time;
    private bool isTimerActive;
    public static GameManager Instance { get; private set; }
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnStateChanged += Lander_OnStateChanged;

        GameInput.Instance.OnMenuButtonPressed += GameInput_OnMenuButtonPressed;
        LoadCurrentLevel();
    }

    private void GameInput_OnMenuButtonPressed(object sender, EventArgs e)
    {
        PauseUnPauseGame();
    }

   

    private void Lander_OnStateChanged(object sender, Lander.OnStateChangedEventArgs e)
    {
        isTimerActive = e.state == Lander.State.Normal;

        if(e.state == Lander.State.Normal)
        {
            cinemachineCamera.Target.TrackingTarget = Lander.Instance.transform;
            CinemachineCameraZoom2D.Instance.SetNormalOrthographicSize(); 
            statusUI.SetActive(true);
        }
        
    }

    private void Update()
    {
        if(isTimerActive)
            time += Time.deltaTime;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        AddScore(e.score);
    }

    private void Lander_OnCoinPickup(object sender, EventArgs e)
    {
        AddScore(500);
    }

    private void AddScore(int scoreAmount)
    {
        score += scoreAmount;
    }

    public int GetScore() => score;

    public float GetTime() => time;
    private void LoadCurrentLevel()
    {
        GameLevel gameLevel = GetGameLevel();
        if(gameLevel != null)
        {   
            GameLevel spawnedGameLevel = Instantiate(gameLevel, Vector3.zero, Quaternion.identity);
            Lander.Instance.transform.position = spawnedGameLevel.landerStartPosition;
            cinemachineCamera.Target.TrackingTarget = spawnedGameLevel.CameraStartTargetPosition;
            CinemachineCameraZoom2D.Instance.TargetOrthographicSize = spawnedGameLevel.ZoomedOutOrthographicSize;
        }
    }

    private GameLevel GetGameLevel()
    {
        foreach (var gameLevel in gameLevels)
        {
            if (gameLevel.LevelNumber == currentLevelNumber)
                return gameLevel;
        }

        return null;
    }

    public void RetryLevel()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }
    public void GoToNextLevel()
    {
        currentLevelNumber++;
        totalScore += score;

        if(GetGameLevel() == null)
            SceneLoader.LoadScene(SceneLoader.Scene.GameOverScene);
        else
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }

    private void PauseUnPauseGame()
    {
        if(Time.timeScale == 1f)
            PauseGame(); 
        else
            UnPauseGame();
        
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        OnGamePaused(this, EventArgs.Empty);
    }
    public void UnPauseGame()
    {
        Time.timeScale = 1f; 
        OnGameUnpaused(this, EventArgs.Empty);
    }

}
