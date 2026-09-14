namespace ExtensionComponents.Entity;

public interface IOutputBuilder
{
	public nint DestinationPtr { get; set; }
	public int OutputSize { get; set; }
	public void Append(string data);
}

public record struct OutputBuilder(nint DestinationPtr, int OutputSize) : IOutputBuilder
{
	/// <summary>
	/// Construct output buffer for Arma
	/// </summary>
	/// <param name="data">String data that will be output</param>
	public readonly void Append(string data)
	{
		ExtensionStartup.LocalServices?.Output(DestinationPtr, OutputSize, data);
	}
}
