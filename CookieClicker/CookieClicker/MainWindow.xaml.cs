using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace CookieClicker
{
    public partial class MainWindow : Window
    {
        private int cookieCount;
        private int cookiesPerClick;
        private int cookiesPerSecond;
        

        private Dictionary<string, double> basePrices;
        private Dictionary<string, double> investmentPrice;
        private Dictionary<string, int> investmentCount;

        private Random random;

        public static class NumberFormatter
        {
            public static string FormatNumber(double number)
            {
                string[] suffixes = { "", "Miljoen", "Miljard", "Biljoen", "Biljard", "Triljoen" };

                int suffixIndex = 0;

                while (number >= 1000.0 && suffixIndex < suffixes.Length - 1)
                {
                    number /= 1000.0;
                    suffixIndex++;
                }

                // Voeg een if-else controle toe voor de afronding
                double roundedNumber = (number >= 1_000_000.0) ? Math.Round(number, 3) : Math.Round(number, 0);

                string formattedNumber = $"{roundedNumber:F3} {suffixes[suffixIndex]}";

                if (roundedNumber < 1_000_000.0 && roundedNumber >= 1000.0)
                {
                    int spaceIndex = formattedNumber.IndexOf('.') - 1;
                    formattedNumber = formattedNumber.Insert(spaceIndex, " ");
                }

                return formattedNumber;
            }
        }
        

        public MainWindow()
        {
            InitializeComponent();

            basePrices = new Dictionary<string, double>
            {
                { "🍪", 1 },
                { "Cursor", 15 },
                { "Grandma", 100 },
                { "Farm", 1100 },
                { "Mine", 12000 },
                { "Factory", 130000 },
                { "Bank", 1400000 },
                { "Temple", 20000000 }
            };

            string cookieKey = "🍪";
            double cookiePrice = basePrices[cookieKey];

            cookiePriceLabel.Content = $"De prijs van 🍪 is {cookiePrice}";
            investmentPrice = new Dictionary<string, double>(basePrices);
            investmentCount = new Dictionary<string, int>();

            // Cookies per seconde
            Thread t = new Thread(GenerateCookiesPerSecond);
            t.Start();

            // Golden cookies timer
            Thread goldenCookieThread = new Thread(GenerateGoldenCookies);
            goldenCookieThread.Start();
        }
        private void GenerateGoldenCookies()
        {
            while (true)
            {
               
                if (random.NextDouble() < 0.3)
                {
                    double goldenCookie = cookiesPerSecond * 15 * 60;
                }

                Thread.Sleep(random.Next(1000, 5000));
            }
        }
      

        private double CalculatePrice(string investmentName)
        {

            double basePrice = basePrices[investmentName];
            int count = investmentCount.ContainsKey(investmentName) ? investmentCount[investmentName] : 0;

            // kosten berekenen
            double cost = basePrice * Math.Pow(1.15, count);

            return Math.Ceiling(cost); // prijs afronden
        }

        private void InvestmentButton_Click(object sender, RoutedEventArgs e)
        {
            // investeringsknoppen
            Button button = (Button)sender;
            string investment = button.Content.ToString();
            double cost = CalculatePrice(investment);

            if (cookieCount >= cost)
            {
                cookieCount -= (int)cost;
                cookiesPerSecond += (int)(cost / 10); // cookies per seconde

                // Update de teller voor de specifieke investering
                if (investmentCount.ContainsKey(investment))
                    investmentCount[investment]++;
                else
                    investmentCount.Add(investment, 1);

                investmentPrice[investment] = cost; // Update de prijs van de investering
                UpdateLabels();
            }
            else
            {
                MessageBox.Show("Niet genoeg cookies om deze investering te kopen!");
            }
        }

        private void CookieButton_Click(object sender, RoutedEventArgs e)
        {
            // cookie clicker
            cookieCount += cookiesPerClick;
            UpdateLabels();
        }

        private void GenerateCookiesPerSecond()
        {
            while (true)
            {
                foreach (var investmentName in investmentCount.Keys)
                {
                    double passiveIncomePerSecond = GetPassiveIncomePerSecond(investmentName);
                    double passiveIncomePerTick = GetPassiveIncomePerTick(investmentName);

                    cookiesPerSecond += (int)passiveIncomePerSecond;
                    cookieCount += (int)passiveIncomePerTick;
                }

                UpdateLabels();
                Thread.Sleep(10); // 10 miliseconden
            }
        }

        private double GetPassiveIncomePerSecond(string investmentName)
        {
            int count = investmentCount.ContainsKey(investmentName) ? investmentCount[investmentName] : 0;
            double incomePerSecond = count * GetIncomePerSecond(investmentName);
            return incomePerSecond;
        }

        private double GetIncomePerSecond(string investmentName)
        {
            switch (investmentName)
            {
                case "Cursor":
                    return 0.1;
                case "Grandma":
                    return 1;
                case "Farm":
                    return 8;
                case "Mine":
                    return 47;
                case "Factory":
                    return 260;
                case "Bank":
                    return 1400;
                case "Temple":
                    return 7800;
                default:
                        return 0;
            }
        }

        private double GetPassiveIncomePerTick(string investmentName)
        {
            return GetPassiveIncomePerSecond(investmentName) / 100.0; // 100 ticks per seconde
        }

        private void UpdateLabels()
        {
            // Wordt gebruikt om de labels bij te werken
            Dispatcher.Invoke(() =>
            {
                // Update venstertitel
                Title = $"Cookie Clicker - Cookies: {cookieCount}";
                clickCountLabel.Content = $"Klikken: {cookieCount}";
                cpsLabel.Content = $"Cookies per seconde: {cookiesPerSecond}";

                foreach (var investmentButton in grid.Children.OfType<Button>())
                {
                    string investmentName = investmentButton.Content.ToString();
                    double cost = CalculatePrice(investmentName);
                    investmentButton.Content = $"{investmentName} - {cost} cookies";
                }

                foreach (var investmentLabel in grid.Children.OfType<Label>())
                {
                    string investmentName = investmentLabel.Content.ToString();
                    if (investmentCount.ContainsKey(investmentName))
                    {
                        investmentLabel.Content = $"{investmentName} gekocht: {investmentCount[investmentName]}";
                    }
                }
            });
        }
    }
}
