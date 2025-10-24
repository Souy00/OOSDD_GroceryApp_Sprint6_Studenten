using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        // ObservableCollection van Product
        public ObservableCollection<Product> Products { get; set; }

        // Command voor toevoegen nieuw product
        public ICommand AddProductCommand { get; }

        // Constructor
        public ProductViewModel(IProductService productService)
        {
            _productService = productService;

            // Haal alle producten op en voeg toe aan ObservableCollection
            Products = new ObservableCollection<Product>(_productService.GetAll());

            // Command instellen
            AddProductCommand = new Command(async () =>
            {
                // Navigeer naar NewProductView
                await Shell.Current.GoToAsync(nameof(Views.NewProductView));

                // Lijst refreshen na terugkomst
                RefreshProducts();
            });
        }

        // Hulpmethode om Products te refreshen
        private void RefreshProducts()
        {
            Products.Clear();
            foreach (var p in _productService.GetAll())
            {
                Products.Add(p);
            }
        }
    }
}
