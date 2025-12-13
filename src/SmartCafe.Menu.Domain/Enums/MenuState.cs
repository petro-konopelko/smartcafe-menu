namespace SmartCafe.Menu.Domain.Enums;

/// <summary>
/// Represents the lifecycle state of a Menu.
/// New: Initial state, can be edited and deleted.
/// Published: Ready for activation, cannot be deleted.
/// Active: Currently displayed to customers.
/// Deleted: Soft-deleted, excluded from queries.
/// </summary>
public enum MenuState
{
    New = 0,
    Published = 1,
    Active = 2,
    Deleted = 3
}
