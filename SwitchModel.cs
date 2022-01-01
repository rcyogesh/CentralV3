namespace CentralV3
{
    public class SwitchModel
    {
        static Dictionary<string, int> locations = new Dictionary<string, int>();
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<SwitchModel> logger;
        private Guid currentOperationGuid;

        public SwitchModel(IHttpClientFactory clientFactory, ILogger<SwitchModel> logger)
        {
            this.clientFactory = clientFactory;
            this.logger = logger;
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

        public void Switch(bool on)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://192.168.1.228/" + (on ? "on" : "off"));
            var client = clientFactory.CreateClient();
            var response = client.Send(request);
            using (var stream = response.Content.ReadAsStream())
            {
                StreamReader reader = new StreamReader(stream);
                reader.ReadToEnd();
            }
        }

        public void StartCycle(int[] offOnPattern)
        {
            currentOperationGuid = Guid.NewGuid();
            Task.Run(() =>
            {
                var operationGuid = currentOperationGuid;
                while (operationGuid == currentOperationGuid)
                {
                    int totalSleepTime = 0;

                    bool on = true;
                    for (int i = 0; i < offOnPattern.Length; i++)
                    {
                        int msTimeout = offOnPattern[i];
                        totalSleepTime += msTimeout;
                        NextStateChangeAt = DateTime.Now + TimeSpan.FromMilliseconds(msTimeout);
                        Thread.Sleep(msTimeout);
                        if (currentOperationGuid != operationGuid)
                        {
                            break;
                        }

                        logger?.LogInformation($"Turning {on}");
                        try
                        {
                            Switch(on);
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError("Error while switching", ex);
                        }
                        on = !on;
                    }

                    if (totalSleepTime < 50)
                    {
                        Thread.Sleep(50); //Ensure minimum 50 ms timeout
                    }
                }
            });
        }
    }
}
