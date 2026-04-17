# Vector Flow – Detaylı Oyun Tasarım Dokümanı

## 1. Proje Özeti

**Vector Flow**, portre modda oynanan, mobil odaklı, grid tabanlı bir puzzle oyunudur. Oyuncu her level’da sınırlı sayıda hamle hakkı ile bir veya daha fazla **Arrow Block** tetikler. Bu tetikleme sonucunda ortaya çıkan ışınlar (beam/pulse), grid üzerinde ilerleyerek aynalar, splitter’lar, TNT, kırılabilir engeller ve diğer yönlendirici yapılarla etkileşime girer. Amaç, en verimli zincir reaksiyonu kurarak ışını hedefe ulaştırmak veya level hedeflerini tamamlamaktır.

Oyunun temel gücü, **az oyuncu girdisi ile büyük ve tatmin edici sonuçlar üretmesi**dir. Oyuncu uzun bir manuel aksiyon zinciri gerçekleştirmez; bunun yerine doğru başlangıç kararını verir ve sistemin tetiklediği reaksiyonu izler. Bu nedenle oyun, hem stratejik planlama hem de görsel tatmin üretir.

Bu oyunun geliştirme yönü artık netleşmiştir: proje **Unity içinde üretilecektir** ve mevcut demo da bu fikrin doğrulanmış bir temelini oluşturur.

Başlangıçta mentor sunumu için hızlı prototipleme odaklı düşünülmüş olsa da, artık fikir ve demo **onaylandığı** için tasarım kararları geçici sunum mantığına göre değil, **Unity’de sürdürülebilir şekilde geliştirilecek gerçek oyun yapısına** göre okunmalıdır.

Bu noktadan sonra demo, yalnızca fikir gösterimi yapan geçici bir araç değil; Unity sürümünün çekirdek referansı ve ilk vertical slice’ı olarak ele alınmalıdır.

---

## 2. Tasarım Hedefleri

Bu proje için ana tasarım hedefleri şunlardır:

1. **Mobilde rahat anlaşılabilir bir puzzle deneyimi sunmak**
2. **Tek dokunuşun önemli olduğu bir karar anı yaratmak**
3. **Zincir reaksiyon hissini güçlü şekilde vermek**
4. **İlk 5 saniyede anlaşılabilir, sonrasında derinleşebilir olmak**
5. **Game jam süresine uygun, hızlı prototiplenebilir bir yapı kurmak**
6. **Jüri sunumunda izlenmesi keyifli bir “wow moment” üretmek**
7. **Mentora çok kısa sürede gösterilebilecek pratik bir demo çıkarmak**

Bu nedenle tasarım kararları gereksiz sistemlerden arındırılmış, okunabilirliği yüksek ve dikey dilim üretmeye uygun biçimde seçilmiştir.

---

## 3. Yüksek Seviye Pitch

### Kısa Pitch

Vector Flow, tek bir dokunuşla başlayan zincirleme ışın reaksiyonları üzerinden oyuncuya planlama ve tatmin duygusunu aynı anda veren mobil bir puzzle oyunudur.

### Geniş Pitch

Oyuncu sınırlı enerji ile grid üzerindeki doğru oku tetikler. Tetiklenen ışın, aynalardan sekerek, splitter’larda çoğalarak, TNT ile engelleri yok ederek ve yeni okları aktive ederek tüm level’ı bir domino etkisi gibi çözer. Oyuncunun görevi çok sayıda hamle yapmak değil, doğru hamleyi seçmektir.

---

## 4. Tür, Platform ve Sunum Kimliği

* **Tür:** Chain-reaction puzzle / logic puzzle
* **Platform:** Mobil odaklı, portre ekran
* **Geliştirme motoru:** Unity
* **Mevcut durum:** Demo hazırlanmış ve fikir onaylanmış durumda
* **Yeni hedef:** Mevcut demonun Unity içinde düzgün, genişletilebilir ve polish edilebilir oyuna dönüştürülmesi
* **Sunum kimliği:** “Small input, big payoff”
* **Tematik omurga:** Arrow / Vector + Hole + Block

