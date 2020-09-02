using System.Windows;
using System.Windows.Controls;
using Zadatak_1.ViewModels;

namespace Zadatak_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(this);
        }
        private void Slide_Clicked(object sender, RoutedEventArgs e)
        {
            var game = DataContext as MainWindowViewModel;
            var button = sender as Button;
            game.ClickedCard(button.DataContext);
        }
    }
}
