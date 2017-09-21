using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Linkup.Common
{
    /// <summary>
    /// 请求限制辅助
    /// </summary>
    public class LimitedRequestHelper
    {
        private static readonly LimitedRequestHelper _instance = new LimitedRequestHelper();
        public static LimitedRequestHelper Instance
        {
            get { return _instance; }
        }

        private Dictionary<string, LimitedRequestTarget> _targetList = new Dictionary<string, LimitedRequestTarget>();

        private object _lockObj = new object();

        private LimitedRequestHelper()
        {

        }

        /// <summary>
        /// 增加一个目标
        /// </summary>
        /// <param name="targetName">目标名</param>
        /// <param name="intervalMinute">单位时间</param>
        /// <param name="shootingTimes">单位内允许的命中次数</param>
        public void AddTarget(string targetName, int intervalMinute, int shootingTimes)
        {
            if (_targetList.ContainsKey(targetName) == false)
            {
                lock (_lockObj)
                {
                    if (_targetList.ContainsKey(targetName) == false)
                    {
                        LimitedRequestTarget target = new LimitedRequestTarget(targetName, intervalMinute, shootingTimes);
                        _targetList.Add(targetName, target);
                    }
                }
            }
        }

        /// <summary>
        /// 尝试射击目标
        /// </summary>
        /// <param name="targetName">目标名</param>
        /// <param name="bulletName">子弹名称</param>
        /// <returns>是否命中目标</returns>
        public bool Shoot(string targetName, string bulletName)
        {
            if (_targetList.ContainsKey(targetName) == false)
            {
                throw new ArgumentOutOfRangeException("没指定的靶。");
            }

            LimitedRequestTarget target = _targetList[targetName];
            return target.Shoot(bulletName);
        }
    }

    public class LimitedRequestTarget
    {
        private object _lockObj = new object();

        private Timer _clearTimer;

        public string Name
        {
            get;
            set;
        }

        public int IntervalMinute
        {
            get;
            set;
        }

        public int ShootingTimes
        {
            get;
            set;
        }

        private Hashtable _bulletPool = new Hashtable();

        public LimitedRequestTarget(string name, int intervalMinute, int shootingTimes)
        {
            Name = name;
            IntervalMinute = intervalMinute;
            ShootingTimes = shootingTimes;

            _clearTimer = new Timer(Clear, null, 0, 1000 * 60 * 1);
        }

        public bool Shoot(string bulletName)
        {
            LimitedRequestBullet bullet = _bulletPool[bulletName] as LimitedRequestBullet;
            if (bullet == null)
            {
                lock (_lockObj)
                {
                    bullet = _bulletPool[bulletName] as LimitedRequestBullet;
                    if (bullet == null)
                    {
                        bullet = new LimitedRequestBullet(bulletName, this);
                        _bulletPool.Add(bulletName, bullet);
                    }
                }
            }

            return bullet.Shoot();


            //LimitedRequestBullet bullet;
            //if (_bulletPool.ContainsKey(bulletName) == false)
            //{
            //    lock (_lockObj)
            //    {
            //        if (_bulletPool.ContainsKey(bulletName) == false)
            //        {
            //            bullet = new LimitedRequestBullet(bulletName, this);
            //            _bulletPool.Add(bulletName, bullet);
            //        }
            //    }
            //}

            //bullet = (LimitedRequestBullet)_bulletPool[bulletName];
            //return bullet.Shoot();
        }

        private void Clear(object state)
        {
           List<LimitedRequestBullet> bulletList = _bulletPool.Values.Cast<LimitedRequestBullet>().ToList();
           foreach (var bullet in bulletList)
           {
               if ((DateTime.Now - bullet.LastShootingTime).TotalMinutes > IntervalMinute)
               {
                   lock (_lockObj)
                   {
                       _bulletPool.Remove(bullet.Name);
                   }
               }
           }
        }
    }

    public class LimitedRequestBullet
    {
        private List<LimitedRequestShooting> _shootingList = new List<LimitedRequestShooting>();

        public string Name
        {
            get;
            set;
        }

        public DateTime LastShootingTime
        {
            get;
            set;
        }

        public LimitedRequestTarget Target
        {
            get;
            set;
        }

        public LimitedRequestBullet(string name, LimitedRequestTarget target)
        {
            Name = name;
            Target = target;
        }

        public bool Shoot()
        {
            if (_shootingList.Count >= Target.ShootingTimes)
            {
                LimitedRequestShooting firstShooting = _shootingList[_shootingList.Count - Target.ShootingTimes];
                if ((DateTime.Now - firstShooting.ShootingTime).TotalMinutes <= Target.IntervalMinute)
                {
                    return false;
                }
            }

            LimitedRequestShooting shooting = new LimitedRequestShooting();
            _shootingList.Add(shooting);
            LastShootingTime = shooting.ShootingTime;
            return true;
        }
    }

    public class LimitedRequestShooting
    {
        public DateTime ShootingTime
        {
            get;
            set;
        }

        public LimitedRequestShooting()
        {
            ShootingTime = DateTime.Now;
        }
    }
}