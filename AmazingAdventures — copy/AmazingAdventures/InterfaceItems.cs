using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AmazingAdventures
{
    public enum itemTag : byte
    {
        tag_none,
        tag_sword,
        tag_bow,
        tag_shovel
    }

    class InterfaceItems
    {
        #region поля
        private Point[] itemsPosition = new Point[4];
        private itemTag currentItem;
        private string itemsKey = "";
        #endregion

        #region свойства
        public Point[] ItemsPosition
        {
            get
            {
                return itemsPosition;
            }
        }
        public itemTag CurrentItem
        {
            get
            {
                return currentItem;
            }
            set
            {
                currentItem = value;
            }
        }
        #endregion

        #region methods
        public string GetItemsKey(bool sword, bool bow, int shovel, int countWood)
        {
            this.itemsKey = "";
            if (sword) this.itemsKey += "s";
            if (bow) this.itemsKey += "h";
            if (shovel >= 0) this.itemsKey += "p";
            if (countWood >= 0)
            {
                this.itemsKey += "a";
            }
            return this.itemsKey;
        }
        public void GetItemsPosition(Point interfacePosition)
        {
            int b = 0; // размер картинок + свободное пространство между ними
            // начальные точки отрисовки элементов
            int x = interfacePosition.X;
            int y = interfacePosition.Y;
            for(int i = 0; i < itemsPosition.Length; i++)
            {
                itemsPosition[i] = new Point(x + b, y);
                b += 52;
            }
        }
        #endregion
    }
}
