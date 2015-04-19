using _7DTDManager.Interfaces;
using _7DTDManager.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.ShopSystem
{
    public class Shop : IPositionTrackable, IShop
    {
        public int ShopID { get; set; }
        public string ShopName { get; set; }
        public AreaDefiniton ShopPosition { get; set; }
        
        public int TotalSales { get; set; }
        public int TotalRevenue { get; set; }
        public int TotalDeals { get; set; }
        public int TotalCustomers { get; set; }
        public double EconomyFactor { get; set; }
               
        public bool HasOpeningHours { get; set; }
        public int ShopOpensAt { get; set; }
        public int ShopClosesAt { get; set; }
        public bool ShopRestocks { get;set; }
        public bool SecretShop { get; set; }

        public List<ShopItem> ShopItems { get; set; }

        public Shop()
        {
            ShopItems = new List<ShopItem>();
        }

        public void Init()
        {
            PositionManager.AddTrackableObject(this);
            foreach (var item in ShopItems)
            {
                item.Shop = this;
                if (ShopRestocks)
                    item.StartRestocking();
            }
        }

        public void Deinit()
        {
            PositionManager.RemoveTrackableObject(this);
            if (ShopRestocks)
            {
                foreach (var item in ShopItems)
                {
                    item.StopRestocking();
                }
            }
        }

        public void TrackPosition(IPlayer p, IPosition oldPos, IPosition newPos)
        {
            bool oldInside = ShopPosition.IsInside(oldPos), newInside = ShopPosition.IsInside(newPos);
            if (oldInside == newInside)
                return;
            if ( !oldInside && newInside )
            {
                p.Message("You entered the '{0}'-Shop. You can now use the shop related commands.",ShopName);
                p.SetCurrentShop(this);
                return;
            }
            if (oldInside && !newInside)
            {
                p.Message("You left the '{0}'-Shop.",ShopName);
                p.SetCurrentShop(null);
                return;
            }
            
        }

        public bool NeedsTracking(IPosition pos)
        {
            return ShopPosition.IsNear(pos);
        }


        public int RegisterItem(ShopSystem.ShopItem item)
        {
            var findItem = (from s in ShopItems select s.ItemID);
            int lastItem = 0;
            if ( (findItem != null) && (findItem.Count() > 0))
                lastItem = findItem.Max();

            item.ItemID = lastItem + 1;
            item.Shop = this;
            ShopItems.Add(item);
            Program.Config.IsDirty = true;
            return item.ItemID;
        }

        public bool IsOpen()
        {
            //TODO: Opening Hours
            return true;
        }

        IReadOnlyList<IShopItem> IShop.ShopItems
        {
            get { return ShopItems as IReadOnlyList<IShopItem>; }
        }

        IAreaDefiniton IShop.ShopPosition
        {
            get { return ShopPosition; }
        }
    }
}
