using Grocery.App.ViewModels;

namespace Grocery.App.Views;

public partial class ProductView : ContentPage
{
	public ProductView(ProductViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}