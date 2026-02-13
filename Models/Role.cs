using System.ComponentModel.DataAnnotations;
namespace EMS.Api.Models;

public class Role
{
    public int Id {get; set;}
    [Required]
    public string RoleName {get; set;} = "";
}

