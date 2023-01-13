using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

namespace Prb.ee.TrainDelays.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    //comboboxen opvullen x2

    public partial class MainWindow : Window
    {
        string[] startStation = new string[] { "Brugge", "Kortrijk", "Harelbeke", "Gent", "Antwerpen", "Brussel" };
        string[] endStation = new string[] { "Brugge", "Kortrijk", "Harelbeke", "Gent", "Antwerpen", "Brussel" };

        List<decimal> compensationAmount = new List<decimal>();
        List<int> totalDelays = new List<int>();
        List<decimal> receivedCompensationAmount = new List<decimal>();

        decimal compensationTotal;
        decimal receivedTotal;

        int delay;

        string selectedStartStation;
        string selectedEndStation;

        public MainWindow()
        {
            InitializeComponent();
            FillComboBox();
            CleanStartUp();
        }

        //opvullen van de comboboxen met begin & eind stations
        void FillComboBox()
        {
            cmbFrom.Items.Clear();
            cmbTo.Items.Clear();
            foreach (string s in startStation)
            {
                cmbFrom.Items.Add(s);
            }
            foreach (string s in endStation)
            {
                cmbTo.Items.Add(s);
            }
        }

        private void BtnAddDelay_Click(object sender, RoutedEventArgs e)
        {
            //Fout opvangen wanneer geen cijfer in de textbox geplaatst word
            try
            {
                delay = int.Parse(txtDelay.Text);

                selectedStartStation = cmbFrom.SelectedItem.ToString();
                selectedEndStation = cmbTo.SelectedItem.ToString();

                CalculateTotalDelayTime();
                CalculateCompensation();
                MoreThenTenDelays();
               

                lstAllDelays.Items.Add($"Je had een vertraging van {delay} minuten tussen {selectedStartStation} en {selectedEndStation}");

            }
            catch (Exception)
            {
                MessageBox.Show("Gelieve een getal in te geven voor het aantal minuten vertraging.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);

            }


            if (lstAllDelays.Items.Count == 10)
            {
                btnAddDelay.IsEnabled = false;
                MessageBox.Show("Je hebt al te veel grote vertragingen, dien eerst je formulier in bij de NMBS!", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }

           


        }

        //het berekenen van het compensatie bedrag en dit in het correcte label plaatsen
        void CalculateCompensation()
        {
            delay = int.Parse(txtDelay.Text);
            decimal compensation;

            if (delay >= 10)
            {
                lstBigDelays.Items.Add($"Je had een vertraging van {delay} minuten tussen {selectedStartStation} en {selectedEndStation}");
                compensation = (delay - 9) * 0.5m;
                compensationAmount.Add(compensation);
                compensationTotal = compensationAmount.Sum();
                lblPending.Content = compensationTotal.ToString() + " EUR";
            }
        }

        //Berekend de totale compensatie
        void CalculateReceivedCompensation()
        {
            decimal received = compensationTotal;
            receivedCompensationAmount.Add(received);
            receivedTotal = receivedCompensationAmount.Sum();
            lblReceived.Content = receivedTotal.ToString() + " EUR";

            compensationAmount.Clear();
            lblPending.Content = "0,00 EUR";
        }

        //berekenen van de totale vertraging
        void CalculateTotalDelayTime()
        {
            delay = int.Parse(txtDelay.Text);

            totalDelays.Add(delay);
            int sum = totalDelays.Sum();
            lblTotalDelays.Content = sum.ToString();
        }

        //wat moet er gebeuren met meer dan 10 vertragingen
        void MoreThenTenDelays()
        {
            if (lstBigDelays.Items.Count >= 10)
            {
                btnSubmit.IsEnabled = true;
            }
        }

        //zorgt voor een 'propere' opstart van het programma maakt buttons onbeschikbaar en text leeg, plaatst basis info op het scherm
        void CleanStartUp()
        {
            txtDelay.Focus();

            cmbFrom.SelectedIndex = 0;
            cmbTo.SelectedIndex = 0;

            txtDelay.Text = "0";

            btnSubmit.IsEnabled = false;

            lblTotalDelays.Content = "0 Minutes";
            lblPending.Content = "0,00 EUR";
            lblReceived.Content = "0,00 EUR";
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            lstBigDelays.Items.Clear();

            CalculateReceivedCompensation();

            btnAddDelay.IsEnabled = true;
            btnSubmit.IsEnabled = false;
        }


        //onderstaande werkt niet 100% enkel bij het aanpassen van de rechter slider.
        private void SldUpperBound_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sldUpperBound.Minimum = 0;
            sldUpperBound.Maximum = delay;
            double lowerBound = sldLowerBound.Value;
            double upperBound = sldUpperBound.Value;

            var filteredDelays = totalDelays.Where(d => d >= lowerBound && d <= upperBound);
            int count = filteredDelays.Count();
            lblFilteredDelay.Content = count + " Vertragingen";
        }

        private void SldLowerBound_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sldLowerBound.Minimum = 0;
            sldLowerBound.Maximum = delay;

            double lowerBound = sldLowerBound.Value;
            double upperBound = sldUpperBound.Value;

            var filteredDelays = totalDelays.Where(d => d >= lowerBound && d <= upperBound);
            int count = filteredDelays.Count();
            lblFilteredDelay.Content = count + " Vertragingen";
        }

        //Zorgt er voor dat de buttons enkel werken wanneer ze verschillende value hebben
        private void CmbFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnAddDelay.IsEnabled = cmbFrom.SelectedItem != cmbTo.SelectedItem;
        }

        private void CmbTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnAddDelay.IsEnabled = cmbFrom.SelectedItem != cmbTo.SelectedItem;
        }
    }
}
