﻿using System;
using System.Collections.Generic;
using System.Linq;//for poGame.Minions.Contains(task.Target) ~ 47
using System.Text;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks;
using SabberStoneBasicAI.AIAgents;
using SabberStoneCore.Tasks.PlayerTasks;

using SabberStoneBasicAI.PartialObservation;


//My Bot from 2018 (was bad)
//Developed by Alex Voigt and Tobias Benecke
namespace SabberStoneBasicAI.AIAgents
{
	class BordControl : AbstractAgent
	{
		private Random Rnd = new Random();
		public override void InitializeAgent()
		{
			Rnd = new Random();
		}

		public override void FinalizeAgent()
		{//nothing to do
		}

		public override void InitializeGame()
		{ //nothing to do
		}

		public override void FinalizeGame()
		{ //nothing to do
		}

		public override PlayerTask GetMove(POGame poGame)
		{


			switch (poGame.CurrentPlayer.HeroClass)
			{
				case SabberStoneCore.Enums.CardClass.SHAMAN:
					return computeShamanMove(poGame);
				case SabberStoneCore.Enums.CardClass.WARRIOR:
					return computeWarriorMove(poGame);
				default:
					return computeMove(poGame);

			}
		}

		//own method
		private PlayerTask computeMove(POGame poGame)
		{//strategie: bord control; play monsters an d kill enemy monsters
			List<PlayerTask> options = poGame.CurrentPlayer.Options();

			//strategy: in the first two rounds play a minion, then go face
			if (poGame.Turn > 2)
			{
				//can i play a minion?
				List<PlayerTask> minionsToSummon = filterTasks(options, PlayerTaskType.PLAY_CARD);
				if (minionsToSummon.Count > 0)//TODO: chose the best card for face
					return getBestManaValue(minionsToSummon, poGame.CurrentPlayer.RemainingMana);

				//can i go face?
				List<PlayerTask> attackFace = filterTasks(options, poGame.CurrentOpponent.Hero);
				if (attackFace.Count > 0)//TODO: chose the best card for face
					return getBestManaValue(attackFace, poGame.CurrentPlayer.RemainingMana);

				//is there a minion i can kill?
				List<PlayerTask> attackMinion = filterTasks(options, PlayerTaskType.MINION_ATTACK);
				if (attackMinion.Count > 0)//TODO: compute best value
					return attackMinion[Rnd.Next(attackMinion.Count)];
			}
			else
			{
				//can i go face?
				List<PlayerTask> attackFace = filterTasks(options, poGame.CurrentOpponent.Hero);
				if (attackFace.Count > 0)//TODO: chose the best card for face
					return getBestManaValue(attackFace, poGame.CurrentPlayer.RemainingMana);

				//can i play a minion?
				List<PlayerTask> minionsToSummon = filterTasks(options, PlayerTaskType.PLAY_CARD);
				if (minionsToSummon.Count > 0)//TODO: chose the best card for face
					return getBestManaValue(minionsToSummon, poGame.CurrentPlayer.RemainingMana);

				//is there a minion i can kill?
				List<PlayerTask> attackMinion = filterTasks(options, PlayerTaskType.MINION_ATTACK);
				if (attackMinion.Count > 0)//TODO: compute best value
					return attackMinion[Rnd.Next(attackMinion.Count)];

			}

			//if all else fails: go Random, but dont end turn until you have to
			if (options.Count > 1)
			{
				// filter all non EndTurn Tasks
				List<PlayerTask> validTasks = new List<PlayerTask>();
				foreach (PlayerTask task in options)
				{
					if (task.PlayerTaskType != PlayerTaskType.END_TURN)
						validTasks.Add(task);
				}
				return validTasks[Rnd.Next(validTasks.Count)];
			}
			return options[0];
		}

