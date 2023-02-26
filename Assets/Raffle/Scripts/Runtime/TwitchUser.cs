using System;
using UnityEngine;

namespace Raffle
{
    [Serializable]
    public class TwitchUser : IEquatable<TwitchUser>
    {
        public string DisplayName;
        public string UserId;
        public Color UserColor;

        public bool Equals(TwitchUser other)
        {
            if (other == null) return false;
            return this.UserId == other.UserId;
        }
    }
}
