using UnityEngine;
using System.Collections.Generic;

public class TheoryUIManager : MonoBehaviour
{
    // [HARUS DIPERHATIKAN]
    // Referensi ke TheoryTrigger. Dapatkan dari Game Object Meja yang memiliki collider.
    public TheoryTrigger theoryTrigger; 

    // Ganti referensi individual dengan Array publik
    // Urutan Panel yang Anda masukkan di Inspector akan menjadi urutan navigasi (misal: Sub2, Panel_2, Panel_3, Panel_4)
    public GameObject[] navigationPanels; 
    
    // List internal yang menyimpan urutan panel dari array di atas
    private List<GameObject> uiPanels;
    private int currentPanelIndex = 0;

    void Awake()
    {
        // Pindahkan array publik ke List internal untuk navigasi
        uiPanels = new List<GameObject>(navigationPanels);
        
        // Cek dasar
        if (uiPanels.Count == 0 || theoryTrigger == null)
        {
            Debug.LogError("TheoryUIManager Error: Panel atau TheoryTrigger belum terhubung di Inspector!");
            return;
        }
        
        // Pastikan hanya panel pertama (index 0) yang aktif, sisanya nonaktif
        ShowPanel(uiPanels[0]);
    }
    
    // ... (Fungsi ShowInitialPanel dan ShowPanel tetap sama, tetapi ShowInitialPanel harus dipanggil oleh trigger) ...
    public void ShowInitialPanel()
    {
        currentPanelIndex = 0;
        ShowPanel(uiPanels[currentPanelIndex]);
    }

    private void ShowPanel(GameObject panelToShow)
    {
        foreach (GameObject panel in uiPanels)
        {
            if (panel != null)
            {
                // Set panel yang aktif ke true, sisanya ke false
                panel.SetActive(panel == panelToShow);
            }
        }
    }

    // --- FUNGSI PUBLIK UNTUK TOMBOL ---

    /// <summary>
    /// Digunakan untuk tombol "Next". Pindah ke panel berikutnya dalam urutan.
    /// </summary>
    public void GoToNextPanel()
    {
        if (currentPanelIndex < uiPanels.Count - 1)
        {
            currentPanelIndex++;
            ShowPanel(uiPanels[currentPanelIndex]);
        }
        else
        {
            Debug.Log("Sudah di Panel UI terakhir.");
        }
    }

    /// <summary>
    /// Digunakan untuk tombol "Prev". Kembali ke panel sebelumnya dalam urutan.
    /// </summary>
    public void GoToPreviousPanel()
    {
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            ShowPanel(uiPanels[currentPanelIndex]);
        }
        else
        {
            Debug.Log("Sudah di Panel UI pertama.");
        }
    }

    /// <summary>
    /// Digunakan untuk tombol "Mulai" (atau tombol penyelesaian).
    /// </summary>
    public void CompleteMenu()
    {
        // Panggil fungsi di TheoryTrigger untuk menyelesaikan teori dan mengaktifkan objek simulasi
        if (theoryTrigger != null)
        {
            theoryTrigger.CompleteTheory();
        }
        // Sembunyikan semua panel UI.
        foreach (GameObject panel in uiPanels)
        {
            if (panel != null) panel.SetActive(false);
        }
        currentPanelIndex = 0; // Reset index
    }
}