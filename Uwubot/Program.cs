using Discord;
using Discord.WebSocket;

using Newtonsoft.Json;

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Uwubot
{
	using Models;

	public class Program
	{
		public const string configurePath = "Secret/cfg.json";
		public static Config AppConfig { get; set; }
	//	public static CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
		public static DiscordSocketClient DiscordClient { get; set; }
		public static DiscordSocketConfig DiscordClientConfig { get; set; }
		public static bool IsLoggerLocked { get; private set; }



		public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			if(File.Exists(configurePath))
            {
				AppConfig = await LoadConfigAsync().ConfigureAwait(false);
            }
			else
            {
				await SaveConfigAsync(new Config()).ConfigureAwait(false);
				return;
            }

			DiscordClientConfig = new DiscordSocketConfig()
			{
				LogLevel = LogSeverity.Verbose
			};
			DiscordClient = new DiscordSocketClient(DiscordClientConfig);
			DiscordClient.Log += RequestLogAsync;

			await DiscordClient.LoginAsync(TokenType.Bot, AppConfig.DiscordToken).ConfigureAwait(false);
			await DiscordClient.StartAsync().ConfigureAwait(false);

		//	while (!CancellationTokenSource.Token.IsCancellationRequested)
		//	{
		//		await Task.Delay(1024).ConfigureAwait(false);
		//	}


			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		public async Task<Config> LoadConfigAsync(string path = configurePath)
        {
			Config result;

			try
            {
				using (var stream = File.OpenRead(path))
				using (var reader = new StreamReader(stream))
                {
					result = JsonConvert.DeserializeObject<Config>(await reader.ReadToEndAsync().ConfigureAwait(false));
					await RequestLogAsync(new LogMessage(LogSeverity.Verbose, "Program", "The config has been loaded")).ConfigureAwait(false);
                }
			}
			catch(Exception e)
            {
				await RequestLogAsync(new LogMessage(LogSeverity.Error, "Program", e.Message, e)).ConfigureAwait(false);
				throw;
            }
			return result;
        }

		public static async Task SaveConfigAsync(Config config, string path = configurePath)
		{
			try
			{
				using (var stream = File.OpenWrite(path))
				using (var writer = new StreamWriter(stream))
				{
					await writer.WriteLineAsync(JsonConvert.SerializeObject(config, Formatting.Indented)).ConfigureAwait(false);
					await RequestLogAsync(new LogMessage(LogSeverity.Verbose, "Program", "The config has been saved successfully.")).ConfigureAwait(false);
				}
			}
			catch (Exception ex)
			{
				await RequestLogAsync(new LogMessage(LogSeverity.Error, "Program", ex.Message, ex)).ConfigureAwait(false);
				throw;
			}
		}

		public static async Task RequestLogAsync(LogMessage message)
		{
			await Task.WhenAny
			(
				LogAsync(message),
				Task.Delay(0)
			).ConfigureAwait(false);
		}

		public static async Task LogAsync(LogMessage message)
		{
			while (IsLoggerLocked)
			{
				await Task.Delay(1).ConfigureAwait(false);
			}
			IsLoggerLocked = true;
			switch (message.Severity)
			{
				case LogSeverity.Critical:
					Console.ForegroundColor = ConsoleColor.DarkRed;
					break;
				case LogSeverity.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case LogSeverity.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case LogSeverity.Info:
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				case LogSeverity.Verbose:
					Console.ForegroundColor = ConsoleColor.Cyan;
					break;
				case LogSeverity.Debug:
					Console.ForegroundColor = ConsoleColor.DarkGray;
					break;
			}
			await Console.Out.WriteLineAsync($"[{message.Source}]{message.Message}").ConfigureAwait(false);
			Console.ResetColor();
			IsLoggerLocked = false;
		}
	}
}
