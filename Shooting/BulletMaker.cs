using System.Data;
using System.Numerics;

namespace Shooting
{
    /// <summary>
    /// 1種類の敵弾の集まりを作り出すクラス
    /// </summary>
    internal class BulletMaker
    {
        int time;
        readonly int ID;
        List<Bullet> bullets;

        public BulletMaker(int ID, List<Bullet> bullets)
        {
            this.ID = ID;
            this.bullets = bullets;
        }

        public void Make(Vector2 source_position, Vector2 target_position)
        {
            if (ID == 0)
            {
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
            }
            else if (ID == 1)
            {
                // TODO
            }
            else if (ID == 2)
            {
                // TODO
            }
            ++time;
        }
    }
}
