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
                            bullets.Add(new Bullet(0, 0, false, position, speed));
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
                            bullets.Add(new Bullet(1, 0, true, position, speed));                            
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
                            bullets.Add(new Bullet(2, 0, true, position, speed));
                        }
                    }
                    break;
                case 3: // 放射状固定弾×24方向×3段 (墨染め要素)。等加速度直線運動
                    if (time % 200 == 0)
                    {
                    }
                    break;
                case 4: // 放射状固定弾×30方向×3段 (墨染め要素)。等加速度直線運動
                    if (time < 100) return;
                    if ((time - 100) % 200 == 0)
                    {
                    }
                    break;
                case 5: // 放射状固定弾×30方向×3段 (墨染め要素)。等速直線運動
                    {
                        var waits = new int[] { 100, 105, 110, 115 };
                        if (time < waits[0]) return;
                        foreach (var wait in waits)
                        {
                            if ((time - wait) % 200 == 0)
                            {

                            }
                        }
                    }
                    break;
                case 6: // 放射状固定弾×30方向×3段 (墨染め要素)。等加速度直線運動
                    if (time < 105) return;
                    if ((time - 105) % 200 == 0)
                    {
                    }
                    break;
                case 7: // 放射状固定弾×30方向×3段 (墨染め要素)。等速直線運動
                    if (time < 10) return;
                    if ((time - 10) % 100 == 0)
                    {
                    }
                    break;
                case 8: // 自機狙い19-wayと自機外し20-wayを交互
                    if (time % 3 == 0)
                    {
                    }
                    break;
                case 9: // 画面上部矩形領域からランダム発射位置、ランダム速度下向き
                    if (time % 2 == 0)
                    {
                    }
                    break;
                case 10: // 上方向8way弾(弾源振動) (ケロちゃん要素)。重力加速度移動
                    if (time % 2 == 0)
                    {
                    }
                    break;
                case 11: // 上方向8way弾(弾源振動) (ケロちゃん要素)。重力加速度移動
                    if (time % 2 == 0)
                    {
                    }
                    break;
            }
        }
    }
}
