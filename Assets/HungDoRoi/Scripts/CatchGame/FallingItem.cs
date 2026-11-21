using UnityEngine;

namespace MiniGames.CatchGame
{
    public enum ItemKind { Apple, Orange, Grape, GoldenApple, Bomb, Clock, Heart }

    /// <summary>
    /// Đồ vật rơi. Dùng Rigidbody2D với gravityScale để rơi bằng TRỌNG LỰC thật của Unity.
    /// Khi bị hứng hoặc chạm đất, đồ vật tự quyết định chuyện gì xảy ra (OnCaught / OnHitGround).
    /// </summary>
    public class FallingItem : MonoBehaviour
    {
        public ItemKind kind;
        FruitCatchGame game;
        Rigidbody2D rb;
        Transform visual;
        SpriteRenderer spark;
        float gravityFactor, wobble, t;
        bool handled;

        public bool IsFruit => kind == ItemKind.Apple || kind == ItemKind.Orange || kind == ItemKind.Grape || kind == ItemKind.GoldenApple;

        public int Points
        {
            get
            {
                switch (kind)
                {
                    case ItemKind.Apple: return 1;
                    case ItemKind.Orange: return 2;
                    case ItemKind.Grape: return 3;
                    case ItemKind.GoldenApple: return 10;
                    default: return 0;
                }
            }
        }

        public Color MainColor
        {
            get
            {
                switch (kind)
                {
                    case ItemKind.Apple: return Shape.Hex("#e8333d");
                    case ItemKind.Orange: return Shape.Hex("#ff9a1f");
                    case ItemKind.Grape: return Shape.Hex("#c8102e");
                    case ItemKind.GoldenApple: return Shape.Hex("#ffd23f");
                    case ItemKind.Bomb: return Shape.Hex("#22252b");
                    case ItemKind.Clock: return Shape.Hex("#3cc8f0");
                    default: return Shape.Hex("#ff5c8a");
                }
            }
        }

        public static FallingItem Create(Transform parent, Vector2 pos, ItemKind kind, FruitCatchGame game)
        {
            var go = Shape.Root(kind.ToString(), parent, pos);
            var visual = new GameObject("Visual").transform;
            visual.SetParent(go.transform, false);

            var item = go.AddComponent<FallingItem>();
            item.kind = kind;
            item.game = game;
            item.visual = visual;
            item.BuildVisual();

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.33f;
            col.isTrigger = true;

            item.rb = Phys.Body(go, game.BaseGravity * game.SlowMul);
            item.rb.SetVel(new Vector2(0f, -0.5f));
            item.gravityFactor = Random.Range(0.85f, 1.15f);
            item.wobble = Random.Range(-25f, 25f);
            game.items.Add(item);
            return item;
        }

        /// <summary>Tên ảnh thật của từng loại đồ rơi (đồng hồ chưa có ảnh nên vẫn vẽ bằng hình khối).</summary>
        string ArtName
        {
            get
            {
                switch (kind)
                {
                    case ItemKind.Apple: return "catch/apple";
                    case ItemKind.Orange: return "catch/orange";
                    case ItemKind.Grape: return "catch/cherries";
                    case ItemKind.GoldenApple: return "catch/golden_apple";
                    case ItemKind.Bomb: return "catch/bomb";
                    case ItemKind.Heart: return "catch/heart";
                    default: return null;
                }
            }
        }

