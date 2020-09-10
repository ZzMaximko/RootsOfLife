using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AmazingAdventures
{
    enum stoneStatus : byte
    {
        is_rock,
        is_stone,
        is_none
    }
    class Stone
    {
        #region fields
        private stoneStatus stoneState;
        private Point stoneLocation;
        private int containStone = 5;
        #endregion

        #region properties
        public stoneStatus StoneState
        {
            get
            {
                return stoneState;
            }
            set
            {
                stoneState = value;
            }
        }
        public Point StoneLocation
        {
            get
            {
                return stoneLocation;
            }
            set
            {
                stoneLocation = value;
            }
        }
        public int ContainStone
        {
            get
            {
                return containStone;
            }
        }
        #endregion

        #region Constructor
        public Stone(stoneStatus stoneState, Point stoneLocation)
        {
            this.stoneState = stoneState;
            this.stoneLocation = stoneLocation;
        }
        #endregion
    }
}
