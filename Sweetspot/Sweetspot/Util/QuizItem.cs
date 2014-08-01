namespace SweetspotApp.Util
{
    public class QuizItem
    {
        public int Id { get; protected set; }
        public string Question { get; protected set; }
        public string AnswerText { get; protected set; }
        public string AnswerFilename { get; protected set; }

        public QuizItem(int id, string question, string answerText, string answerFilename )
        {
            Id = id;
            Question = question;
            AnswerText = answerText;
            AnswerFilename = answerFilename;
        }
    }
}
