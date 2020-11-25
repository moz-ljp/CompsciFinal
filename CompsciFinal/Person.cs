using System;
using System.Collections.Generic;
using System.Text;

namespace CompsciFinal
{
    public class Person
    {

        public string PersonId { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int totalAnswered { get; set; }

        //individual scores :(

        public int hardwareScore { get; set; }
        public int totalHardware { get; set; }
        public int softwareScore { get; set; }
        public int totalSoftware { get; set; }
        public int cyberScore { get; set; }
        public int totalCyber { get; set; }
        public int programmingScore { get; set; }
        public int totalProgramming { get; set; }
        public int conversionsScore { get; set; }
        public int totalConversions { get; set; }


    }
}
