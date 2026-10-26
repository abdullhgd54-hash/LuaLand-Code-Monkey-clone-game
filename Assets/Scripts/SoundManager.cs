using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    private const int SOUND_VOLUME_MAX = 10;
    private int soundVolume = 6; public int SoundVolume => soundVolume; 
    public float SoundVolumeNormalized => ((float)soundVolume)/SOUND_VOLUME_MAX;
    [SerializeField] private AudioClip fuelPickupAudioClip;
    [SerializeField] private AudioClip coinPickupAudioClip;
    [SerializeField] private AudioClip crashAudioClip;
    [SerializeField] private AudioClip landingSuccessAudioClip;
    public event EventHandler OnSoundVolumeChanged;
    private void Awake() => Instance = this;
    private void Start()
    {
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnFuelPickup += Lander_OnFuelPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        switch (e.landingType)
        {
            case Lander.LandingType.Success:
                AudioSource.PlayClipAtPoint(landingSuccessAudioClip, Camera.main.transform.position, SoundVolumeNormalized);
                break;
            default:
                AudioSource.PlayClipAtPoint(crashAudioClip, Camera.main.transform.position, SoundVolumeNormalized);
                break;
        }
    }

    private void Lander_OnFuelPickup(object sender, EventArgs e)
    {
        AudioSource.PlayClipAtPoint(fuelPickupAudioClip, Camera.main.transform.position, SoundVolumeNormalized);
    }

    private void Lander_OnCoinPickup(object sender, EventArgs e)
    {
        AudioSource.PlayClipAtPoint(coinPickupAudioClip, Camera.main.transform.position, SoundVolumeNormalized);
    }

    public void ChangeSoundVolume()
    {
        int soundVolumeMax = 10;
        soundVolume = (soundVolume + 1) % soundVolumeMax;
        OnSoundVolumeChanged(this, EventArgs.Empty);
    }

}
