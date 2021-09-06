using EntityFrameworkNet5.Data;
using EntityFrameworkNet5.Domain;
using EntityFrameworkNet5.Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkNet5.ConsoleApp
{
    internal class Program
    {
        private static FootballLeagueDbContext context = new FootballLeagueDbContext();

        private static async Task Main(string[] args)
        {
            //await TrackingVsNoTracking();
            //await AddNewLeagueWithTeams();
            //await AddNewMatches();
            //await AddNewCoach();
            //await QueryRelatedData();

            //await SelectOneProperty();
            //await AnonymousProjection();
            //await StronglyTypedProjection();
            //await FilteringWithRelatedData();
            //await RawSqlQuery();
            //await UpdateNewLeagueWithUsername();

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }


        // Add New League with username
        private static async Task UpdateNewLeagueWithUsername()
        {
            var league = await context.Leagues.FindAsync(10);

            league.Name = "Cool League";

            await context.SaveChangesAsync("Kakon");
        }


        // Raw Sql
        private static async Task RawSqlQuery()
        {
            // Vulnerable to sql injection
            var name = "Hello World";
            var team1 = await context.Teams
                .FromSqlRaw($"SELECT * FROM Teams WHERE name = '{name}'")
                .Include(q => q.Coach)
                .ToListAsync();

            var team2 = await context.Teams
                .FromSqlInterpolated($"SELECT * FROM Teams WHERE name = {name}")
                .Include(q => q.Coach)
                .ToListAsync();
        }


        // Filtering with related data
        private static async Task FilteringWithRelatedData()
        {
            var leagues = await context.Leagues
                .Where(q => q.Teams.Any(x => x.Name.Contains("a")))
                .ToArrayAsync();
        }


        // Projections and Anonymous Data Types
        private static async Task SelectOneProperty()
        {
            var teams = await context.Teams.Select(q => q.Name).ToListAsync();
        }

        private static async Task AnonymousProjection()
        {
            // AnonymousProjection is not recommended
            var teams = await context.Teams.Include(q => q.Coach)
                .Select(q => new { TeamName = q.Name, CoachName = q.Coach.Name })
                .ToListAsync();

            foreach (var team in teams)
            {
                Console.WriteLine($"Team:{team.TeamName}, Coach:{team.CoachName}");
            }
        }

        private static async Task StronglyTypedProjection()
        {
            var teams = await context.Teams
                .Include(q => q.Coach)
                .Include(q => q.League)
                .Select(q => new TeamDetail
                {
                    Name = q.Name,
                    CoachName = q.Coach.Name,
                    LeagueName = q.League.Name
                })
                .ToListAsync();

            foreach (var team in teams)
            {
                Console.WriteLine($"Team:{team.Name}, Coach:{team.CoachName}, League:{team.LeagueName}");
            }
        }


        // Including Related Data - Eager Loading
        private static async Task QueryRelatedData()
        {
            //// Get many Related records - Leagues -> Teams
            var leagues = await context.Leagues.Include(q => q.Teams).ToListAsync();

            //// Get one Related records - Team -> Coach
            var team = await context.Teams
                .AsNoTracking()
                .Include(q => q.Coach)
                .FirstOrDefaultAsync(q => q.Id == 3);

            //// Get 'Grand Children' Related record - Team -> Matches -> Home/Away Team
            var teamsWithMatchesAndOpponents = await context.Teams
                .Include(q => q.AwayMatches).ThenInclude(q => q.HomeTeam).ThenInclude(q => q.Coach)
                .Include(q => q.HomeMatches).ThenInclude(q => q.AwayTeam).ThenInclude(q => q.Coach)
                .FirstOrDefaultAsync(q => q.Id == 1);

            //// Get Includes with filters
            var teams = await context.Teams
                .AsNoTracking()
                .Where(q => q.HomeMatches.Count > 0)
                .Include(q => q.Coach)
                .ToListAsync();
        }


        // RELATIONAL INSERT
        private static async Task AddNewMatches()
        {
            var matches = new List<Match>
            {
                new Match
                {
                    AwayTeamId = 1, HomeTeamId = 2, Date = new DateTime(2021, 10, 20)
                },
                new Match
                {
                    AwayTeamId = 3, HomeTeamId = 6, Date = new DateTime(2019, 1, 12)
                },
                new Match
                {
                    AwayTeamId = 2, HomeTeamId = 7, Date = new DateTime(2021, 5, 25)
                },
            };

            await context.AddRangeAsync(matches);
            await context.SaveChangesAsync();
        }

        private static async Task AddNewCoach()
        {
            var coach1 = new Coach { Name = "Nasim Uddin", TeamId = 1 };
            var coach2 = new Coach { Name = "Mehedi Hasan" };

            await context.AddAsync(coach1);
            await context.AddAsync(coach2);
            await context.SaveChangesAsync();
        }


        // BAIC INSERT, UPDATE, DELETE
        private static async Task TrackingVsNoTracking()
        {
            var withTracking = await context.Teams.FirstOrDefaultAsync(t => t.Id == 2);
            var withNoTracking = await context.Teams.AsNoTracking().FirstOrDefaultAsync(t => t.Id == 3);

            withTracking.Name = "Changed";
            withNoTracking.Name = "Not Changed";

            var entriesBeforeSave = context.ChangeTracker.Entries();

            await context.SaveChangesAsync();

            var entriesAfterSave = context.ChangeTracker.Entries();
        }

        private static async Task SimpleDelete()
        {
            var league = await context.Leagues.FindAsync(1);
            context.Leagues.Remove(league);
            await context.SaveChangesAsync();
        }

        private static async Task SimpleDeleteWithRelationship()
        {
            //// since cascade delete in on in migration it will also delete the relation table
            var league = await context.Leagues.FindAsync(5);
            context.Leagues.Remove(league);
            await context.SaveChangesAsync();
        }

        private static async Task SimpleUpdateTeamRecord()
        {
            var team = new Team
            {
                Id = 4,
                Name = "New Teams",
                LeagueId = 5,
            };
            context.Teams.Update(team); //// if team.Id is not present it will insert into the database 😲
            await context.SaveChangesAsync();
        }

        private static async Task GetRecord()
        {
            var league = await context.Leagues.FindAsync(1);
            Console.WriteLine($"{league.Id} - {league.Name}");
        }

        private static async Task SimpleUpdateLeagueRecord()
        {
            var league = await context.Leagues.FindAsync(1);
            league.Name = "Updated";
            await context.SaveChangesAsync();
            await GetRecord();
        }

        private static async Task AdditionalExecutionMethodsAsync()
        {
            //var l = await context.Leagues.Where(l => l.Name.Contains("A")).FirstOrDefaultAsync();
            //var l = await context.Leagues.FirstOrDefaultAsync(l => l.Name.Contains("A"));

            var leagues = context.Leagues;
            var list = await leagues.ToListAsync();
            var first = await leagues.FirstAsync(); // throw exception if first row is not found
            var firstOrDefaults = await leagues.FirstOrDefaultAsync();
            var single = await leagues.SingleAsync(); // expects only one row, if not then throw error
            var singleOrDefaults = await leagues.SingleOrDefaultAsync();

            var count = await leagues.CountAsync(e => e.Name == "Hello");
            var longCount = await leagues.LongCountAsync(e => e.Id == 2);
            var min = await leagues.MinAsync(e => e.Id);
            var max = await leagues.MaxAsync(e => e.Id);
        }

        private static async Task QueryFiltersAsync()
        {
            Console.Write("Enter league name: ");
            var leagueName = Console.ReadLine();

            Console.WriteLine("Exact Matches: ");

            var exactMatches = await context.Leagues.Where(l => l.Name.Equals(leagueName)).ToListAsync();
            foreach (var league in exactMatches)
            {
                Console.WriteLine($"{league.Id} - {league.Name}");
            }

            Console.WriteLine("Partial Matches: ");

            var partialMatches = await context.Leagues.Where(l => l.Name.Contains(leagueName)).ToListAsync();
            //var partialMatches = await context.Leagues.Where(l => EF.Functions.Like(l.Name, $"%{leagueName}%")).ToListAsync();
            foreach (var league in partialMatches)
            {
                Console.WriteLine($"{league.Id} - {league.Name}");
            }
        }

        private static async Task SimpleSelectQuery()
        {
            var leagues = await context.Leagues.ToListAsync();
            foreach (var league in leagues)
            {
                Console.WriteLine($"{league.Id} - {league.Name}");
            }
        }

        private static async Task AddNewLeague()
        {
            //// Adding a new league object.
            var league = new League { Name = "New League 2" };
            await context.Leagues.AddAsync(league);

            //// Passing league object because after saving league it updated the league Id.
            await AddTeamsWithLeague(league);
            await context.SaveChangesAsync();
        }

        private static async Task AddTeamsWithLeague(League league)
        {
            var teams = new List<Team>
            {
                new Team
                {
                    Name = "Juventus",
                    LeagueId = league.Id,
                },
                new Team
                {
                    Name = "AC Milan",
                    LeagueId = league.Id,
                },
                new Team
                {
                    Name = "AS Roma",
                    League = league,
                },
            };

            await context.AddRangeAsync(teams);
        }

        private static async Task AddNewTeamWithLeague()
        {
            var league = new League { Name = "New League 2" };
            var team = new Team { Name = "New Teams 2", League = league };
            await context.AddAsync(team);
            await context.SaveChangesAsync();
        }

        private static async Task AddNewTeamWithLeagueId()
        {
            var team = new Team { Name = "New Teams 3", LeagueId = 4 };
            await context.AddAsync(team);
            await context.SaveChangesAsync();
        }

        private static async Task AddNewLeagueWithTeams()
        {
            var teams = new List<Team>
            {
                new Team
                {
                    Name = "Team list 3"
                },
                new Team
                {
                    Name = "Team list 4"
                },
            };
            var league = new League { Name = "Brand new League", Teams = teams };

            await context.AddAsync(league);
            await context.SaveChangesAsync();
        }
    }
}