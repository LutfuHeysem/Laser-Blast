using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    [Header("Lazer Ayarları")]
    public float maxDistance = 50f; // Lazerin gidebileceği maksimum mesafe
    public bool isActivated = false; 
    
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0; // Başlangıçta lazer görünmez
    }

    // Fareyle (veya mobilde parmakla) bu objeye tıklandığında çalışır
    void OnMouseDown()
    {
        Debug.Log("OKA TIKLANDI!");
        // Enerji kontrolü vs. eklenebilir, şimdilik sadece bir kere tetiklenmesini sağlıyoruz
        if (!isActivated)
        {
            isActivated = true;
            ShootLaser();
        }
    }

    void ShootLaser()
    {
        // Lazer çizgisinin 2 noktası olacak: Başlangıç ve Bitiş
        lineRenderer.positionCount = 2;
        
        // 1. Nokta: Okun kendi merkezi
        lineRenderer.SetPosition(0, transform.position);

        // Oku kendi "Yukarı" (Y ekseni) yönünde ateşliyoruz. 
        // Okun rotasyonunu çevirdiğinizde lazer de o yöne gider.
        Vector2 direction = transform.up; 

        // Fizik motorundan o yöne doğru görünmez bir ışın (Raycast) fırlatıyoruz
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance);

        if (hit.collider != null)
        {
            // Işın bir şeye ÇARPTI! 
            // 2. Noktayı (Bitişi) çarptığı yer olarak belirliyoruz.
            lineRenderer.SetPosition(1, hit.point);

            // Çarptığımız objeye ne olduğunu anlamak için bir fonksiyon çağırıyoruz
            HandleHit(hit.collider.gameObject);
        }
        else
        {
            // Işın hiçbir şeye çarpmadı (Boşluğa gitti)
            // Lazer sonsuza (maxDistance) uzasın
            lineRenderer.SetPosition(1, (Vector2)transform.position + (direction * maxDistance));
        }
    }

    void HandleHit(GameObject hitObj)
    {
        // Çarptığımız objede "TNTBlock" scripti var mı diye bakıyoruz
        TNTBlock tnt = hitObj.GetComponent<TNTBlock>();
        if (tnt != null) 
        {
            tnt.Explode(); // Varsa patlat!
        }

        // Çarptığımız objede "GlassBlock" scripti var mı diye bakıyoruz
        GlassBlock glass = hitObj.GetComponent<GlassBlock>();
        if (glass != null) 
        {
            glass.Shatter(); // Varsa kır!
        }

        // Çarptığımız obje "Block_Steel" ise hiçbir şey yapma, lazer sadece orada dursun.
    }
}