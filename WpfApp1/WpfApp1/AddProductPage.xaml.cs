using Microsoft.Win32;
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
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для AddProductPage.xaml
    /// </summary>
    public partial class AddProductPage : Page
    {
        private Product myprod = new Product();
        IgishevSchoolEntities6 context = new IgishevSchoolEntities6();

        public AddProductPage(int id)
        {
            InitializeComponent();
            CBManufacturer.ItemsSource = context.Manufacturers.ToList();

            Product editproduct = context.Products.Find(id);

            if (editproduct != null)
            {
                myprod = editproduct;
                if (myprod.Активен != "Активен") nbut.IsChecked = true;
            }

            DataContext = myprod;
        }        

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.myFrame.GoBack();
        }

        private void BtnPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == true)
            {
                FOT.Source = new BitmapImage(new Uri(op.FileName));
            }

            var FileNameToSave = DateTime.Now.ToFileTime() + Path.GetExtension(op.FileName);
            var IMG = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}Товары школы\\{FileNameToSave}");
            TBImage.Text = IMG;
            TBImage.Focus();
            File.Copy(op.FileName, IMG);
        }

        private void TBImage_TextChanged(object sender, TextChangedEventArgs e)
        {
            Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string k = "Активен";
            if (nbut.IsChecked.Value) k = "Не активен";
            myprod.Активен = k.ToString();

            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(myprod.Наименование_товара))
                errors.AppendLine("Укажите наименование товара");              
            if (myprod.Manufacturers == null)
                errors.AppendLine("Укажите производителя");
            if (myprod.Цена == null || myprod.Цена < 1)
                errors.AppendLine("Укажите цену");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }            

            if (myprod.ID == 0)            
                context.Products.Add(myprod);
            
            try
            {
                context.SaveChanges();
                MessageBox.Show("Информация сохранена!");
                Manager.myFrame.Navigate(new ProductPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }       
        }
    }
}
