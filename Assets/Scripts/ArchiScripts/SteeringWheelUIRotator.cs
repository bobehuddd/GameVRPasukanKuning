using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG; // supaya bisa pakai SteeringWheel

public class SteeringWheelUIRotator : MonoBehaviour {

    [Header("Referensi")]
    [Tooltip("SteeringWheel yang ada di meja ini (objek stir kapal).")]
    public SteeringWheel steeringWheel;

    [Tooltip("Transform dari cube 'UI Berputar' yang akan diputar.")]
    public Transform uiBerputar;

    [Header("Pengaturan Rotasi UI")]
    [Tooltip("Maksimum rotasi Z untuk UI (default 360 derajat).")]
    public float maxUIRotation = 360f;

    [Tooltip("Centang jika arah rotasi terasa terbalik.")]
    public bool invertDirection = false;

    private void OnEnable() {
        if (steeringWheel != null) {
            // value dari onValueChange: -1 s/d 1
            steeringWheel.onValueChange.AddListener(OnSteeringValueChanged);
        }
    }

    private void OnDisable() {
        if (steeringWheel != null) {
            steeringWheel.onValueChange.RemoveListener(OnSteeringValueChanged);
        }
    }

    /// <summary>
    /// Dipanggil setiap frame oleh SteeringWheel.onValueChange
    /// value : -1 (MinAngle) sampai 1 (MaxAngle)
    /// </summary>
    private void OnSteeringValueChanged(float value) {
        if (uiBerputar == null) {
            return;
        }

        if (invertDirection) {
            value *= -1f;
        }

        // value -1..1 -> 0..1
        float t = (value + 1f) * 0.5f;

        // 0..1 -> 0..maxUIRotation (biasanya 360)
        float angleZ = Mathf.Lerp(0f, maxUIRotation, t);

        // Terapkan ke rotasi Z cube
        Vector3 euler = uiBerputar.localEulerAngles;
        euler.z = angleZ;
        uiBerputar.localEulerAngles = euler;
    }
}