using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ArchimedesVehicle : MonoBehaviour
{
    [Header("Tipe Kendaraan")]
    public VehicleType vehicleType;

    private void Reset()
    {
        // Biar collider di-set jadi non-trigger untuk fisika
        var col = GetComponent<Collider>();
        col.isTrigger = false;
    }
}
