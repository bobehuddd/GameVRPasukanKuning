using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BNG {
    public class SubmarineOxygenUI : MonoBehaviour {

        [Header("References")]
        [Tooltip("Controller kedalaman kapal selam (yang punya bool IsSubmerged).")]
        public SubmarineDepthController depthController;

        [Tooltip("UI Text untuk menampilkan persentase oksigen (misal: \"Oksigen : 100%\").")]
        public TMP_Text oxygenText;   // Jika pakai TextMeshPro, ganti tipe ke TMP_Text

        [Header("Oxygen Settings")]
        [Tooltip("Nilai oksigen awal / maksimum (dalam %).")]
        public float maxOxygen = 100f;

        [Tooltip("Kecepatan berkurangnya oksigen saat menyelam (persen per detik).")]
        public float depletionRate = 1f;   // 1% per detik

        [Tooltip("Apakah oksigen akan terisi lagi saat kapal di permukaan.")]
        public bool regenerateOnSurface = false;

        [Tooltip("Kecepatan pengisian ulang oksigen di permukaan (persen per detik).")]
        public float regenRate = 5f;

        private float currentOxygen;

        public float CurrentOxygenPercent {
            get { return currentOxygen; }
        }

        void Start() {
            currentOxygen = maxOxygen;
            UpdateOxygenUI();
        }

        void Update() {

            if (depthController == null) {
                return;
            }

            // Kapal SELAM menyelam → oksigen berkurang
            if (depthController.IsSubmerged) {
                if (currentOxygen > 0f) {
                    currentOxygen -= depletionRate * Time.deltaTime;
                    if (currentOxygen < 0f) {
                        currentOxygen = 0f;
                    }
                    UpdateOxygenUI();
                }
            }
            // Kapal di permukaan → bisa isi ulang (opsional)
            else if (regenerateOnSurface && currentOxygen < maxOxygen) {
                currentOxygen += regenRate * Time.deltaTime;
                if (currentOxygen > maxOxygen) {
                    currentOxygen = maxOxygen;
                }
                UpdateOxygenUI();
            }

            // Contoh: kalau mau ada efek saat oksigen habis, bisa tambahkan di sini:
            // if(currentOxygen <= 0f) { ... }
        }

        public void RefillOxygen() {
            currentOxygen = maxOxygen;
            UpdateOxygenUI();
        }

        void UpdateOxygenUI() {
            if (oxygenText != null) {
                int displayValue = Mathf.RoundToInt(currentOxygen);
                oxygenText.text = "Oksigen : " + displayValue.ToString() + "%";
            }
        }
    }
}
