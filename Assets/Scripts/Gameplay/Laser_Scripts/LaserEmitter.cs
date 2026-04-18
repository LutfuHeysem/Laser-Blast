using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    [Header("Lazer Ayarları")]
    public float maxDistance = 50f;
    public int maxBounces = 50;
    public float laserSpeed = 20f; // Saniyede kaç birim ilerleyeceği
    public bool isActivated = false;
    public Transform startingPoint;

    private LineRenderer lineRenderer;
    private Coroutine laserCoroutine;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        if (lineRenderer != null && !isActivated)
            lineRenderer.positionCount = 0;
    }

    void OnMouseDown()
    {
        if (!isActivated)
        {
            isActivated = true;
            if (laserCoroutine != null) StopCoroutine(laserCoroutine);
            laserCoroutine = StartCoroutine(AnimateLaser());
        }
    }

    // Bu metod dışarıdan çağrılırsa (örneğin bir aynadan tetiklenirse) diye duruyor
    public void ShootLaser(Collider2D ignoreCollider = null)
    {
        if (laserCoroutine != null) StopCoroutine(laserCoroutine);
        laserCoroutine = StartCoroutine(AnimateLaser(ignoreCollider));
    }

    IEnumerator AnimateLaser(Collider2D ignoreCollider = null)
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startingPoint.position);

        List<Vector3> points = new List<Vector3> { startingPoint.position };
        Vector2 currentPos = startingPoint.position;
        Vector2 currentDir = transform.right;
        int bounces = 0;

        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null) myCollider.enabled = false;
        if (ignoreCollider != null) ignoreCollider.enabled = false;

        while (bounces < maxBounces)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDir, maxDistance);
            Vector3 targetPos;
            bool hitSomething = false;

            if (hit.collider != null)
            {
                targetPos = hit.point;
                hitSomething = true;
            }
            else
            {
                targetPos = (Vector3)(currentPos + currentDir * maxDistance);
            }

            // --- ADIM ADIM İLERLEME MANTIĞI ---
            float distanceToTarget = Vector3.Distance(currentPos, targetPos);
            float travelledDistance = 0f;

            // Mevcut noktadan hedef noktaya lazeri uzatıyoruz
            lineRenderer.positionCount++;
            int currentIndex = lineRenderer.positionCount - 1;

            while (travelledDistance < distanceToTarget)
            {
                travelledDistance += laserSpeed * Time.deltaTime;
                float t = Mathf.Clamp01(travelledDistance / distanceToTarget);
                Vector3 currentTipPos = Vector3.Lerp(currentPos, targetPos, t);

                lineRenderer.SetPosition(currentIndex, currentTipPos);
                yield return null; // Bir sonraki frame'e kadar bekle
            }
            // ----------------------------------

            points.Add(targetPos);
            lineRenderer.SetPosition(currentIndex, targetPos);

            if (hitSomething)
            {
                ILaserInteractable interactable = hit.collider.GetComponent<ILaserInteractable>();
                if (interactable != null)
                {
                    Vector2 newDirection;
                    bool shouldContinue = interactable.OnLaserHit(hit.point, currentDir, this, out newDirection);

                    if (shouldContinue)
                    {
                        currentPos = hit.point + (newDirection.normalized * 0.01f);
                        currentDir = newDirection.normalized;
                        bounces++;
                    }
                    else break;
                }
                else break;
            }
            else break;
        }

        if (ignoreCollider != null) ignoreCollider.enabled = true;
        if (myCollider != null) myCollider.enabled = true;
    }
}