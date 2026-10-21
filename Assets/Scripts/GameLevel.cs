using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    public int LevelNumber => levelNumber;

    [SerializeField] private Transform landerStartTransform;    public Vector3 landerStartPosition => landerStartTransform.position;

    [SerializeField] private Transform cameraStartTargetTransform;  public Transform CameraStartTargetPosition => cameraStartTargetTransform;
    
    [SerializeField] private float zoomedOutOrthographicSize;   public float ZoomedOutOrthographicSize => zoomedOutOrthographicSize;

    private void Start()
    {
        Lander.Instance.OnStateChanged += Lander_OnStateChanged;
    }

    private void Lander_OnStateChanged(object sender, Lander.OnStateChangedEventArgs e)
    {
        landerStartTransform.gameObject.SetActive(false);
    }
}
