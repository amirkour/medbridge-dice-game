using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using GameObjects;

namespace GameAgents
{
    /// <summary>
    /// The AgentFactory knows how to map player 'types' to implementations of
    /// IGameAgent - it's a basic factory!
    /// </summary>
    public class AgentFactory
    {

        // the following strings are the currently supported agent types - 
        // they're lowercased on purpose to simplify the mapping process
        // (because then we only have to worry about mapping lowercase strings.)
        public const string AGENT_TYPE_HUMAN_CONSOLE = "human_console";
        public const string AGENT_TYPE_AI_REFLEX = "ai_reflex";

        /// <summary>
        /// Factory method for mapping player type strings to implementations of
        /// IGameAgent - returns null if no supported type is found.
        /// </summary>
        public static IGameAgent GetAgentFor(String playerType)
        {
            if (playerType.IsNullOrEmpty()) { return null; }

            IGameAgent agent = null;
            switch(playerType.ToLower())
            {
                case AGENT_TYPE_HUMAN_CONSOLE:
                    agent = new UserInputConsoleAgent();
                    break;
                case AGENT_TYPE_AI_REFLEX:
                    agent = new LowestDieAgent();
                    break;
                default:
                    // TODO - log this case
                    break;
            }

            return agent;
        }
    }
}
