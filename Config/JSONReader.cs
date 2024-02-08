using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Config
{
    internal class JSONReader
    {
        public string token { get; set; }
        public string prefix { get; set; }

        public async Task ReadJSON() 
        {
            using (StreamReader sr = new StreamReader("config.json", new UTF8Encoding(false)))
            {
                string json = await sr.ReadToEndAsync(); //Reading whole file
                ConfigJSON obj = JsonConvert.DeserializeObject<ConfigJSON>(json); //Deserialising file into the ConfigJSON structure

                this.token = obj.Token; //Setting our token & prefix that we extracted from our file
                this.prefix = obj.Prefix;
            }
        }
    }

    internal sealed class ConfigJSON
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
    }
}
