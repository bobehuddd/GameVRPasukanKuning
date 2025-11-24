using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class BurnUpButton : Button {

    [Tooltip("Referensi ke HotAirBalloonController di balon udara ini")]
    public HotAirBalloonController controller;

    public override void OnButtonDown() {
        // jalankan logika Button bawaan (klik, suara, haptic, dll)
        base.OnButtonDown();

        if (controller != null) {
            controller.IncreaseFireLevel();
        }
        else {
            Debug.LogWarning("[BurnUpButton] Controller belum di-assign.");
        }
    }
}
