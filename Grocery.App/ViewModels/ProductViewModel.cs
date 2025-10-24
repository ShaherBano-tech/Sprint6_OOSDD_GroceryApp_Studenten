using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        public ObservableCollection<Product> Products { get; } = new();

        public ProductViewModel(IProductService productService)
        {
            Title = "Producten";
            _productService = productService;
            LoadProducts();

            MessagingCenter.Subscribe<object>(this, "RefreshProducts", _ => LoadProducts());

        }

        private void LoadProducts()
        {
            Products.Clear();
            foreach (Product p in _productService.GetAll()) Products.Add(p);
        }

        [RelayCommand]
        private async Task NewProduct()
        {
            await Shell.Current.GoToAsync(nameof(Grocery.App.Views.NewProductView));
        }
    }
}
