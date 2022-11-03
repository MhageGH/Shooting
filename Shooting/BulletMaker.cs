using System.Data;
using System.Numerics;

namespace Shooting
{
    /// <summary>
    /// 1種類の敵弾の集まりを作り出すクラス
    /// </summary>
    internal class BulletMaker
    {
        public int time;
        List<Bullet> bullets;
        int count = 0;
        float vib_angle = 0;

        public BulletMaker(List<Bullet> bullets)
        {
            this.bullets = bullets;
        }

        public void Make(Vector2 source_position, Vector2 target_position, int ID)
        {
            switch (ID)
            {
                case 0: // 3-way 固定
                    if (time % 10 == 0)
                    {
                        var dirs = new Vector2[] { new(0, 1), new(0.5f, 0.866f), new(-0.5f, 0.866f) };
                        for (int i = 0; i < dirs.Length; ++i)
                        {
                            var position = source_position + 30 * dirs[i];
                            var speed = 4 * dirs[i];
                            bullets.Add(new(0, 0, false, position, speed));
                        }
                    }
                    break;
                case 1:　//自機狙い×3
                    if (time % 2 == 0)
                    {
                        var offsets = new Vector2[] { new(0, 50), new(-50, 50), new(50, 50) };
                        foreach(var offset in offsets)
                        {
                            var position = source_position + offset;
                            var v = target_position - position;
                            var speed = 10 * v / v.Length();
                            bullets.Add(new(1, 0, true, position, speed));
                        }
                    }
                    break;
                case 2: // 放射状固定弾×4。一定速度で発射方向回転
                    if (time % 3 == 0)
                    {
                        const float pi = MathF.PI;
                        var angles = new float[] { pi * 5 / 12 + pi * time / 50, -pi * 5 / 12 - pi * time / 50, pi * 7 / 12 - pi * time / 50, -pi * 7 / 12 + pi * time / 50 };
                        foreach(var angle in angles)
                        {
                            var dir = new Vector2(MathF.Sin(angle), MathF.Cos(angle));
                            var position = source_position + 30 * dir;
                            var speed = 8 * dir;
                            bullets.Add(new(2, 0, true, position, speed));
                        }
                    }
                    break;
                case 3: // 放射状固定弾×24方向×3段 (墨染め要素)。等加速度直線運動
                    if (time % 200 == 0)
                    {
                        for (int i = 0; i < 24; ++i)
                        {
                            const float angle = 2 * MathF.PI / 24;
                            var dir = new Vector2(MathF.Sin(i * angle), MathF.Cos(i * angle));
                            var position = source_position + 20 * dir;
                            var vs = new float[] { 6, 8, 10 };
                            foreach (var v in vs) bullets.Add(new(3, 1, false, position, v * dir));
                        }
                    }
                    break;
                case 4: // 放射状固定弾×30方向×4段 (墨染め要素)。等加速度直線運動
                    if (time < 100) return;
                    if ((time - 100) % 200 == 0)
                    {
                        for (int i = 0; i < 30; ++i)
                        {
                            const float angle = 2 * MathF.PI / 30;
                            var dir = new Vector2(MathF.Sin(i * angle - MathF.PI), MathF.Cos(i * angle - MathF.PI));
                            var position = source_position + 50 * dir;
                            var vs = new float[] { 1, 2, 3, 4 };
                            foreach (var v in vs) bullets.Add(new(4, 1, false, position, v * dir));
                        }
                    }
                    break;
                case 5: // 放射状固定弾×40方向×4段 (墨染め要素)。等速直線運動
                    {
                        var waits = new int[] { 100, 105, 110, 115 };
                        if (time < waits[0]) return;
                        foreach (var wait in waits)
                        {
                            if ((time - wait) % 200 == 0)
                            {
                                for (int i = 0; i < 40; ++i)
                                {
                                    const float angle = 2 * MathF.PI / 40;
                                    var dir = new Vector2(MathF.Sin(i * angle - MathF.PI), MathF.Cos(i * angle - MathF.PI));
                                    var position = source_position + 50 * dir;
                                    bullets.Add(new(4, 0, false, position, 10 * dir));
                                }
                            }
                        }
                    }
                    break;
                case 6: // 放射状固定弾×30方向×3段 (墨染め要素)。等加速度直線運動
                    if (time < 105) return;
                    if ((time - 105) % 200 == 0)
                    {
                        for (int i = 0; i < 40; ++i)
                        {
                            const float angle = 2 * MathF.PI / 40;
                            var dir = new Vector2(MathF.Sin(i * angle - MathF.PI + angle / 2), MathF.Cos(i * angle - MathF.PI + angle / 2));
                            var position = source_position + 50 * dir;
                            var vs = new float[] { 1, 2, 3, 4 };
                            foreach (var v in vs) bullets.Add(new(5, 1, false, position, v * dir));
                        }
                    }
                    break;
                case 7: // 放射状固定弾×N方向 Nが徐々に増えていく (墨染め要素)。等速直線運動
                    if (time < 10) return;
                    if ((time - 10) % 100 == 0)
                    {
                        int n = (time - 10) / 100;
                        int N = 4 + 2 * n;
                        for (int i = 0; i < N; ++i)
                        {
                            float angle = 2 * MathF.PI / N;
                            var dir = new Vector2(MathF.Sin(i * angle - MathF.PI + angle / 2), MathF.Cos(i * angle - MathF.PI + angle / 2));
                            var position = source_position - 50 * dir;
                            bullets.Add(new(6, 0, true, position, 10 * dir));
                        }
                    }
                    break;
                case 8: // 自機狙い19-wayと自機外し20-wayを交互
                    if (time % 3 == 0)
                    {
                        int N = 20;
                        if (count >= 5) N += 1;
                        if (count >= 10) count = 0;
                        for (int i = 0; i < N; ++i)
                        {
                            var d = target_position - source_position;
                            var d1 = d / d.Length();
                            float angle = i * 2 * MathF.PI / N + ((N + 1) % 2) * MathF.PI / N;
                            float c = MathF.Cos(angle), s = MathF.Sin(angle);
                            var dir = new Vector2(d1.X * c - d1.Y * s, d1.X * s + d1.Y * c);
                            var position = source_position + 50 * dir;
                            bullets.Add(new(1, 0, true, position, 6 * dir));
                        }
                    }
                    break;
                case 9: // 画面上部矩形領域からランダム発射位置、ランダム速度下向き
                    if (time % 2 == 0)
                    {
                        var rand = new Random();
                        var position = new Vector2(
                            BackGround.position.X + BackGround.screen_size.Width * rand.NextSingle(),
                            BackGround.position.Y + 100 * rand.NextSingle());
                        var speed = new Vector2(0, 4 + 4 * rand.NextSingle());
                        bullets.Add(new(7, 0, true, position, speed));
                    }
                    break;
                case 10: // 上方向8way弾(弾源振動) (ケロちゃん要素)。重力加速度移動
                    if (time % 6 == 0)
                    {
                        vib_angle = (MathF.PI / 6) * MathF.Sin(2 * MathF.PI * time / 50);
                        for (int i = 0; i < 8; ++i)
                        {
                            float angle = 2 * MathF.PI / 3 + 2 * (MathF.PI / 3) * i / 8 + vib_angle;
                            var dir = new Vector2(MathF.Sin(angle), MathF.Cos(angle));
                            var position = source_position + 50 * dir;
                            bullets.Add(new(4, 2, true, position, 3 * dir));
                        }
                    }
                    break;
                case 11: // 上方向8way弾(弾源振動) (ケロちゃん要素)。重力加速度移動
                    if (time % 3 == 0)
                    {
                        var rand = new Random();
                        vib_angle = MathF.PI / 6 * rand.NextSingle();
                        for (int i = 0; i < 2; ++i)
                        {
                            float angle = 2 * MathF.PI / 3 + 2 * MathF.PI / 3 * i / 2 + vib_angle;
                            var dir = new Vector2(MathF.Sin(angle), MathF.Cos(angle));
                            var position = source_position + 50 * dir;
                            bullets.Add(new(7, 2, true, position, 2 * dir));
                        }
                    }
                    break;
            }
        }
    }
}
