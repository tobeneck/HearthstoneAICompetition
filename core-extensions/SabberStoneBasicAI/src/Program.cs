#region copyright
// SabberStone, Hearthstone Simulator in C# .NET Core
// Copyright (C) 2017-2019 SabberStone Team, darkfriend77 & rnilva
//
// SabberStone is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License.
// SabberStone is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
#endregion
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneBasicAI.Meta;
using SabberStoneBasicAI.Nodes;
using SabberStoneBasicAI.Score;
using SabberStoneBasicAI.AIAgents;
using SabberStoneBasicAI.PartialObservation;
using SabberStoneBasicAI.CompetitionEvaluation;

using SabberStoneBasicAI.AIAgents.Iteration4;

namespace SabberStoneBasicAI
{
	internal class Program
	{
		private static readonly Random Rnd = new Random();

		private static void Main()
		{
			Console.WriteLine("Starting test setup.");

			// TEST BASIC AI

			//OneTurn();
			//FullGame();
			//RandomGames();
			//TestPOGame();
			//TestFullGames();
			//TestTournament();

			AbstractAgent player1 = new Iteration4();
			CardClass player1Class = CardClass.MAGE;
			List<Card> player1Deck = Decks.RenoKazakusMage;

			AbstractAgent player2 = new OneStepLookahead();//new DynamicLookaheadAgent();  GreedyAgent  OneStepLookahead
			int nrGames = 15;

			TestPOGame(nrGames, 1, player1, player2, player1Class, CardClass.MAGE, player1Deck, Decks.RenoKazakusMage);
			TestPOGame(nrGames, 2, player1, player2, player1Class, CardClass.MAGE, player1Deck, Decks.RenoKazakusMage);

			/*
			Console.WriteLine("---Evaluate vs Mage---");
			TestPOGame(nrGames, 1, player1, player2, player1Class, CardClass.MAGE, player1Deck, Decks.RenoKazakusMage);
			TestPOGame(nrGames, 2, player1, player2, player1Class, CardClass.MAGE, player1Deck, Decks.RenoKazakusMage);

			Console.WriteLine("---Evaluate vs Warrior---");
			TestPOGame(nrGames, 1, player1, player2, player1Class, CardClass.WARRIOR, player1Deck, Decks.AggroPirateWarrior);
			TestPOGame(nrGames, 2, player1, player2, player1Class, CardClass.WARRIOR, player1Deck, Decks.AggroPirateWarrior);

			//Console.WriteLine("---Evaluate vs Druid---");
			//TestPOGame(nrGames, 1, player1, player2, player1Class, CardClass.DRUID, player1Deck, Decks.MurlocDruid);
			//TestPOGame(nrGames, 2, player1, player2, player1Class, CardClass.DRUID, player1Deck, Decks.MurlocDruid);

			Console.WriteLine("---Evaluate vs Shaman---");
			TestPOGame(nrGames, 1, player1, player2, player1Class, CardClass.SHAMAN, player1Deck, Decks.MidrangeJadeShaman);
			TestPOGame(nrGames, 2, player1, player2, player1Class, CardClass.SHAMAN, player1Deck, Decks.MidrangeJadeShaman);

			Console.WriteLine("---Evaluate vs Paladin---");
			TestPOGame(nrGames, 1, player1, player2, player1Class, CardClass.PALADIN, player1Deck, Decks.MidrangeBuffPaladin);
			TestPOGame(nrGames, 2, player1, player2, player1Class, CardClass.PALADIN, player1Deck, Decks.MidrangeBuffPaladin);

			//error in the hunter? -> hunter is not evaluated in 2020 and throws some errors
			//Console.WriteLine("---Evaluate vs Hunter---");
			//TestPOGame(nrGames, 1, player1, player2, player1Class, CardClass.HUNTER, player1Deck, Decks.MidrangeSecretHunter);
			//TestPOGame(nrGames, 2, player1, player2, player1Class, CardClass.HUNTER, player1Deck, Decks.MidrangeSecretHunter);

			Console.WriteLine("---Evaluate vs Warlock---");
			TestPOGame(nrGames, 1, player1, player2, player1Class, CardClass.WARLOCK, player1Deck, Decks.ZooDiscardWarlock);
			TestPOGame(nrGames, 2, player1, player2, player1Class, CardClass.WARLOCK, player1Deck, Decks.ZooDiscardWarlock);

			//error in the rougue?
			//Console.WriteLine("---Evaluate vs Rogue---");
			//TestPOGame(nrGames, 1, player1, player2, player1Class, CardClass.ROGUE, player1Deck, Decks.MiraclePirateRogue); //ERROR!
			//TestPOGame(nrGames, 2, player1, player2, player1Class, CardClass.ROGUE, player1Deck, Decks.MiraclePirateRogue);

			Console.WriteLine("---Evaluate vs Priest---");
			TestPOGame(nrGames, 1, player1, player2, player1Class, CardClass.PRIEST, player1Deck, Decks.RenoKazakusDragonPriest);
			TestPOGame(nrGames, 2, player1, player2, player1Class, CardClass.PRIEST, player1Deck, Decks.RenoKazakusDragonPriest);
			*/

			Console.WriteLine("Test ended!");
			Console.WriteLine("EXIT"); //print EXIT for the learning program to know when to stop it
			Console.ReadLine();
		}

