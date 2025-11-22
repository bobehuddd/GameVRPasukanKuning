using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTrigger : MonoBehaviour
{
    [Tooltip("Nama scene tujuan, harus sama dengan yang ada di Build Settings")]
    [SerializeField] public string sceneToLoad = "NamaSceneBerikutnya";

    [Tooltip("Tag dari objek player VR yang ingin dideteksi")]
    [SerializeField] public string playerTag = "Player";

    private bool isLoading = false;

    private void OnTriggerEnter(Collider other)
    {
        // Cegah double-load
        if (isLoading) return;

        // Pastikan yang masuk adalah player
        if (!other.CompareTag(playerTag))
            return;

        isLoading = true;

        Debug.Log("[LoadSceneOnTrigger] Player masuk pintu, memuat scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
        // Kalau mau async:
        // StartCoroutine(LoadSceneAsync());
    }

    /*
    private IEnumerator LoadSceneAsync() {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!op.isDone) {
            yield return null;
        }
    }
    */
}

