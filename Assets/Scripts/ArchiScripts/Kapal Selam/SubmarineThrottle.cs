using UnityEngine;


public class SubmarineThrottle : MonoBehaviour
{
    // Objek Kapal Selam yang akan dikendalikan (memiliki SubmarineMovement.cs)
    [Tooltip("Seret objek Kapal Selam (yang memiliki SubmarineMovement.cs) ke sini.")]
    public SubmarineMovement submarineController;

    [Header("Pengaturan Tuas")]
    [Tooltip("Posisi Y lokal terendah dari tuas (0: Mesin OFF)")]
    public float minLocalY = -0.05f; 

    [Tooltip("Posisi Y lokal tertinggi dari tuas (1: Kecepatan Maks)")]
    public float maxLocalY = 0.05f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        // Pemeriksaan komponen XR Grab Interactable
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        // Pemeriksaan referensi ke kapal selam
        if (submarineController == null)
        {
            Debug.LogError("Submarine Controller belum diatur pada SubmarineThrottle! Harap seret objek kapal selam di Inspector.");
        }
    }

    void Update()
    {
        // 1. Dapatkan posisi Y lokal saat ini (relatif terhadap parent)
        float currentLocalY = transform.localPosition.y;

        // 2. Membatasi posisi Y (agar tuas tidak bisa ditarik melewati batas)
        float clampedY = Mathf.Clamp(currentLocalY, minLocalY, maxLocalY);

        // 3. Jika posisi Y berubah karena batasan, update posisi transform lokal
        if (clampedY != currentLocalY)
        {
            Vector3 newPosition = transform.localPosition;
            newPosition.y = clampedY;
            transform.localPosition = newPosition;
        }

        // 4. Mengonversi posisi Y yang sudah dibatasi menjadi input throttle (0 hingga 1)
        // InverseLerp mengubah rentang (minLocalY hingga maxLocalY) menjadi 0.0 hingga 1.0.
        float throttleInput = Mathf.InverseLerp(minLocalY, maxLocalY, clampedY);

        // 5. Mengirim input throttle ke kapal selam
        if (submarineController != null)
        {
            submarineController.SetThrottleInput(throttleInput);
        }
    }
}