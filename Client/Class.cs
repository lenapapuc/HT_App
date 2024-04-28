using Domain;

namespace Client
{
    public class Class
    {
        public List<Message> GiveMeMessages()
        {
            Message message2 = new Message();
            message2.Content = "World";
            message2.CreatedDate = DateTime.Now;
            message2.IntendedFor = "Community";

            Message message1 = new Message();
            message1.Content = "Hello";
            message1.CreatedDate = DateTime.Now;
            message1.IntendedFor = "Community";
            message1.Replies = new List<Message>{message2};

            Message message3 = new Message();
            message3.Content = "Hello from me too";
            message3.CreatedDate = DateTime.Now;
            message3.IntendedFor = "Community";

            return new List<Message> { message1, message3};

        }
    }
}
