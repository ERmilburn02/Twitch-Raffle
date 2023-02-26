# Twitch Raffle

## Setup Instructions

To get started with this, you'll need to fill in the values in [Secrets.cs.example](Assets/Raffle/Scripts/Runtime/Secrets.cs.example) with your own Twitch credentials. Here's how to do it:

1. Rename the file from `Secrets.cs.example` to `Secrets.cs`.
2. Open the `Secrets.cs` file in your preferred text editor.
3. Replace the placeholder values in the file with your own Twitch credentials. Specifically, you'll need to replace the following values:
   - `CLIENT_ID`: Your application's client ID, which you can register at https://dev.twitch.tv/dashboard.
   - `OAUTH_TOKEN`: An OAuth token which can be used to connect to the Twitch chat. You can generate one at https://twitchapps.com/tmi/.
   - `USERNAME_FROM_OAUTH_TOKEN`: The username associated with the OAuth token you generated in step 3.
   - `CHANNEL_ID_FROM_OAUTH_TOKEN`: The channel ID for the Twitch channel you want to connect to. You can find it using https://www.streamweasels.com/tools/convert-twitch-username-to-user-id/.

Once you've filled in the values in the `Secrets.cs` file, you should be ready to run the program! If you have any trouble with the setup process, please don't hesitate to ask for help.

## License

This project is available under the MIT license. See the [LICENSE](LICENSE) file for more info.
