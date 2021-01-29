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

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        void Main()

            //DIRECTIONS FOR USE
            //label hydrogen thrusters as UpThrust
            //create a remote control called "RC IceJumper Roof"
            //create a timer called "Timer Stop Ascent"
            //which triggers the remote control's inertial dampeners
        {
            // Get blocks 
            var blocks = new List<IMyTerminalBlock>();

            // Get the antenna 
            GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(blocks);

            IMyTimerBlock timerUp = GridTerminalSystem.GetBlockWithName("Timer Stop Ascent") as IMyTimerBlock;

            IMyRemoteControl remote = GridTerminalSystem.GetBlockWithName("RC IceJumper Roof") as IMyRemoteControl;
            
            Vector3D gravityVector = remote.GetNaturalGravity();
            float gravity = (float)(gravityVector.Length() / 9.81);

            if (blocks.Count > 0)
            {
                IMyTerminalBlock Block = blocks[0];

                // Get time now 
                var Time1 = System.DateTime.Now;
                long OldTime = 0;

                // We pull this here to prevent conversions being *too* weird 
                long CurrentTime = Time1.ToBinary();

                // Store the current name 
                String CurrentName = Block.CustomName;

                // Get the fragments (or get one fragment) 
                String[] Fragments = CurrentName.Split('|');

                // Get coordinates (VRageMath.Vector3D, so pull it in the ugly way) 
                double x = Math.Round(Block.GetPosition().GetDim(0), 4);
                double y = Math.Round(Block.GetPosition().GetDim(1), 4);
                double z = Math.Round(Block.GetPosition().GetDim(2), 4);

                // Allocate this here 
                double X = 0;
                double Y = 0;
                double Z = 0;

                // Start with "the unknown" speed, stored in m/s 
                double Speed = 0.0;

                // Total distance moved 
                double Distance = 0;

                // Do we actually have fragments? 
                if (Fragments.Length == 3)
                {
                    // Yes? Excellent. 
                    OldTime = Convert.ToInt64(Fragments[1]);

                    // Vomit a bit here because this is how we have to store variables at the moment ... 
                    string[] Coords = Fragments[2].Split(',');
                    X = Math.Round(Convert.ToDouble(Coords[0]), 4);
                    Y = Math.Round(Convert.ToDouble(Coords[1]), 4);
                    Z = Math.Round(Convert.ToDouble(Coords[2]), 4);

                    // Nothing fancy here 
                    Distance = System.Math.Sqrt(
                        ((x - X) * (x - X)) + ((y - Y) * (y - Y)) + ((z - Z) * (z - Z))
                    );
                }

                // If the base coordinates 
                if (Distance != 0 && X != 0 && Y != 0 && Z != 0 && OldTime != 0)
                {
                    // Update time 
                    var Time0 = System.DateTime.FromBinary(OldTime);

                    // We have 's' for m/s. 
                    var TimeDelta = (Time1 - Time0).TotalSeconds;

                    // We have our distance 
                    Speed = Distance / TimeDelta;
                    Speed = Math.Round(Convert.ToDouble(Speed), 4);

                    double SpeedLimit = 95;
                    List<IMyTerminalBlock> thrusters = new List<IMyTerminalBlock>();

                    GridTerminalSystem.GetBlockGroupWithName("UpThrust").GetBlocksOfType<IMyTerminalBlock>(thrusters);
                    var factor = 0.0f;
                    if (gravity == 0)
                    {
                        //trigger timer to turn on inertial dampeners
                        timerUp.Trigger();
                    }
                    else if (Speed < SpeedLimit)
                    {
                        factor = 0.9f;
                    }

                    for (int i = 0; i < thrusters.Count; i++)
                    {
                        IMyThrust thruster = thrusters[i] as IMyThrust;

                        var min = thruster.GetMinimum<float>("Override");
                        var max = thruster.GetMaximum<float>("Override");

                        thruster.SetValueFloat("Override", min + ((max - min) * factor));
                        Echo("Speed: "+ Speed.ToString());
                        Echo("Thrust Ratio: " + factor.ToString());
                        Echo("Gravity: " + gravity.ToString());
                    }
                }

            

                // Speed|Time|X,Y,Z 
                String NewName = Speed.ToString() + "|" +
                    CurrentTime.ToString() + "|" +
                    x.ToString() + "," + y.ToString() + "," + z.ToString();

                // Store it 
                Block.CustomName=NewName;

                // Show it on the HUD so we can check it 
                Block.ShowOnHUD=true;
            }
        }
        

    }
}
