using UnityEngine;

namespace MiniGames.CatchGame
{
    /// <summary>Cảnh nền trang trí: mặt trời, đồi, mây trôi. Không có va chạm.</summary>
    public class Scenery : MonoBehaviour
    {
        MiniGameBase game;
        Transform[] clouds;
        float[] speeds;

        public static void Create(Transform parent, MiniGameBase game)
        {
            var go = new GameObject("Scenery");
            go.transform.SetParent(parent, false);
            var s = go.AddComponent<Scenery>();
            s.game = game;
            var t = go.transform;
            float hw = game.HalfW, hh = game.HalfH;

            Shape.Add(t, SpriteFactory.Circle, new Color(1f, 0.95f, 0.5f, 0.35f), new Vector2(hw - 1.6f, hh - 1.4f), Vector2.one * 2.2f, -20);
            Shape.Add(t, SpriteFactory.Circle, Shape.Hex("#ffe45c"), new Vector2(hw - 1.6f, hh - 1.4f), Vector2.one * 1.4f, -19);

            var bg = Art.Get("platformer/bg_grass");
            if (bg != null)
            {
                // Ảnh nền đồng cỏ (trời, mây xa, đồi) phủ kín màn hình
                Art.AddTiled(t, bg, Vector2.zero, new Vector2(hw * 2f + 2f, hh * 2f), hh * 2f, -25, 0f, "Background");
            }
            else
            {
                Shape.Add(t, SpriteFactory.Circle, Shape.Hex("#79c66a"), new Vector2(-hw * 0.6f, -hh + 0.2f), new Vector2(7f, 3.6f), -15);
                Shape.Add(t, SpriteFactory.Circle, Shape.Hex("#6ab85c"), new Vector2(hw * 0.5f, -hh + 0.1f), new Vector2(8f, 3f), -14);
            }

            s.clouds = new Transform[4];
            s.speeds = new float[4];
            for (int i = 0; i < 4; i++)
            {
                var cloud = new GameObject("Cloud").transform;
                cloud.SetParent(t, false);
                cloud.localPosition = new Vector3(Random.Range(-hw, hw), Random.Range(hh * 0.1f, hh - 0.8f), 0f);
                var white = new Color(1f, 1f, 1f, 0.85f);
                Shape.Add(cloud, SpriteFactory.Circle, white, Vector2.zero, new Vector2(1.4f, 0.8f), -12);
                Shape.Add(cloud, SpriteFactory.Circle, white, new Vector2(0.6f, 0.15f), new Vector2(1f, 0.8f), -12);
                Shape.Add(cloud, SpriteFactory.Circle, white, new Vector2(-0.6f, -0.05f), new Vector2(0.9f, 0.6f), -12);
                s.clouds[i] = cloud;
                s.speeds[i] = Random.Range(0.2f, 0.6f);
            }
        }

        void Update()
        {
            for (int i = 0; i < clouds.Length; i++)
            {
                var p = clouds[i].localPosition;
                p.x += speeds[i] * Time.deltaTime;
                if (p.x > game.HalfW + 2f) p.x = -game.HalfW - 2f;
                clouds[i].localPosition = p;
            }
        }
    }
}
