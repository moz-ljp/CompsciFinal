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

    public class Rank
    {
        public int requiredScore { get; set; }
        public string rankName { get; set; }
    }
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
            calculateRank();

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

        public void calculateRank()
        {

            Rank beginner = new Rank();
            beginner.rankName = "Beginner";
            beginner.requiredScore = 0;

            Rank novice = new Rank();
            novice.rankName = "Novice";
            novice.requiredScore = 100;

            Rank adept = new Rank();
            adept.rankName = "Adept";
            adept.requiredScore = 200;

            Rank expert = new Rank();
            expert.rankName = "Expert";
            expert.requiredScore = 400;

            Rank master = new Rank();
            master.rankName = "Master";
            master.requiredScore = 800;

            Rank god = new Rank();
            god.rankName = "God";
            god.requiredScore = 1600;

            Rank genius = new Rank();
            genius.rankName = "Genius";
            genius.requiredScore = 3200;

            List<Rank> ranks = new List<Rank>();

            ranks.Add(beginner);
            ranks.Add(novice);
            ranks.Add(adept);
            ranks.Add(expert);
            ranks.Add(master);
            ranks.Add(god);
            ranks.Add(genius);

            int thisScore = person.Score;
            double successRate = successRateCalculator(thisScore, person.totalAnswered);

            string rank = "";

            int element=0;

            
            while(thisScore >= ranks[element].requiredScore && element < ranks.Count)
            {
                if(ranks[element+1].requiredScore > thisScore)
                    {
                    rank = ranks[element].rankName;
                    System.Diagnostics.Debug.Write("Set Rank");
                    break;
                }
                else
                {
                    element++;
                    System.Diagnostics.Debug.Write("Increased element");
                }
            }

            if(successRate < 40)
            {
                rank = ranks[element - 1].rankName;
            }
            else if(successRate > 60)
            {
                rank = ranks[element + 1].rankName;
            }

            rankLabel.Text = "Rank: " + rank;

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