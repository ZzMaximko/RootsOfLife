using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace AmazingAdventures
{
    //enum itemStatus : byte
    //{
    //    is_none,
    //    is_available
    //}
    class MapItem
    {
        #region fields
        //private itemStatus itemState;
        private Point itemPosition;
        private int itemFoodsAnimateStep = 0; // счетчик анимации еды
        private int otherItemsAnimateStep = 48; // счетчик анимации монет и сердец
        #endregion

        #region properties
        //public itemStatus ItemState
        //{
        //    get
        //    {
        //        return itemState;
        //    }
        //}
        public Point ItemPosition
        {
            get
            {
                return itemPosition;
            }
        }
        #endregion

        #region methods
        private Point RandomizePositionItem(int[,] map)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            int x, y;
            do
            {
                x = rand.Next(1, 31);
                y = rand.Next(1, 18);
            } while ((map[x, y] >= 145 && map[x, y] <= 152) || (map[x, y] >= 2 && map[x, y] <= 5) || map[x, y] == 143 || map[x, y] == 144 || map[x, y] == 1 || map[x, y] == 79 || map[x, y] == 95 || map[x, y] == 80 || (map[x, y] >= 157 && map[x, y] <= 165));
            return new Point(x, y);
        }
        //public void SetItemStatus(itemStatus itemState)
        //{
        //    this.itemState = itemState;
        //}
        public void SetItemPositionOnMap(ref int[,] map)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            this.itemPosition = RandomizePositionItem(map);
            map[this.itemPosition.X, this.itemPosition.Y] = rnd.Next(157, 164);
        }
        public void ItemAnimation(Graphics myGraph, ImageList imgItems, int[,] map, int arrN, int arrM)
        {
            int picSizeWidth = 63;
            int picSizeHeight = 63;
            #region food items
            if (itemFoodsAnimateStep <= 7)
            {
                for(int i = 0; i < arrN; i++)
                    for(int j = 0; j < arrM; j++)
                    {
                        switch(map[i, j])
                        {
                            case 157:
                                {
                                    imgItems.Draw(myGraph, i * picSizeWidth, j * picSizeHeight, itemFoodsAnimateStep);
                                    break;
                                }
                            case 158:
                                {
                                    imgItems.Draw(myGraph, i * picSizeWidth, j * picSizeHeight, itemFoodsAnimateStep + 8);
                                    break;
                                }
                            case 159:
                                {
                                    imgItems.Draw(myGraph, i * picSizeWidth, j * picSizeHeight, itemFoodsAnimateStep + 16);
                                    break;
                                }
                            case 160:
                                {
                                    imgItems.Draw(myGraph, i * picSizeWidth, j * picSizeHeight, itemFoodsAnimateStep + 24);
                                    break;
                                }
                            case 161:
                                {
                                    imgItems.Draw(myGraph, i * picSizeWidth, j * picSizeHeight, itemFoodsAnimateStep + 32);
                                    break;
                                }
                            case 162:
                                {
                                    imgItems.Draw(myGraph, i * picSizeWidth, j * picSizeHeight, itemFoodsAnimateStep + 40);
                                    break;
                                }
                            case 163:
                                {
                                    imgItems.Draw(myGraph, i * picSizeWidth, j * picSizeHeight, itemFoodsAnimateStep + 62); // анимация лопаты
                                    break;
                                }
                        }
                    }
                itemFoodsAnimateStep++;
            } else
            {
                itemFoodsAnimateStep = 0;
            }
            #endregion
            #region coins and hearts
            if (otherItemsAnimateStep <= 51)
            {
                for (int i = 0; i < arrN; i++)
                    for (int j = 0; j < arrM; j++)
                    {
                        switch(map[i, j])
                        {
                            case 164:
                                {
                                    imgItems.Draw(myGraph, i * picSizeWidth, j * picSizeHeight, otherItemsAnimateStep);
                                    break;
                                }
                            case 165:
                                {
                                    imgItems.Draw(myGraph, i * picSizeWidth, j * picSizeHeight, otherItemsAnimateStep + 4);
                                    break;
                                }
                        }
                    }
                otherItemsAnimateStep++;
            } else
            {
                otherItemsAnimateStep = 48;
            }
            #endregion
        }
        #endregion

        public MapItem()
        {

        }
    }
}
