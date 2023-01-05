namespace Chloride.RA2.IniExt.Scripts;

public interface IScript<T> where T : IConfig
{
    public T Config { get; }
    public void Run();
}