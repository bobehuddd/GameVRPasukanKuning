using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SubmarineDepthControl : MonoBehaviour
{
    // REFERENSI UTAMA KE KAPAL SELAM
    [Tooltip("Seret objek Kapal Selam (yang memiliki SubmarineMovement.cs) ke sini.")]
    public SubmarineMovement submarineController;

    [Header("Pengaturan Tuas Kedalaman")]
    // minLocalY = Tangki Kosong (0%)
    [Tooltip("Posisi Y lokal terendah (0%: Tangki Kosong/Mengapung)")]
    public float minLocalY = -0.05f; 

    // maxLocalY = Tangki Penuh (100%)
    [Tooltip("Posisi Y lokal tertinggi (100%: Tangki Penuh/Menyelam)")]
    public float maxLocalY = 0.05f;

    void Update()
    {
        float currentLocalY = transform.localPosition.y;
        float clampedY = Mathf.Clamp(currentLocalY, minLocalY, maxLocalY);

        // Membatasi posisi transform lokal secara fisik
        if (clampedY != currentLocalY)
        {
            Vector3 newPosition = transform.localPosition;
            newPosition.y = clampedY;
            transform.localPosition = newPosition;
        }

        // Konversi posisi Y menjadi fraksi (0 hingga 1) isi tangki pemberat
        // 0 = Tangki Kosong, 1 = Tangki Penuh
        float ballastFraction = Mathf.InverseLerp(minLocalY, maxLocalY, clampedY);
        
        // Mengirim fraksi isi tangki ke SubmarineMovement.cs
        if (submarineController != null)
        {
            submarineController.SetDepthInput(ballastFraction);
        }
    }
}