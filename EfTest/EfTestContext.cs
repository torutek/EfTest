﻿using EfTest.Models;
using Microsoft.EntityFrameworkCore;

namespace EfTest;
public class EfTestContext : DbContext
{
	public DbSet<Team> Teams { get; set; }
	public DbSet<Person> People { get; set; }
	public DbSet<Vehicle> Vehicles { get; set; }
	public DbSet<PersonPhoto> PersonPhotos { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		//TODO: Try MultipleActiveResultSets
		//TODO: Point at azure sql
		optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EfTest;Trusted_Connection=True");

		optionsBuilder.EnableSensitiveDataLogging();
		optionsBuilder.EnableDetailedErrors();
		optionsBuilder.LogTo((str) =>
		{
			if (!str.StartsWith("dbug"))
				Console.WriteLine(str);

			//When EF has just ran the second query (to get teams), add a new team with a new vehicle
			//EF8
			/*
			  info: 19/11/2024 09:20:59.994 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
			  Executed DbCommand (2ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
			  SELECT [t0].[PeoplePersonId], [t0].[TeamsId], [t0].[Id], [t0].[Name], [p].[PersonId]
			  FROM [People] AS [p]
			  INNER JOIN (	
				  SELECT [p0].[PeoplePersonId], [p0].[TeamsId], [t].[Id], [t].[Name]
				  FROM [PersonTeam] AS [p0]
				  INNER JOIN [Teams] AS [t] ON [p0].[TeamsId] = [t].[Id]
			  ) AS [t0] ON [p].[PersonId] = [t0].[PeoplePersonId]
			  ORDER BY [p].[PersonId], [t0].[PeoplePersonId], [t0].[TeamsId], [t0].[Id]
			*/

			//EF9
			/*
			 info: 19/11/2024 09:45:03.415 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
			  Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
			  SELECT [s].[PeoplePersonId], [s].[TeamsId], [s].[Id], [s].[Name], [p].[PersonId]
			  FROM [People] AS [p]
			  INNER JOIN (
				  SELECT [p1].[PeoplePersonId], [p1].[TeamsId], [t].[Id], [t].[Name]
				  FROM [PersonTeam] AS [p1]
				  INNER JOIN [Teams] AS [t] ON [p1].[TeamsId] = [t].[Id]
			  ) AS [s] ON [p].[PersonId] = [s].[PeoplePersonId]
			  ORDER BY [p].[PersonId], [s].[PeoplePersonId], [s].[TeamsId], [s].[Id]
			*/
			if (str.Contains("Executed DbCommand") &&
				(str.Contains("SELECT [t0].[PeoplePersonId], [t0].[TeamsId], [t0].[Id], [t0].[Name], [p].[PersonId]") //EF8
				|| str.Contains("SELECT [s].[PeoplePersonId], [s].[TeamsId], [s].[Id], [s].[Name], [p].[PersonId]"))) //EF9
			{
				Console.WriteLine(" ----- Inserting in a new row like a race condition ----- ");

				using var context = new EfTestContext();

				var person = context.People.Single(p => p.PersonId == 30);
				context.Add(new Team
				{
					Id = 201,
					Name = "Team20",
					People = [person],
					Vehicles = [new() { Id = 202, Name = "Vehicle20" }]
				});
				context.SaveChanges();
				Console.WriteLine(" ----- End Insert ----- ");
			}

		});
	}
}
