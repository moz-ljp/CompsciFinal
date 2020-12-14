using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Entry = Microcharts.ChartEntry;

namespace CompsciFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class studentViewer : ContentPage
    {

        List<Entry> entries = new List<Entry>();

        Person person;

        public studentViewer(Person person, string rank)
        {
            InitializeComponent();

            studentNameLabel.Text = person.Name;
            studentRank.Text = rank;

            this.person = person;

            double successRate = Math.Round(Convert.ToDouble((Convert.ToDecimal(person.Score) / Convert.ToDecimal(person.totalAnswered)) * 100));
            successRateLabel.Text = successRate.ToString();
            scoreLabel.Text = person.Score.ToString();
            

            int totalIncorrect = person.totalAnswered - person.Score;

            showHideGraphs.IsVisible = false;

            Entry a = new Entry(totalIncorrect)
            {
                Color = SKColor.Parse("#FF1943"),
                Label = "Total Incorrect",
                ValueLabelColor = SKColor.Parse("#FF1943"),
                TextColor = SKColor.Parse("#FF1943"),
                ValueLabel = totalIncorrect.ToString()
            };
            Entry b = new Entry(person.Score)
            {
                Color = SKColor.Parse("#00BFFF"),
                Label = "Total Correct",
                ValueLabelColor = SKColor.Parse("#00BFFF"),
                TextColor = SKColor.Parse("#00BFFF"),
                ValueLabel = person.Score.ToString()
            };

            entries.Add(a);
            entries.Add(b);

            SuccessRateChartRadial.Chart = new RadialGaugeChart() { Entries = entries };
            SuccessRateChartRadial.Chart.BackgroundColor = SKColor.Parse("#3d3a3c");
            SuccessRateChartRadial.Chart.LabelColor = SKColor.Parse("#ffffff");

            SuccessRateChartBar.Chart = new BarChart() { Entries = entries };
            SuccessRateChartBar.Chart.BackgroundColor = SKColor.Parse("#3d3a3c");
            SuccessRateChartBar.Chart.LabelColor = SKColor.Parse("#ffffff");

            topicSorter();

        }

        public async void topicSorter()
        {
            double conversionsSuccess = successRateCalculator(person.conversionsScore, person.totalConversions);
            double cyberSuccess = successRateCalculator(person.cyberScore, person.totalCyber);
            double programmingSuccess = successRateCalculator(person.programmingScore, person.totalProgramming);
            double hardwareSuccess = successRateCalculator(person.hardwareScore, person.totalHardware);
            double softwareSuccess = successRateCalculator(person.softwareScore, person.totalSoftware);

            Dictionary<string, double> successRates = new Dictionary<string, double>();

            successRates.Add("conversions", conversionsSuccess);
            successRates.Add("cybersecurity", cyberSuccess);
            successRates.Add("programming", programmingSuccess);
            successRates.Add("hardware", hardwareSuccess);
            successRates.Add("software", softwareSuccess);

            var successSorted = successRates.OrderBy(f => f.Value);

            bestTopicLabel.Text = successSorted.Last().Key;
            worstTopicLabel.Text = successSorted.First().Key;
            secondWorstTopicLabel.Text = successSorted.ElementAt(1).Key;


        }

        public double successRateCalculator(int correct, int total)
        {

            return Convert.ToDouble((Convert.ToDecimal(correct) / Convert.ToDecimal(total)) * 100);

        }

        private void showHideGraphs_Clicked(object sender, EventArgs e)
        {
            showHideGraphs.IsVisible = !showHideGraphs.IsVisible;
        }
    }
}