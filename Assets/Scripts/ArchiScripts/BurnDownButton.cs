using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class BurnDownButton : Button {

    [Tooltip("Referensi ke HotAirBalloonController di balon udara ini")]
    public HotAirBalloonController controller;

    public override void OnButtonDown() {
        // jalankan logika Button bawaan
        base.OnButtonDown();

        if (controller != null) {
            controller.DecreaseFireLevel();
        }
        else {
            Debug.LogWarning("[BurnDownButton] Controller belum di-assign.");
        }
    }
}
