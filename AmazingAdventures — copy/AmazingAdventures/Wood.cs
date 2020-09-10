using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AmazingAdventures
{
    enum countStatus : byte
    {
        wood_empty,
        wood_one,
        wood_more_three
    }
    class Wood
    {
        #region Поля
        private countStatus countState;
        private Point woodLocation;
        private bool grabbed;
        private int containWood;
        #endregion

        #region Свойства
        public countStatus CountState
        {
            get
            {
                return countState;
            }
            set
            {
                countState = value;
            }
        }
        public Point WoodLocation
        {
            get
            {
                return woodLocation;
            }
            set
            {
                woodLocation = value;
            }
        }
        public bool Grabbed
        {
            get
            {
                return grabbed;
            }
            set
            {
                grabbed = value;
            }
        }
        public int ContainWood
        {
            get
            {
                return containWood;
            }
            set
            {
                containWood = value;
            }
        }
        #endregion

        #region Конструктор
        public Wood(countStatus countState, Point woodLocation, bool grabbed, int containWood)
        {
            this.countState = countState;
            this.woodLocation = woodLocation;
            this.grabbed = grabbed;
            this.containWood = containWood;
        }
        #endregion
    }
}
