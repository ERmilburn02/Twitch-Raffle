using System.Collections.Generic;
using UnityEngine;

// TODO: Rewrite entire UserManager
namespace Raffle
{
    public class UserManager : Singleton<UserManager>
    {
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
                if (((TwitchUser)gameUser).Equals(user))
                {
                    return gameUser;
                }
            }

            return null;
        }
    }
}
