using System.Collections.Generic;
using UnityEngine;

// TODO: Rewrite entire UserManager
namespace Raffle
{
    public class UserManager : Singleton<UserManager>
    {
        private const string SaveDataPath = "Saves/Users";

        private List<GameUser> m_KnownUsers = new List<GameUser>();
        private List<GameUser> m_CurrentUsers = new List<GameUser>();

        public void OnMeCommand(TwitchUser user)
        {
            GameUser gameUser = FindUser(user);
            switch (gameUser.Status)
            {
                case GameUserStatus.OUT:
                    gameUser.Status = GameUserStatus.ME;
                    m_CurrentUsers.Add(gameUser);
                    break;
                case GameUserStatus.JOIN:
                case GameUserStatus.ME:
                default:
                    break;
            }
        }

        public void OnJoinCommand(TwitchUser user)
        {
            GameUser gameUser = FindUser(user);
            switch (gameUser.Status)
            {
                case GameUserStatus.OUT:
                    gameUser.Status = GameUserStatus.JOIN;
                    m_CurrentUsers.Add(gameUser);
                    break;
                case GameUserStatus.JOIN:
                case GameUserStatus.ME:
                default:
                    break;
            }
        }

        public void OnLeaveCommand(TwitchUser user)
        {
            GameUser gameUser = FindUser(user);
            switch (gameUser.Status)
            {
                case GameUserStatus.JOIN:
                case GameUserStatus.ME:
                    m_CurrentUsers.Remove(gameUser);
                    gameUser.Status = GameUserStatus.OUT;
                    break;
                case GameUserStatus.OUT:
                default:
                    break;
            }
        }

        public void OnBallsCommand(TwitchUser user, TwitchConnection connection)
        {
            GameUser gameUser = FindUser(user);
            ushort balls = 0;
            ushort savedBalls = 0;
            switch (gameUser.Status)
            {
                case GameUserStatus.JOIN:
                    savedBalls = gameUser.Balls;
                    balls = 1;
                    break;
                case GameUserStatus.ME:
                    savedBalls = gameUser.Balls;
                    balls = (ushort)(savedBalls + 1);
                    break;
                case GameUserStatus.OUT:
                    savedBalls = gameUser.Balls;
                    balls = 0;
                    break;
                default:
                    return;
            }

            string messageToFormat = "{0} you have {1} active {2} and {3} saved {4}";
            string ballWord = balls == 1 ? "ball" : "balls";
            string savedBallWord = savedBalls == 1 ? "ball" : "balls";

            string formattedMessage = string.Format(messageToFormat, gameUser.DisplayName, balls, ballWord, savedBalls, savedBallWord);

            connection.SendChatMessage(formattedMessage);
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            LoadFromSavedData();
        }

        /// <summary>
        /// Callback sent to all game objects before the application is quit.
        /// </summary>
        void OnApplicationQuit()
        {
            SaveToSavedData();
        }

        private void LoadFromSavedData()
        {
            SaveData data = SaveSystem.LoadData<SaveData>(SaveDataPath);
            if (data != null)
            {
                m_KnownUsers = data.Users;
            }
        }

        private void SaveToSavedData()
        {
            SaveData data = new SaveData { Users = m_KnownUsers };
            SaveSystem.SaveData(SaveDataPath, data);
        }

        public void HandleWin(TwitchUser winningUser)
        {
            foreach (var user in m_CurrentUsers)
            {
                if (user.Equals(winningUser))
                {
                    user.Balls = 0;
                }
                else
                {
                    if (user.Status == GameUserStatus.ME)
                    {
                        user.Balls++;
                    }
                }
            }

            ClearCurrentUsers();
        }

        private void ClearCurrentUsers()
        {
            foreach (var user in m_CurrentUsers)
            {
                user.Status = GameUserStatus.OUT;
            }

            m_CurrentUsers.Clear();
        }

        private GameUser FindUser(TwitchUser user)
        {
            GameUser gameUser = null;

            // First, check if they're currently in
            gameUser = FindUser(user, m_CurrentUsers);
            if (gameUser != null)
            {
                gameUser.UpdateUser(user);
                return gameUser;
            }

            // Second, check if we know about them
            gameUser = FindUser(user, m_KnownUsers);
            if (gameUser != null)
            {
                gameUser.UpdateUser(user);
                return gameUser;
            }

            // Finally, create them as a new user
            gameUser = new GameUser { DisplayName = user.DisplayName, UserId = user.UserId, UserColor = user.UserColor };
            m_KnownUsers.Add(gameUser);
            return gameUser;
        }

        private GameUser FindUser(TwitchUser user, List<GameUser> list)
        {
            foreach (var gameUser in list)
            {
                if (gameUser.Equals(user))
                {
                    return gameUser;
                }
            }

            return null;
        }
    }
}
