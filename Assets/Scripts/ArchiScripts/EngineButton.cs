using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    // Turunan dari Button, jadi semua fungsi Button tetap dipakai
    public class EngineButton : Button {

        [Header("Engine Reference")]
        [Tooltip("KapalController dari kapal laut")]
        public KapalController kapal;

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

            // Toggle mesin
            if (!kapal.EngineOn) {
                // Mesin sedang mati -> nyalakan dengan crank
                kapal.CrankEngine();
            }
            else {
                // Mesin sedang menyala -> matikan
                kapal.EngineOn = false;
                kapal.MotorInput = 0f;  // pastikan tidak ada input gas

                // Matikan suara mesin
                if (kapal.EngineAudio != null) {
                    kapal.EngineAudio.Stop();
                }
            }
        }
    }
}

