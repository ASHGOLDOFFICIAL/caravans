namespace CaravansCore.Objects;

public interface IObject
{
    public ObjectId Id { get; }
}

public record StoneWall : IObject
{
    ObjectId IObject.Id => ObjectId.StoneWall;
}