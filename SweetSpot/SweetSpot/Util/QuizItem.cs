using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SweetSpot.Util
{
    public class QuizItem
    {
        public String Question { get; set; }
        public String Answer { get; set; }

        public QuizItem(String Question, String Answer)
        {
            this.Question = Question;
            this.Answer = Answer;
        }
    }
}
