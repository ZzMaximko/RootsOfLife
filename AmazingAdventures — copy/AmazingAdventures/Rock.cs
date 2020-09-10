using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AmazingAdventures
{
    enum rockHealthStatus: int
    {
        HP10 = 0,
        HP20 = 1,
        HP30,
        HP40,
        HP55,
        HP70,
        HP85,
        HP100,
        HP0 = -1
    }
    class Rock
    {
        #region fields
        private rockHealthStatus hitPoints;
        private Point rockLocation;
        private bool damaged = false;
        #endregion

        #region properties
        public rockHealthStatus HitPoints
        {
            get
            {
                return hitPoints;
            }
            set
            {
                hitPoints = value;
            }
        }
        public Point RockLocation
        {
            get
            {
                return rockLocation;
            }
            set
            {
                rockLocation = value;
            }
        }
        public bool Damaged
        {
            get
            {
                return damaged;
            }
            set
            {
                damaged = value;
            }
        }
        #endregion

        #region Constructor
        public Rock(rockHealthStatus hitPoints, Point rockLocation)
        {
            this.hitPoints = hitPoints;
            this.rockLocation = rockLocation;
        }
        #endregion
    }
}
