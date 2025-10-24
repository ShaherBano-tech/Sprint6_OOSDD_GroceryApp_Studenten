
using CommunityToolkit.Mvvm.ComponentModel;

namespace Grocery.Core.Models
{
    public partial class BestSellingProducts : Model
    {
        public int Stock { get; set; }
        public int Sold => nrOfSells;

        [ObservableProperty]
        public int nrOfSells;
        [ObservableProperty]
        public int ranking;
        private int rank;

        public BestSellingProducts(int productId, string name, int stock, int nrOfSells, int ranking) : base(productId, name)
        {
            Stock=stock;
            NrOfSells=nrOfSells;
            Ranking=ranking;
        }

        public BestSellingProducts(int id, string name, int stock, object nrOfSells, int rank) : base(id, name)
        {
            Stock = stock;
            NrOfSells = (int)nrOfSells;
            this.rank = rank;
        }
    }
}
