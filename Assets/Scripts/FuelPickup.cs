using UnityEngine;

public class FuelPickup : MonoBehaviour
{
    [SerializeField] private float amount = 10f;
    public float Amount => amount;
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

}
