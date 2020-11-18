using System;
using System.Collections.Generic;
using System.Text;

namespace CompsciFinal
{
    class Question
    {

        public string QuestionText { get; set; } //set all the things
        public string CorrectAnswer { get; set; }
        public string IncorrectAnswerOne { get; set; }
        public string IncorrectAnswerTwo { get; set; }

        public string IncorrectAnswerThree { get; set; }

        public int votecount { get; set; }

        public string tag { get; set; }
    }
}
