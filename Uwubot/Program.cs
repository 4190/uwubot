﻿using Discord;
using Discord.WebSocket;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Uwubot
{
	public class Program
	{
		private DiscordSocketClient _client;

		public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			_client = new DiscordSocketClient();

			_client.Log += Log;


			//  You can assign your bot token to a string, and pass that in to connect.
			//  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
			var token = "";

			// Some alternative options would be to keep your token in an Environment Variable or a standalone file.
			// var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
			// var token = File.ReadAllText("token.txt");
			// var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

			await _client.LoginAsync(TokenType.Bot, token);
			await _client.StartAsync();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}
	}
}
