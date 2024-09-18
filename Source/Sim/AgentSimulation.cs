using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FellSky.Sim;

public interface IAgentPerk
{

}

public record struct Agent(
    Guid Id,
    IAgentPerk[] Perks
);

public class AgentSimulation
{
    List<Agent>[] agents;

    public AgentSimulation()
    {
        agents = Enumerable.Range(0, Global.SimThreadCount).Select(i => new List<Agent>()).ToArray();
    }
    public ValueTask Update(float delta, int threadId)
    {

        return ValueTask.CompletedTask;
    }
}