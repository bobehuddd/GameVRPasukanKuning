using UnityEngine;
using UnityEngine.Video;

public class TriggerVideoController : MonoBehaviour
{
    // Variabel yang akan dihubungkan di Inspector Unity
    [Header("Video Player")]
    public VideoPlayer videoPlayer; // Seret komponen VideoPlayer dari Monitor ke sini

    [Header("Player Tag")]
    // Pastikan Player Anda memiliki Tag "Player"
    public string playerTag = "Player"; 

    void Start()
    {
        // Pastikan VideoPlayer diinisialisasi dan tidak langsung memutar saat Start
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            // Opsional: Sembunyikan objek layar/monitor jika VideoPlayer memiliki render mode tertentu
            // Contoh: Jika VideoPlayer merender ke MeshRenderer/GameObject
            // videoPlayer.gameObject.SetActive(false); 
        }
    }

    // Dipanggil ketika objek lain masuk ke dalam Collider (yang harus 'Is Trigger')
    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah objek yang masuk adalah Player
        if (other.CompareTag(playerTag))
        {
            Debug.Log(gameObject.name + " Triggered. Memutar video.");
            if (videoPlayer != null)
            {
                // videoPlayer.gameObject.SetActive(true); // Aktifkan layar jika disembunyikan
                videoPlayer.Play();
            }
        }
    }

    // Dipanggil ketika objek lain keluar dari Collider (yang harus 'Is Trigger')
    private void OnTriggerExit(Collider other)
    {
        // Cek apakah objek yang keluar adalah Player
        if (other.CompareTag(playerTag))
        {
            Debug.Log(gameObject.name + " Un-Triggered. Menghentikan video.");
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
                // videoPlayer.gameObject.SetActive(false); // Sembunyikan layar jika diaktifkan
            }
        }
    }
}