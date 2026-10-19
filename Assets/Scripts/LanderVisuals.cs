
using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class LanderVisuals : MonoBehaviour
{
    [SerializeField] private ParticleSystem leftThrusterParticleSystem;
    [SerializeField] private ParticleSystem midThrusterParticleSystem;
    [SerializeField] private ParticleSystem rightThrusterParticleSystem;
    [SerializeField] private GameObject landerExplosionVfx;

    private Lander lander;
    private void Awake()
    {
        lander = GetComponent<Lander>();

        lander.OnUpForce += Lander_OnUpForce; 
        lander.OnRightForce += Lander_OnRightForce; 
        lander.OnLeftForce += Lander_OnLeftForce; 
        lander.OnBeforeForce += Lander_OnBeforeForce; 
    }

    private void Start()
    {
        lander.OnLanded += Lander_OnLanded;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        switch (e.landingType)
        {
            case Lander.LandingType.TooFastLanding:
            case Lander.LandingType.TooSteepAngle:
            case Lander.LandingType.WrongLandingArea:
                Instantiate(landerExplosionVfx,transform.position,quaternion.identity);
                gameObject.SetActive(false);
                break;
        }
    }

    private void Lander_OnBeforeForce(object sender, EventArgs e)
    {
        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem,false);
        SetEnabledThrusterParticleSystem(midThrusterParticleSystem,false);
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem,false);
    }

    private void Lander_OnRightForce(object sender, EventArgs e)
    {
        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem,true);
    }
    private void Lander_OnLeftForce(object sender, EventArgs e)
    {
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem,true);
    }

    private void Lander_OnUpForce(object sender, EventArgs e)
    {
        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem,true);
        SetEnabledThrusterParticleSystem(midThrusterParticleSystem,true);
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem,true);
    }

    private void SetEnabledThrusterParticleSystem(ParticleSystem particleSystem, bool enabled)
    {
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        emissionModule.enabled = enabled;
    }

}
