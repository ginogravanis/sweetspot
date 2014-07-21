namespace SweetSpot.Util
{
    public class QuizItem
    {
        public int Id { get; internal set; }
        public string Question { get; internal set; }
        public string Answer { get; internal set; }

        public QuizItem(int id, string question, string answer)
        {
            Id = id;
            Question = question;
            Answer = answer;
        }
    }
}
