# Hứng Đồ Rơi

Game 2D viết hoàn toàn bằng C# cho Unity. Bấm **Play** là vào thẳng game, không cần kéo thả gì.
Nhân vật và cảnh nền dùng ảnh thật CC0 (xem `ART_CREDITS.md`), nằm trong `Resources/MiniGamesArt`.

## Mở project

1. Unity Hub → **Projects** → **Add** → **Add project from disk** → chọn thư mục `HungDoRoi`.
2. Mở project (lần đầu Unity tạo thư mục `Library`, mất vài phút).
3. Mở `Assets/Scenes/SampleScene` rồi bấm **Play** ▶.

## Điều khiển

| Game | Di chuyển | Hành động |
|---|---|---|
| Hứng Đồ Rơi | ← → hoặc A D | (chỉ di chuyển) |

**Esc / P**: tạm dừng. Khi thua hoặc thắng: **R / Enter** chơi lại, **Esc** thoát game.

## Đối tượng và tương tác

| Đối tượng | Lớp | Thành phần Unity | Vai trò |
|---|---|---|---|
| Người cầm giỏ | `Catcher` | Rigidbody2D, BoxCollider2D (Trigger, chỉ ở giỏ) | Chạy trái/phải để hứng đồ |
| Đồ rơi | `FallingItem` | Rigidbody2D (**có trọng lực**), CircleCollider2D (Trigger) | Táo +1, Cam +2, Anh đào +3, Táo vàng +10, Bom, Đồng hồ, Tim |
| Mặt đất | `Ground` | BoxCollider2D (Trigger) | Phát hiện đồ rơi chạm đất |

| Tương tác | Kết quả |
|---|---|
| Giỏ hứng quả | Cộng điểm theo loại quả |
| Giỏ hứng bom | Nổ, mất 1 mạng, người chơi bị choáng 0.6 giây |
| Giỏ hứng đồng hồ | Mọi đồ vật rơi chậm lại trong 5 giây |
| Giỏ hứng tim | +1 mạng |
| Quả rơi chạm đất | Trừ 1 điểm |
| Thời gian trôi | Trọng lực tăng, đồ rơi dày hơn, bom nhiều hơn |

## Hình ảnh

| Game | Nhân vật / đối tượng | Môi trường |
|---|---|---|
| Hứng Đồ Rơi | Nhân vật hồng (đứng, 2 khung bước, khung choáng) giơ giỏ; táo, cam, anh đào, táo vàng, bom, tim | Nền đồng cỏ, mặt trời, mây trôi, nền đất cỏ |

## Cấu trúc code

```
Scripts/
  Core/            Phần khung: MiniGameBase (điểm, mạng, HUD, tạm dừng), Launcher (tự khởi động game),
                   Art (nạp ảnh), SpriteFactory, SoundFx, Fx, UI, GameInput
  CatchGame/   Code riêng của game
```
