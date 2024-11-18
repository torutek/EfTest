using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfTest.Models;
public class PersonPhoto
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int PersonPhotoId { get; set; }

	public Person Person { get; set; } = null!;

	public required string Url { get; set; }
}
