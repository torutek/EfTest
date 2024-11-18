using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfTest.Models;
public class Vehicle
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Id { get; set; }

	public required string Name { get; set; }
}
