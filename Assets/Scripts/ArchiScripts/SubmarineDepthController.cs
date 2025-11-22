using UnityEngine;

namespace BNG {
    public class SubmarineDepthController : MonoBehaviour {

        [Header("References")]
        public Transform submarineTransform;
        public Lever depthLever;
        public VehicleController vehicle;   // optional, untuk auto-matiin mesin

        [Header("Depth Settings")]
        [Tooltip("Seberapa jauh kapal menyelam dari posisi awal (nilai positif).")]
        public float maxDiveOffset = 2f;

        [Tooltip("Kecepatan naik / turun kapal selam.")]
        public float depthMoveSpeed = 1.5f;

        [Header("Lever Thresholds")]
        [Tooltip("Persen lever (0-100). Di bawah nilai ini dianggap MENYELAM.")]
        public float submergedThresholdPercent = 20f;

        [Tooltip("Persen lever (0-100). Di atas nilai ini dianggap DI PERMUKAAN.")]
        public float surfacedThresholdPercent = 80f;

        // ---------- STATUS YANG DIBUTUHKAN ENGINE BUTTON ----------
        public bool IsSubmerged { get; private set; }
        // ------------------------------------------------------------

        float baseY;
        Rigidbody rb;
        bool lastIsSubmerged;

        void Awake() {
            if (submarineTransform == null) {
                submarineTransform = transform;
            }

            rb = submarineTransform.GetComponent<Rigidbody>();
        }

        void Start() {
            if (submarineTransform != null) {
                // posisi kapal saat mengapung di permukaan
                baseY = submarineTransform.position.y;
            }
        }

        void FixedUpdate() {
            if (submarineTransform == null || depthLever == null || rb == null) {
                return;
            }

            // 0..100 dari Lever
            float p = Mathf.Clamp(depthLever.LeverPercentage, 0f, 100f);

            // ------------------ HITUNG TARGET KEDALAMAN ------------------
            float targetY;

            // Tuas diturunkan -> menyelam ke kedalaman maksimal
            if (p <= submergedThresholdPercent) {
                targetY = baseY - maxDiveOffset;
            }
            // Tuas dinaikkan -> kembali ke permukaan
            else if (p >= surfacedThresholdPercent) {
                targetY = baseY;
            }
            // Di tengah-tengah -> interpolasi (biar transisinya halus)
            else {
                float t = Mathf.InverseLerp(surfacedThresholdPercent, submergedThresholdPercent, p);
                targetY = Mathf.Lerp(baseY, baseY - maxDiveOffset, t);
            }
            // ------------------------------------------------------------

            // Gerakkan kapal secara halus di axis Y
            Vector3 pos = rb.position;
            pos.y = Mathf.Lerp(pos.y, targetY, depthMoveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(pos);

            // Update status menyelam (binary: hanya true jika lever cukup turun)
            IsSubmerged = p <= submergedThresholdPercent;

            // Kalau sebelumnya menyelam, sekarang tidak -> matikan mesin
            if (vehicle != null && lastIsSubmerged && !IsSubmerged && vehicle.EngineOn) {
                vehicle.EngineOn = false;
                vehicle.MotorInput = 0f;

                if (vehicle.EngineAudio != null) {
                    vehicle.EngineAudio.Stop();
                }
            }

            lastIsSubmerged = IsSubmerged;
        }
    }
}
