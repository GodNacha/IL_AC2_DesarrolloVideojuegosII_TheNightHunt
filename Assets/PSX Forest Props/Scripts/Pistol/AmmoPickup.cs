using UnityEngine;

public class AmmoPickup : MonoBehaviour, IShootable
{
    public int ammoAmount = 1;

    public void OnShot()
    {
        Pistol pistol = FindFirstObjectByType<Pistol>();

        if (pistol != null)
        {
            pistol.AddAmmo(ammoAmount);
        }

        Destroy(gameObject);
    }
}