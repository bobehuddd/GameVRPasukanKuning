using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    public class EngineButtonKS : Button {

        [Header("Engine Reference")]
        [Tooltip("VehicleController dari kapal / kapal selam")]
        public KapalController kapalselam;

        [Header("Optional Submarine Depth Condition")]
        [Tooltip("Jika diisi & RequireSubmergedToStart = true, mesin hanya boleh dinyalakan jika kapal sudah menyelam.")]
        public SubmarineDepthController depthController;
        public bool RequireSubmergedToStart = false;

        public override void OnButtonDown() {
            // Jalankan semua logika Button asli (suara klik, event, dll)
            base.OnButtonDown();

            if (kapalselam == null) {
                Debug.LogWarning("EngineButton : VehicleController belum di-assign");
                return;
            }

            // Kalau diset harus menyelam dulu, cek dulu kondisi kedalaman
            if (RequireSubmergedToStart && depthController != null && !depthController.IsSubmerged) {
                Debug.Log("EngineButton : Tidak bisa menyalakan mesin, kapal belum menyelam.");
                return;
            }

            // Toggle mesin
            if (!kapalselam.EngineOn) {
                // Mesin OFF → crank engine
                kapalselam.CrankEngine();
            }
            else {
                // Mesin ON → matikan
                kapalselam.EngineOn = false;
                kapalselam.MotorInput = 0f;

                if (kapalselam.EngineAudio != null) {
                    kapalselam.EngineAudio.Stop();
                }
            }
        }
    }
}
