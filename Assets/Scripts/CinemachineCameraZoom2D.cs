using System;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraZoom2D : MonoBehaviour
{
    private const float NORMAL_ORTHOGRAPHIC_SIZE = 10f;
    public static CinemachineCameraZoom2D Instance { get; private set; }
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private float targetOrthographicSize = 10f;
    private float zoomSpeed = 2f;
    private float zoomSpeedRate => targetOrthographicSize/NORMAL_ORTHOGRAPHIC_SIZE;

    public float TargetOrthographicSize { set => targetOrthographicSize = value; }
    public void SetNormalOrthographicSize() => targetOrthographicSize = NORMAL_ORTHOGRAPHIC_SIZE;
    private void Awake()
    {
        Instance = this;
    }

    
    private void Update()
    {
        cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed * zoomSpeedRate);
    }

}
