using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObstacleRing : MonoBehaviour
{
    [Header("Ring untuk kendaraan apa?")]
    public VehicleType habitatType;

    [Header("Behaviour setelah kena kendaraan")]
    public bool destroyOnHit = true;
    public GameObject hitEffectPrefab; // optional, efek partikel

    private void Reset()
    {
        // Pastikan collider jadi trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Cari komponen ArchimedesVehicle di objek yang kena
        ArchimedesVehicle vehicle = other.GetComponentInParent<ArchimedesVehicle>();
        if (vehicle == null)
            return;

        // Cek apakah kendaraan yang masuk ring sesuai habitat
        if (vehicle.vehicleType != habitatType)
            return;

        // Tambah skor lewat ChallengeManager
        if (ChallengeManager.Instance != null)
        {
            ChallengeManager.Instance.AddScore(vehicle.vehicleType);
        }

        // Optional : efek
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // Hilangkan ring
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
