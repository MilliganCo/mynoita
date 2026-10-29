# Noita Lite

> Public source preview: see [PUBLICATION.md](PUBLICATION.md) for actual checks, known failures and runtime limits.

Noita-подобная пиксельная симуляция на MonoGame с разделением на слой симуляции и слой рендеринга.

## Структура проекта

- `src/Sim` - Чистая логика симуляции без зависимостей от MonoGame
- `src/Game` - MonoGame слой для рендеринга и ввода

## Требования

- .NET SDK 8.0 или выше
- MonoGame 3.8.1 (устанавливается автоматически через NuGet)

## Установка .NET SDK

Если у вас не установлен .NET SDK, выполните один из вариантов:

### Вариант 1: Через официальный сайт (рекомендуется)
1. Скачайте .NET SDK 8.0 с [официального сайта](https://dotnet.microsoft.com/download)
2. Установите загруженный установщик
3. Перезапустите терминал

### Вариант 2: Через PowerShell (Windows)
Выполните команды по отдельности:

```powershell
# Скачать установщик
Invoke-WebRequest -Uri "https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.ps1" -OutFile "$env:TEMP\dotnet-install.ps1"

# Установить SDK (выполните отдельной командой)
& "$env:TEMP\dotnet-install.ps1" -Channel 8.0 -InstallDir "$env:ProgramFiles\dotnet"
```

Или одной строкой с точкой с запятой:
```powershell
Invoke-WebRequest -Uri "https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.ps1" -OutFile "$env:TEMP\dotnet-install.ps1"; & "$env:TEMP\dotnet-install.ps1" -Channel 8.0 -InstallDir "$env:ProgramFiles\dotnet"
```

### Проверка установки
После установки проверьте версию:
```bash
dotnet --version
```
Должна отобразиться версия 8.0.x или выше.

## Запуск

```bash
cd src/Game
dotnet restore
dotnet run
```

## Управление

### Игрок
- **A / ←** - Движение влево
- **D / →** - Движение вправо
- **W / ↑ / Пробел** - Прыжок

### Инструменты
- **ЛКМ** - Разместить Sand (песок)
- **ПКМ** - Разместить Water (вода)
- **ESC** - Выход

## Материалы

- **Sand** (песок) - Падает вниз, если ниже пусто, иначе падает по диагонали
- **Water** (вода) - Течёт вниз, если ниже пусто, иначе течёт в стороны
- **Stone** (камень) - Статичный материал
- **Fire** (огонь) - Живёт 60 тиков, испаряет воду в дым, затем превращается в дым
- **Smoke** (дым) - Поднимается вверх, исчезает через 120 тиков

## Особенности

- **Игрок** - Управляемый персонаж с физикой (гравитация, прыжки, коллизии)
- **Локация** - Генерируемый уровень с платформами и полом
- **Симуляция материалов** - Реалистичная физика для песка, воды, огня и дыма
- **Интерактивность** - Размещение материалов мышью

## Технические детали

- Размер сетки: 320×180 пикселей
- Масштаб отображения: x4 (итоговое разрешение 1280×720)
- Фиксированный timestep: 60 Hz
- Детерминированный RNG с seed
- Физика игрока: гравитация, трение, коллизии с твёрдыми материалами

## Public source status — 2026-10-29

See [PUBLICATION.md](PUBLICATION.md) for the current verification scope, prototype limitations and runtime boundaries. Earlier audit checklists describe historical work.
