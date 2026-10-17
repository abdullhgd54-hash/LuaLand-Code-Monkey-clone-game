using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler<OnLandedEventArgs> OnLanded; public class OnLandedEventArgs{ public int score; }

    private Rigidbody2D landerRigidbody2D;
    private float fuelAmount;
    private float fuelAmountMax = 10f;

    public static Lander Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        fuelAmount = fuelAmountMax;
        landerRigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        OnBeforeForce(this, EventArgs.Empty);

        if(fuelAmount <= 0)
            return;
        var current = Keyboard.current;
        
        if(current.wKey.isPressed || current.aKey.isPressed || current.dKey.isPressed)
        {
            ConsumeFuel();
        }



        if (current.wKey.isPressed)
        {
            float force = 700f;
            landerRigidbody2D.AddForce(force * transform.up * Time.deltaTime);
            OnUpForce(this, EventArgs.Empty);
        }

        if (current.dKey.isPressed)
        {
            float turnSpeed = -200f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
            OnRightForce(this, EventArgs.Empty);
        }

        if (current.aKey.isPressed)
        {
            float turnSpeed = 200f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
            OnLeftForce(this, EventArgs.Empty);
        }
    }

    private void ConsumeFuel()
    {
        float fuelConsumptionAmount = 1f;
        fuelAmount -= fuelConsumptionAmount * Time.deltaTime; 
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            //Crashed on the terrain
            return;
        }

        float softLandingVelocityMagnitude = 5f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;

        if(relativeVelocityMagnitude > softLandingVelocityMagnitude)
        {
            //landed to hard
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up,transform.up);
        float minDotVector = .80f;

        if(dotVector < minDotVector)
        {
            //landed too steep
            return;
        }


        //landed successfully

        float maxLandingAngleScore = 100f;  
        float angleRate = Math.Abs(dotVector - 1f);  
        float scoreDotVectorMultiplier = 100f;  
        float landingAngleScore = maxLandingAngleScore - (angleRate * maxLandingAngleScore * scoreDotVectorMultiplier);

        float maxLandingSpeedScore = 100f;  
        float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxLandingSpeedScore;  

        int score = (int)(landingAngleScore + landingSpeedScore) * landingPad.ScoreMultiplier;

        Debug.Log($"score: {score}");
        OnLanded(this,new OnLandedEventArgs{ score = score });
    }
    
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if(collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
           fuelAmount += fuelPickup.Amount;

           if(fuelAmount > fuelAmountMax) 
                fuelAmount = fuelAmountMax; 
                
           fuelPickup.DestroySelf(); 
        }

        if(collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
           OnCoinPickup(this, EventArgs.Empty);
           coinPickup.DestroySelf(); 
        }
    }

    public float GetSpeedX() => landerRigidbody2D.linearVelocityX;
    public float GetSpeedY() => landerRigidbody2D.linearVelocityY;
    public float GetFeul() => fuelAmount;
    public float GetFeulNormalized() => fuelAmount/fuelAmountMax;

}
