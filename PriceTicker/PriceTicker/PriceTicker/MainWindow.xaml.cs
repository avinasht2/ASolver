using PriceTicker.Service;
using System.Windows;

namespace PriceTicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.PricesListView.DataContext = new PricesListViewModel(new RandomWalkPriceService());
        }
    }
}