using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.CatchGame
{
    /// <summary>
    /// GAME 2 – HỨNG ĐỒ RƠI
    /// Đồ vật rơi tự do bằng trọng lực của Rigidbody2D. Càng chơi lâu, trọng lực càng mạnh
    /// và đồ rơi càng dày.
    ///
    /// Đối tượng: Catcher (người cầm giỏ), FallingItem (táo, cam, anh đào, táo vàng, bom, đồng hồ, tim), Ground
    /// Tương tác:
    ///   Giỏ hứng quả      → cộng điểm theo loại quả            (Catcher → FallingItem.OnCaught)
    ///   Giỏ hứng bom      → nổ, mất 1 mạng, người chơi bị choáng
    ///   Giỏ hứng đồng hồ  → mọi thứ rơi chậm lại trong 5 giây
    ///   Giỏ hứng tim      → +1 mạng
    ///   Quả rơi chạm đất  → trừ 1 điểm                          (Ground → FallingItem.OnHitGround)
    /// </summary>
    public class FruitCatchGame : MiniGameBase
    {
        public override string Title => "Hứng Đồ Rơi";
        protected override string SaveKey => "catch";
        protected override Color Background => Shape.Hex("#8fd3ff");
        protected override string HelpText =>
            "← → hoặc A D: chạy qua lại để hứng đồ bằng giỏ\n\n" +
            "<color=#ff6666>Táo</color> +1   <color=#ffaa33>Cam</color> +2   <color=#ff4d6d>Anh đào</color> +3   <color=#ffe066>Táo vàng</color> +10\n" +
            "<color=#ff5555>Bom</color>: mất 1 mạng!   <color=#66e0ff>Đồng hồ</color>: làm chậm 5 giây   <color=#ff77aa>♥</color>: +1 mạng\n" +
            "Để quả rơi xuống đất sẽ bị trừ 1 điểm.\n" +
            "Càng lâu đồ rơi càng nhanh và càng nhiều.";

        public Catcher catcher;
        public readonly List<FallingItem> items = new List<FallingItem>();
        public int caughtCount;

        float elapsed;
        float spawnTimer;
        float slowTimer;

        /// <summary>Hệ số làm chậm khi nhặt đồng hồ.</summary>
        public float SlowMul => slowTimer > 0f ? 0.4f : 1f;
        /// <summary>Trọng lực tăng dần theo thời gian chơi.</summary>
        public float BaseGravity => Mathf.Min(1.3f, 0.3f + elapsed * 0.008f);
        public float GroundTop => -halfH + 0.7f;

        protected override void BuildWorld()
        {
            items.Clear();
            elapsed = 0f;
            spawnTimer = 1f;
            slowTimer = 0f;
            caughtCount = 0;

            Scenery.Create(world, this);
            Ground.Create(world, this);
            catcher = Catcher.Create(world, new Vector2(0f, GroundTop + 0.62f), this);
        }

        protected override void Tick()
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            if (slowTimer > 0f) slowTimer -= dt;

            spawnTimer -= dt * SlowMul;
            if (spawnTimer <= 0f)
            {
                SpawnItem();
                spawnTimer = Mathf.Max(0.35f, 1.0f - elapsed * 0.012f) * Random.Range(0.75f, 1.25f);
            }
        }

        void SpawnItem()
        {
            float bombChance = Mathf.Min(0.3f, 0.12f + elapsed * 0.002f);
            float r = Random.value;
            ItemKind kind;
            if (r < bombChance) kind = ItemKind.Bomb;
            else if (r < bombChance + 0.04f) kind = ItemKind.Clock;
            else if (r < bombChance + 0.065f && lives < 5) kind = ItemKind.Heart;
            else
            {
                float f = Random.value;
                kind = f < 0.5f ? ItemKind.Apple : f < 0.8f ? ItemKind.Orange : f < 0.95f ? ItemKind.Grape : ItemKind.GoldenApple;
            }
            var pos = new Vector2(Random.Range(-halfW + 0.6f, halfW - 0.6f), halfH + 0.8f);
            FallingItem.Create(world, pos, kind, this);
        }

        /// <summary>Kích hoạt hiệu ứng đồng hồ: mọi đồ vật đang rơi chậm lại.</summary>
        public void ActivateSlow()
        {
            if (slowTimer <= 0f)
                foreach (var it in items) it.HalveSpeed();
            slowTimer = 5f;
        }

        protected override void DrawHud()
        {
            UI.Text(new Rect(0, 14, UI.W, 40), "Đã hứng: " + caughtCount, 26, Color.white, TextAnchor.UpperCenter);
            if (slowTimer > 0f)
                UI.Text(new Rect(0, 50, UI.W, 30), "Chậm lại: " + Mathf.CeilToInt(slowTimer) + "s", 22, new Color(0.2f, 0.4f, 0.9f), TextAnchor.UpperCenter, false);
        }
    }
}
