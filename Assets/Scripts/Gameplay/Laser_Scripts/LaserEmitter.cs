using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    [Header("Lazer Ayarları")]
    public float maxDistance = 50f; // Lazerin gidebileceği maksimum mesafe
    public int maxBounces = 50; 
    public bool isActivated = false; 
    
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0; // Başlangıçta lazer görünmez
    }

    void OnMouseDown()
    {
        if (!isActivated)
        {
            isActivated = true;
            ShootLaser();
        }
    }

    public void ShootLaser()
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(transform.position);

        Vector2 currentPos = transform.position;
        Vector2 currentDir = transform.up;
        int bounces = 0;

        // Lazerin kendi collider'ına (Arrow'a) çarpmasını geçici olarak engelliyoruz
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null) myCollider.enabled = false;

        while (bounces < maxBounces)
        {
            // Sizin eski scriptinizdeki gibi temiz ve basit bir Raycast atıyoruz
            RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDir, maxDistance);

            if (hit.collider != null)
            {
                // Işın bir şeye ÇARPTI!
                points.Add(hit.point);

                // Çarptığı obje yeni sistemdeki ILaserInteractable arayüzüne sahip mi?
                ILaserInteractable interactable = hit.collider.GetComponent<ILaserInteractable>();
                if (interactable != null)
                {
                    Vector2 newDirection;
                    // Objeye ne yapması gerektiğini sor (Ayna ise yansıtacak, çelik ise durduracak vs.)
                    bool shouldContinue = interactable.OnLaserHit(hit.point, currentDir, this, out newDirection);
                    
                    if (shouldContinue)
                    {
                        // Ayna gibi seken objelerde, bir sonraki ışının tekrar aynı aynaya çarpıp 
                        // içine hapsolmasını engellemek için başlangıç noktasını yeni yönde "çok çok az" ileri itiyoruz (0.01f)
                        currentPos = hit.point + (newDirection.normalized * 0.01f);
                        currentDir = newDirection.normalized;
                        bounces++;
                    }
                    else
                    {
                        // Işın hedefe ulaştı, çeliğe çarptı veya TNT patlattı, devam etmeyecek.
                        break;
                    }
                }
                else
                {
                    // ILaserInteractable olmayan düz bir objeye çarptı, dur.
                    break;
                }
            }
            else
            {
                // Işın hiçbir şeye çarpmadı (Boşluğa gitti)
                points.Add(currentPos + currentDir * maxDistance);
                break;
            }
        }

        // Kendi collider'ımızı geri açıyoruz
        if (myCollider != null) myCollider.enabled = true;

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}