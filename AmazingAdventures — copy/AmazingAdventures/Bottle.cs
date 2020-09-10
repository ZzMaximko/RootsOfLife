using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AmazingAdventures
{
    enum bottleStatus : byte
    {
        bottle_none,
        bottle_empty,
        bottle_water
    }

    class Bottle
    {
        #region поля
        private bottleStatus bottleState;
        private Point bottleLocation;
        private int containBottle = 1;
        #endregion

        #region свойства
        public bottleStatus BottleState
        {
            get
            {
                return bottleState;
            }
            set
            {
                bottleState = value;
            }
        }
        public Point BottleLocation
        {
            get
            {
                return bottleLocation;
            }
            set
            {
                bottleLocation = value;
            }
        }
        public int ContainBottle
        {
            get
            {
                return containBottle;
            }
        }
        #endregion

        #region Конструктор
        public Bottle(bottleStatus bottleState, Point bottleLocation)
        {
            this.bottleState = bottleState;
            this.bottleLocation = bottleLocation;
        }
        #endregion
    }
}
