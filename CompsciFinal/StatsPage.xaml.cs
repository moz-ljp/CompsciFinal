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

    public class Rank //rank objects for creating a rank
    {
        public int requiredScore { get; set; }
        public string rankName { get; set; }
    }
    public partial class StatsPage : ContentPage
    {

        Person person;

        List<Entry> entries = new List<Entry>(); //entries list for microchart graphs

        List<string> questionsOrdered = new List<string>(); //ordered questions list

        List<int> scores = new List<int>(); //scores list
        List<int> scoresOrdered = new List<int>(); //ordered scores list

        string topTopic; //best topic string

        public StatsPage(Person person)
        {
            InitializeComponent();
            this.person = person;
            usernameLabel.Text = person.Name;
            int totalIncorrect = person.totalAnswered - person.Score;
            System.Diagnostics.Debug.Write("Person SCORE", person.Score.ToString());
            System.Diagnostics.Debug.Write("Total answered:", person.totalAnswered.ToString());
            double successRate = 0;
            if (person.Score != 0 && person.totalAnswered != 0)
                successRate = Convert.ToDouble((Convert.ToDecimal(person.Score) / Convert.ToDecimal(person.totalAnswered)) * 100);
            successRate = Math.Round(successRate);
            System.Diagnostics.Debug.Write("Success Rate:", successRate.ToString());
            successRateLabel.Text = successRate.ToString() + '%';
            scoreLabel.Text = person.Score.ToString();

            graphContainer.IsVisible = false; //hide graphs until requested to be shown

            Entry a = new Entry(totalIncorrect) //create our entries out of the values
            {
                Color = SKColor.Parse("#FF1943"), //sets the colour of the graph part
                Label = "Total Incorrect", //sets the label for that part
                ValueLabelColor = SKColor.Parse("#FF1943"), //sets the label colour
                TextColor = SKColor.Parse("#FF1943"), //sets the colour of the text
                ValueLabel = totalIncorrect.ToString() //assigns the text for the label
            };
            Entry b = new Entry(person.Score)
            {
                Color = SKColor.Parse("#00BFFF"),
                Label = "Total Correct",
                ValueLabelColor = SKColor.Parse("#00BFFF"),
                TextColor = SKColor.Parse("#00BFFF"),
                ValueLabel = person.Score.ToString()
            };

            entries.Add(a); //add the entries to a list
            entries.Add(b);

            SuccessRateChartRadial.Chart = new RadialGaugeChart() { Entries = entries }; //set the data for the graph and draw it
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

            double conversionsSuccess = 0;
            double cyberSuccess = 0;
            double programmingSuccess = 0;
            double hardwareSuccess = 0;
            double softwareSuccess = 0;


            System.Diagnostics.Debug.Write("Hardware" + person.hardwareScore.ToString());


            if (person.conversionsScore != 0 && person.totalConversions != 0)
                conversionsSuccess = successRateCalculator(person.conversionsScore, person.totalConversions); //calculate each success rate
            if(person.cyberScore !=0 && person.totalCyber != 0)
                cyberSuccess = successRateCalculator(person.cyberScore, person.totalCyber);
            if(person.programmingScore != 0 && person.totalProgramming != 0)
                programmingSuccess = successRateCalculator(person.programmingScore, person.totalProgramming);
            if(person.hardwareScore != 0 && person.totalHardware != 0)
                hardwareSuccess = successRateCalculator(person.hardwareScore, person.totalHardware);
            if(person.softwareScore != 0 && person.totalSoftware != 0)
                softwareSuccess = successRateCalculator(person.softwareScore, person.totalSoftware);

            Dictionary<string, double> successRates = new Dictionary<string, double>(); //make a dictionary of the success rates that i can identify

            successRates.Add("Conversions", conversionsSuccess);
            successRates.Add("Cybersecurity", cyberSuccess);
            successRates.Add("Programming", programmingSuccess);
            successRates.Add("Hardware", hardwareSuccess);
            successRates.Add("Software", softwareSuccess);

            var successSorted = successRates.OrderBy(f => f.Value); //and sort the success rates by score

            System.Diagnostics.Debug.Write("Hardware" + " " + person.hardwareScore.ToString() +  " " +person.totalHardware.ToString());

            foreach (var x in successSorted)
            {
                System.Diagnostics.Debug.Write(x.Key, x.Value.ToString());
            }

            bestTopicLabel.Text = successSorted.Last().Key; //store the success rates, assuming first is worst and last is best.
            worstTopicLabel.Text = successSorted.First().Key;
            secondWorstTopicLabel.Text = successSorted.ElementAt(1).Key;
            

        }

        public void calculateRank()
        {

            Rank beginner = new Rank(); //create all the ranks I need for my system
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

            List<Rank> ranks = new List<Rank>(); //create a list for the ranks

            ranks.Add(beginner); //and add them all in
            ranks.Add(novice);
            ranks.Add(adept);
            ranks.Add(expert);
            ranks.Add(master);
            ranks.Add(god);
            ranks.Add(genius);

            int thisScore = person.Score;
            double successRate = successRateCalculator(thisScore, person.totalAnswered); //calculate the individuals success rate globally

            string rank = "";

            int element=0; //used as a pointer

            
            while(thisScore >= ranks[element].requiredScore && element < ranks.Count) //if the score is greater than the current found rank or equal to the minimum and less than the total ranks
            {
                if(ranks[element+1].requiredScore > thisScore) //if the next ranks required score is less than the users score
                    {
                    rank = ranks[element].rankName; //give them that rank
                    System.Diagnostics.Debug.Write("Set Rank");
                    break;
                }
                else
                {
                    element++; //increase pointer by 1
                    System.Diagnostics.Debug.Write("Increased element");
                }
            }

            if(successRate < 40) //if the success rate is less than 40
            {
                if (successRate < 25) //if the success rate is less than 25
                    if(rank != beginner.rankName) //check the user is not already at the bottom rank which would cause an error
                        rank = ranks[element - 1].rankName; //give them a really low rank
                else
                    rank = ranks[0].rankName; //if the success rate is ok, leave it as is
            }
            else if(successRate > 60) //if its greater than 60
            {
                rank = ranks[element + 1].rankName; //go to the next rank up
            }

            rankLabel.Text = "Rank: " + rank;

        }

        public double successRateCalculator(int correct, int total) //calculates success rate percentage ((score / total) * 100)
        {

            return Convert.ToDouble((Convert.ToDecimal(correct) / Convert.ToDecimal(total)) * 100);
            
        }


        private void showHideGraphs_Clicked(object sender, EventArgs e)
        {
            graphContainer.IsVisible = !graphContainer.IsVisible;
        }

        private async void backBTN_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}