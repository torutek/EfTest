using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfTest.Models;
public class Team
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Id { get; set; }

	public required string Name { get; set; }

	public List<Person> People { get; set; }
	public List<Vehicle> Vehicles { get; set; }
}
