using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnDownButton : BNG.Button {

    public HotAirBalloonController controller;

    public override void OnButtonDown() {
        base.OnButtonDown();    // panggil fungsi Button asli

        if (controller != null) {
            controller.DecreaseFireLevel();
        }
    }
}

