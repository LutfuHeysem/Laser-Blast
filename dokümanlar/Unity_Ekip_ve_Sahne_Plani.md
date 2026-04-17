# Vector Flow - Unity Ekip Organizasyonu & Sahne Planlaması

Bu döküman, 3 kişiden oluşan geliştirici takımının Unity ortamında **merge conflict (kod çakışması)** yaşamadan, organize ve modüler biçimde çalışabilmesi için hazırlanmıştır. Mimarinin temeli: **"Farklı kişiler, farklı sahnelerde ağırlıklı olarak çalışmalı ve işlerini sadece Prefab veya Kodlar üzerinden modüler olarak taşımalıdır."**

---

## 1. Sahne (Scene) Mimarisi 

Projeniz, görevleri net olarak ayrılmış 3 ana sahneden oluşacaktır:

### 🎮 Sahne 1: `Assets/Scenes/MainMenu.unity`
* **Amacı:** Oyunun girişi (Start), Ayarlar menüsü, Bölüm seçimi (Level Select) ve jenerik (Credits) alanları barındırır. Oyunun asıl mekanikleri burada bulunmaz, sadece geçiş logic'leri vardır.
* **Ana Sorumlusu:** Rol 3 (UI, Görsel & UX)
* **Destek Veren:** Rol 1 (Sahne Geçiş Kodları)

### 🕹️ Sahne 2: `Assets/Scenes/Game_Core.unity`
* **Amacı:** Nihai oyun bölümümüz. İçerisinde stabil hale gelmiş (bitmiş) tüm mekanikler ve Level Data dosyaları çalıştırılır. Burası "Canlı Test (Production)" alanıdır. Oynanış programcısı buraya hatalı olabilecek kod atamaz. Sadece bitmiş yapılar buradaki `GridManager`'a tanıtılır.
* **Ana Sorumlusu:** Rol 1 (Sistem Bütünü) ve Rol 3 (Bitmiş Level Dizaynlarını Sahnede Test Eden)

### 🧪 Sahne 3: `Assets/Scenes/Game_Sandbox.unity`
* **Amacı:** Çekirdek (Core) sahnenin birebir kopyasıdır. Ancak bu sayfa bir "Ar-Ge" laboratuvarıdır. İleride eklenebilecek yeni bulmaca dinamikleri algoritmaları, portallar, yeni sekme matematiği burada inşa edilir.
* **Ana Sorumlusu:** Rol 2 (Oynanış, Fizik & Mekanik)
* **Destek Veren:** Mekanikler çalışıyorsa kodları alıp Core sahneye ekleyen Rol 1.

---

## 2. Kişi Başı Pay / Görev Dağılımı

Takım 3 kişiden oluştuğu için herkesin birbirine bağımlı olduğu fakat işini bağımsız yürüttüğü aşağıdaki rol dağıtımı planlanmıştır:

### 👨‍💻 KİŞİ 1: Çekirdek Sistem Mimarı (Core Systems & Data)
**Uzmanlık:** Veri akışları, Game State ve Proje Mimarisi.
* `GameManager.cs` yapılarını kontrol eder (Enerji durumu, Oyunu Kazanma/Kaybetme süreçleri).
* Level Editor arkaplan verileri (`LevelData` ScriptableObject yapısı) ile `GridManager.cs` iletişimini yönetir.
* Sahneler arası bağlantıları (`SceneManager`) kodlar ve data kayıplarını (Örn. en yüksek level skoru hafızası vb.) gerçekleştirir.
* **Ana Görev Yeri:** Tüm root kodlar, `Game_Core.unity` controller nesneleri (Hierarchy).

### 🛠️ KİŞİ 2: Oynanış ve Mekanik Programcısı (Sandbox & Physics)
**Uzmanlık:** Oyunun ana bulmaca eğlencesi, "Juice" temelleri, Lazer Hareket Algoritmaları.
* `BeamManager.cs` ve hücreler (Mirror, TNT, Wall) arasındaki etkileşimin matematiksel formüllerini oluşturur. Işının adım adım gidişinin kusursuz hissettirmesini sağlar.
* Menü ve genel ayarlar ile ilgilenmez. Dünyası `Game_Sandbox.unity` içerisidir. Orada çılgın yeni ok kombinasyonlarını dener. 
* Çalışan bir mekanik yazıldığında kodlarını GitHub/GitLab'a atar ve Kişi 1'e "Hocam Prism kodları hazır" diyerek topu paslar.
* **Ana Görev Yeri:** `Game_Sandbox.unity` ve `BeamController` Scriptleri.

### 🎨 KİŞİ 3: UX, UI ve Level Designer (Visual Polish)
**Uzmanlık:** Görsellik hissi, Canvas ekranları, Bölüm Dizaynı.
* Kodla minimal ilgilenir! Ancak Editor hakimiyeti iyidir. Unity'de objeleri birbirine bağlar, görsel şöleni sağlar.
* `MainMenu.unity` sahnesinde Canvas UI tasarlar.
* `Game_Core`'de ışın giderken arkasından çıkacak Lazer Efekti (Trail Renderer/Particle System) veya patlama anında Particle patlamalarını Editör içerisinden hazırlar.
* Ekip kodları bitirdikten sonra ana GDD'de yer alan "The Splitter" gibi karmaşık levelleri Unity Inspector içerisinden `GridManager`'a level level tasarlar, dizayn eder, oyuncunun beynini yakan zeka dolu bulmacaları inşa eder.
* **Ana Görev Yeri:** `MainMenu.unity`, Prefab Tasarımları, Editor Inspector ile Level Building (Scriptable Objects).

---

## 3. GitHub (VCS) ve Modüler Çalışma Kuralları

Unity'de ekipçe çalışırken en çok yaşanan problem sahnede (Scene dosyasında) çakışma olabilmesidir.

1. **Hiçbir zaman iki kişi aynı Sahneyi (.unity) dosyasını eş zamanlı değiştirip commit atmamalıdır.**
2. Eğer "Kişi 2", yeni bir `Prism_V` (Prizma hücresi) tasarlıyorsa bunu bir **Prefab** haline getirir. Sahnede düzenleme yapmaz, Prefab'e kaydeder.
3. Kilit objeler (TNT, Mirrors, Arrow) **Prefab** olmalıdır. "Kişi 3" görsel efektleri değiştirmek için sadece bu prefabların içine girip düzenler, sahneyi kurcalamaz. Bu sayede konflikt yaşanmaz!
4. Yeni bir kod/bölüm deneneceği zaman Branch açılmalıdır; `feature/menu-ui` (Kişi 3), `feature/new-prism-logic` (Kişi 2) gibi. 

Bu yapı oturur vaziyette takip edilirse oyununuz hızlı ve modüler bir şekilde tamamlanacaktır. Başarılar Takım!
