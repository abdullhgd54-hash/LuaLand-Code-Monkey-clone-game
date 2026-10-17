using System;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatusUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusTextMesh;
    [SerializeField] private GameObject speedUpArrowDameObject;
    [SerializeField] private GameObject speedDownArrowDameObject;
    [SerializeField] private GameObject speedLeftArrowDameObject;
    [SerializeField] private GameObject speedRightArrowDameObject;
    [SerializeField] private Image fuelImage;

    private void Update() => UpdateStatusTextMesh();
    private void UpdateStatusTextMesh()
    {
        speedUpArrowDameObject.SetActive(Lander.Instance.GetSpeedY() >= 0);
        speedDownArrowDameObject.SetActive(Lander.Instance.GetSpeedY() < 0);
        speedRightArrowDameObject.SetActive(Lander.Instance.GetSpeedX() >= 0);
        speedLeftArrowDameObject.SetActive(Lander.Instance.GetSpeedX() < 0);

        fuelImage.fillAmount = Lander.Instance.GetFeulNormalized();

        statusTextMesh.text = 
            GameManager.Instance.GetScore() + "\n" +
            Mathf.Round(GameManager.Instance.GetTime()) + "\n" +
            Mathf.Abs(Mathf.Round(Lander.Instance.GetSpeedX() * 10f)) + "\n" +
            Mathf.Abs(Mathf.Round(Lander.Instance.GetSpeedY() * 10f)) + "\n";
    }
}
