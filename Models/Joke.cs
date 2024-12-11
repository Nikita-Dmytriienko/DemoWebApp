namespace DemoWebApp.Models
{
    public class Joke
    {
        public int Id { get; set; }
        public string JokeQuestion { get; set; }
        public string JokeAnswer { get; set; }
        public string Category {  get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }
        public Joke()
        {
          
        }
    }
}
