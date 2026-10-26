using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button soundVolumeButton;
    [SerializeField] private TextMeshProUGUI soundVolumeTextMesh;
    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.UnPauseGame();
        });

        soundVolumeButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeSoundVolume();
            soundVolumeTextMesh.text = "SOUND " + SoundManager.Instance.SoundVolume;
        });


        mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });
        
    }

    private void Start()
    {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused; 
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused; 

        soundVolumeTextMesh.text = "SOUND " + SoundManager.Instance.SoundVolume;

        gameObject.SetActive(false);
    }

    private void GameManager_OnGamePaused(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        resumeButton.Select();
    }
    private void GameManager_OnGameUnpaused(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
    }

}
