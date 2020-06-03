namespace UnityEngine
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    public class FileUtils
    {
        private List<Question> questions = new List<Question>();
        
        public FileUtils()
        {
            LoadAllQuestions();
        }
        
        private void LoadAllQuestions()
        {
            TextAsset json = (TextAsset)Resources.Load("questions", typeof(TextAsset)); 
            questions = JsonConvert.DeserializeObject<List<Question>>(json.text);
        }

        public Question GetRandomQuestion()
        {
            int randomElement = Random.Range(0, questions.Count);
            return questions[randomElement];
        }
    }
}