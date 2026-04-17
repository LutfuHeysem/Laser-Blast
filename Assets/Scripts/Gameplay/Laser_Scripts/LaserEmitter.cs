using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    [Header("Lazer Ayarları")]
    public float maxDistance = 50f;
    public int maxBounces = 50; // Sonsuz döngüleri engellemek için
    public bool isActivated = false; 
    
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
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

        while (bounces < maxBounces)
        {
            // Kendi collider'ımıza çarpmamak için ray'i çok ufak bir miktar ileriden başlatıyoruz
            RaycastHit2D hit = Physics2D.Raycast(currentPos + currentDir * 0.05f, currentDir, maxDistance);

            if (hit.collider != null)
            {
                points.Add(hit.point);

                ILaserInteractable interactable = hit.collider.GetComponent<ILaserInteractable>();
                if (interactable != null)
                {
                    Vector2 newDirection;
                    // Obje ile etkileşime gir. Eğer true dönerse lazer sekmeye/devam etmeye karar verir.
                    bool shouldContinue = interactable.OnLaserHit(hit.point, currentDir, this, out newDirection);
                    
                    if (shouldContinue)
                    {
                        currentPos = hit.point;
                        currentDir = newDirection.normalized;
                        bounces++;
                    }
                    else
                    {
                        // Lazer hedefte durdu veya yok oldu
                        break;
                    }
                }
                else
                {
                    // ILaserInteractable olmayan düz bir duvara çarptı, dur.
                    break;
                }
            }
            else
            {
                // Boşluğa gitti, uca kadar çiz ve döngüyü bitir.
                points.Add(currentPos + currentDir * maxDistance);
                break;
            }
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}