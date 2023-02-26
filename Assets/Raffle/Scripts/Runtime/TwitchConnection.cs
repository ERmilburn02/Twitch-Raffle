using UnityEngine;
using UnityEngine.Events;
using TwitchLib.Unity;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;

namespace Raffle
{
    public class TwitchConnection : MonoBehaviour
    {
        public UnityEvent<TwitchUser> OnMeCommand;
        public UnityEvent<TwitchUser> OnJoinCommand;
        public UnityEvent<TwitchUser> OnLeaveCommand;
        public UnityEvent<TwitchUser, TwitchConnection> OnBallsCommand;

        [SerializeField]
        private string m_ChannelToConnectTo = Secrets.USERNAME_FROM_OAUTH_TOKEN;

        private Client m_Client;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(Secrets.USERNAME_FROM_OAUTH_TOKEN, Secrets.OAUTH_TOKEN);

            m_Client = new Client();
            m_Client.Initialize(credentials, m_ChannelToConnectTo);

            m_Client.OnConnected += OnConnected;
            m_Client.OnDisconnected += OnDisconnected;
            m_Client.OnChatCommandReceived += OnChatCommandReceived;

            m_Client.Connect();
        }

        /// <summary>
        /// Callback sent to all game objects before the application is quit.
        /// </summary>
        private void OnApplicationQuit()
        {
            m_Client.OnConnected -= OnConnected;
            m_Client.OnDisconnected -= OnDisconnected;
            m_Client.OnChatCommandReceived -= OnChatCommandReceived;

            m_Client.Disconnect();
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            Debug.Log($"{e.BotUsername} successfully connected to Twitch");
        }

        private void OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            Debug.Log($"Disconnected from Twitch");
        }

        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            string command = e.Command.CommandText.ToLower();
            Color userColor = Color.black;
            string colorHex = e.Command.ChatMessage.ColorHex;
            if (ColorUtility.TryParseHtmlString(colorHex, out Color color))
            {
                userColor = color;
            }
            else
            {
                // Debug.Break();
                Debug.LogWarning($"Unable to parse color: {colorHex}");
            }

            TwitchUser user = new TwitchUser { DisplayName = e.Command.ChatMessage.DisplayName, UserId = e.Command.ChatMessage.UserId, UserColor = userColor };

            switch (command)
            {
                case "me":
                    {
                        OnMeCommand?.Invoke(user);
                    }
                    break;
                case "join":
                    {
                        OnJoinCommand?.Invoke(user);
                    }
                    break;
                case "leave":
                    {
                        OnLeaveCommand?.Invoke(user);
                    }
                    break;
                case "balls":
                    {
                        OnBallsCommand?.Invoke(user, this);
                    }
                    break;
                default:
                    break;
            }
        }

        public void SendChatMessage(string message)
        {
            m_Client.SendMessage(m_ChannelToConnectTo, message);
        }
    }
}
