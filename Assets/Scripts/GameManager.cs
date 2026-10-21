using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static int currentLevelNumber = 1;
    public int CurrentLevelNumber => currentLevelNumber;
    [SerializeField] private List<GameLevel> gameLevels;
    private int score;
    private float time;
    private bool isTimerActive;
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnStateChanged += Lander_OnStateChanged;

        LoadCurrentLevel();
    }


    private void Lander_OnStateChanged(object sender, Lander.OnStateChangedEventArgs e)
    {
        isTimerActive = e.state == Lander.State.Normal;
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
        foreach (var gameLevel in gameLevels)
        {
            if(gameLevel.LevelNumber == currentLevelNumber)
            {
                GameLevel spawnedGameLevel = Instantiate(gameLevel,Vector3.zero,Quaternion.identity);
                Lander.Instance.transform.position = spawnedGameLevel.landerStartPosition;
            }
        }
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(3);
    }
    public void GoToNextLevel()
    {
        if(currentLevelNumber++ >= gameLevels.Count)
            currentLevelNumber = 1;
        SceneManager.LoadScene(3);
    }

}
