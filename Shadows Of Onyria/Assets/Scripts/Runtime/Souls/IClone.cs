namespace DoaT
{
    public interface IClone<out T>
    {
        T Clone();
    }
}