using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class BurnUpButton : BNG.Button {

    public HotAirBalloonController controller;

    public override void OnButtonDown() {
        base.OnButtonDown();    // panggil fungsi Button asli (klik, suara, dll)

        if (controller != null) {
            controller.IncreaseFireLevel();
        }
    }
}