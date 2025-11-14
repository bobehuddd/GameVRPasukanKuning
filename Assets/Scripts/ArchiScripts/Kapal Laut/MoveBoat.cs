using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveBoat : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    private Rigidbody rb;

    [Header("Button References")]
    public ButtonControll onButton;
    public ButtonControll offButton;

    private bool isBoatOn = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Pastikan tombol dihubungkan
        if (onButton == null || offButton == null)
        {
            Debug.LogError("[MoveBoat] Harap hubungkan kedua tombol (onButton dan offButton) di Inspector!");
            return;
        }

        // Daftarkan event listener
        onButton.OnButtonPressed += HandleButtonPressed;
        offButton.OnButtonPressed += HandleButtonPressed;
    }

    private void Update()
    {
        if (isBoatOn)
        {
            Vector3 step = transform.forward * moveSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + step);
        }
    }

    private void HandleButtonPressed(ButtonControll button)
    {
        if (button.buttonType == ButtonControll.ButtonType.OnButton)
        {
            isBoatOn = true;
            Debug.Log("[MoveBoat] Mesin kapal dinyalakan.");
        }
        else if (button.buttonType == ButtonControll.ButtonType.OffButton)
        {
            isBoatOn = false;
            Debug.Log("[MoveBoat] Mesin kapal dimatikan.");
        }
    }

    private void OnDestroy()
    {
        // Bersihkan event listener agar tidak error di runtime
        if (onButton != null) onButton.OnButtonPressed -= HandleButtonPressed;
        if (offButton != null) offButton.OnButtonPressed -= HandleButtonPressed;
    }
}
