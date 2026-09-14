namespace ExtensionComponents.Entity;

public interface IArgsBuilder
{
	public nint SourcePtr { get; set; }
	public int ArgCount { get; set; }
}

public record struct ArgsBuilder(nint SourcePtr, int ArgCount) : IArgsBuilder;