		public static void TestTournament()
		{
			Agent[] agents = new Agent[2];
			//agents[0] = new Agent(typeof(RandomAgent), "Random Agent");
			//agents[1] = new Agent(typeof(GreedyAgent), "Greedy Agent");
			//agents[2] = new Agent(typeof(DynamicLookaheadAgent), "Dynamic Lookahead Agent");
			//agents[3] = new Agent(typeof(BeamSearchAgent), "Beam Search Agent");
			//agents[4] = new Agent(typeof(Iteration4), "Iteration4");
			agents[0] = new Agent(typeof(Iteration4), "Iteration4");
			agents[1] = new Agent(typeof(DynamicLookaheadAgent), "Dynamic Lookahead Agent");

			CompetitionEvaluation.Deck[] decks = new CompetitionEvaluation.Deck[6];
			decks[0] = new CompetitionEvaluation.Deck(Decks.RenoKazakusMage, CardClass.MAGE, "Mage");
			decks[1] = new CompetitionEvaluation.Deck(Decks.AggroPirateWarrior, CardClass.WARRIOR, "Warrior");
			decks[2] = new CompetitionEvaluation.Deck(Decks.MidrangeJadeShaman, CardClass.SHAMAN, "Shaman");
			decks[3] = new CompetitionEvaluation.Deck(Decks.ZooDiscardWarlock, CardClass.WARLOCK, "Warlock");
			decks[4] = new CompetitionEvaluation.Deck(Decks.RenoKazakusDragonPriest, CardClass.PRIEST, "Priest");
			decks[5] = new CompetitionEvaluation.Deck(Decks.MurlocDruid, CardClass.DRUID, "Druid");

			RoundRobinCompetition competition = new RoundRobinCompetition(agents, decks, "results.txt");
			competition.CreateTasks(100);
			competition.startEvaluation(8);

			Console.WriteLine("Total Games Played: " + competition.GetTotalGamesPlayed());
			competition.PrintAgentStats();
		}

		public static void TestPOGame(int numberOfRuns, int startPlayer, AbstractAgent p1, AbstractAgent p2, CardClass p1HeroClass, CardClass p2HeroClass, List<Card> p1Deck, List<Card> p2Deck)
		{
			Console.WriteLine("Setup gameConfig");

			var gameConfig = new GameConfig()
			{
				StartPlayer = startPlayer,
				Player1HeroClass = p1HeroClass,
				Player2HeroClass = p2HeroClass,
				Player1Deck = p1Deck,
				Player2Deck = p2Deck,
				FillDecks = false,
				Shuffle = true,
				Logging = true
			};

			Console.WriteLine("Setup POGameHandler");
			var gameHandler = new POGameHandler(gameConfig, p1, p2, repeatDraws: false);

			Console.WriteLine("Simulate Games");
			//gameHandler.PlayGame();
			gameHandler.PlayGames(nr_of_games: numberOfRuns, addResultToGameStats: true, debug: false);
			GameStats gameStats = gameHandler.getGameStats();

			gameStats.printResults();

			Console.WriteLine("Writing results to the file");

			//TODO: write the results here
			String wins = "";

			for(int i = 0; i < gameStats.PlayerA_Wins; i++)
				wins += "1";


			for(int i = 0; i < numberOfRuns - gameStats.PlayerA_Wins; i++)
				wins += "2";

			//write the results
			string outputFile = "/home/tobias/Develop/Java/CrushingBots/data/TRAINING_results.txt";
			using (StreamWriter sw = File.AppendText(outputFile))
			{
				sw.Write(wins);
			}

			Console.WriteLine("Test successful");
			//Console.ReadLine();
		}

