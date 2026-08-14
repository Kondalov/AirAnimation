# AirAnimation — Travel Route Animator

Windows 11 аналог TravelBoast. Создайте красивую анимацию маршрута и экспортируйте в MP4.

## Технологии
- WPF + .NET 10
- MapLibre GL JS 5.6 (через WebView2)
- OpenStreetMap тайлы
- OSRM маршрутизация
- FFMpegCore для видеоэкспорта

## 50+ транспортов
Авто, Авиация, Космос, Море, Ж/д, Мото, Экзотика, Военные, Фантастика

## Использование
1. Открыть в VS 2026
2. F5 — запустить
3. Нажимать на карту для добавления точек
4. Выбрать транспорт во вкладке 🚗 Транспорт
5. Нажать ▶ Play для анимации
6. Экспорт MP4 требует ffmpeg.exe в папке `/ffmpeg/`

## FFmpeg (для экспорта видео)
Скачайте с https://ffmpeg.org/download.html и поместите `ffmpeg.exe` в:
```
AirAnimation.App/bin/Release/net10.0-windows/ffmpeg/ffmpeg.exe
```

## Карты
- Тёмная тема: OpenStreetMap (без API ключа)
- Спутник: Esri World Imagery (без API ключа)
- Для MapTiler premium стилей: добавьте ключ в настройки
