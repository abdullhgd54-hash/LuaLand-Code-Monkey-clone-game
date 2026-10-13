using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    private Rigidbody2D landerRigidbody2D;

    private void Awake()
    {
        landerRigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        var current = Keyboard.current;

        if (current.wKey.isPressed)
        {
            float force = 700f;
            landerRigidbody2D.AddForce(force * transform.up * Time.deltaTime);
        }

        if (current.dKey.isPressed)
        {
            float turnSpeed = -200f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
        }

        if (current.aKey.isPressed)
        {
            float turnSpeed = 200f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log($"Crashed on the terrain");

        }

        float softLandingVelocityMagnitude = 5f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;

        if(relativeVelocityMagnitude > softLandingVelocityMagnitude)
        {
            Debug.Log("landed to hard");
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up,transform.up);
        float minDotVector = .80f;

        if(dotVector < minDotVector)
        {
            Debug.Log("landed too steep");
            return;
        }


        Debug.Log("landed successfully");

        float maxLandingAngleScore = 100f;  
        float angleRate = Math.Abs(dotVector - 1f);  
        float scoreDotVectorMultiplier = 100f;  
        float landingAngleScore = maxLandingAngleScore - (angleRate * maxLandingAngleScore * scoreDotVectorMultiplier);

        float maxLandingSpeedScore = 100f;  
        float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxLandingSpeedScore;  

        Debug.Log($"landingAngleScore {landingAngleScore}");
        Debug.Log($"landingSpeedScore {landingSpeedScore}");
    }
}
