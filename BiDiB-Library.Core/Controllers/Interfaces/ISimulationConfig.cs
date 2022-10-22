namespace org.bidib.netbidibc.core.Controllers.Interfaces;

public interface ISimulationConfig : IConnectionConfig
{
    /// <summary>
    /// Gets or sets the simulation file path
    /// </summary>
    string SimulationFilePath { get; set; }
}