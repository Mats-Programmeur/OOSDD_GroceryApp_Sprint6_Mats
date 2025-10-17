using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.App.Views;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        [ObservableProperty]
        private Client client;
        public ObservableCollection<Product> Products { get; set; }

        public ProductViewModel(IProductService productService, GlobalViewModel global)
        {
            _productService = productService;
            Products = [];
            foreach (Product p in _productService.GetAll()) Products.Add(p);
            Client = global.Client;
        }

        [RelayCommand]
        public async Task ShowNewProductPage()
        {
            if (Client.Role == Role.Admin)
                await Shell.Current.GoToAsync(nameof(NewProductView));
            else
                await Shell.Current.DisplayAlert("Geen toegang", "Alleen admins mogen producten aanmaken.", "OK");
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            // Herlaad de productenlijst
            Products.Clear();
            foreach (Product p in _productService.GetAll())
            {
                Products.Add(p);
            }
        }
    }
}