		private PlayerTask computeShamanMove(POGame poGame)
		{
			List<PlayerTask> options = poGame.CurrentPlayer.Options();

			//can i go face?
			List<PlayerTask> attackFace = filterTasks(options, poGame.CurrentOpponent.Hero);
			if (attackFace.Count > 0)//TODO: chose the best card for face
				return getBestManaValue(attackFace, poGame.CurrentPlayer.RemainingMana);

			//can i play a minion?
			List<PlayerTask> minionsToSummon = filterTasks(options, PlayerTaskType.PLAY_CARD);
			if (minionsToSummon.Count > 0)//TODO: chose the best card for face
				return getBestManaValue(minionsToSummon, poGame.CurrentPlayer.RemainingMana);

			//is there a minion i can kill?
			List<PlayerTask> attackMinion = filterTasks(options, PlayerTaskType.MINION_ATTACK);
			if (attackMinion.Count > 0)//TODO: compute best value
				return attackMinion[Rnd.Next(attackMinion.Count)];

			//if all else fails: go Random, but dont end turn until you have to
			if (options.Count > 1)
			{
				// filter all non EndTurn Tasks
				List<PlayerTask> validTasks = new List<PlayerTask>();
				foreach (PlayerTask task in options)
				{
					if (task.PlayerTaskType != PlayerTaskType.END_TURN)
						validTasks.Add(task);
				}
				return validTasks[Rnd.Next(validTasks.Count)];
			}
			return options[0];
		}

		private PlayerTask computeWarriorMove(POGame poGame)
		{
			List<PlayerTask> options = poGame.CurrentPlayer.Options();

			foreach (PlayerTask task in options)
			{
				if (task.FullPrint().Contains("The Coin"))
					return task;
			}

			//if (poGame.FirstPlayer.Name == "Player2" && poGame.Turn < 2)
			//{

			//}

			////is there a minion i can kill?
			//List<PlayerTask> attackMinion = filterTasks(options, PlayerTaskType.MINION_ATTACK);
			//if (attackMinion.Count > 0)//TODO: compute best value
			//	return getBestManaValue(attackMinion, poGame.CurrentPlayer.RemainingMana);



			//can i play a aggroPirateWarrior card?
			foreach (PlayerTask task in options)
			{
				string s = task.FullPrint();
				if (task.FullPrint().Contains("Pirate"))
					return task;
			}

			//can i play a minion?
			List<PlayerTask> minionsToSummon = filterTasks(options, PlayerTaskType.PLAY_CARD);
			if (minionsToSummon.Count > 0)//TODO: chose the best card for face
				return getBestManaValueWarrior(minionsToSummon, poGame.CurrentPlayer.RemainingMana);

			//can i play strike?
			foreach (PlayerTask task in options)
			{
				string s = task.FullPrint();
				if (task.FullPrint().Contains("Heroic Strike"))
					return task;
			}

			//can i go face?
			List<PlayerTask> attackFace = filterTasks(options, poGame.CurrentOpponent.Hero);
			if (attackFace.Count > 0)//TODO: chose the best card for face
				return getBestManaValueWarrior(attackFace, poGame.CurrentPlayer.RemainingMana);


			//if all else fails: go Random, but dont end turn until you have to
			if (options.Count > 1)
			{
				// filter all non EndTurn Tasks
				List<PlayerTask> validTasks = new List<PlayerTask>();
				foreach (PlayerTask task in options)
				{
					if (task.PlayerTaskType != PlayerTaskType.END_TURN)
						validTasks.Add(task);
				}
				return validTasks[Rnd.Next(validTasks.Count)];
			}
			return options[0];
		}

		//filter for tasks
		private List<PlayerTask> filterTasks(List<PlayerTask> tasks, PlayerTaskType type)
		{
			List<PlayerTask> filtertTasks = new List<PlayerTask>();

			foreach (PlayerTask task in tasks)
			{
				if (task.PlayerTaskType == type)
				{
					filtertTasks.Add(task);
				}
			}

			return filtertTasks;
		}

		//filter for target
		private List<PlayerTask> filterTasks(List<PlayerTask> tasks, IEntity target)
		{
			List<PlayerTask> filtertTasks = new List<PlayerTask>();

			foreach (PlayerTask task in tasks)
			{
				if (task.Target == target)
				{
					filtertTasks.Add(task);
				}
			}

			return filtertTasks;
		}

		private PlayerTask getBestManaValue(List<PlayerTask> tasks, int currentMana)
		{

			return tasks[0];
		}

		private PlayerTask getBestManaValueWarrior(List<PlayerTask> tasks, int currentMana)
		{
			PlayerTask bestMove = tasks[0];

			return bestMove;
		}

	}
}
