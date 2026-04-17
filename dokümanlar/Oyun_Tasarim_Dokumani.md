# Vector Flow – Detaylı Oyun Tasarım Dokümanı

## 1. Proje Özeti

**Vector Flow**, portre modda oynanan, mobil odaklı, grid tabanlı bir puzzle oyunudur. Oyuncu her level’da sınırlı sayıda hamle hakkı ile bir veya daha fazla **Arrow Block** tetikler. Bu tetikleme sonucunda ortaya çıkan ışınlar (beam/pulse), grid üzerinde ilerleyerek aynalar, splitter’lar, TNT, kırılabilir engeller ve diğer yönlendirici yapılarla etkileşime girer. Amaç, en verimli zincir reaksiyonu kurarak ışını hedefe ulaştırmak veya level hedeflerini tamamlamaktır.

Oyunun temel gücü, **az oyuncu girdisi ile büyük ve tatmin edici sonuçlar üretmesi**dir. Oyuncu uzun bir manuel aksiyon zinciri gerçekleştirmez; bunun yerine doğru başlangıç kararını verir ve sistemin tetiklediği reaksiyonu izler. Bu nedenle oyun, hem stratejik planlama hem de görsel tatmin üretir.

Bu oyunun **nihai üretim hedefi Unity tabanlı bir sürüm**dür. Ancak mevcut ihtiyaç, mentora fikri çok hızlı gösterebilecek, kısa sürede ayağa kaldırılabilecek bir **demo / vertical slice** üretmektir. Bu yüzden ilk aşamada teknoloji seçimi “uzun vadede en doğru olan”a göre değil, **en hızlı prototipin en az eforla çıkarılabileceği** yaklaşıma göre yapılacaktır.

Bu bağlamda hızlı demo için şu ilke benimsenir:

* Asıl proje Unity’de geliştirilecektir.
* Ancak mentor sunumu için gerekirse **HTML/CSS/JavaScript tabanlı**, tek dosyada veya çok küçük dosya yapısında çalışan, hızlı derlenebilen/pratikçe ayağa kaldırılabilen bir prototip tercih edilebilir.
* Bu demo, son ürünün teknolojik temeli olmak zorunda değildir; amacı yalnızca çekirdek fikri, zincir reaksiyon hissini ve temel level mantığını görünür kılmaktır.

Yani burada üretilen ilk demo, “final architecture” değil, **fikir doğrulama ve hızlı sunum aracı** olarak değerlendirilmelidir.

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
* **Nihai geliştirme hedefi:** Unity
* **Hızlı demo hedefi:** Mentora kısa sürede gösterilecek oynanabilir prototip
* **Demo için teknoloji yaklaşımı:** En pratik ve en hızlı compile/deploy edilebilen sistem
* **Sunum kimliği:** “Small input, big payoff”
* **Tematik omurga:** Arrow / Vector + Hole + Block

### Teknoloji stratejisi

Bu proje iki katmanda düşünülmelidir:

1. **Gerçek proje katmanı**

   * Oyunun asıl versiyonu Unity ile geliştirilecektir.
   * Level yapısı, efektler, polish, mobil packaging ve genişleme burada olacaktır.

2. **Hızlı demo katmanı**

   * Mentor gösterimi için en hızlı şekilde çalışan çekirdek prototip çıkarılacaktır.
   * Bu katmanda hedef, teknolojik doğruluk değil, **mekaniğin görünür ve oynanabilir olması**dır.
   * Bu nedenle Unity yerine daha pratik bir seçenek kullanılabilir.

### Hızlı demo için önerilen yaklaşım

En mantıklı hızlı demo yaklaşımı:

* basit grid render
* tıklama/tap algılama
* beam propagation
* birkaç blok tipi
* reset / next level

Bunu en kısa sürede üretmek için en uygun yaklaşım genellikle:

* **HTML + JavaScript + Canvas**
  veya
* çok hafif bir 2D framework

olacaktır.

Sebep:

* kurulum yükü düşüktür
* export süreciyle vakit kaybedilmez
* tek tarayıcı penceresinde hemen gösterilebilir
* mentor sunumu için yeterince hızlı iterate edilir
* oyunun asıl mantığı olan grid, raycast benzeri beam çözümü ve event zinciri çok rahat prototiplenir

Bu yüzden bu dokümanda alınan tasarım kararları, özellikle **hızlı prototipleme dostu** olacak şekilde sade tutulmuştur.

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

### 9.4 Steel Wall / Pillar

* Beam’i durdurur.
* Normal beam ile yok olmaz.
* TNT ile kırılabilir.

