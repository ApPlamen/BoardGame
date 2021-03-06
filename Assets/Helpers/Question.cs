namespace UnityEngine
{
    public class Question
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public Answer[] Answers { get; set; }
        
        public string AdditionalInfo { get; set; }
    }
}