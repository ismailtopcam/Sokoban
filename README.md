# Sokoban Game

## Proje Hakkında
Sokoban oyunu labirent içindeki bir bekçinin kutuları belirli hedef noktalara taşımasına dayanan bir oyundur. Bu proje, klasik Sokoban oyununa ek özellikler katılarak geliştirilmiş bir versiyondur.

## Özellikler
1. Temel Sokoban Özellikleri:
   - Bekçi kutuları iterek hareket ettirebilir
   - Kutular hedef noktalara yerleştirilmelidir
   - Bekçi duvarlardan ve kutulardan geçemez
   - Tüm kutular hedeflere yerleştirildiğinde seviye tamamlanır

2. Güç Geliştirmeleri:
   - **Çekme (Pull - P)**: Kutuları çekebilme özelliği
   - **Güçlü İtme (Strong Push - S)**: İki kutuyu aynı anda itebilme
   - **Koşma (Sprint - R)**: Her adımda iki kare hareket edebilme
   - **Fırlatma (Throw - T)**: Kutuları iki kare öteye fırlatabilme
   - **Kaykay (Skateboard - K)**: Engele kadar kayabilme
   - **Güçlü Yumruk (Strong Punch - F)**: Yıkılabilir duvarları kırabilme
  
3. Harita Sistemi:
   - Özel haritalar oluşturabilme ve yükleyebilme
   - .sok uzantılı harita dosyaları desteği
   - Haritaları menüden seçebilme
   - Özel hazırlanmış örnek haritalar
   - Farklı zorluk seviyelerinde haritalar

## Teknik Detaylar
- .NET 9 ile geliştirilmiştir
- WPF kullanılarak oluşturulmuştur
- Clean Architecture prensiplerine uygun tasarlanmıştır
- Repository Pattern kullanılmıştır

## Nasıl Oynanır?
1. **Temel Kontroller:**
   - ←↑→↓ (Yön Tuşları): Karakteri hareket ettirme
   - ESC: Oyundan çıkış
   - F5: Seviyeyi yeniden başlat

2. **Güç Kullanımı:**
   - P: Kutuları çekme
   - S: Güçlü itme
   - R: Koşma
   - T: Fırlatma
   - K: Kaykay
   - F: Güçlü yumruk

3. **Seviye Sembolleri:**
   ```
   # : Duvar
   @ : Bekçi
   $ : Kutu
   . : Hedef nokta
   B : Yıkılabilir duvar
   P : Çekme gücü
   S : Güçlü itme gücü
   R : Koşma gücü
   T : Fırlatma gücü
   K : Kaykay gücü
   F : Güçlü yumruk gücü
   ```

## Kurulum
1. Projeyi indirin
2. Projeyi Visual Studio 2022'de açın
3. Solution'ı derleyin
4. Sokoban.UI projesini çalıştırın

## Özel Harita Oluşturma
1. Yeni bir metin dosyası oluşturun
2. Yukarıdaki sembolleri kullanarak haritanızı tasarlayın
3. Dosyayı .sok uzantısı ile kaydedin, Levels klasörü altına taşıyın
4. Oyun içinden "Harita Yükle" seçeneği ile haritanızı yükleyin

Örnek harita formatı:
```
     #######
     #  #  #
######     #
#    #  #  #
#       #.##
##$######.##
#   $  @   #
#   ####   #
#####  #####

```

## Proje Yapısı
```
Sokoban/
├── src/
│   ├── Sokoban.Core/                # Domain Layer
│   │   ├── Entities/
│   │   │   ├── Player.cs
│   │   │   ├── Box.cs
│   │   │   ├── Wall.cs
│   │   │   ├── Target.cs
│   │   │   └── PowerUp.cs
│   │   ├── Enums/
│   │   │   ├── Direction.cs
│   │   │   ├── PowerUpType.cs
│   │   │   └── GameState.cs
│   │   └── Interfaces/
│   │       ├── IMovable.cs
│   │
│   ├── Sokoban.Application/         # Application Layer
│   │   ├── Services/
│   │   │   ├── GameService.cs
│   │   │   ├── MovementService.cs
│   │   │   └── PowerUpService.cs
│   │   ├── DTOs/
│   │   │   ├── GameStateDto.cs
│   │   │   └── LevelDto.cs
│   │   └── Interfaces/
│   │       ├── IGameService.cs
│   │       ├── IGameStateRepository.cs
│   │       ├── IMovementService.cs
│   │       ├── ILevelRepository.cs
│   │       └── IPowerUpService.cs
│   │
│   ├── Sokoban.Infrastructure/      # Infrastructure Layer
│   │   └── Repositories/
│   │       ├── LevelRepository.cs
│   │       └── GameStateRepository.cs
│   │
│   └── Sokoban.UI/                  # Presentation Layer
│       ├── Commands/                # Komut sınıfları
│       │   └── RelayCommand.cs
│       │
│       ├── Images/                  # Oyun görselleri
│       │   ├── player.png
│       │   ├── box.png
│       │   ├── wall.png
│       │   ├── target.png
│       │   ├── floor.png
│       │   └── powerup_*.png
│       │
│       ├── Levels/                  # Harita dosyaları
│       │   ├── level1.sok
│       │   ├── level2.sok
│       │   ├── level3.sok
│       │   ├── level4.sok
│       │   ├── level5.sok
│       │   ├── level6.sok
│       │   └── level7.sok
│       │
│       ├── Models/                  # UI model sınıfları
│       │   └── GameBoardCell.cs
│       │
│       ├── ViewModels/             # MVVM ViewModels
│       │   ├── PowerUpViewModel.cs
│       │   └── MainViewModel.cs
│       │
│       ├── App.xaml                # Uygulama giriş noktası
│       ├── App.xaml.cs
│       ├── MainWindow.xaml         # Ana pencere
│       └── MainWindow.xaml.cs
```

## Geliştirici Notları
- Her güç için önce **o gücün sembolünü** toplamanız gerekir
- Güçler aktif olduktan sonra ilgili tuşlarla kullanılabilir
- Yıkılabilir duvarlar (B) sadece güçlü yumruk ile kırılabilir
- Her seviye için hamle sayısı takibi yapılmaktadır
- Oyun süresi tutulmaktadır
- Haritalar Levels klasörü altında .sok uzantılı dosyalar olarak saklanır
- Kendi haritalarınızı oluşturup oyuna ekleyebilirsiniz
