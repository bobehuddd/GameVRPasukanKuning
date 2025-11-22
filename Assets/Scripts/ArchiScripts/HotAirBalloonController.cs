using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG; // pakai tipe JoystickControl & Button

public class HotAirBalloonController : MonoBehaviour {

    [Header("References")]
    [Tooltip("Transform balon / basket yang akan digerakkan")]
    public Transform balloonTransform;

    [Tooltip("Joystick fisik yang mengontrol arah balon")]
    public JoystickControl joystick;

    [Tooltip("Button untuk menaikkan level api (Burn Up)")]
    public Button burnUpButton;

    [Tooltip("Button untuk menurunkan level api (Burn Down)")]
    public Button burnDownButton;

    [Header("Flame Objects")]
    public GameObject apiKecil;
    public GameObject apiSedang;
    public GameObject apiBesar;

    [Header("Ketinggian / Level Api (offset dari posisi awal)")]
    public float offsetApiKecil = 2f;
    public float offsetApiSedang = 5f;
    public float offsetApiBesar = 10f;

    [Header("Kecepatan Gerak Balon")]
    public float verticalSpeed = 1.5f;
    public float horizontalSpeed = 1.5f;

    // 0 = api mati, 1 = kecil, 2 = sedang, 3 = besar
    int currentLevel = 0;

    float baseHeight;
    float targetHeight;

    // Input dari joystick (-1..1, -1..1)
    Vector2 joystickInput = Vector2.zero;

    void Awake() {
        if (balloonTransform == null) {
            balloonTransform = transform;
        }
    }

    void Start() {
        // Simpan tinggi awal
        baseHeight = balloonTransform.position.y;

        // Kondisi awal : api mati + balon di posisi awal
        SetFireLevel(0);

        // ====== DAFTARKAN EVENT KE JOYSTICK & BUTTON DI SINI ======
        if (joystick != null) {
            joystick.onJoystickVectorChange.AddListener(OnJoystickVectorChanged);
        }

        if (burnUpButton != null) {
            burnUpButton.onButtonDown.AddListener(OnBurnUpPressed);
        }

        if (burnDownButton != null) {
            burnDownButton.onButtonDown.AddListener(OnBurnDownPressed);
        }
    }

    void Update() {
        if (balloonTransform == null) {
            return;
        }

        Vector3 pos = balloonTransform.position;

        // --- Gerak vertikal (naik / turun) berdasarkan level api ---
        pos.y = Mathf.Lerp(pos.y, targetHeight, Time.deltaTime * verticalSpeed);

        // --- Gerak horizontal dari joystick ---
        Vector3 horizontalMove =
            new Vector3(joystickInput.x, 0f, joystickInput.y) *
            horizontalSpeed * Time.deltaTime;

        pos += horizontalMove;

        balloonTransform.position = pos;
    }

    // ===================== INPUT JOYSTICK ======================

    // Dipanggil ketika joystick berubah (via event di Start, bukan lewat Inspector)
    public void OnJoystickVectorChanged(Vector2 value) {
        joystickInput = value; // -1..1
    }

    // ===================== INPUT BUTTON ======================

    void OnBurnUpPressed() {
        IncreaseFireLevel();
    }

    void OnBurnDownPressed() {
        DecreaseFireLevel();
    }

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

    void UpdateFlameVisual() {
        if (apiKecil != null)  apiKecil.SetActive(currentLevel == 1);
        if (apiSedang != null) apiSedang.SetActive(currentLevel == 2);
        if (apiBesar != null)  apiBesar.SetActive(currentLevel == 3);
    }

    void SetFireLevel(int level) {
        currentLevel = level;
        SetLevelHeight(level);
        UpdateFlameVisual();
    }
}
