using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для SalesHistoryProduct.xaml
    /// </summary>
    public partial class SalesHistoryProduct : Page
    {
        IgishevSchoolEntities6 context = new IgishevSchoolEntities6();
        private Product myproduct;
        public SalesHistoryProduct(Product product)
        {
            InitializeComponent();

            myproduct = product;
            if (product.ProductSales.Count == 0)
            {
                MessageBox.Show("Нет продаж у выбранного товара");
            }
            else
            {
                var table = context.ProductSales.Select(a => new
                {
                    id = a.ID,
                    productid = a.ID_товара,
                    prod = "Наименование товара:   " + a.Product.Наименование_товара,
                    data = "Дата и время продажи:   " + (DateTime?)a.Дата_и_время_продажи,
                    col = "Количество:   " + a.Количество
                })
                    .Where(s => s.productid == myproduct.ID)
                    .ToList();

                LVSales.ItemsSource = table;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.myFrame.Navigate(new ProductPage());
        }
    }
}
