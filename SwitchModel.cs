namespace CentralV3
{
    public class SwitchModel
    {
        static Dictionary<string, int> locations = new Dictionary<string, int>();
        private readonly IHttpClientFactory clientFactory;

        public SwitchModel(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public DateTime NextStateChangeAt { get; private set; }

        public int Get(string location)
        {
            int result;
            locations.TryGetValue(location, out result);
            return result;
        }

        public int Get(string location, int value)
        {
            locations[location] = value;
            return Get(location);
        }

        public bool GetValue()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://192.168.1.228/value");
            var client = clientFactory.CreateClient();
            var response = client.Send(request);
            string s;
            using (var stream = response.Content.ReadAsStream())
            {
                StreamReader reader = new StreamReader(stream);
                s = reader.ReadToEnd();
            }
            return s == "1";
        }

        public void SwitchOn()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://192.168.1.228/on");
            var client = clientFactory.CreateClient();
            var response = client.Send(request);
            using (var stream = response.Content.ReadAsStream())
            {
                StreamReader reader = new StreamReader(stream);
                reader.ReadToEnd();
            }
        }

        public void SwitchOff()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://192.168.1.228/off");
            var client = clientFactory.CreateClient();
            var response = client.Send(request);
            using (var stream = response.Content.ReadAsStream())
            {
                StreamReader reader = new StreamReader(stream);
                reader.ReadToEnd();
            }
        }

        public void StartCycle()
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                while (true)
                {
                    DateTime dateTime = DateTime.Now;
                    int msTimeout = 10 * 60 * 1000;
                    NextStateChangeAt = DateTime.Now + TimeSpan.FromMilliseconds(msTimeout);
                    Thread.Sleep(msTimeout);

                    Console.WriteLine("Turning ON");
                    SwitchOn();
                    msTimeout = 30 * 1000;
                    NextStateChangeAt = DateTime.Now + TimeSpan.FromMilliseconds(msTimeout);

                    Thread.Sleep(msTimeout); //ON for 30 seconds
                    Console.WriteLine("Turning OFF");
                    SwitchOff();
                }
            });
        }
    }
}
