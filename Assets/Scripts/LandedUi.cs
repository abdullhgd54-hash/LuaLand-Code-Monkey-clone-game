using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LandedUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleTextMesh;
    [SerializeField] private TextMeshProUGUI statusTextMesh;
    [SerializeField] private TextMeshProUGUI nextButtonTextMesh;
    [SerializeField] private Button nextButton;
    private Action nextButtonClickAction;

    private void Awake()
    {
        nextButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
            nextButtonClickAction();
        });

    }
    private void Start()
    {
        Lander.Instance.OnLanded += Lander_OnLanded;
        gameObject.SetActive(false);
        nextButton.Select();
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        statusTextMesh.text =
            Mathf.Round(e.landingSpeed)  + "\n" +
            Mathf.Round(e.angle * 100f) + "\n" +
            e.scoreMultiplier + "\n";

        if(e.landingType == Lander.LandingType.Success)
        {
            titleTextMesh.text = "SUCCESSFUL LANDING!";
            statusTextMesh.text += e.score + GameManager.Instance.GetScore(); 
            nextButtonTextMesh.text = "CONTINUE!";
            nextButtonClickAction = GameManager.Instance.GoToNextLevel;
        }
        else
        {
          titleTextMesh.text = "<color=#FF0000>CRASH!</color>";
          statusTextMesh.text += e.score; 
            nextButtonTextMesh.text = "RETRY";
            nextButtonClickAction = GameManager.Instance.RetryLevel;
            
        }


        gameObject.SetActive(true);

    }
}
