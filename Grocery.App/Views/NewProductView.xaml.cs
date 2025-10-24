using Grocery.App.ViewModels;
using Grocery.Core.Data.Repositories;
using Grocery.Core.Services;

namespace Grocery.App.Views
{
    public partial class NewProductView : ContentPage
    {
        
        public NewProductView()
        {
            InitializeComponent();

            
            var repo = new ProductRepository();
            var svc = new ProductService(repo);
            BindingContext = new NewProductViewModel(svc);
        }
    }
}
