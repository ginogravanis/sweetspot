namespace Sweetspot.Util
{
    public class QuizItem
    {
        public int Id { get; protected set; }
        public string Question { get; protected set; }
        public string AnswerFilename { get; protected set; }

        public QuizItem(int id, string question, string answer)
        {
            Id = id;
            Question = question;
            AnswerFilename = answer;
        }
    }
}