		public static void RandomGames()
		{
			int total = 1;
			var watch = Stopwatch.StartNew();

			var gameConfig = new GameConfig()
			{
				StartPlayer = -1,
				Player1Name = "FitzVonGerald",
				Player1HeroClass = CardClass.PALADIN,
				Player1Deck = new List<Card>()
						{
						Cards.FromName("Blessing of Might"),
						Cards.FromName("Blessing of Might"),
						Cards.FromName("Gnomish Inventor"),
						Cards.FromName("Gnomish Inventor"),
						Cards.FromName("Goldshire Footman"),
						Cards.FromName("Goldshire Footman"),
						Cards.FromName("Hammer of Wrath"),
						Cards.FromName("Hammer of Wrath"),
						Cards.FromName("Hand of Protection"),
						Cards.FromName("Hand of Protection"),
						Cards.FromName("Holy Light"),
						Cards.FromName("Holy Light"),
						Cards.FromName("Ironforge Rifleman"),
						Cards.FromName("Ironforge Rifleman"),
						Cards.FromName("Light's Justice"),
						Cards.FromName("Light's Justice"),
						Cards.FromName("Lord of the Arena"),
						Cards.FromName("Lord of the Arena"),
						Cards.FromName("Nightblade"),
						Cards.FromName("Nightblade"),
						Cards.FromName("Raid Leader"),
						Cards.FromName("Raid Leader"),
						Cards.FromName("Stonetusk Boar"),
						Cards.FromName("Stonetusk Boar"),
						Cards.FromName("Stormpike Commando"),
						Cards.FromName("Stormpike Commando"),
						Cards.FromName("Stormwind Champion"),
						Cards.FromName("Stormwind Champion"),
						Cards.FromName("Stormwind Knight"),
						Cards.FromName("Stormwind Knight")
						},
				Player2Name = "RehHausZuckFuchs",
				Player2HeroClass = CardClass.PALADIN,
				Player2Deck = new List<Card>()
						{
						Cards.FromName("Blessing of Might"),
						Cards.FromName("Blessing of Might"),
						Cards.FromName("Gnomish Inventor"),
						Cards.FromName("Gnomish Inventor"),
						Cards.FromName("Goldshire Footman"),
						Cards.FromName("Goldshire Footman"),
						Cards.FromName("Hammer of Wrath"),
						Cards.FromName("Hammer of Wrath"),
						Cards.FromName("Hand of Protection"),
						Cards.FromName("Hand of Protection"),
						Cards.FromName("Holy Light"),
						Cards.FromName("Holy Light"),
						Cards.FromName("Ironforge Rifleman"),
						Cards.FromName("Ironforge Rifleman"),
						Cards.FromName("Light's Justice"),
						Cards.FromName("Light's Justice"),
						Cards.FromName("Lord of the Arena"),
						Cards.FromName("Lord of the Arena"),
						Cards.FromName("Nightblade"),
						Cards.FromName("Nightblade"),
						Cards.FromName("Raid Leader"),
						Cards.FromName("Raid Leader"),
						Cards.FromName("Stonetusk Boar"),
						Cards.FromName("Stonetusk Boar"),
						Cards.FromName("Stormpike Commando"),
						Cards.FromName("Stormpike Commando"),
						Cards.FromName("Stormwind Champion"),
						Cards.FromName("Stormwind Champion"),
						Cards.FromName("Stormwind Knight"),
						Cards.FromName("Stormwind Knight")
						},
				FillDecks = false,
				Shuffle = true,
				SkipMulligan = false,
				Logging = true,
				History = true
			};

			int turns = 0;
			int[] wins = new[] { 0, 0 };
			for (int i = 0; i < total; i++)
			{
				var game = new Game(gameConfig);
				game.StartGame();

				game.Process(ChooseTask.Mulligan(game.Player1, new List<int>()));
				game.Process(ChooseTask.Mulligan(game.Player2, new List<int>()));

				game.MainReady();

				while (game.State != State.COMPLETE)
				{
					List<PlayerTask> options = game.CurrentPlayer.Options();
					PlayerTask option = options[Rnd.Next(options.Count)];
					//Console.WriteLine(option.FullPrint());
					game.Process(option);


				}
				turns += game.Turn;
				if (game.Player1.PlayState == PlayState.WON)
					wins[0]++;
				if (game.Player2.PlayState == PlayState.WON)
					wins[1]++;
				Console.WriteLine("game ended");
				// Console.Write(game.PowerHistory.ToString());
				using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"powerhistory.log")) {
							file.WriteLine(game.PowerHistory.Print());
				}
				using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"logger.log"))
				{
					foreach (LogEntry log in game.Logs)
					{
						file.WriteLine(log.ToString());
					}
				}
			}

			watch.Stop();

			Console.WriteLine($"{total} games with {turns} turns took {watch.ElapsedMilliseconds} ms => " +
							  $"Avg. {watch.ElapsedMilliseconds / total} per game " +
							  $"and {watch.ElapsedMilliseconds / (total * turns)} per turn!");
			Console.WriteLine($"playerA {wins[0] * 100 / total}% vs. playerB {wins[1] * 100 / total}%!");
		}

		public static void OneTurn()
		{
			var game = new Game(
				new GameConfig()
				{
					StartPlayer = 1,
					Player1Name = "FitzVonGerald",
					Player1HeroClass = CardClass.WARRIOR,
					Player1Deck = Decks.AggroPirateWarrior,
					Player2Name = "RehHausZuckFuchs",
					Player2HeroClass = CardClass.SHAMAN,
					Player2Deck = Decks.MidrangeJadeShaman,
					FillDecks = false,
					Shuffle = false,
					SkipMulligan = false
				});
			game.Player1.BaseMana = 10;
			game.StartGame();

			var aiPlayer1 = new AggroScore();
			var aiPlayer2 = new AggroScore();

			game.Process(ChooseTask.Mulligan(game.Player1, aiPlayer1.MulliganRule().Invoke(game.Player1.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList())));
			game.Process(ChooseTask.Mulligan(game.Player2, aiPlayer2.MulliganRule().Invoke(game.Player2.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList())));

			game.MainReady();

			while (game.CurrentPlayer == game.Player1)
			{
				Console.WriteLine($"* Calculating solutions *** Player 1 ***");

				List<OptionNode> solutions = OptionNode.GetSolutions(game, game.Player1.Id, aiPlayer1, 10, 500);

				var solution = new List<PlayerTask>();
				solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);
				Console.WriteLine($"- Player 1 - <{game.CurrentPlayer.Name}> ---------------------------");

				foreach (PlayerTask task in solution)
				{
					Console.WriteLine(task.FullPrint());
					game.Process(task);
					if (game.CurrentPlayer.Choice != null)
						break;
				}
			}

			Console.WriteLine(game.Player1.HandZone.FullPrint());
			Console.WriteLine(game.Player1.BoardZone.FullPrint());
		}

		public static void FullGame()
		{
			var game = new Game(
				new GameConfig()
				{
					StartPlayer = 1,
					Player1Name = "FitzVonGerald",
					Player1HeroClass = CardClass.WARRIOR,
					Player1Deck = Decks.AggroPirateWarrior,
					Player2Name = "RehHausZuckFuchs",
					Player2HeroClass = CardClass.WARRIOR,
					Player2Deck = Decks.AggroPirateWarrior,
					FillDecks = false,
					Shuffle = true,
					SkipMulligan = false,
					History = false
				});
			game.StartGame();

			var aiPlayer1 = new AggroScore();
			var aiPlayer2 = new AggroScore();

			List<int> mulligan1 = aiPlayer1.MulliganRule().Invoke(game.Player1.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList());
			List<int> mulligan2 = aiPlayer2.MulliganRule().Invoke(game.Player2.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList());

			Console.WriteLine($"Player1: Mulligan {String.Join(",", mulligan1)}");
			Console.WriteLine($"Player2: Mulligan {String.Join(",", mulligan2)}");

			game.Process(ChooseTask.Mulligan(game.Player1, mulligan1));
			game.Process(ChooseTask.Mulligan(game.Player2, mulligan2));

			game.MainReady();

			while (game.State != State.COMPLETE)
			{
				Console.WriteLine("");
				Console.WriteLine($"Player1: {game.Player1.PlayState} / Player2: {game.Player2.PlayState} - " +
								  $"ROUND {(game.Turn + 1) / 2} - {game.CurrentPlayer.Name}");
				Console.WriteLine($"Hero[P1]: {game.Player1.Hero.Health} / Hero[P2]: {game.Player2.Hero.Health}");
				Console.WriteLine("");
				while (game.State == State.RUNNING && game.CurrentPlayer == game.Player1)
				{
					Console.WriteLine($"* Calculating solutions *** Player 1 ***");
					List<OptionNode> solutions = OptionNode.GetSolutions(game, game.Player1.Id, aiPlayer1, 10, 500);
					var solution = new List<PlayerTask>();
					solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);
					Console.WriteLine($"- Player 1 - <{game.CurrentPlayer.Name}> ---------------------------");
					foreach (PlayerTask task in solution)
					{
						Console.WriteLine(task.FullPrint());
						game.Process(task);
						if (game.CurrentPlayer.Choice != null)
						{
							Console.WriteLine($"* Recaclulating due to a final solution ...");
							break;
						}
					}
				}

				// Random mode for Player 2
				Console.WriteLine($"- Player 2 - <{game.CurrentPlayer.Name}> ---------------------------");
				while (game.State == State.RUNNING && game.CurrentPlayer == game.Player2)
				{
					//var options = game.Options(game.CurrentPlayer);
					//var option = options[Rnd.Next(options.Count)];
					//Log.Info($"[{option.FullPrint()}]");
					//game.Process(option);
					Console.WriteLine($"* Calculating solutions *** Player 2 ***");
					List<OptionNode> solutions = OptionNode.GetSolutions(game, game.Player2.Id, aiPlayer2, 10, 500);
					var solution = new List<PlayerTask>();
					solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);
					Console.WriteLine($"- Player 2 - <{game.CurrentPlayer.Name}> ---------------------------");
					foreach (PlayerTask task in solution)
					{
						Console.WriteLine(task.FullPrint());
						game.Process(task);
						if (game.CurrentPlayer.Choice != null)
						{
							Console.WriteLine($"* Recaclulating due to a final solution ...");
							break;
						}
					}
				}
			}
			Console.WriteLine($"Game: {game.State}, Player1: {game.Player1.PlayState} / Player2: {game.Player2.PlayState}");

		}

		public static void TestFullGames()
		{

			int maxGames = 100;
			int maxDepth = 10;
			int maxWidth = 14;
			int[] player1Stats = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			int[] player2Stats = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

			var gameConfig = new GameConfig()
			{
				StartPlayer = -1,
				Player1Name = "FitzVonGerald",
				Player1HeroClass = CardClass.PALADIN,
				Player1Deck = new List<Card>()
						{
						Cards.FromName("Blessing of Might"),
						Cards.FromName("Blessing of Might"),
						Cards.FromName("Gnomish Inventor"),
						Cards.FromName("Gnomish Inventor"),
						Cards.FromName("Goldshire Footman"),
						Cards.FromName("Goldshire Footman"),
						Cards.FromName("Hammer of Wrath"),
						Cards.FromName("Hammer of Wrath"),
						Cards.FromName("Hand of Protection"),
						Cards.FromName("Hand of Protection"),
						Cards.FromName("Holy Light"),
						Cards.FromName("Holy Light"),
						Cards.FromName("Ironforge Rifleman"),
						Cards.FromName("Ironforge Rifleman"),
						Cards.FromName("Light's Justice"),
						Cards.FromName("Light's Justice"),
						Cards.FromName("Lord of the Arena"),
						Cards.FromName("Lord of the Arena"),
						Cards.FromName("Nightblade"),
						Cards.FromName("Nightblade"),
						Cards.FromName("Raid Leader"),
						Cards.FromName("Raid Leader"),
						Cards.FromName("Stonetusk Boar"),
						Cards.FromName("Stonetusk Boar"),
						Cards.FromName("Stormpike Commando"),
						Cards.FromName("Stormpike Commando"),
						Cards.FromName("Stormwind Champion"),
						Cards.FromName("Stormwind Champion"),
						Cards.FromName("Stormwind Knight"),
						Cards.FromName("Stormwind Knight")
						},
				Player2Name = "RehHausZuckFuchs",
				Player2HeroClass = CardClass.PALADIN,
				Player2Deck = new List<Card>()
						{
						Cards.FromName("Blessing of Might"),
						Cards.FromName("Blessing of Might"),
						Cards.FromName("Gnomish Inventor"),
						Cards.FromName("Gnomish Inventor"),
						Cards.FromName("Goldshire Footman"),
						Cards.FromName("Goldshire Footman"),
						Cards.FromName("Hammer of Wrath"),
						Cards.FromName("Hammer of Wrath"),
						Cards.FromName("Hand of Protection"),
						Cards.FromName("Hand of Protection"),
						Cards.FromName("Holy Light"),
						Cards.FromName("Holy Light"),
						Cards.FromName("Ironforge Rifleman"),
						Cards.FromName("Ironforge Rifleman"),
						Cards.FromName("Light's Justice"),
						Cards.FromName("Light's Justice"),
						Cards.FromName("Lord of the Arena"),
						Cards.FromName("Lord of the Arena"),
						Cards.FromName("Nightblade"),
						Cards.FromName("Nightblade"),
						Cards.FromName("Raid Leader"),
						Cards.FromName("Raid Leader"),
						Cards.FromName("Stonetusk Boar"),
						Cards.FromName("Stonetusk Boar"),
						Cards.FromName("Stormpike Commando"),
						Cards.FromName("Stormpike Commando"),
						Cards.FromName("Stormwind Champion"),
						Cards.FromName("Stormwind Champion"),
						Cards.FromName("Stormwind Knight"),
						Cards.FromName("Stormwind Knight")
						},
				FillDecks = false,
				Shuffle = true,
				SkipMulligan = false,
				Logging = false,
				History = false
			};

			for (int i = 0; i < maxGames; i++)
			{
				var game = new Game(gameConfig);
				game.StartGame();

				var aiPlayer1 = new AggroScore();
				var aiPlayer2 = new AggroScore();

				List<int> mulligan1 = aiPlayer1.MulliganRule().Invoke(game.Player1.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList());
				List<int> mulligan2 = aiPlayer2.MulliganRule().Invoke(game.Player2.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList());

				game.Process(ChooseTask.Mulligan(game.Player1, mulligan1));
				game.Process(ChooseTask.Mulligan(game.Player2, mulligan2));

				game.MainReady();

				while (game.State != State.COMPLETE)
				{
					while (game.State == State.RUNNING && game.CurrentPlayer == game.Player1)
					{
						List<OptionNode> solutions = OptionNode.GetSolutions(game, game.Player1.Id, aiPlayer1, maxDepth, maxWidth);
						var solution = new List<PlayerTask>();
						solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);
						foreach (PlayerTask task in solution)
						{
							game.Process(task);
							if (game.CurrentPlayer.Choice != null)
								break;
						}
					}
					while (game.State == State.RUNNING && game.CurrentPlayer == game.Player2)
					{
						List<OptionNode> solutions = OptionNode.GetSolutions(game, game.Player2.Id, aiPlayer2, maxDepth, maxWidth);
						var solution = new List<PlayerTask>();
						solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);
						foreach (PlayerTask task in solution)
						{
							game.Process(task);
							if (game.CurrentPlayer.Choice != null)
								break;
						}
					}
				}

				player1Stats[(int)game.Player1.PlayState]++;
				player2Stats[(int)game.Player2.PlayState]++;

				Console.WriteLine($"{i}.Game: {game.State}, Player1: {game.Player1.PlayState} / Player2: {game.Player2.PlayState}");
			}

			Console.WriteLine($"Player1: {String.Join(",", player1Stats)}");
			Console.WriteLine($"Player2: {String.Join(",", player2Stats)}");
		}

	}
}
