using System;
using System.Collections.Generic;
using System.Data;
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
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;

namespace CurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Root val = new Root();

        public class Root
        {
            public Rate rates { get; set; }
            public long timestamp;
            public string license;

        }

        public class Rate
        {
            public double INR { get; set; }
            public double JPY { get; set; }

            public double USD { get; set; }

            public double NZD { get; set; }

            public double EUR { get; set; }

            public double CAD { get; set; }

            public double ISK { get; set; }

            public double PHP { get; set; }

            public double DKK { get; set; }

            public double CZK { get; set; }

        }


        private int currencyID = 0;
        private double fromAmount = 0;
        private double toAmount = 0;

        public MainWindow()
        {
            InitializeComponent();
            GetValue();
        }

        private async void GetValue()
        {
            try
            {
                string apikey = "ExampleKey";
                val = await GetData<Root>($"https://openexchangerates.org/api/latest.json?app_id={apikey}");
                BindCurrency();
            }
            catch(Exception ex) {
                MessageBox.Show(ex.ToString(), "Failed to get the currencies", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }


        private void BindCurrency()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Text");
            dt.Columns.Add("Value");

            dt.Rows.Add("---SELECT---", 0);
            dt.Rows.Add("INR", val.rates.INR);
            dt.Rows.Add("USD", val.rates.USD);
            dt.Rows.Add("NZD", val.rates.NZD);
            dt.Rows.Add("JPY", val.rates.JPY);
            dt.Rows.Add("EUR", val.rates.EUR);
            dt.Rows.Add("CAD", val.rates.CAD);
            dt.Rows.Add("ISK", val.rates.ISK);
            dt.Rows.Add("PHP", val.rates.PHP);
            dt.Rows.Add("DKK", val.rates.DKK);
            dt.Rows.Add("CZK", val.rates.CZK);



            from.ItemsSource = dt.DefaultView;
            from.DisplayMemberPath = "Text";
            from.SelectedValuePath = "Value";
            from.SelectedIndex = 0;

            to.ItemsSource = dt.DefaultView;
            to.DisplayMemberPath = "Text";
            to.SelectedValuePath = "Value";
            to.SelectedIndex = 0;
        }

        public static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(1);
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonConvert.DeserializeObject<Root>(responseString);

                        MessageBox.Show("License: " + responseObject.license, "Eyy", MessageBoxButton.OK, MessageBoxImage.Information);
                        return responseObject;
                    }
                    return myRoot;
                }
            }
            catch
            {
                return myRoot;
            }
        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            result.Text = string.Empty;
            amount.Text = string.Empty;
            from.SelectedIndex = 0;
            to.SelectedIndex = 0;
            from_output.Content = $"Result:";
            to_output.Content = $".";
            amount.Focus();
        }

        private void Convert_Button_Click(object sender, RoutedEventArgs e)
        {
            double converted;

            if(amount.Text == null || amount.Text.Trim() == "")
            {
                MessageBox.Show("Please enter the amount.", "Eyy", MessageBoxButton.OK, MessageBoxImage.Information);
                amount.Focus();
                return;
            }

            else if (from.SelectedValue == null || from.SelectedIndex == 0)
            {
                MessageBox.Show("Please select first currency", "Eyy", MessageBoxButton.OK, MessageBoxImage.Information);
                from.Focus();
                return;
            }

            else if (to.SelectedValue == null || to.SelectedIndex == 0)
            {
                MessageBox.Show("Please select second currency", "Eyy", MessageBoxButton.OK, MessageBoxImage.Information);
                to.Focus();
                return;
            }

            if(from.Text == to.Text)
            {
                converted = double.Parse(amount.Text);
                result.Text = converted.ToString("N2");
            }
            else
            {
                converted = double.Parse(amount.Text) * double.Parse(from.SelectedValue.ToString()) /
                    double.Parse(to.SelectedValue.ToString());
                result.Text = converted.ToString("N2");
            }
            from_output.Content = $"{amount.Text} {from.Text} is:";
            to_output.Content = $"{to.Text}.";
            return;

        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex rgx = new Regex("[^0-9]+");
            e.Handled = rgx.IsMatch(e.Text);
        }


    }
}
