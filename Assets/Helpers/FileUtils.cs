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
            using (StreamReader r = new StreamReader("Assets/Helpers/questions.json"))
            {
                string json = r.ReadToEnd();
                questions = JsonConvert.DeserializeObject<List<Question>>(json);
            }
        }

        public Question GetRandomQuestion()
        {
            int randomElement = Random.Range(0, questions.Count);
            return questions[randomElement];
        }
    }
}