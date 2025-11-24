using UnityEngine;
using System.Collections;

public class SimpleUITrigger : MonoBehaviour
{
    [Header("Pengaturan UI")]
    [Tooltip("Seret Canvas atau Panel UI yang ingin ditampilkan.")]
    public GameObject targetUI;

    [Header("Pengaturan Waktu")]
    [Tooltip("Durasi (detik) UI akan terbuka sebelum menutup otomatis.")]
    public float displayDuration = 15f; 

    private Coroutine autoCloseCoroutine; // Untuk mengelola timer

    void Start()
    {
        // Pastikan UI target nonaktif saat scene dimulai
        if (targetUI != null)
        {
            targetUI.SetActive(false);
        }
    }

    /// <summary>
    /// Dipanggil saat Player memasuki Collider (Trigger).
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Asumsi tag Player VR adalah "VRPlayer" atau "Player"
        if (other.CompareTag("Player"))
        {
            if (targetUI != null && !targetUI.activeInHierarchy)
            {
                // 1. Tampilkan UI
                targetUI.SetActive(true);
                Debug.Log(targetUI.name + " diaktifkan oleh trigger.");

                // 2. Hentikan timer sebelumnya (jika ada) dan mulai timer baru
                if (autoCloseCoroutine != null)
                {
                    StopCoroutine(autoCloseCoroutine);
                }
                autoCloseCoroutine = StartCoroutine(AutoCloseUI());
            }
        }
    }

    /// <summary>
    /// Fungsi Coroutine untuk menutup UI setelah waktu tertentu.
    /// </summary>
    private IEnumerator AutoCloseUI()
    {
        // Tunggu selama durasi yang ditentukan
        yield return new WaitForSeconds(displayDuration);
        
        // Tutup UI
        if (targetUI != null && targetUI.activeInHierarchy)
        {
            targetUI.SetActive(false);
            Debug.Log(targetUI.name + " ditutup otomatis setelah " + displayDuration + " detik.");
        }
    }
    
    /// <summary>
    /// Opsional: Membatalkan timer jika Player meninggalkan area sebelum 15 detik.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("VRPlayer") || other.CompareTag("Player"))
        {
            // Jika Player keluar, dan UI masih aktif, hentikan timer.
            if (autoCloseCoroutine != null)
            {
                 StopCoroutine(autoCloseCoroutine);
                 autoCloseCoroutine = null;
            }
        }
    }
}