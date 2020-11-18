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

        public StatsPage(Person person)
        {
            InitializeComponent();
            this.person = person;
            int totalIncorrect = person.totalAnswered - person.Score;
            System.Diagnostics.Debug.Write("Person SCORE", person.Score.ToString());
            System.Diagnostics.Debug.Write("Total answered:", person.totalAnswered.ToString());
            double successRate = Convert.ToDouble((Convert.ToDecimal(person.Score) / Convert.ToDecimal(person.totalAnswered)) * 100);
            System.Diagnostics.Debug.Write("Success Rate:", successRate.ToString());
            successRateLabel.Text = successRate.ToString() + '%';
            scoreLabel.Text = person.Score.ToString() ;

            Entry a = new Entry(totalIncorrect)
            {
                Color = SKColor.Parse("#FF1943"),
                Label = "Total Incorrect",
                ValueLabel = totalIncorrect.ToString()
            };
            Entry b = new Entry(person.Score)
            {
                Color = SKColor.Parse("#00BFFF"),
                Label = "Total Correct",
                ValueLabel = person.Score.ToString()
            };

            entries.Add(a);
            entries.Add(b);

            SuccessRateChart.Chart = new RadialGaugeChart() { Entries = entries };
            SuccessRateChart.Chart.BackgroundColor = SKColor.Parse("#3d3a3c");
            SuccessRateChart.Chart.LabelColor = SKColor.Parse("#ffffff");

        }
    }
}