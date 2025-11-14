using UnityEngine;
using System.Collections.Generic; // Diperlukan untuk List

public class TheoryTrigger : MonoBehaviour
{
    // Objek yang perlu dinonaktifkan/diaktifkan bersamaan dengan UI Teori
    public GameObject canvasUITeori;

    // Gunakan List/Array untuk menampung SEMUA objek simulasi (Tuas, Stir, UI Interaksi, dll.)
    // Anda akan memasukkan objek simulasi yang berbeda untuk setiap meja di Inspector.
    public List<GameObject> simulationObjectsToToggle;

    // Variabel untuk melacak apakah Player sudah menyelesaikan teori
    private TheoryUIManager uiManager;
    private bool isTheoryCompleted = false;

    void Start()
    {
        // Pastikan Canvas_UI_Teori awalnya nonaktif
        if (canvasUITeori != null)
        {
            canvasUITeori.SetActive(false);
            // Dapatkan referensi ke TheoryUIManager
            uiManager = canvasUITeori.GetComponent<TheoryUIManager>();
            if (uiManager == null)
            {
                Debug.LogError("TheoryUIManager component not found on Canvas_UI_Teori in " + gameObject.name + "!");
            }
        }
        
        // Pastikan objek simulasi awalnya aktif
        SetSimulationObjectsActive(true);
    }

    /// <summary>
    /// Digunakan oleh TheoryUIManager saat tombol MULAI ditekan.
    /// </summary>
    public void CompleteTheory()
    {
        isTheoryCompleted = true;
        // Setelah teori selesai, aktifkan kembali objek simulasi
        SetSimulationObjectsActive(true);
        // Nonaktifkan Canvas UI Teori
        if (canvasUITeori != null)
        {
            canvasUITeori.SetActive(false);
        }
        Debug.Log("Teori " + gameObject.name + " Selesai. Objek Simulasi Aktif!");
    }

    /// <summary>
    /// Mengatur status aktif semua objek simulasi dalam list.
    /// </summary>
    private void SetSimulationObjectsActive(bool isActive)
    {
        foreach (GameObject obj in simulationObjectsToToggle)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }

    // Dipanggil saat objek lain memasuki collider Meja
    private void OnTriggerEnter(Collider other)
    {
        // Asumsi tag Player VR adalah "VRPlayer" atau "Player"
        if (other.CompareTag("VRPlayer") || other.CompareTag("Player"))
        {
            if (!isTheoryCompleted && canvasUITeori != null)
            {
                // 1. Nonaktifkan objek simulasi
                SetSimulationObjectsActive(false);
                
                // 2. Aktifkan Canvas UI Teori
                canvasUITeori.SetActive(true);
                
                // 3. Reset/tampilkan panel awal 
                if (uiManager != null)
                {
                    uiManager.ShowInitialPanel();
                }
                Debug.Log("Player memasuki area " + gameObject.name + ". UI Teori aktif.");
            }
        }
    }
}