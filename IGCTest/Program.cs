using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        int _runcount = 0;
        string _broadCastTag = "MDK IGC EXAMPLE 1";
        IMyBroadcastListener _myBroadcastListener;

        public Program()
        {
            Echo("Creator");
            _myBroadcastListener = IGC.RegisterBroadcastListener(_broadCastTag);
            _myBroadcastListener.SetMessageCallback(_broadCastTag);
        }

        public void Main(string argument, UpdateType updateSource)
        {
            _runcount++;
            Echo(_runcount.ToString() + ":" + updateSource.ToString());

            if (
                (updateSource & (UpdateType.Trigger | UpdateType.Terminal)) > 0
                || (updateSource & (UpdateType.Mod)) > 0
                || (updateSource & (UpdateType.Script)) > 0
                )
            {
                if (argument != "")
                {
                    IGC.SendBroadcastMessage(_broadCastTag, argument);
                    Echo("Sending message:\n" + argument);
                }
            }

            if ((updateSource & UpdateType.IGC) > 0)
            {
                while (_myBroadcastListener.HasPendingMessage)
                {
                    MyIGCMessage myIGCMessage = _myBroadcastListener.AcceptMessage();
                    if (myIGCMessage.Tag == _broadCastTag)
                    { // This is our tag
                        if (myIGCMessage.Data is string)
                        {
                            string str = myIGCMessage.Data.ToString();
                            Echo("Received IGC Public Message");
                            Echo("Tag=" + myIGCMessage.Tag);
                            Echo("Data=" + myIGCMessage.Data.ToString());
                            Echo("Source=" + myIGCMessage.Source.ToString("X"));
                        }
                        else // if(msg.Data is XXX)
                        {
                        }
                    }
                    else
                    {
                    }
                }
            }
        }
    }
}
