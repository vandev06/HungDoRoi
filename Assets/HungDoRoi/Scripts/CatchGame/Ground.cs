using UnityEngine;

namespace MiniGames.CatchGame
{
    /// <summary>Mặt đất: vùng Trigger ở đáy màn hình, báo cho đồ vật biết khi chúng rơi chạm đất.</summary>
    public class Ground : MonoBehaviour
    {
        FruitCatchGame game;

        public static void Create(Transform parent, FruitCatchGame game)
        {
            float w = game.HalfW * 2f + 4f;
            var go = Shape.Root("Ground", parent, new Vector2(0f, -game.HalfH + 0.35f));
            var grass = Art.Get("platformer/tiles_grass_mid");
            if (grass != null)
            {
                // Dải đất cỏ lát gạch (mỗi viên 0.7 x 0.7)
                Art.AddTiled(go.transform, grass, Vector2.zero, new Vector2(w, 0.7f), 0.7f, 0, 0f, "GrassTiles");
            }
            else
            {
                Shape.Add(go.transform, SpriteFactory.Square, Shape.Hex("#8b5a2b"), Vector2.zero, new Vector2(w, 0.7f), 0);
                Shape.Add(go.transform, SpriteFactory.Square, Shape.Hex("#5cbf3a"), new Vector2(0f, 0.3f), new Vector2(w, 0.18f), 1);
            }

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(w, 0.6f);
            col.offset = new Vector2(0f, -0.05f);
            go.AddComponent<Ground>().game = game;
        }

        // ===== TƯƠNG TÁC: đồ vật rơi chạm đất =====
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!game.IsPlaying) return;
            var item = other.GetComponent<FallingItem>();
            if (item != null) item.OnHitGround();
        }
    }
}
