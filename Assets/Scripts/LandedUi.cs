using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LandedUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleTextMesh;
    [SerializeField] private TextMeshProUGUI statusTextMesh;
    [SerializeField] private Button nextButton;

    private void Awake()
    {
        nextButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(3);
        });
    }
    private void Start()
    {
        Lander.Instance.OnLanded += Lander_OnLanded;
        gameObject.SetActive(false);
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        if(e.landingType == Lander.LandingType.Success)
          titleTextMesh.text = "SUCCESSFUL LANDING!";
        else
          titleTextMesh.text = "<color=#FF0000>CRASH!</color>";

        statusTextMesh.text =
            Mathf.Round(e.landingSpeed) + "\n" +
            Mathf.Round(e.angle) + "\n" +
            e.scoreMultiplier + "\n" +
            e.score; 

        gameObject.SetActive(true);
    }
}
