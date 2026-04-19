using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton erişimi için Instance oluşturuyoruz
    public static SoundManager Instance { get; private set; }

    [Header("Ses Ayarları")]
    public AudioSource audioSource;

    public AudioClip shootSound;
    public AudioClip breakGlass;
    public AudioClip MirrorLaser;
    public AudioClip SplitLaser;
    public AudioClip Explosion;
    public AudioClip HitWall;
    public AudioClip win;
    public AudioClip lose;

    [Range(0, 1)] public float volume = 0.7f;

    // Obje sahneye yüklendiğinde Instance'ı ayarlıyoruz
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void audioPlay(string audio)
    {
        // Sadece audioSource'un dolu olup olmadığını kontrol etmemiz yeterli
        if (audioSource != null)
        {
            switch (audio)
            {
                case "laser":
                    if (shootSound != null) audioSource.PlayOneShot(shootSound, volume);
                    break;
                case "break":
                    if (breakGlass != null) audioSource.PlayOneShot(breakGlass, volume);
                    break;
                case "mirror":
                    if (MirrorLaser != null) audioSource.PlayOneShot(MirrorLaser, volume);
                    break;
                case "split":
                    if(SplitLaser != null) audioSource.PlayOneShot(SplitLaser, volume);
                    break;
                case "explode":
                    if (Explosion != null) audioSource.PlayOneShot(Explosion, volume);
                    break;
                case "hit":
                    if (HitWall != null) audioSource.PlayOneShot(HitWall, volume);
                    break;
                case "win":
                    if (win != null) audioSource.PlayOneShot(win, volume);
                    break;
                case "lose":
                    if (lose != null) audioSource.PlayOneShot(lose, volume);
                    break;
                default:
                    break;
            }
        }
    }
}