        void BuildVisual()
        {
            var art = ArtName != null ? Art.Get(ArtName) : null;
            if (art != null)
            {
                switch (kind)
                {
                    case ItemKind.Orange:
                        Art.Add(visual, art, Vector2.zero, 0.42f, 5, 0f, "Fruit");
                        break;
                    case ItemKind.Grape:
                        Art.Add(visual, art, Vector2.zero, 0.74f, 5, 0f, "Fruit");
                        break;
                    case ItemKind.Bomb:
                        Art.Add(visual, art, Vector2.zero, 0.8f, 5, 0f, "Bomb");
                        // Tia lửa ở đầu ngòi nổ (ngòi nằm phía trên bên trái của ảnh)
                        spark = Shape.Add(visual, SpriteFactory.Star, Shape.Hex("#ffcc33"), new Vector2(-0.3f, 0.4f), Vector2.one * 0.22f, 7);
                        break;
                    case ItemKind.Heart:
                        Shape.Add(visual, SpriteFactory.Circle, new Color(1f, 0.6f, 0.75f, 0.35f), Vector2.zero, Vector2.one * 0.85f, 4);
                        Art.Add(visual, art, Vector2.zero, 0.55f, 5, 0f, "Heart");
                        break;
                    default: // táo, táo vàng
                        Art.Add(visual, art, Vector2.zero, 0.74f, 5, 0f, "Fruit");
                        if (kind == ItemKind.GoldenApple)
                            Shape.Add(visual, SpriteFactory.Star, new Color(1f, 1f, 0.8f, 0.9f), new Vector2(0.22f, -0.15f), Vector2.one * 0.28f, 7);
                        break;
                }
                return;
            }

            Color c = MainColor;
            Color white = new Color(1f, 1f, 1f, 0.45f);
            switch (kind)
            {
                case ItemKind.Apple:
                case ItemKind.GoldenApple:
                    Shape.Add(visual, SpriteFactory.Circle, c, Vector2.zero, new Vector2(0.72f, 0.66f), 5);
                    Shape.Add(visual, SpriteFactory.Square, Shape.Hex("#6b4226"), new Vector2(0f, 0.36f), new Vector2(0.06f, 0.18f), 4);
                    Shape.Add(visual, SpriteFactory.Circle, Shape.Hex("#4caf50"), new Vector2(0.13f, 0.4f), new Vector2(0.24f, 0.12f), 6, 30f);
                    Shape.Add(visual, SpriteFactory.Circle, white, new Vector2(-0.15f, 0.12f), new Vector2(0.14f, 0.2f), 6);
                    if (kind == ItemKind.GoldenApple)
                        Shape.Add(visual, SpriteFactory.Star, new Color(1f, 1f, 0.8f, 0.9f), new Vector2(0.22f, -0.15f), Vector2.one * 0.28f, 7);
                    break;
                case ItemKind.Orange:
                    Shape.Add(visual, SpriteFactory.Circle, c, Vector2.zero, Vector2.one * 0.68f, 5);
                    Shape.Add(visual, SpriteFactory.Circle, Shape.Hex("#3d8b3d"), new Vector2(0f, 0.33f), new Vector2(0.2f, 0.1f), 6);
                    Shape.Add(visual, SpriteFactory.Circle, white, new Vector2(-0.14f, 0.12f), new Vector2(0.14f, 0.18f), 6);
                    break;
                case ItemKind.Grape:
                    Vector2[] offs =
                    {
                        new Vector2(-0.2f, 0.18f), new Vector2(0f, 0.2f), new Vector2(0.2f, 0.18f),
                        new Vector2(-0.1f, 0f), new Vector2(0.1f, 0f), new Vector2(0f, -0.2f)
                    };
                    foreach (var o in offs) Shape.Add(visual, SpriteFactory.Circle, c, o, Vector2.one * 0.27f, 5);
                    Shape.Add(visual, SpriteFactory.Square, Shape.Hex("#6b4226"), new Vector2(0f, 0.38f), new Vector2(0.05f, 0.16f), 4);
                    break;
                case ItemKind.Bomb:
                    Shape.Add(visual, SpriteFactory.Circle, c, Vector2.zero, Vector2.one * 0.74f, 5);
                    Shape.Add(visual, SpriteFactory.Square, Shape.Hex("#888888"), new Vector2(0.12f, 0.4f), new Vector2(0.08f, 0.22f), 4, -30f);
                    Shape.Add(visual, SpriteFactory.Circle, white, new Vector2(-0.15f, 0.13f), new Vector2(0.12f, 0.16f), 6);
                    spark = Shape.Add(visual, SpriteFactory.Star, Shape.Hex("#ffcc33"), new Vector2(0.19f, 0.52f), Vector2.one * 0.22f, 7);
                    break;
                case ItemKind.Clock:
                    Shape.Add(visual, SpriteFactory.Circle, c, Vector2.zero, Vector2.one * 0.74f, 5);
                    Shape.Add(visual, SpriteFactory.Circle, Color.white, Vector2.zero, Vector2.one * 0.56f, 6);
                    Shape.Add(visual, SpriteFactory.Square, Color.black, new Vector2(0f, 0.1f), new Vector2(0.05f, 0.22f), 7);
                    Shape.Add(visual, SpriteFactory.Square, Color.black, new Vector2(0.07f, 0f), new Vector2(0.16f, 0.05f), 7);
                    Shape.Add(visual, SpriteFactory.Circle, c, new Vector2(0f, 0.42f), new Vector2(0.18f, 0.12f), 4);
                    break;
                case ItemKind.Heart:
                    Shape.Add(visual, SpriteFactory.Circle, new Color(1f, 0.6f, 0.75f, 0.35f), Vector2.zero, Vector2.one * 0.85f, 4);
                    Shape.Add(visual, SpriteFactory.Heart, c, Vector2.zero, Vector2.one * 0.62f, 5);
                    break;
            }
        }

