using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    // Tombol mesin khusus kapal selam
    public class EngineButtonKS : Button {

        [Header("Engine Reference")]
        [Tooltip("KapalController dari kapal selam")]
        public KapalController kapalselam;

        [Header("Submarine Depth Condition")]
        [Tooltip("Controller kedalaman kapal selam. Dipakai untuk cek apakah kapal sudah menyelam.")]
        public SubmarineDepthController depthController;

        [Tooltip("Jika true, mesin hanya boleh dinyalakan jika kapal sudah menyelam (IsSubmerged == true).")]
        public bool RequireSubmergedToStart = true;

        [Header("Auto Throttle")]
        [Tooltip("Nilai gas otomatis saat mesin dinyalakan (0 - 1).")]
        [Range(0f, 1f)]
        public float AutoThrottle = 0.6f;

        public override void OnButtonDown() {
            // Jalankan semua logika Button asli (suara klik, event, dll)
            base.OnButtonDown();

            if (kapalselam == null) {
                Debug.LogWarning("EngineButtonKS : KapalController (kapalselam) belum di-assign");
                return;
            }

            // ---- BATASAN : HARUS MENYELAM DULU ----
            if (RequireSubmergedToStart) {
                if (depthController == null) {
                    Debug.LogWarning("EngineButtonKS : RequireSubmergedToStart = true tapi depthController belum di-assign");
                    return;
                }

                if (!depthController.IsSubmerged) {
                    Debug.Log("EngineButtonKS : Tidak bisa menyalakan mesin, tuas belum diturunkan (kapal belum menyelam).");
                    return;
                }
            }
            // ---------------------------------------

            // ============= TOGGLE MESIN =============
            if (!kapalselam.EngineOn) {
                // Mesin OFF → nyalakan + gas otomatis
                Debug.Log("[EngineButtonKS] Menyalakan mesin kapal selam + auto throttle");

                // Set dulu gasnya, supaya begitu mesin ON kapal langsung bergerak
                kapalselam.SetMotorTorqueInput(AutoThrottle);

                // Nyalakan mesin (pakai coroutine CrankEngine di KapalController)
                kapalselam.CrankEngine();
            }
            else {
                // Mesin ON → matikan
                Debug.Log("[EngineButtonKS] Mematikan mesin kapal selam");

                kapalselam.EngineOn = false;
                kapalselam.SetMotorTorqueInput(0f);   // pastikan gas 0

                // Matikan suara mesin
                if (kapalselam.EngineAudio != null) {
                    kapalselam.EngineAudio.Stop();
                }
            }
            // ========================================
        }
    }
}
