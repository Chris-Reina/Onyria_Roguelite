namespace DoaT
{
    public interface IGridEntity : IPositionProperty, IDirectionProperty, IGameObjectProperty
    {
        event System.Action<IGridEntity> OnPositionChange;
    }
}