using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AmazingAdventures
{
    class Inventory
    {
        #region поля
        private Point[] itemsPos = new Point[6];
        private string bagKey = "";
        #endregion


        #region свойства
        public string Key
        {
            get
            {
                return bagKey;
            }
            set
            {
                bagKey = value;
            }
        }
        public Point[] ItemsPos
        {
            get
            {
                return itemsPos;
            }
        }
        #endregion

        #region методы
        public void SetItemsPosition(Point inventoryForm)
        {
            int b = 0; // size of pic item
            int x = inventoryForm.X + 40;
            int y = inventoryForm.Y + 80;
            for(int i = 0; i < 6; i++)
            {
                itemsPos[i] = new Point(x + b, y);
                b += 68;
            }
        }
        #endregion
    }
}
