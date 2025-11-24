using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG; // JoystickControl

public class HotAirBalloonController : MonoBehaviour {

    [Header("References")]
    [Tooltip("Transform balon / basket yang akan digerakkan")]
    public Transform balloonTransform;

    [Tooltip("Joystick fisik yang mengontrol arah balon")]
    public JoystickControl joystick;

    [Header("Flame Objects (Prefab di dalam burner)")]
    public GameObject apiKecil;
    public GameObject apiSedang;
    public GameObject apiBesar;

    // Komponen ParticleSystem untuk tiap level api
    ParticleSystem psApiKecil;
    ParticleSystem psApiSedang;
    ParticleSystem psApiBesar;

    [Header("Ketinggian / Level Api (offset dari posisi awal)")]
    [Tooltip("Tambahan tinggi untuk level api kecil")]
    public float offsetApiKecil = 2f;

    [Tooltip("Tambahan tinggi untuk level api sedang")]
    public float offsetApiSedang = 5f;

    [Tooltip("Tambahan tinggi untuk level api besar")]
    public float offsetApiBesar = 10f;

    [Header("Kecepatan Gerak Balon")]
    [Tooltip("Kecepatan naik / turun menuju targetHeight")]
    public float verticalSpeed = 1.5f;

    [Tooltip("Kecepatan gerak horizontal dari joystick")]
    public float horizontalSpeed = 1.5f;

    // 0 = api mati, 1 = kecil, 2 = sedang, 3 = besar
    int currentLevel = 0;

    float baseHeight;   // tinggi awal balon
    float targetHeight; // tinggi tujuan berdasarkan level api

    // Input dari joystick (-1..1, -1..1)
    Vector2 joystickInput = Vector2.zero;

    void Awake() {
        if (balloonTransform == null) {
            balloonTransform = transform;
        }
    }

    void Start() {
        // Simpan tinggi awal sebagai "permukaan"
        baseHeight = balloonTransform.position.y;

        // Ambil komponen ParticleSystem dari masing-masing VFX (jika ada)
        if (apiKecil != null)  psApiKecil  = apiKecil.GetComponent<ParticleSystem>();
        if (apiSedang != null) psApiSedang = apiSedang.GetComponent<ParticleSystem>();
        if (apiBesar != null)  psApiBesar  = apiBesar.GetComponent<ParticleSystem>();

        // Kondisi awal : api level 0 (mati) + balon di posisi awal
        SetFireLevel(0);

        // Daftarkan event joystick supaya kirim Vector2 ke controller
        if (joystick != null) {
            joystick.onJoystickVectorChange.AddListener(OnJoystickVectorChanged);
        }
    }

    void OnDestroy() {
        // bersihkan listener supaya aman saat scene berganti
        if (joystick != null) {
            joystick.onJoystickVectorChange.RemoveListener(OnJoystickVectorChanged);
        }
    }

    void Update() {
        if (balloonTransform == null) {
            return;
        }

        Vector3 pos = balloonTransform.position;

        // --- Gerak vertikal (naik / turun) berdasarkan targetHeight ---
        pos.y = Mathf.Lerp(pos.y, targetHeight, Time.deltaTime * verticalSpeed);

        // --- Gerak horizontal dari joystick ---
        Vector3 horizontalMove =
            new Vector3(joystickInput.x, 0f, joystickInput.y) *
            horizontalSpeed * Time.deltaTime;

        pos += horizontalMove;

        balloonTransform.position = pos;
    }

    // ===================== INPUT JOYSTICK ======================

    // Dipanggil ketika joystick berubah (via event di Start)
    public void OnJoystickVectorChanged(Vector2 value) {
        joystickInput = value; // nilai -1..1 di X/Z
    }

    // ===================== API / LEVEL KETINGGIAN ======================

    public void IncreaseFireLevel() {
        currentLevel++;
        if (currentLevel > 3) currentLevel = 3;

        SetLevelHeight(currentLevel);
        UpdateFlameVisual();
    }

    public void DecreaseFireLevel() {
        currentLevel--;
        if (currentLevel < 0) currentLevel = 0;

        SetLevelHeight(currentLevel);
        UpdateFlameVisual();
    }

    // Set targetHeight berdasarkan level api
    void SetLevelHeight(int level) {
        switch (level) {
            case 0:
                targetHeight = baseHeight;
                break;
            case 1:
                targetHeight = baseHeight + offsetApiKecil;
                break;
            case 2:
                targetHeight = baseHeight + offsetApiSedang;
                break;
            case 3:
                targetHeight = baseHeight + offsetApiBesar;
                break;
        }
    }

    // Nyalakan / matikan VFX api berdasarkan level
    void UpdateFlameVisual() {
        // Optional : sembunyikan gameobject yang tidak dipakai
        if (apiKecil  != null) apiKecil.SetActive (currentLevel == 1);
        if (apiSedang != null) apiSedang.SetActive(currentLevel == 2);
        if (apiBesar  != null) apiBesar.SetActive (currentLevel == 3);

        // --- Level 1 : Api Kecil ---
        if (psApiKecil != null) {
            if (currentLevel == 1 && !psApiKecil.isPlaying) {
                psApiKecil.Play();
            }
            else if (currentLevel != 1 && psApiKecil.isPlaying) {
                psApiKecil.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        // --- Level 2 : Api Sedang ---
        if (psApiSedang != null) {
            if (currentLevel == 2 && !psApiSedang.isPlaying) {
                psApiSedang.Play();
            }
            else if (currentLevel != 2 && psApiSedang.isPlaying) {
                psApiSedang.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        // --- Level 3 : Api Besar ---
        if (psApiBesar != null) {
            if (currentLevel == 3 && !psApiBesar.isPlaying) {
                psApiBesar.Play();
            }
            else if (currentLevel != 3 && psApiBesar.isPlaying) {
                psApiBesar.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    // Set level api sekaligus update tinggi + visual
    void SetFireLevel(int level) {
        currentLevel = Mathf.Clamp(level, 0, 3);
        SetLevelHeight(currentLevel);
        UpdateFlameVisual();
    }
}
