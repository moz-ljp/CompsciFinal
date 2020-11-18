using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompsciFinal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatsPage : ContentPage
    {
        public StatsPage(Person person)
        {
            InitializeComponent();
            System.Diagnostics.Debug.Write("Person SCORE", person.Score.ToString());
            System.Diagnostics.Debug.Write("Total answered:", person.totalAnswered.ToString());
            double successRate = Convert.ToDouble((Convert.ToDecimal(person.Score) / Convert.ToDecimal(person.totalAnswered)) * 100);
            System.Diagnostics.Debug.Write("Success Rate:", successRate.ToString());
            successRateLabel.Text = successRate.ToString() + '%';
            scoreLabel.Text = person.Score.ToString() ;
        }
    }
}