namespace Es2al.Services.Events.CustomEventArgs
{
    public class QuestionAnswerEventArgs : EventArgs
    {
        public int ReceiverId { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }

    }
}
