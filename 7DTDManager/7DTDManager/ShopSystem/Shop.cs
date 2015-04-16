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
        
        public bool IsOpen { get; set; }
        public bool HasOpeningHours { get; set; }
        public int ShopOpensAt { get; set; }
        public int ShopClosesAt { get; set; }
        public bool ShopRestocks { get;set; }

        public List<ShopItem> ShopItems { get; set; }

        public void Init()
        {
            PositionManager.AddTrackableObject(this);
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
