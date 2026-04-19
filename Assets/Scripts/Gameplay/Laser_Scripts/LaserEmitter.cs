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
    public int currentBounces = 0;

    private Coroutine laserCoroutine;
    private List<GameObject> spawnedLaserParts = new List<GameObject>();

    // Animasyon kontrolcüsü
    private Animator animator;

    void Start()
    {
        // Başlangıçta objenin üzerindeki Animator bileşenini bul
        animator = GetComponent<Animator>();

        if (!isActivated) ClearLaser();
    }

    void OnMouseDown()
    {
        if (!isActivated)
        {
            if (VectorFlow.Managers.GameManager.Instance != null)
            {
                // Enerji tüketmeye çalış, yeterli enerji yoksa ateşleme
                if (!VectorFlow.Managers.GameManager.Instance.TryConsumeEnergy())
                {
                    return;
                }
            }

            // Oyuncu kendi eliyle sıktığında komboyu sıfırlıyoruz.
            if (VectorFlow.Managers.ScoreManager.Instance != null)
            {
                VectorFlow.Managers.ScoreManager.Instance.ResetCombo();
            }

            isActivated = true;
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.audioPlay("laser");
            }

            // Ateşleme başladığında animasyonu oynat
            if (animator != null) animator.Play("Laser_shoot");

            if (laserCoroutine != null) StopCoroutine(laserCoroutine);
            laserCoroutine = StartCoroutine(AnimateLaser());
        }
    }

    public bool OnLaserHit(Vector2 hitPoint, Vector2 incomingDirection, LaserEmitter sourceLaser, out Vector2 outgoingDirection)
    {
        outgoingDirection = Vector2.zero;

        // Eğer başka bir lazer bize çarparsa ve biz henüz ateşlenmemişsek, zincirleme ateş (chain) başlasın!
        if (!isActivated)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.audioPlay("laser");
            }
            // Zincirleme ateşleme olduğu için komboyu arttır!
            if (VectorFlow.Managers.ScoreManager.Instance != null)
            {
                // Eğer LaserEmitter içinde o an çarpılan noktanın Vector2/Vector3 verisi varsa (örn: hitPoint), transform.position yerine onu yazman çok daha şık olur!
                VectorFlow.Managers.ScoreManager.Instance.IncrementCombo(transform.position);
                VectorFlow.Managers.ScoreManager.Instance.AddScore(100, transform.position);
            }

            isActivated = true;

            // Zincirleme ateşleme başladığında da animasyonu oynat
            if (animator != null) animator.Play("Laser_shoot");

            if (laserCoroutine != null) StopCoroutine(laserCoroutine);
            laserCoroutine = StartCoroutine(AnimateLaser());
        }

        return false; // Işın silahın içinden geçmesin, silahta dursun.
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
        Vector2 currentDir = transform.up;

        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null) myCollider.enabled = false;
        if (ignoreCollider != null) ignoreCollider.enabled = false;

        if (tailSprite != null)
        {
            GameObject tailObj = CreateSpriteObject("Laser_Tail", tailSprite, currentPos, currentDir, false);
            float tailScale = laserThickness / tailSprite.bounds.size.x;
            tailObj.transform.localScale = new Vector3(tailScale, tailScale, 1f);
        }

        while (currentBounces < maxBounces)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDir, maxDistance);
            Vector3 targetPos = hit.collider != null ? (Vector3)hit.point : (Vector3)(currentPos + currentDir * maxDistance);
            
            // GÖRSEL DÜZELTME: Eğer çarptığımız şey bir aynaysa veya prizmaysa, ışını tam merkeze kadar uzat ki kesiklik olmasın.
            if (hit.collider != null && hit.collider.GetComponent<ILaserInteractable>() != null)
            {
                targetPos = hit.collider.transform.position;
            }

            bool hitSomething = hit.collider != null;

            if (hitSomething)
            {
                ILaserInteractable interactable = hit.collider.GetComponent<ILaserInteractable>();
                if (interactable is LaserEmitter)
                {
                    Vector3 gunCenter = hit.collider.transform.position;
                    // Kaymayı (sliding) engellemek için, hedefin merkezini ışın çizgisine izdüşürüyoruz:
                    float distanceAlongRay = Vector3.Dot(gunCenter - (Vector3)currentPos, currentDir);
                    targetPos = (Vector3)currentPos + (Vector3)currentDir * distanceAlongRay;
                }
            }

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

            if (isOffScreen)
            {
                break;
            }

            bodyObj.transform.localScale = new Vector3(laserThickness / spriteWidth, distanceToTarget / spriteHeight, 1f);
            bodyObj.transform.position = (Vector3)currentPos + ((Vector3)targetPos - (Vector3)currentPos) / 2f;
            headObj.transform.position = targetPos;

            if (hitSomething && hit.collider != null)
            {
                ILaserInteractable interactable = hit.collider.GetComponent<ILaserInteractable>();
                if (interactable != null)
                {
                    Vector2 newDirection;
                    if (interactable.OnLaserHit(hit.point, currentDir, this, out newDirection))
                    {
                        // HİZA DÜZELTME: Eğer bir aynaya veya blok merkeze vuruyorsa, 
                        // tam ortadan sekmeyi garantilemek için pozisyonu objenin merkezine sabitliyoruz.
                        currentPos = (Vector2)hit.collider.transform.position; 
                        
                        currentDir = newDirection.normalized;
                        currentBounces++;

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

        // Lazer hedefe ulaştıktan sonra çizgilerin ekranda kalma süresi
        yield return new WaitForSeconds(0.1f);

        // Lazer çizgilerini sahneden sil
        ClearLaser();

        if (VectorFlow.Managers.GameManager.Instance != null)
        {
            VectorFlow.Managers.GameManager.Instance.UnregisterActiveLaser(this);
        }

        // Lazer çizimi silindikten sonra, silahın kendisini yok etmeden önce 1.2 saniye bekle
        yield return new WaitForSeconds(1.2f);

        // Süre dolunca silahı sahneden tamamen sil
        Destroy(gameObject);
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