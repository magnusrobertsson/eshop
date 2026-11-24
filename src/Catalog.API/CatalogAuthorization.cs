namespace eShop.Catalog.API;

public static class CatalogAuthorization
{
    /// <summary>Policy required to create, update or delete catalog items.</summary>
    public const string ManagerPolicy = "CatalogManager";

    /// <summary>Value of the "role" claim that grants catalog write access.</summary>
    public const string ManagerRole = "catalog-manager";
}
