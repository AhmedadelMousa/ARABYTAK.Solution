namespace ARABYTAK.APIS.DTOs
{
    public class RoleDto
    {

        public string Id { get; }
        public string RoleName { get; set; }

        public RoleDto()
        {
            Id = Guid.NewGuid().ToString();
        }

    }
}
