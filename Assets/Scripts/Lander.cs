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
    public event EventHandler OnFuelPickup;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs
    {
        public State state;
    }
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs
    { 
        public int score;
        public LandingType landingType;
        public float angle;
        public float landingSpeed;
        public float scoreMultiplier;
    }

    public enum LandingType
    {
        Success,
        WrongLandingArea,
        TooSteepAngle,
        TooFastLanding

    } 
    public enum State
    {
        WaitingToStart,
        Normal,
        GameOver
    } 

    private const float GRAVITY_NORMAL = 0.7f;
    private Rigidbody2D landerRigidbody2D;
    private float fuelAmount;
    private float fuelAmountMax = 10f;
    private State state;

    public static Lander Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        fuelAmount = fuelAmountMax;
        state = State.WaitingToStart;

        landerRigidbody2D = GetComponent<Rigidbody2D>();
        landerRigidbody2D.gravityScale = 0f;
    }
    private void FixedUpdate()
    {
        OnBeforeForce(this, EventArgs.Empty);

        var input = GameInput.Instance;
        var movementInput = input.MovementInput;
        
        

        switch (state)
        {
            case State.WaitingToStart:
                if(input.IsUpActionPressed() || input.IsRightActionPressed() || input.IsLeftActionPressed() || movementInput != Vector2.zero)
                {
                    landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
                    SetState(State.Normal);                
                }
                break;
            case State.Normal:
                if(fuelAmount <= 0)
                    return;
                    
                float movementDeadzone = 0.4f;    
                if (input.IsUpActionPressed() || movementInput.y > movementDeadzone)
                {
                    float force = 700f;
                    landerRigidbody2D.AddForce(force * transform.up * Time.deltaTime);
                    OnUpForce(this, EventArgs.Empty);
                }

                if (input.IsRightActionPressed() || movementInput.x > movementDeadzone)
                {
                    float turnSpeed = -200f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
                    OnRightForce(this, EventArgs.Empty);
                }

                if (input.IsLeftActionPressed() || movementInput.x < -movementDeadzone)
                {
                    float turnSpeed = 200f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
                    OnLeftForce(this, EventArgs.Empty);
                }
                
                if(input.IsUpActionPressed() || input.IsRightActionPressed() || input.IsLeftActionPressed() || movementInput != Vector2.zero)
                {
                    ConsumeFuel();
                }
                break;
            case State.GameOver:
                break;
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
            OnLanded(this,new OnLandedEventArgs
            {
             landingType = LandingType.WrongLandingArea,
             score = 0,
             angle = 0,
             landingSpeed = 0,
             scoreMultiplier = 0
            });
            SetState(State.GameOver);
            return;
        }

        float softLandingVelocityMagnitude = 5f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;

        if(relativeVelocityMagnitude > softLandingVelocityMagnitude)
        {
            //landed to hard
            OnLanded(this,new OnLandedEventArgs
            {
             landingType = LandingType.TooFastLanding,
             score = 0,
             angle = 0,
             landingSpeed = relativeVelocityMagnitude,
             scoreMultiplier = 0
            });
            SetState(State.GameOver);
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up,transform.up);
        float minDotVector = .80f;

        if(dotVector < minDotVector)
        {
            //landed too steep
            OnLanded(this,new OnLandedEventArgs
            {
             landingType = LandingType.TooSteepAngle,
             score = 0,
             angle = dotVector,
             landingSpeed = relativeVelocityMagnitude,
             scoreMultiplier = 0
            });
            SetState(State.GameOver);
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
        OnLanded(this,new OnLandedEventArgs
        {
             score = score,
             angle = dotVector,
             landingSpeed = relativeVelocityMagnitude,
             landingType = LandingType.Success,
             scoreMultiplier = landingPad.ScoreMultiplier
         });
         SetState(State.GameOver);
    }
    
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if(collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
           fuelAmount += fuelPickup.Amount;

           OnFuelPickup(this, EventArgs.Empty);
           
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
    private void OnTriggerExit2D(Collider2D collider2D)
    {
        if(collider2D.gameObject.TryGetComponent(out LevelWall levelWall))
        {
           OnLanded(this,new OnLandedEventArgs
            {
             landingType = LandingType.WrongLandingArea,
             score = 0,
             angle = 0,
             landingSpeed = 0,
             scoreMultiplier = 0
            });
            SetState(State.GameOver);
            return;
        }
    }

    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged(this, new OnStateChangedEventArgs{state = state});
    }
    public float GetSpeedX() => landerRigidbody2D.linearVelocityX;
    public float GetSpeedY() => landerRigidbody2D.linearVelocityY;
    public float GetFeul() => fuelAmount;
    public float GetFeulNormalized() => fuelAmount/fuelAmountMax;

}
