using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AmazingAdventures
{
    enum healthStatus : byte
    {
        HP0_stump = 40,
        HP10 = 41,
        HP20 = 42,
        HP30 = 43,
        HP40 = 44,
        HP55 = 45,
        HP70 = 46,
        HP85 = 47,
        HP100 = 48
    }
    enum ShovelStatus : byte
    {
        is_not,
        is_yes
    }
    enum WaterStatus : byte
    {
        none = 170,
        low = 171,
        mid = 172,
        high = 173
    }

    class Tree
    {
        #region Поля
        private healthStatus hitPoints;
        private Point treeLocation;
        private bool damaged = false;
        private ShovelStatus shovelUsed;
        private WaterStatus wateringStumpState;
        #endregion

        #region Свойства
        public healthStatus HitPoints
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
        public Point TreeLocation
        {
            get
            {
                return treeLocation;
            }
            set
            {
                treeLocation = value;
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
        public ShovelStatus ShovelUsed
        {
            get
            {
                return shovelUsed;
            }
            set
            {
                shovelUsed = value;
            }
        }
        public WaterStatus WateringStumpState
        {
            get
            {
                return wateringStumpState;
            }
            set
            {
                wateringStumpState = value;
            }
        }
        #endregion

        #region Constructor
        public Tree(Point treeLocation, healthStatus hitPoints)
        {
            this.treeLocation = treeLocation;
            this.hitPoints = hitPoints;
            shovelUsed = ShovelStatus.is_not;
            wateringStumpState = WaterStatus.none;
        }
        #endregion
    }
}
