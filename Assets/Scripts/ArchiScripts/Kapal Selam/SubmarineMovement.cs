using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Memastikan Rigidbody ada
public class SubmarineMovement : MonoBehaviour
{
    [Header("Pengaturan Fisika Kapal Selam")]
    [Tooltip("Volume kapal selam (m^3). Semakin besar, semakin besar gaya apung.")]
    public float submarineVolume = 10f; 
    
    [Tooltip("Massa kapal selam tanpa air pemberat (kg)")]
    public float baseMass = 1000f; 

    [Tooltip("Maksimum air yang dapat diisi ke tangki pemberat (kg). Ini adalah massa tambahan.")]
    public float maxBallastMass = 500f; 

    [Tooltip("Massa jenis fluida (air laut) dalam kg/m^3.")]
    public float fluidDensity = 1025f; // Massa jenis air laut

    [Header("Pengaturan Kontrol & Gaya")]
    [Tooltip("Kecepatan maju maksimum (diatur oleh throttle)")]
    public float maxForwardSpeed = 50f; // Nilai lebih tinggi karena ini adalah gaya
    
    [Tooltip("Kecepatan putar kapal (derajat per detik)")]
    public float turnSpeed = 40f; 
    
    [Tooltip("Gaya yang diperlukan untuk menekan kapal ke bawah saat melayang (opsional)")]
    public float pitchForce = 10f; 

    // Input dari Stir: -1 (Kiri) hingga 1 (Kanan)
    private float currentSteeringInput = 0f; 
    
    // Input dari Tuas Kanan (Throttle): 0 (Mati) hingga 1 (Penuh)
    private float currentThrottleInput = 0f; 
    
    // Input dari Tuas Kiri (Kedalaman): 0 (Kosong/Mengapung) hingga 1 (Penuh/Menyelam)
    private float currentDepthInput = 0f; // Ini akan menjadi fraksi (persentase) tangki yang terisi
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody tidak ditemukan! Pastikan SubmarineMovement berada di objek dengan Rigidbody.");
            return;
        }

        // Atur Rigidbody agar tidak berputar secara tidak sengaja di sumbu X dan Z
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Set massa awal kapal (Base Mass + 50% Ballast Mass)
        rb.mass = baseMass + (maxBallastMass * 0.5f); 
    }

    // Dipanggil oleh SubmarineSteering.cs
    public void SetSteeringInput(float input) { currentSteeringInput = input; }

    // Dipanggil oleh SubmarineThrottle.cs
    public void SetThrottleInput(float input) { currentThrottleInput = input; }

    // Dipanggil oleh SubmarineDepthControl.cs
    // Input di sini adalah fraksi (0-1) air pemberat
    public void SetDepthInput(float input) 
    { 
        currentDepthInput = Mathf.Clamp01(input);
        
        // 1. Perhitungan Massa Total
        // Ubah massa kapal secara real-time berdasarkan isi tangki pemberat
        float currentBallastMass = maxBallastMass * currentDepthInput;
        rb.mass = baseMass + currentBallastMass; 
    }

    void FixedUpdate()
    {
        // 1. Gaya Apung (Buoyancy Force)
        // FA = Rho * V * g (Gaya Apung = Massa Jenis Fluida * Volume Objek * Gravitasi)
        // Gaya Apung selalu bekerja ke atas (Vector3.up)
        float gravity = Physics.gravity.magnitude;
        float buoyancyForceMagnitude = fluidDensity * submarineVolume * gravity;
        
        Vector3 buoyancyForce = Vector3.up * buoyancyForceMagnitude;
        rb.AddForce(buoyancyForce, ForceMode.Force);

        // 2. Kontrol Horizontal (Maju & Belok)
        
        // Gaya Maju (Propulsion Force)
        float actualPropulsionForce = currentThrottleInput * maxForwardSpeed;
        rb.AddForce(transform.forward * actualPropulsionForce, ForceMode.Force);

        // Rotasi (Belok) - Menggunakan Rotasi Sudut (Angular Velocity)
        float rotationAmount = currentSteeringInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotationAmount, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // 3. Kontrol Vertikal Tambahan (Optional: Pitch/Dive Planes)
        // Ini adalah cara lain kapal selam dapat mengatur sudutnya saat menyelam.
        // Anda bisa menambahkan logika kontrol pitch/dive plane di sini jika ada input tambahan.
        // Contoh sederhana (tidak diperlukan jika hanya mengandalkan ballast):
        // rb.AddRelativeTorque(Vector3.right * currentDepthInput * pitchForce);
    }
}