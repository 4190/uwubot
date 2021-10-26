using Newtonsoft.Json;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uwubot.Models
{
    [JsonObject]
    public class Config
    {
        [JsonProperty("discordToken")]
        public string DiscordToken { get; set; }
        public string CurDir { get; set; }
    }
}
