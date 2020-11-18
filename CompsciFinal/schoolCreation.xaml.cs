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
    public partial class schoolCreation : ContentPage
    {

        List<string> chars = new List<string>();

        string[] characters = { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "1", "2", "3", "4", "5", "6", "7", "8", "9"
        
        
        };

        public schoolCreation()
        {
            InitializeComponent();



        }

        public async void genCode(int id)
        {
            int x = id / characters.Length;
            int y = id % characters.Length;

            bool completed = false;

            int z = 0;

            while (!completed)
            {
                if(y % characters.Length == 0) //if remainder is 0
                {
                    y = y / characters.Length; //y = y over characterLength
                }

                else if(y / characters.Length == 0) //if the division = 0
                {
                    z = y; //finalise
                    completed = true;
                }
            }


            string thisCode = characters[x].ToString() + characters[z].ToString();

            idLabel.Text = thisCode;
            
        }

        private void btnCreateQ_Clicked(object sender, EventArgs e)
        {

        }

        private void btnSolveID_Clicked(object sender, EventArgs e)
        {
            genCode(Convert.ToInt32(ID.Text));
        }
    }
}