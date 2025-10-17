using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.App.Views;
using System;
using System.Collections.Generic;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private int stock;

        [ObservableProperty]
        private DateOnly shelfLife = DateOnly.FromDateTime(DateTime.Today);

        [ObservableProperty]
        private decimal price;

        public NewProductViewModel(IProductService productService)
        {
            _productService = productService;
        }

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Fout", "Naam is verplicht.", "OK");
                return;
            }

            if (Price < 0)
            {
                await Shell.Current.DisplayAlert("Fout", "Prijs mag niet negatief zijn.", "OK");
                return;
            }

            if (Stock < 0)
            {
                await Shell.Current.DisplayAlert("Fout", "Voorraad mag niet negatief zijn.", "OK");
                return;
            }

            var newProduct = new Product(0, Name, Stock, ShelfLife, Price);
            _productService.Add(newProduct);

            await Shell.Current.DisplayAlert("Succes", "Product succesvol toegevoegd.", "OK");
            await Shell.Current.GoToAsync(".."); // Terug naar vorige pagina
        }

    }
}