### 9.5 Mirror

* `/` ve `\` tipi iki temel ayna düşünülür.
* Beam’i geliş yönüne göre sektirir.

### 9.6 Prism / Splitter

* Beam’i iki yöne böler.
* İlk demo için ikiye ayrılma yeterlidir.

### 9.7 TNT

* Beam ile tetiklenince patlar.
* **3x3 alan** etkiler.
* Diğer TNT’leri zincirleme tetikleyebilir.

### 9.8 Goal Hole

* Ana hedeftir.
* Beam hole’a girerse level clear olur.

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

## 14. Mentor Demo Level’ı: “The Splitter”

### Kurulum

* 6x8 grid
* sol üstte camla kapalı Goal Hole
* sağ altta mirror + TNT kümelenmesi
* ortada başlangıç oku
* oyuncuda 1 enerji

### Çözüm akışı

1. Oyuncu ortadaki aşağı bakan oku tetikler.
2. Beam aşağı ilerler.
3. Prism’e çarpar ve sağ-sol olarak ayrılır.
4. Sağdaki beam aynadan sekerek TNT’yi tetikler.
5. TNT bir steel pillar’ı yok eder.
6. Soldaki beam başka bir arrow’u aktive eder.
7. Yeni beam açılan yoldan yukarı ilerler.
8. Glass kırılır.
9. Beam Goal Hole’a girer.
10. Güçlü clear feedback oynar.

Bu level, oyunun çekirdeğini tek akışta göstermesi açısından sunum için çok değerlidir.

---

## 15. Görsel Stil Kararı

İlk demo için önerilen ana görsel dil:

* **High-contrast neon vector**

### Neden?

* Arrow / Vector temasına doğrudan oturur
* az asset ile güçlü görünür
* beam ve split efektleri daha iyi parlar
* hızlı prototiplemeye uygundur
* mobilde okunaklıdır

Voxel hissi istersek bunu sadece debris / kırılma / patlama detaylarında yardımcı dokunuş olarak kullanabiliriz.

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

### Hızlı demo için teknik yaklaşım

Bu aşamada teknik öncelik, üretim kalitesinde mimari kurmak değil; **mentora oynatılabilir ve anlaşılır bir örnek çıkarmaktır**.

Bu nedenle hızlı demo için önerilen teknik yaklaşım:

* tek sahne / tek ekran yapı
* 2D grid render
* veri tabanlı hücreler
* beam’in adım adım simülasyonu
* olayların sırayla işlenmesi
* level’ların sade veri ile tanımlanması

### Pratik teknoloji tercihi notu

Projenin asıl sürümü Unity’de yapılacak olsa da, bu demo için en verimli yaklaşım büyük ihtimalle:

* **HTML / CSS / JavaScript Canvas prototipi**
* mümkünse küçük dosya yapısı
* minimum dış bağımlılık

Bu sayede:

* birkaç saat içinde oynanabilir sürüm çıkarılabilir
* build/export yükü azalır
* ekip mekanik doğrulamasına odaklanır
* mentor sunumu hızlı yapılır

### Demo ile final ürün arasındaki ilişki

Bu hızlı demo, bir **fikir doğrulama prototipi**dir.

Yani:

* amaç final kodu yazmak değil
* mekanik ispatı üretmek
* mentor geri bildirimi almak
* Unity’de yapılacak gerçek sürüm için referans oluşturmaktır

---

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

Vector Flow, tek dokunuşla başlayan zincirleme ışın reaksiyonları üzerinden hedef deliğe ulaşmayı amaçlayan, mobil odaklı grid tabanlı bir puzzle oyunudur.

### English

Vector Flow is a mobile-first grid-based puzzle game where a single tap triggers a cascading beam reaction through arrows, mirrors, splitters, and explosives to reach the goal hole.

---

## 25. Sonuç

Vector Flow, game jam bağlamında güçlü bir fikirdir çünkü hem tema ile uyumludur hem de kısa sürede görsel olarak etkileyici bir prototipe dönüştürülebilir. Ancak şu an öncelik, oyunun tam sürümünü inşa etmek değil; mentora çok hızlı biçimde çalışır bir örnek gösterebilmektir.

Bu yüzden önerilen yol şudur:

* önce en pratik teknolojiyle hızlı demo üret
* çekirdek mekaniği göster
* mentor geri bildirimi al
* sonra gerçek üretimi Unity üzerinde genişlet

Bu yaklaşım hem zaman açısından güvenlidir hem de fikir doğrulama sürecini hızlandırır.
