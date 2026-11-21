using UnityEngine;

namespace MiniGames.CatchGame
{
    /// <summary>Nhân vật giơ giỏ trên đầu, chạy qua lại để hứng đồ. Collider chỉ nằm ở cái giỏ.</summary>
    public class Catcher : MonoBehaviour
    {
        const float MaxSpeed = 9f;
        const float Accel = 60f;

        FruitCatchGame game;
        Rigidbody2D rb;
        Transform body, legL, legR, eyes;
        SpriteRenderer[] parts;
        Color[] baseColors;
        float vx, stunTimer, walkT;

        // Hình thật (nhân vật Kenney): đổi khung hình khi đứng / đi / bị choáng
        SpriteRenderer artBody;
        Sprite sStand, sWalk1, sWalk2, sHit;
        bool faceLeft;

        public static Catcher Create(Transform parent, Vector2 pos, FruitCatchGame game)
        {
            var go = Shape.Root("Catcher", parent, pos);
            var root = go.transform;
            var body = new GameObject("Body").transform;
            body.SetParent(root, false);

            Transform legL = null, legR = null, eyes = null;
            SpriteRenderer artBody = null;
            bool hasArt = Art.Has("catch/catcher_stand", "catch/catcher_walk1", "catch/catcher_walk2", "catch/catcher_hit");
            if (hasArt)
            {
                // Nhân vật Kenney, cao 1.15 đơn vị, chân chạm đất; hai tay giơ lên đỡ giỏ
                Color arm = Shape.Hex("#f19cb7");
                artBody = Art.Add(body, Art.Get("catch/catcher_stand"), new Vector2(0f, -0.06f), 1.15f, 12, 0f, "Alien");
                Shape.Add(body, SpriteFactory.Square, arm, new Vector2(-0.4f, 0.36f), new Vector2(0.11f, 0.56f), 11, -25f, "ArmL");
                Shape.Add(body, SpriteFactory.Square, arm, new Vector2(0.4f, 0.36f), new Vector2(0.11f, 0.56f), 11, 25f, "ArmR");
            }
            else
            {
                Color skin = Shape.Hex("#ffcf9e");
                legL = Shape.Add(body, SpriteFactory.Square, Shape.Hex("#33415c"), new Vector2(-0.14f, -0.5f), new Vector2(0.15f, 0.26f), 10, 0f, "LegL").transform;
                legR = Shape.Add(body, SpriteFactory.Square, Shape.Hex("#33415c"), new Vector2(0.14f, -0.5f), new Vector2(0.15f, 0.26f), 10, 0f, "LegR").transform;
                Shape.Add(body, SpriteFactory.Square, Shape.Hex("#e8505b"), new Vector2(0f, -0.18f), new Vector2(0.58f, 0.5f), 11, 0f, "Shirt");
                Shape.Add(body, SpriteFactory.Square, skin, new Vector2(-0.42f, 0.33f), new Vector2(0.11f, 0.62f), 10, -25f, "ArmL");
                Shape.Add(body, SpriteFactory.Square, skin, new Vector2(0.42f, 0.33f), new Vector2(0.11f, 0.62f), 10, 25f, "ArmR");
                Shape.Add(body, SpriteFactory.Circle, skin, new Vector2(0f, 0.27f), new Vector2(0.46f, 0.46f), 12, 0f, "Head");
                Shape.Add(body, SpriteFactory.Circle, Shape.Hex("#5a3a22"), new Vector2(0f, 0.42f), new Vector2(0.48f, 0.22f), 13, 0f, "Hair");
                eyes = new GameObject("Eyes").transform;
                eyes.SetParent(body, false);
                eyes.localPosition = new Vector3(0f, 0.25f, 0f);
                Shape.Add(eyes, SpriteFactory.Circle, Color.black, new Vector2(-0.08f, 0f), Vector2.one * 0.07f, 14);
                Shape.Add(eyes, SpriteFactory.Circle, Color.black, new Vector2(0.08f, 0f), Vector2.one * 0.07f, 14);
            }
            // Cái giỏ
            Shape.Add(body, SpriteFactory.Square, Shape.Hex("#a0652d"), new Vector2(0f, 0.8f), new Vector2(1.3f, 0.38f), 15, 0f, "Basket");
            Shape.Add(body, SpriteFactory.Square, Shape.Hex("#7a4a1e"), new Vector2(0f, 0.75f), new Vector2(1.3f, 0.06f), 16, 0f, "Stripe");
            Shape.Add(body, SpriteFactory.Square, Shape.Hex("#c98a4b"), new Vector2(0f, 0.99f), new Vector2(1.45f, 0.1f), 16, 0f, "Rim");

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.offset = new Vector2(0f, 0.86f);
            col.size = new Vector2(1.35f, 0.45f);

            var c = go.AddComponent<Catcher>();
            c.game = game;
            c.rb = Phys.Body(go);
            c.body = body;
            c.legL = legL;
            c.legR = legR;
            c.eyes = eyes;
            if (artBody != null)
            {
                c.artBody = artBody;
                c.sStand = Art.Get("catch/catcher_stand");
                c.sWalk1 = Art.Get("catch/catcher_walk1");
                c.sWalk2 = Art.Get("catch/catcher_walk2");
                c.sHit = Art.Get("catch/catcher_hit");
            }
            c.parts = go.GetComponentsInChildren<SpriteRenderer>();
            c.baseColors = new Color[c.parts.Length];
            for (int i = 0; i < c.parts.Length; i++) c.baseColors[i] = c.parts[i].color;
            return c;
        }

