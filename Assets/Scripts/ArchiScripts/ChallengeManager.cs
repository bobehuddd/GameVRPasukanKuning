using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeManager Instance;

    [Header("Target Poin")]
    public int targetBalonUdara = 5;
    public int targetKapalLaut = 5;
    public int targetKapalSelam = 5;

    [Header("UI Text")]
    public TMP_Text skorBalonUdaraText;
    public TMP_Text skorKapalLautText;
    public TMP_Text skorKapalSelamText;

    private int skorBalonUdara;
    private int skorKapalLaut;
    private int skorKapalSelam;

    private void Awake()
    {
        // Singleton sederhana
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Kalau mau persist antar scene:
        // DontDestroyOnLoad(gameObject);

        UpdateAllUI();
    }

    public void AddScore(VehicleType type)
    {
        switch (type)
        {
            case VehicleType.BalonUdara:
                if (skorBalonUdara < targetBalonUdara)
                    skorBalonUdara++;
                break;

            case VehicleType.KapalLaut:
                if (skorKapalLaut < targetKapalLaut)
                    skorKapalLaut++;
                break;

            case VehicleType.KapalSelam:
                if (skorKapalSelam < targetKapalSelam)
                    skorKapalSelam++;
                break;
        }

        UpdateAllUI();
        CheckCompleted(type);
    }

    private void UpdateAllUI()
    {
        if (skorBalonUdaraText != null)
            skorBalonUdaraText.text = $"Skor Balon Udara : {skorBalonUdara} / {targetBalonUdara}";

        if (skorKapalLautText != null)
            skorKapalLautText.text = $"Skor Kapal Laut : {skorKapalLaut} / {targetKapalLaut}";

        if (skorKapalSelamText != null)
            skorKapalSelamText.text = $"Skor Kapal Selam : {skorKapalSelam} / {targetKapalSelam}";
    }

    private void CheckCompleted(VehicleType type)
    {
        // Di sini kamu bisa kasih efek kalau challenge sudah selesai
        // Contoh sederhana (debug saja)
        switch (type)
        {
            case VehicleType.BalonUdara:
                if (skorBalonUdara >= targetBalonUdara)
                    Debug.Log("Challenge Balon Udara selesai!");
                break;

            case VehicleType.KapalLaut:
                if (skorKapalLaut >= targetKapalLaut)
                    Debug.Log("Challenge Kapal Laut selesai!");
                break;

            case VehicleType.KapalSelam:
                if (skorKapalSelam >= targetKapalSelam)
                    Debug.Log("Challenge Kapal Selam selesai!");
                break;
        }
    }
}
