using System;
using System.IO;
using System.Collections.Generic;
using SabberStoneCore.Enums;
using SabberStoneBasicAI.Score;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneBasicAI.PartialObservation;

using SabberStoneCore.Model.Entities;
using SabberStoneCore.Model.Zones;
using SabberStoneBasicAI.AIAgents;

using System.Linq;






//Controller: no health
//no new PlayerTask endTurn


// TODO choose your own namespace by setting up <submission_tag>
// each added file needs to use this namespace or a subnamespace of it
namespace SabberStoneBasicAI.AIAgents.Iteration4
{
	class Iteration4 : AbstractAgent
	{

		//the variables------------------------------------------------------------------------------
		double[] factors = read_params();

		//variables for training:
		static string inputFile = "/home/tobias/Develop/Java/CrushingBots/data/TRAINING_single.txt";

		bool won = false;
		//\the variables

		//the helper methods-------------------------------------------------------------------------
		private int getTurn(POGame poGame){
			return poGame.Turn;
		}
		private int getMinionStrehgth(Controller player){
			BoardZone boardZone = player.BoardZone;
			int damage = 0;

			foreach(Minion m in boardZone.GetAll()){//NOTE: does not get dormant minions
				damage += m.AttackDamage;
			}

			return damage;
		}

		private int getMinionHealth(Controller player){
			BoardZone boardZone = player.BoardZone;
			int health = 0;

			foreach(Minion m in boardZone.GetAll()){//NOTE: does not get dormant minions
				health += m.Health;
			}

			return health;
		}

		private int getPlayerHealth(Controller player){
			return player.Hero.Health;
		}

		private int getPlayerArmor(Controller player){
			return player.Hero.Armor;
		}

		private int getPlayerAttackDamage(Controller player){
			return player.Hero.AttackDamage;
		}

		private int getCardsLeftInDeck(Controller player){
			DeckZone deckZone = player.DeckZone;
			return deckZone.Count;
		}

		private int getActiveSecrits(Controller player){
			SecretZone secretZone = player.SecretZone;
			return secretZone.Count;
		}

		private int getHandCardCount(Controller player){
			HandZone handZone = player.HandZone;
			return handZone.Count;
		}

		private int getMana(Controller player){
			return player.RemainingMana;
		}

		private bool hasLost(Controller player) //NOTE: unused
		{
			if (getPlayerHealth(player) <= 0)
				return true;

			return false;
		}

		//\the helper methods

		//the worth functions------------------------------------------------------------------------

		private double getWorth(POGame poGame, int playerId){//TODO: implement
			Controller own = poGame.CurrentPlayer.PlayerId == playerId ? poGame.CurrentPlayer : poGame.CurrentOpponent;
			Controller enemy = poGame.CurrentPlayer.PlayerId == playerId ? poGame.CurrentOpponent : poGame.CurrentPlayer;

			//variables bevore the task execution
			int turn = getTurn(poGame);


			double ownStrengthFactor = factors[0] * (getMinionStrehgth(own) + getPlayerAttackDamage(own)) + factors[1] * getMinionHealth(own);
			double ownHealthFactor = factors[2] * (getPlayerArmor(own) + getPlayerHealth(own));
			double ownPotentialFactor = factors[3] * getMana(own) + factors[4] * getHandCardCount(own);
			double ownFactor = (ownStrengthFactor + ownHealthFactor + ownPotentialFactor) * factors[5];

			double enemyStrengthFactor = factors[6] * (getMinionStrehgth(enemy) + getPlayerAttackDamage(enemy)) + factors[7] * getMinionHealth(enemy);
			double enemyHealthFactor = factors[8] * (getPlayerArmor(enemy) + getPlayerHealth(enemy));
			double enemyPotentialFactor = factors[9] * getMana(enemy) + factors[10] * getHandCardCount(enemy);
			double enemyFactor = (enemyStrengthFactor + enemyHealthFactor + enemyPotentialFactor) * factors[11];

			return ownFactor + enemyFactor;
			//unused potential:

			//GraveyardZone myGraveyardZone = poGame.CurrentPlayer.GraveyardZone;
			//GraveyardZone enemyGraveyardZone = poGame.CurrentOpponent.GraveyardZone;

			//concider potential synergy effects from cards (like boost for dragon cards)
			//concider evvects like shield or the unbreakable thing

			//Concider shield/armor(?) on the health of the own player
		}

		//\the worth functions

		//the read function for training
		private void writeFile(string text){

		}
		private static double[] read_params() {
        	double[] weights = {1,1,1,1,1,1, -1,-1,-1,-1,-1,1};//new double[100];

			try {
				string[] lines = System.IO.File.ReadAllLines(inputFile);
				weights = new double[lines.Length];

				for(int i = 0; i < lines.Length; i++)
				{
					weights[i] = Double.Parse(lines[i]);
				}
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}

			//Shaman config: (seems to work well whith warlogs):
			//double[] weights = {1.809399014309586, 2.0650153351224905, 1.1092351502828166, 0.08435252105567392, 0.5252287043438756, 2.252524841811269, -2.7067056150354363, -0.8145803639268613, -0.4538593110719923, -0.4387234329831097, -0.3953901539662016, 2.207155696632741};
        	return weights;
    	}
		//\the read and write functions

		//TODO: a selector method for different deck types and stuff

		public override void InitializeAgent()
		{//nothing to do
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
			int myPlayerId = poGame.CurrentPlayer.PlayerId;
			List<PlayerTask> options = poGame.CurrentPlayer.Options();

			// Implement a simple Mulligan Rule
			if (poGame.CurrentPlayer.MulliganState == Mulligan.INPUT)
			{
				List<int> mulligan = new AggroScore().MulliganRule().Invoke(poGame.CurrentPlayer.Choice.Choices.Select(p => poGame.getGame().IdEntityDic[p]).ToList());//all mulligan handlers are the same for each score
				return ChooseTask.Mulligan(poGame.CurrentPlayer, mulligan);
			}



			var simulationResults = poGame.Simulate(options);

			double bestWorth = getWorth(poGame, myPlayerId); //best worth starts with the current field
			PlayerTask bestTask = null;

			foreach(PlayerTask t in options){

				double resultingWorth = Double.NegativeInfinity;
				if(simulationResults.ContainsKey(t) && t.PlayerTaskType != PlayerTaskType.END_TURN){
					POGame resultingGameState = simulationResults[t];
					resultingWorth = getWorth(resultingGameState, myPlayerId);
				}
				else{ //TODO: think of something to do if the key is unvalid
					//for now, do nothing if the resulting value is negative
				}

				if(bestWorth < resultingWorth){
					bestWorth = resultingWorth;
					bestTask = t;
				}
			}

			if(bestTask == null)
				return EndTurnTask.Any(poGame.CurrentPlayer);

			return bestTask;
		}

	}
}


//Notes for how to approach approximating the parameters for cards in unknown decks:
//Think of it like in a real person:
//In the beginning, the worth of each card must be learned in at least some scenarios
//Then, the player knows if a card can be effective for example in the early game/in aggro decks...
//The same thing could be implemented for the functions: Look, which decks i have already learned for a given card/given cards and approximate the values from that
//Of course, cards that i have never seen bevore can not be evaluated, but that is also true for the player in the real world. The value could just be approximated using the values from similar cards
