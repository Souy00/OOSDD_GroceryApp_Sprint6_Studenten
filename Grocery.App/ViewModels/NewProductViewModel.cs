using Grocery.Core.Models;
using Grocery.Core.Interfaces.Services;
using System.Windows.Input;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        public NewProductViewModel(IProductService productService)
        {
            _productService = productService;
            SaveCommand = new Command(async () => await SaveProductAsync());
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        private int _stock;
        public int Stock
        {
            get => _stock;
            set => SetProperty(ref _stock, value);
        }

        private DateOnly _shelfLife = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly ShelfLife
        {
            get => _shelfLife;
            set => SetProperty(ref _shelfLife, value);
        }

        public ICommand SaveCommand { get; }

        private async Task SaveProductAsync()
        {
            try
            {
                var product = new Product(0, Name, Stock, ShelfLife, Price);
                _productService.Add(product);

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", ex.Message, "OK");
            }
        }
    }
}
