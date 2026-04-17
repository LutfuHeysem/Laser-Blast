using UnityEngine;

public class GlassBlock : MonoBehaviour
{
    [Header("Cam Ayarları")]
    public int bonusPoints = 50; // Kırılınca verilecek ekstra puan
    public GameObject shatterEffectPrefab; // Kırıldığında çıkacak cam kırıkları efekti

    // Lazerin (veya TNT patlamasının) bu bloğu kırmak için çağıracağı fonksiyon
    public void Shatter()
    {
        Debug.Log("Cam Şangırdadı! +" + bonusPoints + " Puan kazanıldı.");

        // Eğer bir parçacık efekti atadıysak, kırılma anında onu sahnede oluştur
        if (shatterEffectPrefab != null)
        {
            Instantiate(shatterEffectPrefab, transform.position, Quaternion.identity);
        }

        // NOT: İleride arkadaşın bir Skor Yöneticisi yazdığında kodu buraya bağlayacaksınız.
        // Örnek: GameManager.instance.AddScore(bonusPoints);

        // Bloğu sahneden yok et
        Destroy(gameObject);
    }
}