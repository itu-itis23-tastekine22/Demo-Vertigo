# Vertigo Wheel – Game Developer Demo

A wheel of fortune game made for the Vertigo Games developer demo. Spin the wheel to collect rewards zone by zone, but watch out for the bomb: it takes everything you've collected. Every 5th zone is a safe silver spin, every 30th zone is a golden spin with special rewards, and you can leave with your rewards at safe or super zones.

## Download & Video
- **APK:** see [Releases](../../releases)
- **Gameplay video:** [Google Drive](https://drive.google.com/file/d/1D_XlJKJ7pLpZfadUqPMQ_0oUjaDy1PxA/view?usp=share_link)

## Tech
- Unity 2021.3.45f1 (LTS)
- ScriptableObjects, DOTween, TextMeshPro

## Architecture
- **Data** – ScriptableObjects editable from the editor: `RewardData`, `WheelConfig`, `ZoneSettings`
- **Core** – plain C# game logic, independent of Unity UI: `GameSession`, `RewardInventory`, `SpinResolver` (randomness injected via `IRandomProvider` for testability)
- **Views** – UI only, no game rules: `WheelView`, `ZoneBarView`, `RewardsPanelView`, `BombPopupView`
- **Controllers** – `GameController` connects logic and views through events

