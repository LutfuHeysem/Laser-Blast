using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Hareket ve Süre")]
    public float moveSpeed = 2f;
    public float destroyTime = 1f;

    [Header("Boyut Ayarları")]
    public float minScale = 1f;
    public float maxScale = 2.5f;
    public float scaleThreshold = 1000f;

    [Header("Rotasyon Ayarları")] // YENİ EKLENEN BÖLÜM
    [Tooltip("Yazı ekrana gelirken en fazla kaç derece sağa veya sola yatık olabilir?")]
    public float maxRotationAngle = 30f; 

    private TextMeshPro textMesh;
    private Color textColor;
    private float timer;

    public void Setup(int scoreAmount)
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = "+" + scoreAmount.ToString();
        textColor = textMesh.color;

        // Boyut hesaplama
        float scaleFactor = scoreAmount / scaleThreshold;
        float finalScale = Mathf.Lerp(minScale, maxScale, scaleFactor);
        transform.localScale = new Vector3(finalScale, finalScale, 1f);

        // --- YENİ: RASTGELE DÖNDÜRME ---
        // -maxRotationAngle ile +maxRotationAngle arasında rastgele bir açı seç
        float randomAngle = Random.Range(-maxRotationAngle, maxRotationAngle);
        // Objeyi sadece Z ekseninde (2D olduğu için) döndür
        transform.rotation = Quaternion.Euler(0f, 0f, randomAngle);

        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        // Yazıyı hareket ettir ve soldur
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        float alpha = 1f - (timer / destroyTime);
        textMesh.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
    }
}