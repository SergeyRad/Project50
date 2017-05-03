using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PlayerIO.GameLibrary;
using System.Drawing;

namespace Shooter {
	public class Player : BasePlayer {
        public int idCommand = 0;
		public float posx = 0;
		public float posy = 0;
        public float rotate = 0;
        public float healht = 100f;
        public int score = 0;

        private float[] bornPositionX = { -15f, -10f, 0f, 10f, 15f, 10f, 0f, -10f };
        private float[] bornPositionY = { 0f, 10f, 15f, 10f, 0f, -10f, 15f, -10f };
        private bool islive = true;

        public string status = "menu";

        public void Reborn() {
            posx = bornPositionX[idCommand];
            posy = bornPositionY[idCommand];
            healht = 100f;
            islive = true;
        }

        public bool isDeath() {
            if (healht < 0) {
                islive = false;
                return true;
            } else {
                return false;
            }
        }
	}

    [RoomType("GameRoom")]
	public class GameCode : Game<Player> {

		private List<Player> players = new List<Player>();
        private string typeGame;
        private int counterCommand = 0;
		
		public override void GameStarted() {
			Console.WriteLine("Game is started: " + RoomId);
			
		}
		
		public override void GameClosed() {
			Console.WriteLine("RoomId: " + RoomId);
		}
		
		public override void UserJoined(Player player) {
            counterCommand++;
            if (counterCommand > 4) {
                counterCommand = 1;
            }
            player.idCommand = counterCommand;
            
            players.Add(player);
            Console.WriteLine("User Jointed " + player.ConnectUserId);
            foreach (Player pl in players) {
                if (pl.ConnectUserId != player.ConnectUserId) {
                    pl.Send("PlayerJoined", player.ConnectUserId, player.posx, player.posy, player.rotate);
                    player.Send("PlayerJoined", pl.ConnectUserId, pl.posx, pl.posy, pl.rotate);
                }
            }
        }
		
		public override void UserLeft(Player player) {
			players.Remove(player);
			Broadcast("PlayerLeft", player.ConnectUserId);
		}
		
		public override void GotMessage(Player player, Message message) {
			switch(message.Type) {
                case "Create":
                    Console.WriteLine("Send " + player.ConnectUserId);
                    player.Reborn();
                    player.Send("Create", player.ConnectUserId, player.posx, player.posy, player.rotate);
                    break;
				case "Move":
                    player.posx = message.GetFloat(1);
					player.posy = message.GetFloat(2);
                    player.rotate = message.GetFloat(3);
					foreach(Player pl in players) {
						if(pl.ConnectUserId != player.ConnectUserId) {
							pl.Send("Move", player.ConnectUserId, player.posx, player.posy, player.rotate);
						}
					}
					break;
				case "Chat":
					foreach(Player pl in players) {
						if(pl.ConnectUserId != player.ConnectUserId) {
							pl.Send("Chat", player.ConnectUserId, message.GetString(0));
						}
					}
					break;
                case "Attack":
                    foreach (Player pl in players)
                    {
                        if (pl.ConnectUserId != player.ConnectUserId)
                        {
                            pl.Send("Attack", player.ConnectUserId, message.GetFloat(1), message.GetFloat(2));
                        }
                    }
                    break;
            }
		}
	}

    [RoomType("LobbyRoom")]
    public class LobbyCode: Game<Player>
    {
        
        public override void GameStarted()
        {
            Console.WriteLine("Lobby is started: " + RoomId);

        }

        public override void GameClosed()
        {
            Console.WriteLine("Lobby closed: " + RoomId);
        }

        public override void UserJoined(Player player)
        {
            
        }

        public override void UserLeft(Player player)
        {
            
        }

        public override void GotMessage(Player player, Message message)
        {
            switch (message.Type)
            {
                case "Access":
                    if (player.status != "game") {
                        player.status = "game";
                        player.Send("Access", message.GetString(0));
                    }
                    break;
                
            }
        }
    }
}