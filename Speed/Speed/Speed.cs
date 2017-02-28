
/* 
     ________  ___  _______   ________      
    |\   __  \|\  \|\  ___ \ |\   __  \     
    \ \  \|\  \ \  \ \   __/|\ \  \|\  \    
     \ \   ____\ \  \ \  \_|/_\ \   __  \   
      \ \  \___|\ \  \ \  \_|\ \ \  \ \  \  
       \ \__\    \ \__\ \_______\ \__\ \__\ 
        \|__|     \|__|\|_______|\|__|\|__|           
      
    PIEA, The MiNET plugins development organization.                           
*/

using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Utils;
using MiNET.Worlds;

using System;
using System.Collections.Generic;
using System.Timers;

namespace Speed
{
    [Plugin(PluginName = "Speed", Description = "Print speed.", PluginVersion = "1.0", Author = "PIEA Organization")]
    public class Speed : Plugin
    {
        private Dictionary<Player, PlayerLocation> LatestLocation = new Dictionary<Player, PlayerLocation>();

        protected override void OnEnable()
        {
            Context.Server.PlayerFactory.PlayerCreated += (sender, args) =>
            {
                Player player = args.Player;
                player.PlayerJoin += PlayerJoin;
            };

            Timer task = new Timer();

            task.Elapsed += new ElapsedEventHandler(onTick);
            task.Interval = 1000;
            task.Enabled = true;
        }

        private void PlayerJoin(object sender, PlayerEventArgs eventArgs)
        {
            Player player = eventArgs.Player;

            LatestLocation.Add(player, player.KnownPosition);
        }

        private void onTick(object sender, ElapsedEventArgs eventArgs)
        {
            foreach(Level level in Context.Server.LevelManager.Levels)
            {
                foreach(var player in level.Players)
                {
                    if (LatestLocation.ContainsKey(player.Value))
                    {
                        Player target = player.Value;

                        double x = Math.Abs(LatestLocation[target].X - target.KnownPosition.X);
                        double y = Math.Abs(LatestLocation[target].Y - target.KnownPosition.Y);
                        double z = Math.Abs(LatestLocation[target].Z - target.KnownPosition.Z);

                        double line = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(z, 2));
                        double diagonal = Math.Sqrt(Math.Pow(line, 2) + Math.Pow(y, 2));

                        double speed = Math.Round((diagonal / 1000) * 3600, 2);

                        if(speed > 40) target.SendMessage(ChatColors.Red + speed + " km/h", MessageType.Popup);
                        else target.SendMessage(ChatColors.Green + speed + " km/h", MessageType.Popup);

                        LatestLocation.Remove(target);
                        LatestLocation.Add(target, target.KnownPosition);
                    }
                }
            }
        }
    }
}
