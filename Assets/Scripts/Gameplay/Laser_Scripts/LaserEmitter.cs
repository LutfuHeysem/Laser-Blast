using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserEmitter : MonoBehaviour
{
    public enum SpriteDrawDirection { Horizontal, Vertical }

    [Header("Sprite Ayarları")]
    public Sprite headSprite;
    public Sprite bodySprite;
    public Sprite tailSprite;
    public float laserThickness = 0.5f;

    [Tooltip("Sprite dosyası yukarı doğru çizildiyse Vertical, sağa doğru çizildiyse Horizontal seçin.")]
    public SpriteDrawDirection spriteDirection = SpriteDrawDirection.Vertical; // Senin için varsayılanı Vertical yaptım

    [Header("Lazer Ayarları")]
    public float maxDistance = 50f;
    public int maxBounces = 50;
    public float laserSpeed = 20f;
    public bool isActivated = false;
    public Transform startingPoint;

    private Coroutine laserCoroutine;
    private List<GameObject> spawnedLaserParts = new List<GameObject>();

    void Start()
    {
        if (!isActivated) ClearLaser();
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

    public void ShootLaser(Collider2D ignoreCollider = null)
    {
        if (laserCoroutine != null) StopCoroutine(laserCoroutine);
        laserCoroutine = StartCoroutine(AnimateLaser(ignoreCollider));
    }

    private void ClearLaser()
    {
        foreach (var part in spawnedLaserParts)
        {
            if (part != null) Destroy(part);
        }
        spawnedLaserParts.Clear();
    }

    IEnumerator AnimateLaser(Collider2D ignoreCollider = null)
    {
        ClearLaser();

        Vector2 currentPos = startingPoint.position;
        Vector2 currentDir = transform.right;
        int bounces = 0;

        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null) myCollider.enabled = false;
        if (ignoreCollider != null) ignoreCollider.enabled = false;

        if (tailSprite != null)
        {
            GameObject tailObj = CreateSpriteObject("Laser_Tail", tailSprite, currentPos, currentDir, false);
            // DÜZELTME 1: Kuyruk sprite'ını lazerin kalınlığına göre orantılı küçült
            float tailScale = laserThickness / tailSprite.bounds.size.x;
            tailObj.transform.localScale = new Vector3(tailScale, tailScale, 1f);
        }

        while (bounces < maxBounces)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDir, maxDistance);
            Vector3 targetPos = hit.collider != null ? (Vector3)hit.point : (Vector3)(currentPos + currentDir * maxDistance);
            bool hitSomething = hit.collider != null;

            float distanceToTarget = Vector3.Distance(currentPos, targetPos);
            float travelledDistance = 0f;

            GameObject bodyObj = CreateSpriteObject("Laser_Body", bodySprite, currentPos, currentDir, true);
            GameObject headObj = CreateSpriteObject("Laser_Head", headSprite, currentPos, currentDir, false);

            // DÜZELTME 2: Kafa sprite'ını lazerin kalınlığına göre orantılı küçült (en-boy oranı bozulmasın diye X ve Y'ye aynı scale veriyoruz)
            if (headSprite != null)
            {
                float headScale = laserThickness / headSprite.bounds.size.x;
                headObj.transform.localScale = new Vector3(headScale, headScale, 1f);
            }

            float spriteWidth = bodySprite.bounds.size.x;
            float spriteHeight = bodySprite.bounds.size.y;

            Camera mainCam = Camera.main;
            bool isOffScreen = false;

            while (travelledDistance < distanceToTarget)
            {
                travelledDistance += laserSpeed * Time.deltaTime;
                float t = Mathf.Clamp01(travelledDistance / distanceToTarget);
                Vector3 currentTipPos = Vector3.Lerp(currentPos, targetPos, t);

                if (mainCam != null)
                {
                    // Kameranın görüş alanını 0 ile 1 arasında bir koordinata çeviriyoruz
                    Vector3 viewPos = mainCam.WorldToViewportPoint(currentTipPos);

                    // x ve y, 0'dan küçük veya 1'den büyükse ekranın dışındadır.
                    // -0.05 ve 1.05 gibi bir tolerans verdik ki aniden değil, ucu tam çıkarken yok olsun.
                    if (viewPos.x < -0.05f || viewPos.x > 1.05f || viewPos.y < -0.05f || viewPos.y > 1.05f)
                    {
                        isOffScreen = true;
                        break; // Ekrandan çıktıysa animasyonu hemen durdur
                    }
                }

                float currentDist = Vector3.Distance(currentPos, currentTipPos);

                // Gövdeyi sündür
                bodyObj.transform.localScale = new Vector3(laserThickness / spriteWidth, currentDist / spriteHeight, 1f);
                bodyObj.transform.position = (Vector3)currentPos + ((Vector3)currentTipPos - (Vector3)currentPos) / 2f;
                headObj.transform.position = currentTipPos;

                yield return null;
            }

            if (isOffScreen)
            {
                break; // Dıştaki while(bounces) döngüsünü de kırar
            }

            bodyObj.transform.localScale = new Vector3(laserThickness / spriteWidth, distanceToTarget / spriteHeight, 1f);
            bodyObj.transform.position = (Vector3)currentPos + ((Vector3)targetPos - (Vector3)currentPos) / 2f;
            headObj.transform.position = targetPos;

            if (hitSomething)
            {
                ILaserInteractable interactable = hit.collider.GetComponent<ILaserInteractable>();
                if (interactable != null)
                {
                    Vector2 newDirection;
                    if (interactable.OnLaserHit(hit.point, currentDir, this, out newDirection))
                    {
                        currentPos = hit.point + (newDirection.normalized * 0.01f);
                        currentDir = newDirection.normalized;
                        bounces++;

                        Destroy(headObj);
                        spawnedLaserParts.Remove(headObj);
                        continue;
                    }
                }
            }
            break;
        }

        if (ignoreCollider != null) ignoreCollider.enabled = true;
        if (myCollider != null) myCollider.enabled = true;

        yield return new WaitForSeconds(0.1f);

        // Ekranda kalan o lazer parçasını tamamen yok et
        ClearLaser();
    }

    private GameObject CreateSpriteObject(string objName, Sprite sprite, Vector3 pos, Vector3 dir, bool isBody)
    {
        GameObject obj = new GameObject(objName);

        // ÖNEMLİ: Parent atamasını bilerek kaldırdık! 
        // Böylece Prizmanın Scale (2,2) değeri lazerleri yanlışlıkla devasa boyutlara çıkarmayacak.
        // obj.transform.parent = transform; 

        obj.transform.position = pos;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        obj.transform.rotation = Quaternion.Euler(0, 0, angle);

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 10;
        sr.drawMode = SpriteDrawMode.Simple;

        spawnedLaserParts.Add(obj);
        return obj;
    }
}