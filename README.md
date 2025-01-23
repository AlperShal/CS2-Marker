# CS2-Marker

## English

A marker or warden ring plugin for Counter-Strike 2 (CS2) made using CounterStrikeSharp (CSS/CS#).

### Dependencies
- Linux Server (Windows servers are not suppoerted.)

- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

### Demo Video

[YouTube: CS2-Marker Demo](https://youtu.be/TX_RxofSDfc)

### How To Install And Use?

1. Download latest release's zip file from [Releases](https://github.com/AlperShal/CS2-Marker/releases) and unzip.
2. When unzipped you will have a directory called `addons`. It's structure matches MetaMod and CounterStrikeSharp's directory structures. Copy the `addons` or any sub-directory of it to the relevant place in your server's file system. (For example copy contents of `addons` into the `addons` directory inside your server's file system.)
3. Connect to your server as a player or from terminal and open the console. Type `css_plugins load Marker` and plugin will be loaded. It will also automatically create the config file inside the `addons/counterstrikesharp/configs/plugins/Marker` directory.
4. [Optional] Edit the config file according to your needs. Reload the plugin with `css_plugins reload Marker`.

> [!NOTE]  
> List of color codes for `ChatPrefixColor`: [CounterStrikeSharp: ChatColors](https://docs.cssharp.dev/api/CounterStrikeSharp.API.Modules.Utils.ChatColors.html#fields)
>
> List of color codes for `PlaceMarker.Color`: [.NET: Color Struct](https://learn.microsoft.com/en-us/dotnet/api/system.drawing.color?view=net-8.0#properties)

5. [Optional] Create a language file (example for German: `de.json`) for your language. To do this create a copy of the `addons/counterstrikesharp/plugins/Marker/lang/en.json` or `tr.json` and rename to your language's code. Reload the plugin with `css_plugins reload Marker`.

> [!NOTE]  
> The language file being used is determined by CounsterStrikeSharp. Edit `addons/counterstrikesharp/configs/core.json` and change `ServerLanguage`'s value to your language's code (example for Turkish: `tr-TR`).
>
> If you are using other plugins with some other language but don't want to translate this one just make a copy of `en.json` or `tr.json` and rename to your language's code. The plugin will not be magically translated but at least it will show text from the language you have decided to make a copy of.

6. Type `!togglemarker [target]` (replace `[target]` with player name) to chat or `css_togglemarker [target]` to console. Follow the instructions printed. (Prints to chat or console, depending on which one you have used to type the command.)
7. To clear markers type `!clearmarkers` to chat or `css_clearmarkers` to console.
8. To take marker from someone type `!togglemarker [target]` to chat or `css_togglemarker [target]` to console again.
9. If you do not want to use the plugin no more type `css_plugins unload Marker` to console.

### Credits

- @KewaiiGamer | [CS2-DeathWatch](https://github.com/KewaiiGamer/CS2-DeathWatch) | Thanks for creating the logic of drawing a marker.
- @NapasP and others from CounterStrikeSharp community | Thanks for creating the RayTrace class.
- @AquaVadis | Thanks for guiding and helping me to make my first CounterStrikeSharp and C# project.

## Türkçe

Counter-Strike 2 (CS2) için CounterStrikeSharp ile yapılmış bir marker ya da komutçu çemberi eklentisi.

### Bağımlılıklar
- Linux Sunucu (Windows sunucular desteklenmemektedir.)

- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

### Demo Videosu

[YouTube: CS2-Marker Demosu](https://youtu.be/TX_RxofSDfc)

### Nasıl Kurulur ve Kullanılır?

1. [Releases](https://github.com/AlperShal/CS2-Marker/releases)'dan sonra sürümün zip dosyasını indirin ve zipten çıkarın.
2. Zipten çıkardığınızda `addons` isimli bir dizin elde edeceksiniz. Bu dizinin yapısı CounterStrikeSharp ve MetaMod'un yapısıyla birebir uyuşuyor. `addons`'u ya da herhangi bir alt dizinini sunucunuzun dosya sistemi içerisindeki ilgili yere kopyalayın. (Örneğin `addons` dizinin içeriğini sunucunuzdaki `addons` dizininin içine kopyalayın.)
3. Sunucunuza ister oyuncu olarak isterseniz terminalden bağlanın ve konsolu açın. `css_plugins load Marker` yazarak eklentiyi yükleyin. Eklenti yüklendiğinde otomatik olarak `addons/counterstrikesharp/configs/plugins/Marker` dizininde konfigürasyon dosyası da oluşacak.
4. [İsteğe Bağlı] İsteklerinize göre konfigürasyon dosyasını düzenleyin. Eklentiyi `css_plugins reload Marker` komutuyla yeniden yükleyin.

> [!NOTE]  
> `ChatPrefixColor` için renk isimleri: [CounterStrikeSharp: ChatColors](https://docs.cssharp.dev/api/CounterStrikeSharp.API.Modules.Utils.ChatColors.html#fields)
>
> `PlaceMarker.Color` için renk isimleri: [.NET: Color Struct](https://learn.microsoft.com/en-us/dotnet/api/system.drawing.color?view=net-8.0#properties)

5. [İsteğe Bağlı] Bu rehberi Türkçe okuyan birinin yeni bir çeviri dosyası oluşturmaya ihtiyacı olacağını düşünmüyorum o yüzden bu adımı geçiyorum.
6. Sohbete `!togglemarker [hedef]` (`[hedef]`'i oyuncu ismiyle değiştirin) ya da konsole `css_togglemarker [hedef]` yazın. Ekranda çıkan talimatları takip edin (Komutu nerede çalıştırdığınıza göre talimatlar sohbette ya da konsolda çıkar.)
7. Markerları silmek için sohbete `!clearmarkers` ya da konsola `css_clearmarkers` yazın.
8. Birinden marker almak için sohbete `!togglemarker [hedef]` ya da konsola `css_togglemarker [hedef]` yazın.
9. Tekrar eklentiyi kullanmak istemezseniz terminale `css_plugins unload Marker` yazarak eklentinin yüklemesini geri alın.

### Katkıda Bulunanlar

- @KewaiiGamer | [CS2-DeathWatch](https://github.com/KewaiiGamer/CS2-DeathWatch) | Marker oluşturmanın mantığını yazdığı için teşekkürler!
- @NapasP ve CounterStrikeSharp topluluğundan diğerleri. | RayTrace class'ını oluşturduğunuz için teşekkürler!
- @AquaVadis | Bana ilk C# ve CounterStrikeSharp projemi yapmamda rehberlik ettiğin ve yardımcı olduğun için teşekkürler!
