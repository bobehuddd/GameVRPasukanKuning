using UnityEngine;

namespace BNG {
    public class SubmarineDepthController : MonoBehaviour {

        [Header("References")]
        [Tooltip("Transform kapal selam. Jika kosong, akan memakai transform sendiri.")]
        public Transform submarineTransform;

        [Tooltip("Lever yang dipakai untuk mengatur naik-turun kapal selam.")]
        public Lever depthLever;

        [Tooltip("KapalController dari kapal selam (opsional, untuk auto-matikan mesin saat naik ke permukaan).")]
        public KapalController kapalController;

        [Header("Depth Settings")]
        [Tooltip("Seberapa jauh kapal menyelam dari posisi awal (nilai positif, dalam satuan world).")]
        public float maxDiveOffset = 1.5f;

        [Tooltip("Kecepatan naik / turun kapal selam.")]
        public float depthMoveSpeed = 1.5f;

        [Header("Lever Thresholds (dalam persen 0-100)")]
        [Tooltip("Persen lever (0-100). Di bawah nilai ini dianggap MENYELAM.")]
        [Range(-60, 100)]
        public float submergedThresholdPercent = -60;

        [Tooltip("Persen lever (0-100). Di atas nilai ini dianggap DI PERMUKAAN.")]
        [Range(-60, 100)]
        public float surfacedThresholdPercent = 60;

        // ---------- STATUS UNTUK SCRIPT LAIN ----------
        /// <summary>
        /// True jika lever cukup turun sehingga dianggap "sedang menyelam".
        /// Dipakai EngineButtonKS untuk menentukan apakah tombol mesin boleh dipakai.
        /// </summary>
        public bool IsSubmerged { get; private set; }
        // ------------------------------------------------

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
                // Posisi kapal saat mengapung di permukaan (awal play mode)
                baseY = submarineTransform.position.y;
            }

            // Pastikan status awal : kapal di permukaan & mesin mati
            IsSubmerged = false;
            lastIsSubmerged = false;

            if (kapalController != null) {
                kapalController.EngineOn = false;
                kapalController.SetMotorTorqueInput(0f);

                if (kapalController.EngineAudio != null) {
                    kapalController.EngineAudio.Stop();
                }
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

            // Tuas diturunkan → menyelam ke kedalaman maksimal
            if (p <= submergedThresholdPercent) {
                targetY = baseY - maxDiveOffset;
            }
            // Tuas dinaikkan → kembali ke permukaan
            else if (p >= surfacedThresholdPercent) {
                targetY = baseY;
            }
            // Di tengah-tengah → interpolasi (biar transisinya halus)
            else {
                // t = 0 (permukaan) -> 1 (kedalaman max)
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

            // Kalau sebelumnya menyelam, sekarang tidak → matikan mesin (opsional)
            if (kapalController != null && lastIsSubmerged && !IsSubmerged && kapalController.EngineOn) {
                Debug.Log("[SubmarineDepthController] Kapal naik ke permukaan / keluar dari status menyelam, mesin dimatikan otomatis.");

                kapalController.EngineOn = false;
                kapalController.SetMotorTorqueInput(0f);

                if (kapalController.EngineAudio != null) {
                    kapalController.EngineAudio.Stop();
                }
            }

            lastIsSubmerged = IsSubmerged;
        }
    }
}
