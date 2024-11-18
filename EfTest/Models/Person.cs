using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfTest.Models;
public class Person
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int PersonId { get; set; }

	public required string Name { get; set; }

	public List<PersonPhoto> Photos { get; set; } = null!;

	public List<Team> Teams { get; set; } = null!;
}
