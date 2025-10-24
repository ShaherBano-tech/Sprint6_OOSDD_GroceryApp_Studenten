using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            var items = _groceriesRepository.GetAllOnGroceryListId(groceryListId);
            FillService(items);
            return items;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            return _groceriesRepository.Delete(item);
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item); 
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 0)
        {
            var allProducts = _productRepository.GetAll();
            var items = _groceriesRepository.GetAll();
            var soldPerProductId = new Dictionary<int, int>();

            foreach (var it in items)
            {
                if (soldPerProductId.ContainsKey(it.ProductId))
                    soldPerProductId[it.ProductId] += it.Amount;   
                else
                    soldPerProductId[it.ProductId] = it.Amount;
            }

            var all = new List<BestSellingProducts>();
            foreach (var p in allProducts)
            {
                int sold = soldPerProductId.TryGetValue(p.Id, out var s) ? s : 0;
                all.Add(new BestSellingProducts(p.Id, p.Name, p.Stock, sold, 0));
            }

            var ordered = all
                .OrderByDescending(x => x.NrOfSells)   
                .ThenByDescending(x => x.Stock)
                .ToList();

            int rank = 0;
            for (int i = 0; i < ordered.Count; i++)
            {
                rank++;
                var bp = ordered[i];
                ordered[i] = new BestSellingProducts(bp.Id, bp.Name, bp.Stock, bp.NrOfSells, rank);
            }

            if (topX > 0) ordered = ordered.Take(topX).ToList();
            return ordered;
        }
    }
}
