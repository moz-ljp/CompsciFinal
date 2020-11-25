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
    public partial class StatsPage : ContentPage
    {

        Person person;

        List<Entry> entries = new List<Entry>();

        List<string> questionsOrdered = new List<string>();

        List<int> scores = new List<int>();
        List<int> scoresOrdered = new List<int>();

        string topTopic;

        public StatsPage(Person person)
        {
            InitializeComponent();
            this.person = person;
            usernameLabel.Text = person.Name;
            int totalIncorrect = person.totalAnswered - person.Score;
            System.Diagnostics.Debug.Write("Person SCORE", person.Score.ToString());
            System.Diagnostics.Debug.Write("Total answered:", person.totalAnswered.ToString());
            double successRate = Convert.ToDouble((Convert.ToDecimal(person.Score) / Convert.ToDecimal(person.totalAnswered)) * 100);
            successRate = Math.Round(successRate);
            System.Diagnostics.Debug.Write("Success Rate:", successRate.ToString());
            successRateLabel.Text = successRate.ToString() + '%';
            scoreLabel.Text = person.Score.ToString();

            graphContainer.IsVisible = false;

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
            graphContainer.IsVisible = !graphContainer.IsVisible;
        }
    }
}