        void FixedUpdate()
        {
            // Trọng lực thay đổi theo độ khó và hiệu ứng đồng hồ
            rb.gravityScale = game.BaseGravity * gravityFactor * game.SlowMul;
            Vector2 v = rb.GetVel();
            float maxFall = 11f * game.SlowMul;
            if (v.y < -maxFall) rb.SetVel(new Vector2(v.x, -maxFall));
        }

        void Update()
        {
            t += Time.deltaTime;
            visual.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * 3f) * wobble);
            if (spark != null) spark.transform.localScale = Vector3.one * (0.16f + Mathf.PingPong(t * 2f, 0.14f));
            if (transform.position.y < -game.HalfH - 2f) Destroy(gameObject);
        }

        public void HalveSpeed() => rb.SetVel(rb.GetVel() * 0.5f);

        /// <summary>Được giỏ hứng.</summary>
        public void OnCaught(Catcher catcher)
        {
            if (handled) return;
            handled = true;
            Vector2 p = transform.position;
            switch (kind)
            {
                case ItemKind.Bomb:
                    SoundFx.Play(Sfx.Bomb);
                    Fx.Burst(p, new Color(1f, 0.5f, 0.1f), 30, 6f, game.World, 0.7f, 0.25f);
                    game.ShowPopup(p, "BÙM!", new Color(1f, 0.4f, 0.3f));
                    catcher.Stun(0.6f);
                    game.LoseLife();
                    break;
                case ItemKind.Clock:
                    SoundFx.Play(Sfx.PowerUp);
                    game.ActivateSlow();
                    game.ShowPopup(p, "CHẬM LẠI!", new Color(0.3f, 0.9f, 1f));
                    Fx.Burst(p, MainColor, 12, 3f, game.World, 0.5f, 0.14f);
                    break;
                case ItemKind.Heart:
                    SoundFx.Play(Sfx.PowerUp);
                    game.GainLife(p);
                    Fx.Burst(p, MainColor, 12, 3f, game.World, 0.5f, 0.14f);
                    break;
                default:
                    SoundFx.Play(kind == ItemKind.GoldenApple ? Sfx.PowerUp : Sfx.Coin, 0.8f);
                    game.caughtCount++;
                    game.AddScore(Points, p);
                    Fx.Burst(p, MainColor, 8, 2.5f, game.World, 0.4f, 0.12f, 0f, false);
                    break;
            }
            Destroy(gameObject);
        }

        /// <summary>Rơi chạm đất.</summary>
        public void OnHitGround()
        {
            if (handled) return;
            handled = true;
            Vector2 p = transform.position;
            if (IsFruit)
            {
                SoundFx.Play(Sfx.Splat);
                game.AddScore(-1, p);
                Fx.Burst(p, MainColor, 10, 3f, game.World, 0.5f, 0.15f, 9f, false); // nước quả bắn tung toé
            }
            else
            {
                Fx.Burst(p, new Color(0.6f, 0.6f, 0.6f), 6, 1.5f, game.World, 0.35f, 0.15f, 0f, false);
            }
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            if (game != null) game.items.Remove(this);
        }
    }
}
