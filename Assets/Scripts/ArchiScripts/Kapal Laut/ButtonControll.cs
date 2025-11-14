using System.Collections;
using UnityEngine;

public class ButtonControll : MonoBehaviour
{
    public enum ButtonType { OnButton, OffButton }
    [Header("Button Type")]
    public ButtonType buttonType = ButtonType.OnButton; // Pilih di Inspector: OnButton atau OffButton

    [Header("Sound Settings")]
    private AudioSource source;
    public AudioClip buttonSound;

    [Header("Button Animation")]
    public float buttonDownDistance = 0.02f;
    public float buttonReturnSpeed = 0.001f;
    private float buttonOriginalY;
    private Vector3 originalPosition;

    [Header("Linked Visuals")]
    public Light indicatorLight; // Lampu kecil di atas tombol (opsional)
    public Color onColor = Color.green;
    public Color offColor = Color.red;

    [Header("Button Cooldown")]
    public float buttonHitCooldown = 0.4f;
    private float nextAvailablePress;

    private bool buttonHit = false;

    // Event (akan dikontrol oleh MoveBoat)
    public System.Action<ButtonControll> OnButtonPressed;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
        }

        originalPosition = transform.localPosition;
        buttonOriginalY = originalPosition.y;

        // Set warna lampu awal
        if (indicatorLight != null)
        {
            indicatorLight.color = (buttonType == ButtonType.OnButton) ? onColor : offColor;
            indicatorLight.enabled = true;
        }
    }

    private void Update()
    {
        // Tombol naik kembali perlahan
        if (!buttonHit && transform.localPosition.y < buttonOriginalY)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                Mathf.MoveTowards(transform.localPosition.y, buttonOriginalY, buttonReturnSpeed),
                transform.localPosition.z
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand") && Time.time >= nextAvailablePress)
        {
            nextAvailablePress = Time.time + buttonHitCooldown;
            buttonHit = true;

            // Animasi tekan ke bawah
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                buttonOriginalY - buttonDownDistance,
                transform.localPosition.z
            );

            // Suara tombol
            if (buttonSound != null)
                source.PlayOneShot(buttonSound);

            // Warna lampu saat ditekan
            if (indicatorLight != null)
                indicatorLight.intensity = 3f;

            // Panggil event ke MoveBoat
            OnButtonPressed?.Invoke(this);

            // Reset animasi setelah beberapa waktu
            StartCoroutine(ResetButton());
        }
    }

    private IEnumerator ResetButton()
    {
        yield return new WaitForSeconds(buttonHitCooldown);
        if (indicatorLight != null)
            indicatorLight.intensity = 1f;

        buttonHit = false;
    }
}
