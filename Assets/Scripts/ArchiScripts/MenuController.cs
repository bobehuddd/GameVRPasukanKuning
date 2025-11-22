using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Pengaturan Scene")]
    [Tooltip("Nama scene tujuan (harus sama persis dengan nama di Build Settings)")]
    [SerializeField] private string targetSceneName = "KelasArchimedes";

    [Header("Root Main Menu (opsional)")]
    [Tooltip("Root canvas / panel main menu. Akan di-nonaktifkan saat pindah scene.")]
    [SerializeField] private GameObject mainMenuRoot;

    private bool isLoading = false;

    // Dipanggil oleh tombol atas: "Scene Kelas Archimedes"
    public void LoadArchimedesScene()
    {
        if (isLoading) return; // cegah double klik
        isLoading = true;

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("[MenuController] targetSceneName kosong!");
            isLoading = false;
            return;
        }

        Debug.Log("[MenuController] Mencoba memuat scene: " + targetSceneName);

        // Matikan UI main menu jika ada
        if (mainMenuRoot != null)
        {
            mainMenuRoot.SetActive(false);
        }

        // Pastikan nama scene sudah ada di File > Build Settings
        SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Single);
    }

    // Dipanggil oleh tombol bawah: "Exit Game"
    public void QuitGame()
    {
        Debug.Log("[MenuController] Mencoba keluar dari aplikasi...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // stop Play di Editor
#else
        Application.Quit();                                // keluar di build (PC / APK)
#endif
    }
}
