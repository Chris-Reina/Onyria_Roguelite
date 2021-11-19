using System;

namespace DoaT.AI
{
    public interface IPath : IPositionProperty
    {
        event Action OnPathUpdated;
        
        int CurrentIndex { get; set; }
        Path Path { get; set; }
        PathRequest PathRequest { get; }
    }
}



