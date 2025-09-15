namespace BioLinker.Enities
{
    public class Role
    {
        public string? RoleId { get; set; } = Guid.NewGuid().ToString();
        public string? RoleName { get; set; }
    }
}
