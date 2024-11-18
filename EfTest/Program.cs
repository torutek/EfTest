// See https://aka.ms/new-console-template for more information
using EfTest;
using EfTest.Models;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");

using (var context = new EfTestContext())
{
	await context.Vehicles.ExecuteDeleteAsync();
	await context.Teams.ExecuteDeleteAsync();
	await context.PersonPhotos.ExecuteDeleteAsync();
	await context.People.ExecuteDeleteAsync();

	context.Add(new Person
	{
		PersonId = 10,
		Name = "Person1",
		Teams =
		[
			new() { Id = 11, Name = "Team1", Vehicles = [new() { Id = 12, Name = "Vehicle1" }] },
			new() { Id = 21, Name = "Team2", Vehicles = [new() { Id = 22, Name = "Vehicle2" }] },
			new() { Id = 31, Name = "Team3", Vehicles = [new() { Id = 32, Name = "Vehicle3" }] }
		]

	});

	context.Add(new Person
	{
		PersonId = 20,
		Name = "Person2",
		Teams =
		[
			new() { Id = 141, Name = "Team4", Vehicles = [new() { Id = 142, Name = "Vehicle4" }] },
			new() { Id = 151, Name = "Team5", Vehicles = [new() { Id = 152, Name = "Vehicle5" }] },
			new() { Id = 161, Name = "Team6", Vehicles = [new() { Id = 162, Name = "Vehicle6" }] }
		]
	});

	context.Add(new Person
	{
		PersonId = 30,
		Name = "Person3",
		Teams =
		[
			new() { Id = 271, Name = "Team7", Vehicles = [new() { Id = 272, Name = "Vehicle7" }] },
			new() { Id = 281, Name = "Team8", Vehicles = [new() { Id = 282, Name = "Vehicle8" }] },
			new() { Id = 291, Name = "Team9", Vehicles = [new() { Id = 292, Name = "Vehicle9" }] }
		]
	});

	await context.SaveChangesAsync();
}

Person[] result;
using (var context = new EfTestContext())
{

	var query = context.People
		.Include(p => p.Photos)
		.Include(p => p.Teams).ThenInclude(h => h.Vehicles)
		.AsSplitQuery();

	//You can work around the issue using a transaction here
	// Serializable blocks during the insert, so it would probably work
	// Snapshot works if it has been allowed in the database
	//  https://learn.microsoft.com/en-us/troubleshoot/sql/analysis-services/enable-snapshot-transaction-isolation-level
	// ReadCommitted + enabling READ_COMMITTED_SNAPSHOT still breaks
	//  https://learn.microsoft.com/en-us/sql/t-sql/statements/set-transaction-isolation-level-transact-sql?view=sql-server-ver16
	//using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

	result = await query.ToArrayAsync();
}

Console.WriteLine($"People {result.Length}:  {string.Join(", ", result.Select(p => p.PersonId).Order().ToArray())}");
Console.WriteLine($"Teams {result.SelectMany(p => p.Teams).Count()}:  {string.Join(", ", result.SelectMany(p => p.Teams).Select(t => t.Id).Order().ToArray())}");
Console.WriteLine($"Vehicles {result.SelectMany(p => p.Teams).SelectMany(t => t.Vehicles).Count()}:  {string.Join(", ", result.SelectMany(p => p.Teams).SelectMany(t => t.Vehicles).Select(v => v.Id).Order().ToArray())}");
