using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        [ObservableProperty] private string name;
        [ObservableProperty] private int stock = 0;
        [ObservableProperty] private DateTime? shelfLifeDate = DateTime.Today;
        [ObservableProperty] private decimal price = 0;
        public NewProductViewModel(IProductService productService)
        {
            Title = "Nieuw Product";
            _productService = productService;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Fout", "Product naam is verplicht.", "OK");
                return;
            }
            if (price < 0)
            {
                await Shell.Current.DisplayAlert("Fout", "Product prijs kan niet negatief zijn.", "OK");
                return; 
            }
            if (Stock < 0)
            {
                await Shell.Current.DisplayAlert("Fout", "Product voorraad kan niet negatief zijn.", "OK");
                return;
            }

            var shelf = shelfLifeDate ?? DateTime.Today;
            var prod = new Product(id: 0, name: Name.Trim(), stock: Stock, shelfLife: DateOnly.FromDateTime(shelf), price: Price);

            _productService.Add(prod);

            MessagingCenter.Send(this, "RefreshProducts");

            await Shell.Current.DisplayAlert("Succes", "Product is toegevoegd.", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}