        void Update()
        {
            float dt = Time.deltaTime;
            float input = game.IsPlaying && stunTimer <= 0f ? GameInput.Horizontal() : 0f;
            vx = Mathf.MoveTowards(vx, input * MaxSpeed, Accel * dt);

            walkT += Mathf.Abs(vx) * dt * 2.2f;
            if (artBody != null)
            {
                // Đổi khung hình: choáng → hit, đang chạy → 2 khung bước chân, đứng yên → stand
                if (Mathf.Abs(input) > 0.01f) faceLeft = input < 0f;
                artBody.flipX = faceLeft;
                if (stunTimer > 0f) artBody.sprite = sHit;
                else if (Mathf.Abs(vx) > 0.5f) artBody.sprite = Mathf.Repeat(walkT, 1f) < 0.5f ? sWalk1 : sWalk2;
                else artBody.sprite = sStand;
            }
            else
            {
                // Mắt nhìn theo hướng chạy, chân bước khi di chuyển
                if (Mathf.Abs(input) > 0.01f) eyes.localPosition = new Vector3(0.06f * Mathf.Sign(input), 0.25f, 0f);
                float step = Mathf.Abs(vx) > 0.5f ? Mathf.Sin(walkT * Mathf.PI) * 0.06f : 0f;
                legL.localPosition = new Vector3(-0.14f, -0.5f + Mathf.Max(0f, step), 0f);
                legR.localPosition = new Vector3(0.14f, -0.5f + Mathf.Max(0f, -step), 0f);
            }

            if (stunTimer > 0f)
            {
                stunTimer -= dt;
                body.localPosition = new Vector3(Mathf.Sin(Time.time * 60f) * 0.04f, 0f, 0f); // rung lắc
                if (stunTimer <= 0f)
                {
                    body.localPosition = Vector3.zero;
                    for (int i = 0; i < parts.Length; i++) parts[i].color = baseColors[i];
                }
            }
        }

        void FixedUpdate()
        {
            Vector2 p = rb.position;
            p.x = Mathf.Clamp(p.x + vx * Time.fixedDeltaTime, -game.HalfW + 0.75f, game.HalfW - 0.75f);
            if (Mathf.Abs(p.x) >= game.HalfW - 0.75f) vx = 0f;
            rb.MovePosition(p);
        }

        /// <summary>Bị bom nổ: choáng một lúc, người đỏ lên.</summary>
        public void Stun(float time)
        {
            stunTimer = time;
            vx = 0f;
            foreach (var p in parts) p.color = Color.Lerp(p.color, Color.red, 0.5f);
        }

        // ===== TƯƠNG TÁC: giỏ hứng được đồ vật =====
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!game.IsPlaying) return;
            var item = other.GetComponent<FallingItem>();
            if (item != null) item.OnCaught(this);
        }
    }
}
