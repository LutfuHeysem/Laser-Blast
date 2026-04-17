# Vector Flow - Unity Geliştirme Planı

Bu plan, **Vector Flow** oyununun ana hedef mimarisine uygun olarak **Unity Editor & C#** ile geliştirilmesi için hazırlanmıştır. Hedef, temiz bir yazılım mimarisi kurmak ve editör içerisinden rahatça level tasarlanabilmesini sağlamaktır.

## 1. Faz: Hazırlık ve Mimarının Kurulması
- [ ] `Assets/Scripts/Core` vb. klasör yapılasının kurulması.
- [ ] Ana oyun yöneticisi (`GameManager.cs`) sistematiğinin yazılması. Aşama kontrolü (Örn: Idle, Playing, Won, Lost) için State Pattern kullanımı.
- [ ] Grid ve Cell verilerini tutacak ScriptableObject ya da Serializable Class yapılarının tasarlanması.

## 2. Faz: Core Grid ve Hücre Sistemi
- [ ] **Level Data Structure**: Level yapılarını ve 2D grid'i görselleştirebilmek için `GridManager.cs`.
- [ ] **Cell Types (Enum)**:
  - `Empty`, `ArrowUp`, `ArrowRight`, `ArrowDown`, `ArrowLeft`
  - `GlassWall`, `SteelWall`
  - `Mirror_NW_SE` (\), `Mirror_NE_SW` (/)
  - `Prism_H`, `Prism_V`
  - `TNT`, `GoalHole`
- [ ] **Prefab Hazırlığı (Kullanıcı Tarafı)**: Boş hücreler (Empty) ve diğer nesnelerin Prefab'lerinin Unity Editör üzerinde hazırlanması. Scriptlerin Prefab'lere uyarlanması.

## 3. Faz: Etkileşim ve İlk Mantık
- [ ] Ekrana dokunma/tıklama ile Raycast göndererek "Arrow" hücrelerinin algılanması (`InputController.cs`).
- [ ] Enerji kontrolü: Tetikleme anında limiti (Enerji=1) kontrol edip uygun hamlenin başlaması.

## 4. Faz: Beam Simülasyonu ve Kurallar (The Logic Engine)
- [ ] **BeamController.cs / BeamManager.cs**:
  - Işının (beam) Grid üzerinde tick-based veya Coroutine tabanlı interpolasyonla hareketi.
- **Çarpışma Çözümleyicisi (Collision Resolver)**:
  - Hedef hücre tipine göre; ayna sekmesi, prizma ile bölünüp birden fazla ışın doğması, "Goal" noktasına varış veya "Steel" noktasına çarpıp yok olma gibi senaryoların C# Event veya Action mekanizmalarıyla tetiklenmesi.
  - Olası "TNT" patlama etkileşimlerinin etrafındaki 3x3 alanda yer alan objelerin `Destroy()` varyasyonları.

## 5. Faz: Animasyonlar, Geri Bildirim ve UI (Juice)
- [ ] **Unity Particle System**: Işın ilerlerken `Trail Renderer` bırakması. TNT patlaması için Particle efektlerinin çağrılması.
- [ ] Cam kırıldığında çalışacak hafif sarsıntı (Camera Shake) ve parıltılar.
- [ ] `UIManager.cs`: Level numarası, Kalan Enerji, Retry butonu işlemlerinin Canvas üzerinde Canikliği.

## 6. Faz: Level Tasarımı (Mentor Showcases)
- [ ] Kullanıcının GameObject'leri sahne üzerinde (veya özel bir Grid Editor scripti ile) "The Splitter" bölümünü görsel olarak inşa etmesi. Tasarım dökümanında öngörülen mekaniklerin Editör üzerinden bağlanması.

## 7. Faz: Polish & Son Dokunuşlar
- [ ] Android / iOS resolution ayarları uyarınca `Canvas Scaler` ve `Camera Orthographic Size` kalibrasyonu.
- [ ] Ses efektleri eklenmesi ve performans kontrolü.
