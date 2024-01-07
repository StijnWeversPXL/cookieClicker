﻿using System;
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
                { "Factory", 130000 }
            };

            string cookieKey = "🍪";
            double cookiePrice = basePrices[cookieKey];

            cookiePriceLabel.Content = $"De prijs van {cookieKey} is {cookiePrice}";
            investmentPrice = new Dictionary<string, double>(basePrices);
            investmentCount = new Dictionary<string, int>();

            // Cookies per seconde
            Thread t = new Thread(GenerateCookiesPerSecond);
            t.Start();
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