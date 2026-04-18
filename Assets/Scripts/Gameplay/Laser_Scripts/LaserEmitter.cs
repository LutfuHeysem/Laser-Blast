using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserEmitter : MonoBehaviour, ILaserInteractable
{
    public enum SpriteDrawDirection { Horizontal, Vertical }

    [Header("Sprite Ayarları")]
    public Sprite headSprite;
    public Sprite bodySprite;
    public Sprite tailSprite;
    public float laserThickness = 0.5f;

    [Tooltip("Sprite dosyası yukarı doğru çizildiyse Vertical, sağa doğru çizildiyse Horizontal seçin.")]
    public SpriteDrawDirection spriteDirection = SpriteDrawDirection.Vertical;

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
            if (VectorFlow.Managers.GameManager.Instance != null)
            {
                if (!VectorFlow.Managers.GameManager.Instance.TryConsumeEnergy()) return;
            }

            if (VectorFlow.Managers.ScoreManager.Instance != null)
            {
                VectorFlow.Managers.ScoreManager.Instance.ResetCombo();
            }

            isActivated = true;
            if (laserCoroutine != null) StopCoroutine(laserCoroutine);
            laserCoroutine = StartCoroutine(AnimateLaser());
        }
    }

    public bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter sourceLaser, out Vector2 outgoingDirection)
    {
        // 1. Gelen lazeri yönünü hiç bozmadan dışarı ver (içinden geç)
        outgoingDirection = incomingDirection;

        // 2. Başka bir lazer bize çarptıysa ateşleme sekansını başlat
        if (!isActivated)
        {
            if (VectorFlow.Managers.ScoreManager.Instance != null)
            {
                VectorFlow.Managers.ScoreManager.Instance.IncrementCombo();
                VectorFlow.Managers.ScoreManager.Instance.AddScore(50);
            }

            isActivated = true;
            if (laserCoroutine != null) StopCoroutine(laserCoroutine);
            laserCoroutine = StartCoroutine(AnimateLaser());
        }

        // 3. True dönüyoruz ki ana ışın bizim içimizden geçip yoluna devam etsin
        return true;
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
        if (VectorFlow.Managers.GameManager.Instance != null)
        {
            VectorFlow.Managers.GameManager.Instance.RegisterActiveLaser(this);
        }

        ClearLaser();

        Vector2 currentPos = startingPoint != null ? (Vector2)startingPoint.position : (Vector2)transform.position;
        Vector2 currentDir = transform.right;
        int bounces = 0;

        // --- YENİ VE KUSURSUZ ÇÖZÜM: Kara Liste Mantığı ---
        // Işının görmezden geleceği tüm objeleri bu listede tutacağız.
        HashSet<Collider2D> ignoredColliders = new HashSet<Collider2D>();

        // Önce lazeri ateşleyen silahın (kendimizin) üzerindeki ve alt objelerindeki tüm collider'ları listeye ekle.
        foreach (var col in GetComponentsInChildren<Collider2D>())
        {
            ignoredColliders.Add(col);
        }

        // Prizma gibi özel bir obje tarafından ateşlendiysek, onu da listeye ekle.
        if (ignoreCollider != null) ignoredColliders.Add(ignoreCollider);
        // ----------------------------------------------------

        if (tailSprite != null)
        {
            GameObject tailObj = CreateSpriteObject("Laser_Tail", tailSprite, currentPos, currentDir, false);
            float tailScale = laserThickness / tailSprite.bounds.size.x;
            tailObj.transform.localScale = new Vector3(tailScale, tailScale, 1f);
        }

        while (bounces < maxBounces)
        {
            // O yöne doğru giden GÖRÜNMEZ BİR ÇİZGİ çek ve çarptığı HER ŞEYİ bul
            RaycastHit2D[] hits = Physics2D.RaycastAll(currentPos, currentDir, maxDistance);

            RaycastHit2D validHit = new RaycastHit2D();
            float closestDist = float.MaxValue;

            // Çarptığımız şeyleri tek tek incele
            foreach (var hit in hits)
            {
                // Eğer vurduğumuz obje "Kara Listemizde" DEĞİLSE, onu geçerli hedef olarak kabul et
                if (hit.collider != null && !ignoredColliders.Contains(hit.collider))
                {
                    float dist = Vector2.Distance(currentPos, hit.point);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        validHit = hit;
                    }
                }
            }

            Vector3 targetPos = validHit.collider != null ? (Vector3)validHit.point : (Vector3)(currentPos + currentDir * maxDistance);
            bool hitSomething = validHit.collider != null;

            float distanceToTarget = Vector3.Distance(currentPos, targetPos);
            float travelledDistance = 0f;

            GameObject bodyObj = CreateSpriteObject("Laser_Body", bodySprite, currentPos, currentDir, true);
            GameObject headObj = CreateSpriteObject("Laser_Head", headSprite, currentPos, currentDir, false);

            if (headSprite != null)
            {
                float headScale = laserThickness / headSprite.bounds.size.x;
                headObj.transform.localScale = new Vector3(headScale, headScale, 1f);
            }

            float spriteWidth = bodySprite.bounds.size.x;
            float spriteHeight = bodySprite.bounds.size.y;

            Camera mainCam = Camera.main;
            bool isOffScreen = false;

            // Lazerin uzama animasyonu
            while (travelledDistance < distanceToTarget)
            {
                travelledDistance += laserSpeed * Time.deltaTime;
                float t = Mathf.Clamp01(travelledDistance / distanceToTarget);
                Vector3 currentTipPos = Vector3.Lerp(currentPos, targetPos, t);

                if (mainCam != null)
                {
                    Vector3 viewPos = mainCam.WorldToViewportPoint(currentTipPos);
                    if (viewPos.x < -0.05f || viewPos.x > 1.05f || viewPos.y < -0.05f || viewPos.y > 1.05f)
                    {
                        isOffScreen = true;
                        break;
                    }
                }

                float currentDist = Vector3.Distance(currentPos, currentTipPos);
                bodyObj.transform.localScale = new Vector3(laserThickness / spriteWidth, currentDist / spriteHeight, 1f);
                bodyObj.transform.position = (Vector3)currentPos + ((Vector3)currentTipPos - (Vector3)currentPos) / 2f;
                headObj.transform.position = currentTipPos;

                yield return null;
            }

            if (isOffScreen) break;

            bodyObj.transform.localScale = new Vector3(laserThickness / spriteWidth, distanceToTarget / spriteHeight, 1f);
            bodyObj.transform.position = (Vector3)currentPos + ((Vector3)targetPos - (Vector3)currentPos) / 2f;
            headObj.transform.position = targetPos;

            // Hedefe ulaşıldığında yapılacak etkileşim
            if (hitSomething && validHit.collider != null)
            {
                // Çarptığımız objede veya onun ebeveyninde etkileşim kodumuz var mı diye bakıyoruz
                ILaserInteractable interactable = validHit.collider.GetComponent<ILaserInteractable>();
                if (interactable == null) interactable = validHit.collider.GetComponentInParent<ILaserInteractable>();

                if (interactable != null)
                {
                    Vector2 newDirection;
                    // Eğer OnLaserHit "True" dönerse (içinden geçme izni verirse)
                    if (interactable.OnLaserHit(validHit.point, currentDir, this, out newDirection))
                    {
                        // --- OYUN DEĞİŞTİRİCİ KISIM ---
                        // İçinden geçeceğimiz objeyi "Kara Listeye" ekliyoruz.
                        ignoredColliders.Add(validHit.collider);

                        // Lazerin başlangıç noktasını çarptığımız yer yapıyoruz
                        currentPos = validHit.point;
                        currentDir = newDirection.normalized;
                        bounces++;

                        // Lazerin kafasını siliyoruz ki arada kötü görünmesin, bir sonraki döngüde yeniden çizilecek.
                        Destroy(headObj);
                        spawnedLaserParts.Remove(headObj);
                        continue;
                    }
                }
            }
            break; // Eğer interactable değilse veya false döndüyse döngüyü kır ve lazeri bitir
        }

        yield return new WaitForSeconds(0.1f);
        ClearLaser();

        if (VectorFlow.Managers.GameManager.Instance != null)
        {
            VectorFlow.Managers.GameManager.Instance.UnregisterActiveLaser(this);
        }
    }

    private GameObject CreateSpriteObject(string objName, Sprite sprite, Vector3 pos, Vector3 dir, bool isBody)
    {
        GameObject obj = new GameObject(objName);
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