### Teknoloji stratejisi

Bu proje artık iki ayrı teknoloji katmanı üzerinden düşünülmeyecektir. Karar verilmiştir:

* Oyun **Unity ile geliştirilecektir**.
* Hazırlanan demo, Unity sürümü için referans kabul edilecektir.
* Bundan sonraki tasarım ve teknik planlama, doğrudan Unity sahne yapısı, prefab mantığı, veri yönetimi ve efekt üretimi düşünülerek yapılmalıdır.

### Unity açısından bunun anlamı

Bu karar sayesinde artık şu başlıklarda daha rahat ilerlenebilir:

* sahne düzeni
* prefab tabanlı blok sistemi
* ScriptableObject veya veri tabanlı level tanımı
* animasyon ve juice’in daha rahat genişletilmesi
* mobil build ve dokunmatik kontrollerin gerçek hedef platforma göre ele alınması

Bu yüzden bu dokümandaki sade çekirdek tasarım korunurken, bundan sonraki bütün teknik genişleme **Unity production path** üzerinden değerlendirilmelidir.

---

## 5. Çekirdek Oynanış Döngüsü

Oyunun temel döngüsü şöyledir:

1. **Grid’i oku**
2. **Işının nasıl ilerleyeceğini zihninde tahmin et**
3. **Doğru Arrow Block’u seç**
4. **Zincir reaksiyonunu izle**
5. **Hedefe ulaşırsan level clear, ulaşamazsan retry**
6. **Sonraki level’a geç ve yeni mekanik öğren**

Bu döngü özellikle kısa oturumlar için uygundur. Oyuncu her level’da küçük ama anlamlı bir problem çözer.

---

## 6. Ana Tasarım Kararları

### 6.1 Level-based yapı

Oyun sonsuz skor kovalamacası yerine **level tabanlı** olacaktır.

### 6.2 Oyuncu girişi minimum tutulacak

Temel aksiyon, doğru **Arrow Block**’u seçmektir. Oyun execution değil decision merkezlidir.

### 6.3 Arrow’lar sabit olacak

İlk demo için Arrow Block’lar sabit yönlü ve önceden yerleştirilmiş olacaktır.

### 6.4 Ana kazanma koşulu: Goal Hole’a ulaşmak

İlk demo için ana win condition:

* **Beam’in Goal Hole’a ulaşması**

### 6.5 Hole, ilk versiyonda hedef olacak

Hole başlangıçta karmaşık fiziksel bir emme sistemi taşımayacak; önce final hedef olarak davranacaktır.

### 6.6 Steel normal beam ile kırılmaz, TNT ile kırılabilir

* **Glass:** beam ile kırılır
* **Steel:** beam geçemez, beam ile kırılmaz
* **TNT:** Steel dahil belirli engelleri yok edebilir

### 6.7 Prism ve splitter sade tutulacak

İlk demo için anlaşılır, deterministik, zayıflama içermeyen split mantığı kullanılacaktır.

---

## 7. Oyunun Temel Felsefesi

Vector Flow’un iyi hissettirmesi için oyun şu üç duyguyu üretmelidir:

* **Öngörü**
* **Doğrulama veya şaşırma**
* **Tatmin**

Yani oyuncu önce çözümü tahmin eder, sonra sistemin reaksiyonunu izler ve sonunda güçlü bir payoff yaşar.

---

## 8. Grid Sistemi

### 8.1 Grid boyutu

Demo için önerilen varsayılan grid boyutu:

* **6 x 8**

### 8.2 Hücre mantığı

Her hücrede şu türlerden biri bulunabilir:

* boş hücre
* yön bloğu
* duvar
* kırılabilir cam
* ayna
* splitter/prism
* TNT
* goal hole

### 8.3 Hareket mantığı

Beam hücre tabanlı ilerler. Her adımda bir sonraki hücreye geçer ve o hücrenin türüne göre yeni bir olay doğar.

---

