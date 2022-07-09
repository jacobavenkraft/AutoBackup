namespace FrameworkLibrary.Interfaces
{
    public interface IWindowFactory
    {
        TWindowType? CreateWindow<TWindowType>();
    }
}
