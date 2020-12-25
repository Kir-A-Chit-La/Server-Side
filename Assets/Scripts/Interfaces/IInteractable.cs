
public enum InteractableType
{
    Lootable,
    Simple
}

public interface IInteractable
{
    InteractableType InteractableType { get; }
    float RespawnTime { get; }
    void Respawn();
    void Despawn();
}
