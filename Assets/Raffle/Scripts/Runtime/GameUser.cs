using System;
using UnityEngine;

namespace Raffle
{
    [Serializable]
    public class GameUser : TwitchUser
    {
        // Inherited
        // public string DisplayName;
        // public string UserId;
        // public Color UserColor;

        public ushort Balls;
        public GameUserStatus Status;

        /// <summary>
        /// Updates the Display Name and User Color
        /// </summary>
        public void UpdateUser(TwitchUser user)
        {
            DisplayName = user.DisplayName;
            UserColor = user.UserColor;
        }
    }
}
