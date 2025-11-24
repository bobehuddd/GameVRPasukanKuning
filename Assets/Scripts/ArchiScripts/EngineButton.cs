using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    // Turunan dari Button (BNG), jadi semua fungsi Button tetap dipakai
    public class EngineButton : Button {

        [Header("Engine Reference")]
        [Tooltip("KapalController dari kapal laut")]
        public KapalController kapal;

        [Header("Auto Throttle")]
        [Tooltip("Nilai gas otomatis saat mesin dinyalakan (0 - 1).")]
        [Range(0f, 1f)]
        public float AutoThrottle = 0.6f;

        [Tooltip("Jika true, saat mesin dimatikan kapal akan langsung berhenti total.")]
        public bool StopCompletelyOnOff = false;

        // Dipanggil saat tombol benar-benar dianggap 'klik down'
        public override void OnButtonDown() {
            // Jalankan dulu logika Button asli :
            // - mainkan audio ButtonClick
            // - trigger UnityEvent onButtonDown
            base.OnButtonDown();

            if (kapal == null) {
                Debug.LogWarning("EngineButton : KapalController belum di-assign");
                return;
            }

            // ========= TOGGLE MESIN =========
            if (!kapal.EngineOn) {
                // Mesin sedang mati -> nyalakan dengan crank dan kasih gas otomatis
                Debug.Log("[EngineButton] Menyalakan mesin + auto throttle");

                // Set dulu gasnya, supaya begitu mesin ON kapal langsung bergerak
                kapal.SetMotorTorqueInput(AutoThrottle);

                // Nyalakan mesin (ada delay CrankTime sedikit)
                kapal.CrankEngine();
            }
            else {
                // Mesin sedang menyala -> matikan
                Debug.Log("[EngineButton] Mematikan mesin");

                kapal.EngineOn = false;
                kapal.SetMotorTorqueInput(0f);  // pastikan tidak ada input gas

                if (StopCompletelyOnOff) {
                    kapal.StopBoatCompletely();
                }

                // Matikan suara mesin
                if (kapal.EngineAudio != null) {
                    kapal.EngineAudio.Stop();
                }
            }
        }
    }
}
