namespace CB.Data.Common.CRUD
{
    public interface IIdKeyEntity<TKey>
    {
        TKey Id { get; set; } 
    }
}