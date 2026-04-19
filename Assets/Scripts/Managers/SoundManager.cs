using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton erişimi için Instance oluşturuyoruz
    public static SoundManager Instance { get; private set; }

    [Header("Ses Ayarları")]
    public AudioSource audioSource;

    [Header("Müzik Ayarları")]
    public AudioSource musicSource; // Müzik için ayrı kanal
    public AudioClip backgroundMusic;

    public AudioClip shootSound;
    public AudioClip breakGlass;
    public AudioClip MirrorLaser;
    public AudioClip SplitLaser;
    public AudioClip Explosion;
    public AudioClip HitWall;
    public AudioClip win;
    public AudioClip lose;

    [Range(0, 1)] public float volume = 0.7f;

    // Ses stringlerini tutacağımız kuyruk (Queue)
    private Queue<string> audioQueue = new Queue<string>();

    // Obje sahneye yüklendiğinde Instance'ı ayarlıyoruz
    void Awake()
    {
        // Singleton Kontrolü
        if (Instance != null && Instance != this)
        {
            // Eğer bu script tek başına bir objeyse objeyi yok et, 
            // ama üzerinde başka önemli scriptler varsa sadece bu scripti yok et.
            if (GetComponents<Component>().Length <= 3) 
                Destroy(gameObject);
            else 
                Destroy(this);
                
            return;
        }
        
        Instance = this;

        // Sahne geçişlerinde müzik kesilmesin diye kök dizine çıkar ve koru
        if (transform.parent != null) transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Eğer Inspector'dan atanmadıysa otomatik bulmaya çalış
        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            if (audioSource == null) audioSource = sources[0];
            if (musicSource == null) musicSource = sources[1];
        }
        else if (sources.Length == 1)
        {
            if (audioSource == null) audioSource = sources[0];
        }
        
        // Müziği başlat (Eğer atanmışsa ve şu an o parça çalmıyorsa)
        if (musicSource != null && backgroundMusic != null)
        {
            if (musicSource.clip != backgroundMusic)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                musicSource.Play();
            }
        }
    }

    void Update()
    {
        // Kuyrukta işlenecek ses varsa ve sürekli kontrol ediyorsak:
        if (audioQueue.Count > 0)
        {
            // Kuyruktaki ilk elemanı al ve kuyruktan çıkar
            string nextAudio = audioQueue.Dequeue();
            ProcessSound(nextAudio);
        }
    }

    public void audioPlay(string audio)
    {
        // Gelen isteği doğrudan kuyruğa atıyoruz
        audioQueue.Enqueue(audio);
    }

    private void ProcessSound(string audio)
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
                    if (SplitLaser != null) audioSource.PlayOneShot(SplitLaser, volume);
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