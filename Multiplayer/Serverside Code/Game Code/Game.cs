using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PlayerIO.GameLibrary;
using System.Drawing;
using System.Security.Cryptography;

namespace Shooter {
    public class Player : BasePlayer {
        protected int command = 0;
        protected float posx = 0;
        protected float posy = 0;
        protected float rotate = 0;
        protected int health = 100;
        protected int score = 0;
        protected string room = "";

        private float[] bornPositionX = { -15f, -10f, 0f, 10f, 15f, 10f, 0f, -10f };
        private float[] bornPositionY = { 0f, 10f, 15f, 10f, 0f, -10f, 15f, -10f };
        private bool isAlive = true;

        public string status = "menu";

        public void Reborn() {
            posx = bornPositionX[this.command];
            posy = bornPositionY[this.command];
            health = 100;
            isAlive = true;
        }

        public bool isDeath() {
            if(health <= 0) {
                isAlive = false;
                return true;
            } else 
                return false;
            
        }

        public void Attack(int damage) {
            this.health -= damage;
        }

        // Getters
        public int GetHealth() {
            return this.health;
        }
        public int GetCommand() {
            return this.command;
        }
        public float GetPosX() {
            return this.posx;
        }
        public float GetPosY() {
            return this.posy;
        }
        public float GetRotate() {
            return this.rotate;
        }
        public string GetRoomId() {
            return this.room;
        }
        // Setters
        public void SetCommand(int id) {
            this.command = id;
        }
        public void SetPosX(float x) {
            this.posx = x;
        }
        public void SetPosY(float y) {
            this.posy = y;
        }
        public void SetRotate(float rotate) {
            this.rotate = rotate;
        }
        public void SetRoomId(string id) {
            this.room = id;
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
        }

        public override void UserLeft(Player player) {
            players.Remove(player);
            Broadcast("PlayerLeft", player.ConnectUserId);
        }

        public override void GotMessage(Player player, Message message) {
            switch(message.Type) {
                case "Create":
                Console.WriteLine("Send " + player.ConnectUserId);

                counterCommand++;
                if(counterCommand > 4)
                    counterCommand = 1;

                player.SetCommand(counterCommand);
                players.Add(player);
                player.Reborn();
                player.Send("Create", player.ConnectUserId, player.GetPosX(), player.GetPosY(), player.GetRotate(), player.GetCommand());
                if(players.Count != 0) {
                    foreach(Player pl in players) {
                        if(pl.ConnectUserId != player.ConnectUserId) {
                            pl.Send("PlayerJoined", player.ConnectUserId, player.GetPosX(), player.GetPosY(), player.GetRotate(), player.GetCommand());
                            player.Send("PlayerJoined", pl.ConnectUserId, pl.GetPosX(), pl.GetPosY(), pl.GetRotate(), pl.GetCommand());
                        }
                    }
                }
                break;
                case "Move":
                player.SetPosX(message.GetFloat(1));
                player.SetPosY(message.GetFloat(2));
                player.SetRotate(message.GetFloat(3));
                foreach(Player pl in players) {
                    if(pl != null)
                        if(pl.ConnectUserId != player.ConnectUserId) 
                            pl.Send("Move", player.ConnectUserId, player.GetPosX(), player.GetPosY(), player.GetRotate());
                }
                break;
                case "Chat":
                foreach(Player pl in players)
                    if(pl.ConnectUserId != player.ConnectUserId) 
                        pl.Send("Chat", player.ConnectUserId, message.GetString(0));
                break;
                case "Attack":
                Broadcast("Attack", player.ConnectUserId, message.GetFloat(1));
                break;
                case "hit":
                foreach(Player pl in players) {
                    if(pl.ConnectUserId == message.GetString(0)) {
                        pl.Attack(20);
                        if(pl.isDeath()) 
                            Broadcast("Death", message.GetString(0));
                        else 
                            pl.Send("hp", message.GetString(0), pl.GetHealth());  
                    }
                }
                break;
                case "Reborn":
                player.Reborn();
                Broadcast("Reborn", player.ConnectUserId, player.GetPosX(), player.GetPosY(), player.GetRotate(), player.GetCommand());
                break;
            }
        }
    }



    [RoomType("LobbyRoom")]
    public class LobbyCode : Game<Player> {

        public override void GameStarted() {
            Console.WriteLine("Lobby is started: " + RoomId);

        }

        public override void GameClosed() {
            Console.WriteLine("Lobby closed: " + RoomId);
        }

        public override void UserJoined(Player player) {

        }

        public override void UserLeft(Player player) {

        }

        public override void GotMessage(Player player, Message message) {
            switch(message.Type) {
                case "Access":
                player.SetRoomId(FindRoom(player));
                player.Send("Access", player.GetRoomId());
                break;
}
        }

        protected string FindRoom(Player player) {
            string name = "";
            PlayerIO.BigDB.LoadSingle(
            "roomsId",
            "isFull",
            new object[] { false },
            delegate (DatabaseObject result) {
                if(result != null) {
                    Console.WriteLine("Find room: " + result.GetString("id"));
                    result.Set("countPlayers", (result.GetInt("countPlayers") + 1));
                    if(result.GetInt("countPlayers") == 16)
                        result.Set("isFull", true);
                    result.Save();
                    name = result.Key;
                } else {
                    Console.WriteLine("Not find room");
                    DatabaseObject obj = new DatabaseObject();
                    obj.Set("id", player.ConnectUserId);
                    obj.Set("countPlayers", 1);
                    obj.Set("isFull", false);
                    PlayerIO.BigDB.CreateObject(
                        "roomsId",
                        "room" + new Random().Next(),
                        obj,
                        delegate (DatabaseObject res) {
                            Console.WriteLine("Create room - " + res.Key);
                            res.Save();
                            name = res.Key;
                        }
                    );
                }
            },
            delegate (PlayerIOError err) {
                Console.WriteLine("Not find room");
                DatabaseObject obj = new DatabaseObject();
                obj.Set("id", player.ConnectUserId);
                obj.Set("countPlayers", 1);
                obj.Set("isFull", false);
                PlayerIO.BigDB.CreateObject(
                    "roomsId",
                    "room" + new Random().Next(),
                    obj,
                    delegate (DatabaseObject result) {
                        Console.WriteLine("Create room - " + result.Key);
                        result.Save();
                        name = result.Key;
                    }
                );
            }
        );
            return name;
        }
    }
}