## 9. Blok Türleri ve Davranışları

### 9.1 Empty Cell

İçinde hiçbir şey olmayan hücredir. Beam bu hücreden etkilenmeden geçer.

### 9.2 Arrow Block

* Oyuncu tarafından tetiklenebilir.
* Beam başka bir Arrow Block’a çarparsa o ok da aktive olur.
* Yönüne göre yeni bir beam üretir.

### 9.3 Glass Wall

* Beam ile temas ettiğinde kırılır.
* Yol açıcı ve tatmin üretici bir engeldir.
* Tematik yorumda bu eleman, **ince buz tabakası / kırılabilir don katmanı** olarak sunulacaktır.

### 9.4 Steel Wall / Pillar

* Beam’i durdurur.
* Normal beam ile yok olmaz.
* TNT ile kırılabilir.
* Tematik yorumda bu eleman, **kalın kaya, sert kök bariyeri veya yoğun donmuş blok** olarak sunulabilir.

### 9.5 Mirror

* `/` ve `\` tipi iki temel ayna düşünülür.
* Beam’i geliş yönüne göre sektirir.
* Tematik yorumda bu eleman, **kristal yüzey, parlak buz plakası veya sihirli orman aynası** olabilir.

### 9.6 Prism / Splitter

* Beam’i iki yöne böler.
* İlk demo için ikiye ayrılma yeterlidir.
* Tematik yorumda bu eleman, **ışık saçan kristal çiçek, mantar veya büyülü taş** olabilir.

### 9.7 TNT

* Beam ile tetiklenince patlar.
* **3x3 alan** etkiler.
* Diğer TNT’leri zincirleme tetikleyebilir.
* Tematik yorumda bu eleman, doğrudan TNT olmak zorunda değildir; bunun yerine **ısı yayan tohum, patlayan mantar, kor çekirdeği veya çevresel enerji düğümü** olarak gösterilebilir.

### 9.8 Goal Hole

* Ana hedeftir.
* Beam hole’a girerse level clear olur.
* Tematik yorumda bu hedef, **buz içindeki kediye ulaşılması / son buz katmanının eritilmesi** olarak yeniden sunulacaktır.

Yani sistemsel olarak tek bir hedef hücresi vardır; fakat oyuncuya görünen şey bir “hole” değil, **kurtarılmayı bekleyen donmuş kedi** olabilir.

---

## 10. Beam Sistemi

### 10.1 Beam’in genel mantığı

Beam bir yönle doğar ve düz çizgide ilerler.

Duracağı durumlar:

* grid dışına çıkarsa
* uygun engelde durursa
* hedefe ulaşırsa
* yeni bir etkileşim sonucu yön değiştirirse

### 10.2 Zincir reaksiyon

Zincir reaksiyon şu yollarla büyür:

* beam başka bir arrow’u tetikler
* beam prism’de bölünür
* beam TNT’yi patlatır
* TNT yeni yollar açar
* açılan yol başka beam’lerin hedefe gitmesini sağlar

### 10.3 Deterministik yapı

Aynı level, aynı dokunuş ile her zaman aynı sonucu vermelidir.

---

## 11. Enerji ve Hamle Sistemi

* Her manuel tap 1 enerji harcar.
* İlk demo seviyelerinde çoğunlukla **1 Energy** olacaktır.
* Zincir içindeki otomatik reaksiyonlar ücretsizdir.

Bu sistemin amacı, çok aksiyon değil **doğru başlangıç kararı** üretmektir.

---

## 12. Başarısızlık ve Tekrar Deneme

### Fail koşulu

* enerji biter
* ama beam hole’a ulaşmaz

### Fail deneyimi

* kısa bir başarısızlık anı
* net geri bildirim
* çok hızlı retry

Demo için retry akışının çok hızlı olması kritik önemdedir.

---

## 13. Level Tasarım Felsefesi

İyi bir level:

* tek bakışta anlaşılmalı
* çözümü hemen vermemeli
* yeni bir mekanik öğretmeli
* sonunda “tabii ya” dedirtmeli

### Öğretim sırası önerisi

1. **Level 1 – Basic Trigger**
2. **Level 2 – Reflection**
3. **Level 3 – Relay**
4. **Level 4 – Split**
5. **Level 5 – Destruction**
6. **Level 6 – Combined Showcase**

Game jam için önemli olan çok level yapmak değil, az ama akılda kalıcı level yapmaktır.

---

## 14. Mentor Demo Level’ı: “Frozen Cat Rescue”

### Kurulum

* 6x8 grid
* üst bölgede buz içinde hapsolmuş bir kedi
* kedinin önünde kırılması/eritilmesi gereken ince buz katmanları
* sağ altta yansıtıcı obje + çevresel patlama düğümü kümelenmesi
* ortada başlangıç oku
* oyuncuda 1 enerji

### Çözüm akışı

1. Oyuncu ortadaki aşağı bakan oku tetikler.
2. Enerji akışı aşağı ilerler.
3. Kristal splitter’a çarpar ve sağ-sol olarak ayrılır.
4. Sağdaki akış yansıtıcı yüzeyden sekerek patlama düğümünü tetikler.
5. Patlama, kalın engellerden birini yok eder ve yolu açar.
6. Soldaki akış başka bir arrow’u aktive eder.
7. Yeni akış açılan yoldan yukarı ilerler.
8. Kedi önündeki buz tabakası çatlar ve kırılır.
9. Son akış hedef hücreye ulaşır; kedi serbest kalır.
10. Güçlü clear feedback oynar.

### Bu level neden önemli?

Bu level, oyunun çekirdeğini tek akışta gösterirken aynı zamanda yeni temayı da görünür kılar:

* tetikleme
* yön değiştirme
* bölünme
* çevresel yıkım
* yol açılması
* buzun eritilmesi
* kedinin kurtarılması

Bu nedenle mentor sunumu için çok daha akılda kalıcı bir showcase level olur.

---

## 15. Görsel Stil ve Tematik Dünya

İlk teknik prototipin okunabilirliği için ana görsel dil yine:

* **High-contrast neon/vector readability**

Ancak oyunun **sunumsal ve tematik kimliği** artık daha net biçimde doğa merkezli, hafif absürt ve karakter odaklı bir yöne çekilmiştir.

### Yeni tematik çerçeve

Takımın konsepti, mekanik omurgayı daha akılda kalıcı bir dünya ile birleştiren şu fantezi etrafında okunabilir:

**Orman temalı bir dünyada, buz içinde kalmış bir kediyi kurtarmak için ışın zincirleri, yön blokları ve çevresel reaksiyonlar kullanılır.**

Bu fikir, mekanik sistemi tamamen değiştirmeden oyuna daha güçlü bir tema ve daha anlatılabilir bir hedef kazandırır.

### Tema ne kazandırıyor?

Bu yeni çerçeve sayesinde oyuncunun yaptığı şey artık sadece soyut bir beam puzzle çözmek değildir. Oyuncu:

* donmuş çevreyi etkiler
* yolu açar
* engelleri parçalar
* sonunda **buz içindeki kediyi eritip kurtarır**

Bu da oyuna üç önemli avantaj sağlar:

1. **Daha akılda kalıcı sunum**
2. **Daha güçlü görsel metafor**
3. **Daha sıcak bir final payoff**

### Mekanik–tema eşleşmesi

Aşağıdaki eşleşme önerilir:

* **Goal Hole / final target** → buz kütlesi içinde hapsolmuş kedi veya kedinin bulunduğu hedef nokta
* **Beam / pulse** → sıcak enerji, güneş ışını, büyülü akış veya orman ruhu enerjisi
* **Glass** → ince buz tabakası / kırılabilir don katmanı
* **Steel** → kalın kaya, donmuş gövde, sert kök bariyeri veya erimeyen blok
* **TNT** → tohum bombası, sıcak patlama mantarı, kor parçası veya çevresel kırılma tetikleyicisi
* **Mirror** → kristal taş, parlak buz yüzeyi, yaprak ayna, sihirli yansıtıcı obje
* **Prism / splitter** → kristal çiçek, ışık mantarı, büyülü taş veya orman kristali

Bu yaklaşım sayesinde soyut sistem korunur ama oyuncuya daha anlamlı bir sahne verilir.

### “Hole” kavramının yeniden yorumlanması

Önceki tasarımda hedef “Goal Hole” idi. Yeni konseptte bu doğrudan bir delik olmak zorunda değildir.

Daha güçlü bir tematik yorum:

* hedef artık **buzda hapsolmuş kedi** olabilir
* beam’in görevi deliğe ulaşmak değil, **son buz katmanını eritmek / kediyi serbest bırakmak** olabilir

Yani mekanik olarak hedef hücre aynı kalır; sadece görsel ve anlatısal yorumu değişir.

Bu çok önemli çünkü:

* kod mantığı sade kalır
* tema çok daha güçlü hale gelir
* mentor sunumunda fikir daha kolay akılda kalır

### Sunum cümlesi için öneri

Oyunu artık şöyle sunmak daha etkili olabilir:

**Vector Flow is a mobile puzzle game where players trigger chain reactions of light through a forest grid to melt the ice trapping a cat.**

Türkçesi:

**Vector Flow, oyuncunun orman içindeki grid üzerinde zincirleme ışık reaksiyonları başlatarak buzda kalmış bir kediyi kurtarmaya çalıştığı mobil bir puzzle oyunudur.**

### Görsel yaklaşım

Burada en iyi çözüm, iki katmanlı bir görsel yaklaşım olacaktır:

#### 1. Mekanik okunabilirlik katmanı

* net grid
* parlak beam
* okunabilir yön ikonları
* açık blok tipleri

#### 2. Tematik kaplama katmanı

* orman zemini
* yosunlu taşlar
* buz parçaları
* donmuş dallar
* küçük yaprak/ışık partikülleri
* finalde kedinin özgürleşme animasyonu

Böylece prototip karışmadan temalı hissedebilir.

### Renk dili önerisi

* arka plan: koyu orman tonları
* etkileşimli enerji: sıcak sarı / turuncu / altın veya büyülü cyan-yeşil
* buz elemanları: açık mavi / beyaz
* engeller: gri-kahverengi doğal tonlar
* kurtarma anı: sıcak parıltı + parçalanan buz

### Final payoff önerisi

Level clear anında:

* son buz tabakası çatlar
* kedi görünür hale gelir
* küçük bir zıplama / kaçış / miyav animasyonu oynar
* çevreye sıcak ışık yayılır

Bu an, “hole’a ulaştım” demekten çok daha etkili bir duygusal payoff üretir.

### Pratik geliştirme notu

Hızlı demo için bu temanın tamamını tam kaliteyle üretmek gerekmez. Öncelik:

* mekanik çalışsın
* hedefin buz içindeki kedi olduğu görsel olarak anlaşılsın
* birkaç tematik sprite/ikon ile atmosfer verilsin

Yani ilk prototipte tam sanat üretmek yerine, **mekanik üstüne oturtulmuş hafif ama net bir tematik skin** yeterlidir.

---

## 16. Juice ve Feedback Tasarımı

Bu oyunda feedback çok kritiktir. Çünkü oyuncu çoğu zaman zinciri izler.

### Beam

* parlak iz
* pulse hissi
* çarpınca parıltı

### Mirror

* küçük flash
* sekmede kıvılcım

### Prism

* split anında neon burst

### TNT

* ekran sarsıntısı
* radial patlama
* zincirleme hissi

### Glass

* çatlama + kırılma

### Hole

* içeri çekiliyormuş hissi
* final clear payoff

### Fail

* hafif sönme
* hızlı retry

---

## 17. UI / UX Tasarımı

Mobil portrait düzen için öneri:

### Üst bölüm

* level numarası
* enerji sayısı
* kısa hedef metni

### Orta bölüm

* ana grid alanı

### Alt bölüm

* retry
* gerekiyorsa next level
* basit kontrol alanı

Temel ilke: arayüz minimum, grid maksimum odaklı olmalıdır.

---

## 18. Skor Sistemi

İlk demo için skor sistemi şart değildir.

İstenirse basit bir yapı kurulabilir:

* level clear
* kalan enerji bonusu
* chain length bonusu
* tüm glass’ı kırma bonusu

Ama bu, hızlı demo için ikinci önceliktir.

---

## 19. Teknik Tasarım Öncelikleri

Kod yazımından önce netleşmiş olması gereken teknik omurga:

1. Grid veri yapısı
2. Hücre tipleri için enum / sabit tanımlar
3. Beam propagation mantığı
4. Etkileşim çözümleme sırası
5. Chain reaction event queue mantığı
6. Level veri formatı
7. Hızlı reset sistemi

### En önemli prensip

Tüm oyun deterministik ve veri odaklı olmalıdır.

### Unity için teknik yaklaşım

Artık odak, mentora gösterilecek geçici prototip değil; **Unity içinde sürdürülebilir bir çekirdek oyun sistemi** kurmaktır.

Bu nedenle önerilen Unity yaklaşımı şudur:

* tek bir ana gameplay scene
* grid yöneticisi (GridManager)
* hücre/blok mantığı için veri tabanlı yapı
* her blok tipi için prefab veya ortak base behaviour
* beam çözümleme için merkezi bir simulation/controller sistemi
* level verilerini sahneden bağımsız okuyabilen yapı
* hızlı restart ve next level akışı

### Unity mimarisi için önerilen yapı

#### Çekirdek sistemler

* **GridManager**: grid boyutu, hücre yerleşimi, koordinat-hücre eşlemesi
* **LevelLoader**: level verisini okuyup sahneyi kurma
* **BeamSystem / SimulationSystem**: beam ilerleme, split, reflection, trigger mantığı
* **GameStateManager**: idle, simulating, win, fail durumları
* **FXManager**: patlama, beam izi, shatter, screen shake, glow
* **UIManager**: enerji, retry, next level, level adı

#### Blok yapısı

Her blok için iki katmanlı düşünmek faydalı olur:

1. **Ga## 20. Onaylanmış Demo Sonrası Üretim Kapsamı

Mevcut demo hazırlanmış, gösterilmiş ve fikir onay almıştır. Bu yüzden bundan sonraki hedef, aynı hissi Unity içinde koruyarak oyunu gerçek üretim hattına taşımaktır.

### Artık korunması gereken şeyler

1. Grid tabanlı okunabilir yapı
2. Tek dokunuşla başlayan reaksiyon hissi
3. Zincirleme çözüm akışı
4. Yön değiştirme / split / çevresel yıkım kombinasyonu
5. Donmuş kediyi kurtarma tematik payoff’ı
6. Hızlı retry ve net win/fail akışı

### Unity sürümünde genişletilecek şeyler

* görsel kalite
* animasyon kalitesi
* daha temiz input sistemi
* level sayısı
* daha güçlü ses ve feedback
* daha iyi geçişler ve UI polish

### Şimdilik dikkat edilmesi gereken şey

Unity’ye geçerken fikri gereksiz yere büyütmek yerine, onay almış demonun netliğini ve okunabilirliğini korumak gerekir.

Yani hedef:
**aynı oyunu daha büyük yapmak değil, aynı iyi hissi daha sağlam ve daha güzel yapmak**.

## 21. Önerilen MVP Kapsamıtrait orientation

* büyük dokunma alanları
* okunaklı hücre boyutları
* düşük karmaşalı HUD
* efektlerin mobil performansı bozmayacak şekilde ayarlanması

### Demo ile Unity sürümü ilişkisi

Hazırlanan demo artık “atılacak geçici kod” mantığında değil, **kanıtlanmış oynanış referansı** olarak görülmelidir.

Yani bundan sonra yapılacak iş:

* onaylanan hissi korumak
* aynı çözüm netliğini Unity’ye taşımak
* üzerine polish, content ve production kalitesi eklemektir

## 20. Mentor Sunumu İçin Hızlı Demo Kapsamı

Bu aşamadaki hedef tam oyun değil, mentora 1–2 dakika içinde fikri anlatabilecek kadar net çalışan bir demo üretmektir.

### Demo neyi mutlaka göstermeli?

1. Grid tabanlı yapı
2. Oyuncunun bir arrow’a tıklayıp sistemi başlatması
3. Beam’in ilerlemesi
4. En az bir yön değiştirme veya split olayı
5. En az bir zincir reaksiyon anı
6. Goal Hole’a ulaşan başarılı çözüm
7. Hızlı retry / replay

### Demo neyi şimdilik göstermese de olur?

* tam menü sistemi
* ayrıntılı skor ekranı
* gelişmiş ses sistemi
* fazla level sayısı
* production art
* mobil package alma

### Hızlı demo için önerilen minimal içerik

* 1 showcase level
* 1 ikinci level veya varyasyon
* temel UI
* net win/fail durumu
* birkaç güçlü efekt

### Hedef

Mentorun şunu demesi yeterlidir:
**“Tamam, oyunun ne olduğunu anladım; fikir çalışıyor.”**

Bu nokta yakalandıktan sonra asıl üretim Unity içinde genişletilebilir.

---

## 21. Önerilen MVP Kapsamı

### Zorunlu

* 6x8 grid
* portre mobil görünüm
* Arrow Block tetikleme
* beam ilerleme sistemi
* Glass
* Steel
* Mirror
* Prism
* TNT
* Goal Hole
* win/fail kontrolü
* retry
* 2–5 level

### Olursa iyi olur

* chain counter
* hafif ses efektleri
* level geçiş animasyonu
* basit skor

### Şimdilik gereksiz

* procedural generation
* meta progression
* leaderboard
* skin sistemi
* çok fazla blok türü

---

## 22. En Büyük Riskler

1. Fazla sistem eklemek
2. Zayıf level design
3. Feedback’in yetersiz olması
4. Mobil okunabilirliğin bozulması
5. Mentor demosu için fazla büyük scope seçmek

---

## 23. Başarı Kriterleri

Bu prototipin başarılı sayılması için:

1. İlk 10 saniyede ne yapıldığı anlaşılmalı
2. İlk level hata vermeden oynanmalı
3. En az bir level güçlü wow anı üretmeli
4. Mobil portrede rahat görünmeli
5. Jüri/mentor karşısında açıklaması 30 saniyede yapılabilmeli
6. Retry ve next akışı pürüzsüz olmalı
7. Hızlı demo, asıl Unity projesine yön verecek kadar net içgörü üretmeli

---

## 24. Sunumda Kullanılabilecek Tanım

### Türkçe

Vector Flow, oyuncunun orman içindeki grid üzerinde zincirleme enerji reaksiyonları başlatarak buzda hapsolmuş bir kediyi kurtarmaya çalıştığı mobil odaklı bir puzzle oyunudur.

### English

Vector Flow is a mobile-first grid-based puzzle game where players trigger cascading energy reactions across a forest board to melt the ice trapping a cat.

### Daha kısa sunum cümlesi

**One tap starts a forest-wide chain reaction to rescue a frozen cat.**

---

## 25. Sonuç

Vector Flow, artık yalnızca güçlü bir game jam fikri değil, **demo ile doğrulanmış ve Unity içinde geliştirilecek net bir proje** konumundadır.

Bu aşamadan sonra doğru yaklaşım şudur:

* onay alan çekirdek hissi koru
* Unity içinde veri odaklı ve genişletilebilir temel sistemi kur
* orman + donmuş kedi temasını görsel olarak güçlendir
* level tasarımını ve juice’i kademeli olarak artır

En kritik ilke şudur:
**Demo neden çalıştıysa, Unity sürümü de o netliği kaybetmeden büyümelidir.**
