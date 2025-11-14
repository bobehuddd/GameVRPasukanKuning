using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Diperlukan untuk interaksi VR jika tombol adalah XR Interactable

public class HotAirBalloonController : MonoBehaviour
{
    // Objek Balon Udara yang posisinya akan diubah (biasanya transform ini sendiri)
    [Header("Pengaturan Balon & Kontrol")]
    [Tooltip("Posisi Y awal balon saat api padam.")]
    public float initialYPosition; 

    [Tooltip("Kenaikan posisi Y per level api (misal: 1 meter)")]
    public float heightIncreasePerLevel = 1.0f;

    [Tooltip("Kecepatan transisi pergerakan Y (interpolasi).")]
    public float movementSpeed = 0.5f; 

    // Level Api: 0 (Padam), 1 (Kecil), 2 (Sedang), 3 (Besar)
    private int flameLevel = 0; 
    
    // Ketinggian target yang akan diinterpolasi oleh Update
    private float targetYPosition;

    // Komponen-komponen visual (Opsional: Seret objek Api Kecil, Api Sedang, Api Besar di Inspector)
    [Header("Visual Api (Opsional)")]
    public GameObject smallFlameVisual;
    public GameObject mediumFlameVisual;
    public GameObject largeFlameVisual;

    void Start()
    {
        // Tetapkan posisi Y awal
        initialYPosition = transform.position.y;
        targetYPosition = initialYPosition;
        
        // Pastikan semua visual api mati saat dimulai
        UpdateFlameVisuals();

        Debug.Log("Balon Udara Siap. Api level: " + flameLevel);
    }

    void Update()
    {
        // Gerakkan balon ke target Y secara bertahap (interpolasi halus)
        float newY = Mathf.Lerp(transform.position.y, targetYPosition, Time.deltaTime * movementSpeed);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // Dipanggil saat Tombol Hijau ditekan
    public void IncreaseFlameLevel()
    {
        if (flameLevel < 3)
        {
            // Naikkan level api (Level 0 -> 1 -> 2 -> 3)
            flameLevel++;
            
            // Perbarui ketinggian target
            targetYPosition = initialYPosition + (flameLevel * heightIncreasePerLevel);
            
            Debug.Log("Level Api Naik ke: " + GetFlameName(flameLevel) + ". Target Y: " + targetYPosition);
        }
        else
        {
            // Kondisi 4: Api Besar, Player tekan tombol hijau = tidak terjadi apa-apa
            Debug.Log("Level Api sudah Maksimum (Besar).");
        }
        
        UpdateFlameVisuals();
    }

    // Dipanggil saat Tombol Merah ditekan
    public void DecreaseFlameLevel()
    {
        if (flameLevel == 1)
        {
            // Kondisi 2: Api Kecil, Player tekan tombol merah = turun ke level padam/mati
            flameLevel = 0; // Turun ke level Padam
            
            // Ketinggian kembali ke posisi semula (initialYPosition)
            targetYPosition = initialYPosition; 
            
            Debug.Log("Level Api Turun ke: Padam. Target Y: " + targetYPosition);
        }
        else if (flameLevel > 1)
        {
            // Kondisi 3 (Sedang) dan Kondisi 4 (Besar), turun satu level
            flameLevel--;
            
            // Ketinggian Y kurang 1
            targetYPosition = initialYPosition + (flameLevel * heightIncreasePerLevel);
            
            Debug.Log("Level Api Turun ke: " + GetFlameName(flameLevel) + ". Target Y: " + targetYPosition);
        }
        else
        {
            // Kondisi 1: Api Mati (level 0), Player tekan tombol merah = tidak terjadi apa-apa
            Debug.Log("Api sudah Padam. Tidak ada perubahan.");
        }

        UpdateFlameVisuals();
    }

    // Fungsi utilitas untuk memperbarui tampilan visual api
    private void UpdateFlameVisuals()
    {
        bool small = (flameLevel == 1);
        bool medium = (flameLevel == 2);
        bool large = (flameLevel == 3);

        if (smallFlameVisual != null) smallFlameVisual.SetActive(small);
        if (mediumFlameVisual != null) mediumFlameVisual.SetActive(medium);
        if (largeFlameVisual != null) largeFlameVisual.SetActive(large);
    }

    // Fungsi utilitas untuk debugging
    private string GetFlameName(int level)
    {
        switch (level)
        {
            case 0: return "Padam";
            case 1: return "Kecil";
            case 2: return "Sedang";
            case 3: return "Besar";
            default: return "Error";
        }
    }
}