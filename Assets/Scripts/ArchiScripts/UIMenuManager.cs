using UnityEngine;
using System.Collections.Generic;

public class UIMenuManager : MonoBehaviour
{
    // Deklarasikan semua Panel UI utama yang akan dikelola
    // Seret objek Panel (GameObject) dari Inspector ke slot ini di Unity Editor.
    public GameObject panelAwal;
    public GameObject panelInstruksi;
    public GameObject panelPenutup;
    
    // List untuk menyimpan urutan panel. Ini penting untuk fungsi Next/Previous.
    private List<GameObject> uiPanels;
    private int currentPanelIndex = 0;

    void Start()
    {
        // Inisialisasi list urutan Panel: [0] Awal, [1] Instruksi, [2] Penutup
        uiPanels = new List<GameObject> { panelAwal, panelInstruksi, panelPenutup };

        // Pastikan semua panel dideklarasikan
        if (uiPanels.Contains(null))
        {
            Debug.LogError("Semua Panel UI harus di-assign di Inspector! Mohon periksa Panel Awal, Instruksi, dan Penutup.");
            return;
        }

        // Tampilkan panel pertama (Panel Awal) dan sembunyikan sisanya saat Start
        ShowPanel(panelAwal);
        currentPanelIndex = 0; // Pastikan indeks diatur ke 0 (Panel Awal)
    }

    /// <summary>
    /// Menampilkan panel yang diberikan dan menyembunyikan semua panel lainnya dalam list.
    /// </summary>
    private void ShowPanel(GameObject panelToShow)
    {
        foreach (GameObject panel in uiPanels)
        {
            // Set panel yang aktif ke true, sisanya ke false
            panel.SetActive(panel == panelToShow);
        }
    }

    // --- FUNGSI PUBLIK UNTUK TOMBOL ---

    /// <summary>
    /// Digunakan untuk tombol "Next". Pindah ke panel berikutnya dalam urutan (Awal -> Instruksi -> Penutup).
    /// </summary>
    public void GoToNextPanel()
    {
        // Cek apakah masih ada panel berikutnya
        if (currentPanelIndex < uiPanels.Count - 1)
        {
            currentPanelIndex++;
            ShowPanel(uiPanels[currentPanelIndex]);
            Debug.Log($"Pindah ke Panel: {uiPanels[currentPanelIndex].name} (Index: {currentPanelIndex})");
        }
        else
        {
            // Ini seharusnya hanya terjadi jika tombol 'Next' ditekan di Panel Penutup (yang tidak seharusnya ada).
            Debug.Log("Sudah di Panel UI terakhir (Panel Penutup). Tidak bisa lanjut.");
        }
    }

    /// <summary>
    /// Digunakan untuk tombol "Prev". Kembali ke panel sebelumnya dalam urutan (Penutup -> Instruksi -> Awal).
    /// </summary>
    public void GoToPreviousPanel()
    {
        // Cek apakah bukan panel pertama
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            ShowPanel(uiPanels[currentPanelIndex]);
            Debug.Log($"Kembali ke Panel: {uiPanels[currentPanelIndex].name} (Index: {currentPanelIndex})");
        }
        else
        {
            Debug.Log("Sudah di Panel UI pertama (Panel Awal). Tidak bisa mundur.");
        }
    }

    /// <summary>
    /// Digunakan untuk tombol "Mulai" (atau tombol penyelesaian) pada Panel Penutup.
    /// Menyelesaikan/menutup semua UI menu dan memulai simulasi.
    /// </summary>
    public void CompleteMenu()
    {
        Debug.Log("Tombol Mulai ditekan. Menyembunyikan UI dan Memulai Simulasi...");
        
        // Sembunyikan semua panel UI, yang secara efektif menyembunyikan Canvas_UI_Awal jika semua panel di bawahnya.
        foreach (GameObject panel in uiPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        // TODO: Tambahkan kode di sini untuk memulai simulasi VR, 
        // seperti mengaktifkan kontrol pemain, memuat scene, dll.
        
        // Opsional: Jika Canvas_UI_Awal adalah parent dari semua panel:
        // Cukup nonaktifkan Canvas_UI_Awal.
        // GameObject canvasRoot = transform.parent.gameObject; // Asumsi script ini ada di Canvas_UI_Awal atau objek di dalamnya
        // if(canvasRoot.name == "Canvas_UI_Awal")
        // {
        //     canvasRoot.SetActive(false);
        // }
    }
}