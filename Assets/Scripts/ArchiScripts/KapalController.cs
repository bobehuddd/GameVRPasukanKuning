using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNG {
    public class KapalController : MonoBehaviour {

        [Header("Engine Properties")]
        [Tooltip("Kekuatan dorong kapal. Semakin besar, semakin cepat akselerasinya.")]
        public float MotorTorque = 50f;

        [Tooltip("Kecepatan maksimum (km/jam) yang diijinkan.")]
        public float MaxSpeed = 20f;

        [Tooltip("Seberapa kuat kapal berbelok saat stir diputar.")]
        public float MaxSteeringAngle = 40f;

        [Header("Steering Grabbable")]
        [Tooltip("Jika true dan SteeringGrabbable sedang dipegang, trigger kanan / kiri menjadi input gas / rem.")]
        // UNTUK MODE TOMBOL: biar tidak bentrok, default = false.
        public bool CheckTriggerInput = false;
        public Grabbable SteeringGrabbable;

        [Header("Engine Status")]
        [Tooltip("Apakah mesin menyala dan siap menerima input. Jika false, perlu distart dulu.")]
        public bool EngineOn = false;

        [Tooltip("Berapa lama waktu crank saat menyalakan mesin.")]
        public float CrankTime = 0.1f;

        [Header("Speedometer")]
        [Tooltip("Label untuk menampilkan kecepatan (km/jam). Boleh dikosongkan.")]
        public Text SpeedLabel;

        [Header("Audio Setup")]
        public AudioSource EngineAudio;

        [Tooltip("Suara idle yang di-loop saat mesin menyala. Pitch akan berubah sesuai kecepatan.")]
        public AudioClip IdleSound;

        [Tooltip("Suara crank (start mesin) sebelum idle.")]
        public AudioClip CrankSound;

        [Tooltip("Suara saat kapal menabrak sesuatu.")]
        public AudioClip CollisionSound;

        [HideInInspector]
        public float SteeringAngle = 0f;   // -1 s/d 1 (dari SteeringWheel)
        [HideInInspector]
        public float MotorInput = 0f;      // -1 s/d 1 (dari tombol / trigger)
        [HideInInspector]
        public float CurrentSpeed;         // km/jam

        Vector3 initialPosition;
        Rigidbody rb;

        bool wasHoldingSteering;
        bool isHoldingSteering;

        protected bool crankingEngine = false;

        void Start() {
            rb = GetComponent<Rigidbody>();
            initialPosition = transform.position;

            // Nilai drag awal agar gerak kapal lebih "berat" seperti di air.
            if (rb != null) {
                if (rb.drag < 1f) rb.drag = 1.5f;
                if (rb.angularDrag < 1f) rb.angularDrag = 2f;
            }
        }

        void Update() {

            isHoldingSteering = SteeringGrabbable != null && SteeringGrabbable.BeingHeld;

            // Kalau mau pakai trigger untuk gas, centang CheckTriggerInput di Inspector
            if (CheckTriggerInput) {
                GetTorqueInputFromTriggers();
            }

            // Kalau ada input motor tapi mesin belum menyala → crank dulu
            if (Mathf.Abs(MotorInput) > 0.01f && !EngineOn) {
                CrankEngine();
            }

            // Selama proses crank, jangan gerakkan kapal
            if (crankingEngine) {
                return;
            }

            UpdateEngineAudio();

            if (SpeedLabel != null) {
                SpeedLabel.text = CurrentSpeed.ToString("n0");
            }

            CheckOutOfBounds();

            wasHoldingSteering = isHoldingSteering;
        }

        // Dipanggil saat ingin menyalakan mesin (misalnya dari tombol)
        public virtual void CrankEngine() {
            if (crankingEngine || EngineOn) {
                return;
            }

            StartCoroutine(crankEngine());
        }

        IEnumerator crankEngine() {
            crankingEngine = true;

            if (EngineAudio != null && CrankSound != null) {
                EngineAudio.clip = CrankSound;
                EngineAudio.loop = false;
                EngineAudio.Play();
            }

            yield return new WaitForSeconds(CrankTime);

            // Ganti ke suara idle
            if (EngineAudio != null && IdleSound != null) {
                EngineAudio.clip = IdleSound;
                EngineAudio.loop = true;
                EngineAudio.Play();
            }

            yield return new WaitForEndOfFrame();

            crankingEngine = false;
            EngineOn = true;
        }

        // Reset posisi kalau jatuh jauh dari dunia
        public virtual void CheckOutOfBounds() {
            if (transform.position.y < -500f) {
                transform.position = initialPosition;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Right Trigger = gas, Left Trigger = rem / mundur (opsional)
        public virtual void GetTorqueInputFromTriggers() {
            if (isHoldingSteering) {
                SetMotorTorqueInput(InputBridge.Instance.RightTrigger - InputBridge.Instance.LeftTrigger);
            }
            else if (wasHoldingSteering && !isHoldingSteering) {
                SetMotorTorqueInput(0);
            }
        }

        void FixedUpdate() {
            if (rb == null) {
                return;
            }

            // Kecepatan dalam km/jam (velocity.magnitude = m/s)
            CurrentSpeed = correctValue(rb.velocity.magnitude * 3.6f);

            ApplyBoatMovement();
        }

        /// <summary>
        /// Gerakkan kapal tanpa WheelCollider: hanya dengan gaya & torsi pada Rigidbody.
        /// </summary>
        protected virtual void ApplyBoatMovement() {

            if (!EngineOn) {
                // Mesin mati → tidak ada dorong (tetap bisa meluncur karena inertia)
                return;
            }

            // ------- DORONG MAJU / MUNDUR -------
            // Batasi kecepatan maksimum berdasarkan CurrentSpeed (km/jam)
            if (CurrentSpeed < MaxSpeed || Mathf.Sign(MotorInput) != Mathf.Sign(CurrentSpeed)) {
                // MotorInput -1 s/d 1 → maju/mundur
                Vector3 forwardForce = transform.forward * MotorInput * MotorTorque;
                rb.AddForce(forwardForce, ForceMode.Acceleration);
            }

            // ------- PUTAR / BELOK -------
            if (Mathf.Abs(SteeringAngle) > 0.01f && rb.velocity.magnitude > 0.01f) {
                float turnPower = SteeringAngle * MaxSteeringAngle;
                rb.AddTorque(Vector3.up * turnPower, ForceMode.Acceleration);
            }
        }

        // Dipanggil oleh SteeringWheel (event) atau input lain
        public virtual void SetSteeringAngle(float steeringAngle) {
            SteeringAngle = steeringAngle;
        }

        public virtual void SetSteeringAngleInverted(float steeringAngle) {
            SteeringAngle = -steeringAngle;
        }

        public virtual void SetSteeringAngle(Vector2 steeringAngle) {
            SteeringAngle = steeringAngle.x;
        }

        public virtual void SetSteeringAngleInverted(Vector2 steeringAngle) {
            SteeringAngle = -steeringAngle.x;
        }

        // Dipanggil oleh tombol / input lain
        public virtual void SetMotorTorqueInput(float input) {
            MotorInput = Mathf.Clamp(input, -1f, 1f);
        }

        public virtual void SetMotorTorqueInputInverted(float input) {
            MotorInput = Mathf.Clamp(-input, -1f, 1f);
        }

        public virtual void SetMotorTorqueInput(Vector2 input) {
            MotorInput = Mathf.Clamp(input.y, -1f, 1f);
        }

        public virtual void SetMotorTorqueInputInverted(Vector2 input) {
            MotorInput = Mathf.Clamp(-input.y, -1f, 1f);
        }

        public virtual void UpdateEngineAudio() {
            if (EngineAudio != null && EngineOn) {
                EngineAudio.pitch = Mathf.Clamp(0.5f + (CurrentSpeed / MaxSpeed), 0.2f, 3f);
            }
        }

        void OnCollisionEnter(Collision collision) {
            if (CollisionSound == null) {
                return;
            }

            float colVelocity = collision.relativeVelocity.magnitude;
            if (colVelocity > 0.1f) {
                VRUtils.Instance.PlaySpatialClipAt(CollisionSound, collision.GetContact(0).point, 1f);
            }
        }

        float correctValue(float inputValue) {
            return (float)System.Math.Round(inputValue * 1000f) / 1000f;
        }

        /// <summary>
        /// Opsional: berhentikan kapal seketika saat mesin dimatikan.
        /// </summary>
        public void StopBoatCompletely() {
            SetMotorTorqueInput(0f);
            if (rb != null) {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
