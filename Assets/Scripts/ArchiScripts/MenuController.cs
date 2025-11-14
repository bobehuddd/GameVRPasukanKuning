using UnityEngine;
using UnityEngine.SceneManagement; // Diperlukan untuk SceneManager
using UnityEngine.UI; // Opsional, jika Anda perlu mengakses komponen UI

public class MenuController : MonoBehaviour
{
    [Header("Pengaturan Scene")]
    [Tooltip("Nama scene yang akan dituju")]
    public string targetSceneName = "Kelas Archimedes";

    // Fungsi yang dipanggil oleh tombol atas ("Scene Kelas Archimedes")
    public void LoadArchimedesScene()
    {
        Debug.Log("Mencoba memuat scene: " + targetSceneName);

        // Pastikan scene sudah ditambahkan ke Build Settings (File > Build Settings)
        try
        {
            SceneManager.LoadScene(targetSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Gagal memuat scene '" + targetSceneName + "'. Pastikan nama scene benar dan ada di Build Settings. Error: " + e.Message);
        }
    }

    // Fungsi yang dipanggil oleh tombol bawah ("Exit Game")
    public void QuitGame()
    {
        Debug.Log("Mencoba keluar dari aplikasi...");

#if UNITY_EDITOR
        // Berfungsi untuk menghentikan pemutaran di Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Berfungsi untuk keluar dari aplikasi yang sudah di-build (PC/APK)
            Application.Quit();
#endif
    }
}
