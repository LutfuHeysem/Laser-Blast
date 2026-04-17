using UnityEngine;

public class TNTBlock : MonoBehaviour
{
    [Header("Explosion Settings")]
    public int explosionRadius = 1; // 3x3 alan için yarıçap 1'dir.
    public GameObject explosionEffectPrefab; // İleride buraya partikül efekti koyacağız.

    // Lazer bu bloğa çarptığında arkadaşının kodu veya Lazer bu fonksiyonu çağıracak
    public void Explode()
    {
        Debug.Log(gameObject.name + " PATLADI! BOOOM!");

        // 1. Eğer bir patlama efekti atadıysak onu sahnede oluştur
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. Arkadaşının Grid Manager'ına haber verilecek kısım burası olacak.
        // Şimdilik sadece kendini yok etmesini söylüyoruz:
        Destroy(gameObject);
    }
}