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
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        PageList PageList;
        static IgishevSchoolEntities6 context = new IgishevSchoolEntities6();

        public ProductPage()
        {
            InitializeComponent();
            myGrid.ItemsSource = IgishevSchoolEntities6.GetContext().Products.ToList();

            var allManufacturers = IgishevSchoolEntities6.GetContext().Manufacturers.ToList();
            allManufacturers.Insert(0, new Manufacturer
            {
                Название_производителя = "Все производители"
            });
            cd.ItemsSource = allManufacturers;
            cd.SelectedIndex = 0;
            Update();   
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Manager.myFrame.Navigate(new AddProductPage(0));
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
           try
            {
                ii.DataContext = myGrid.SelectedItem;
                Product editproduct = context.Products.Find(int.Parse(ii.Text));
                Manager.myFrame.Navigate(new AddProductPage(int.Parse(ii.Text)));
            }
            catch
            {
                MessageBox.Show("Выберите продукт");
            }
        }

        private void TextName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void Update()
        { 
            var currentProduct = context.Products.ToList();

            currentProduct = currentProduct.Where(p => p.Наименование_товара.ToLower().Contains(TextName.Text.ToLower())).ToList();

            if (x1.IsChecked.Value)
                currentProduct = currentProduct.ToList();
            else  if (x2.IsChecked.Value)
                    currentProduct = currentProduct.Where(p => p.Активен == "Активен").ToList();
            else if (x3.IsChecked.Value)
                currentProduct = currentProduct.Where(p => p.Активен == "Не активен").ToList();

            //if (cd.SelectedIndex > 0)
            //    currentProduct = currentProduct.Where(p => p.Manufacturers.Contains(cd.SelectedItem as Manufacturer)).ToList();

            var sort = currentProduct.ToList();
            
            switch (Price.SelectedIndex)
            {
                case 0: { sort = currentProduct.ToList(); break; }
                case 1: { sort = currentProduct.OrderBy(s => s.Цена).ToList(); break; }
                case 2: { sort = currentProduct.OrderByDescending(s => s.Цена).ToList(); break; }
            }

            PageList = new PageList(sort);
            myGrid.ItemsSource = PageList.OffsetProducts;

            int k = IgishevSchoolEntities6.GetContext().Products.Count();
            int n = sort.Count();
            col.Text = "Количество записей: " + n.ToString() + ". Всего записей: " + k.ToString();
        }

        private void x1_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                IgishevSchoolEntities6.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                myGrid.ItemsSource = IgishevSchoolEntities6.GetContext().Products.ToList();
            }
        }

        private void cd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void Price_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void Sales_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ii.DataContext = myGrid.SelectedItem;
                int i = int.Parse(ii.Text);
                Manager.myFrame.Navigate(new SalesHistoryProduct(context.Products.Find(i)));
            }
            catch
            {
                MessageBox.Show("Выберите продукт");
            }            
        }

        private void LastPageButton_Click(object sender, RoutedEventArgs e)
        {
            PageList.CurrentPage--;
            myGrid.ItemsSource = PageList.OffsetProducts;
            LastPageButton.IsEnabled = PageList.IsFirstPage;
            NextPageButton.IsEnabled = PageList.IsLastPage;
        }

        private void rb1_Checked(object sender, RoutedEventArgs e)
        {
            PageList.CountInPage = Convert.ToInt32((sender as RadioButton).Content);
            myGrid.ItemsSource = PageList.OffsetProducts;
            LastPageButton.IsEnabled = PageList.IsFirstPage;
            NextPageButton.IsEnabled = PageList.IsLastPage;
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            PageList.CurrentPage++;
            myGrid.ItemsSource = PageList.OffsetProducts;
            LastPageButton.IsEnabled = PageList.IsFirstPage;
            NextPageButton.IsEnabled = PageList.IsLastPage;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {           
            try
            {
                ii.DataContext = myGrid.SelectedItem;
                int i = int.Parse(ii.Text);
                Product delproduct = context.Products.Find(i);
                if (delproduct.ProductSales.Count != 0)
                {
                    MessageBox.Show("Невозможно удалить продукт, так как есть связанные данные.");
                    return;
                }
                if (MessageBox.Show("Вы действительно хотите удалить продукт?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;

                context.Products.Remove(delproduct);
                context.SaveChanges();
                PageList.Products.Remove(delproduct);
                myGrid.ItemsSource = PageList.OffsetProducts;
                MessageBox.Show("Продукт успешно удален");
            }
            catch
            {
                MessageBox.Show("Выберите продукт");
            }
        } 
    }
}
