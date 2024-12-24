using Microsoft.Data.SqlClient;
using System.Data;
using AlicanteWeb.Models;
using Dapper;
using System.Linq;

namespace AlicanteWeb.Data
{
    public class Repository : IRepository
    {

        private readonly IConfiguration _configuration;

        public Repository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("AlicanteDB"));
        }

        #region Course

        private const string courseSelect = @"SELECT [CourseId],[CourseName],[Par],[CourseRating],[Slope] 
                                                FROM al.Course";

        public async Task<CourseModel> GetCourse(int courseId)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string getQuery = $"{courseSelect} WHERE CourseId = @courseId";
                return await db.QuerySingleAsync<CourseModel>(getQuery, new { courseId });
            }
        }

        public async Task<IEnumerable<CourseModel>> GetAllCourses()
        {
            using (IDbConnection db = CreateConnection())
            {
                var results = await db.QueryAsync<CourseModel>(courseSelect);
                return results;
            }
        }

        public async Task<CourseModel> UpsertCourse(CourseModel model)
        {
            var sql = "EXEC al.UpsertCourse @CourseId, @Par, @courseName, @courseRating, @slope";
            using (var connection = CreateConnection())
            {
                var result = await connection.QuerySingleAsync<CourseModel>(sql, new
                {
                    model.CourseId,
                    model.Par,
                    model.CourseName,
                    model.CourseRating,
                    model.Slope
                });
                return result;
            }
        }

        public async Task<int> DeleteCourse(int courseId)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string deleteQuery = "DELETE al.Course WHERE CourseId = @courseId";
                var rowsAffected = await db.ExecuteScalarAsync<int>(deleteQuery, new { courseId });
                return rowsAffected;
            }
        }

        #endregion Course

        #region Match

        public async Task<MatchViewModel> GetMatch(int matchId)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string query = @"SELECT TournamentId, TournamentName, Active, " +
                    "                   MatchId, MatchDate, CourseId, CourseName, " +
                                        "CourseRating, Slope, Par " +
                                        "FROM al.vMatch " +
                                        "where Active = 1 " +
                                        "and MatchId = @matchId";

                return await db.QuerySingleAsync<MatchViewModel>(query, new { matchId });
   
            }
        }

        public async Task<IEnumerable<MatchViewModel>> GetAllMatches()
        {
            using (IDbConnection db = CreateConnection())
            {
                const string query = @"SELECT TournamentId, TournamentName, Active, " +
                    "                   MatchId, MatchDate, CourseId, CourseName, " +
                                        "CourseRating, Slope, Par " +
                                        "FROM al.vMatch " +
                                        "where Active = 1 " +
                                        "order by MatchDate";

                var results = await db.QueryAsync<MatchViewModel>(query);
                return results;
            }
        }
        public async Task<MatchModel> UpsertMatch(MatchModel match)
        {
            var sql = "EXEC al.UpsertMatch @MatchId, @MatchDate, @CourseId, @TournamentId";
            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync<MatchModel>(sql, new
                {
                    match.MatchId,
                    match.MatchDate,
                    match.CourseId,
                    match.TournamentId
                });
                return result!.SingleOrDefault<MatchModel>();
            }
        }

        public async Task<int> DeleteMatch(int matchId)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string deleteQuery = "DELETE al.Match WHERE MatchId = @matchId";
                var rowsAffected = await db.ExecuteScalarAsync<int>(deleteQuery, new { matchId });
                return rowsAffected;
            }
        }

        // WARNING: The Dapper code generation tool doesn't currently generate merge method(s).

        #endregion Match

        #region Player

        public async Task<PlayerModel> GetPlayer(int playerId)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string getQuery = "SELECT * FROM al.Player WHERE PlayerId = @playerId";
                return await db.QuerySingleAsync<PlayerModel>(getQuery, new { playerId });
            }
        }

        public async Task<IEnumerable<PlayerModel>> GetAllPlayers()
        {
            using (IDbConnection db = CreateConnection())
            {
                const string query = "SELECT * FROM al.Player order by PlayerName";
                var results = await db.QueryAsync<PlayerModel>(query);
                return results;
            }
        }

        public async Task<IEnumerable<PlayerViewModel>> GetPlayersForMatch(int matchId)
        {
            var sql = @"SELECT PlayerId, PlayerName, ResultId, Hcp " +
                "FROM al.vPlayersForMatch " +
                "where MatchId = @matchId and ResultId is null";

            using (IDbConnection db = CreateConnection())
            {
                return await db.QueryAsync<PlayerViewModel>(sql, new { matchId });
            }
        }

        public async Task<PlayerModel> UpsertPlayer(PlayerModel player)
        {
            var sql = "EXEC al.UpsertPlayer @PlayerId, @PlayerName, @HcpIndex";
            using (var connection = CreateConnection())
            {
                var result = await connection.QuerySingleAsync<PlayerModel>(sql, new
                {
                    player.PlayerId,
                    player.PlayerName,
                    player.HcpIndex
                });
                return result;
            }
        }

        public async Task<int> DeletePlayer(int playerId)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string deleteQuery = "DELETE al.Player WHERE PlayerId = @playerId";
                var rowsAffected = await db.ExecuteScalarAsync<int>(deleteQuery, new { playerId });
                return rowsAffected;
            }
        }
        #endregion Player

        #region Tournament

        public async Task<TournamentModel> GetTournament(int tournamentId)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string getQuery = "SELECT TournamentId, TournamentName FROM al.Tournament WHERE TournamentId = @tournamentId";
                return await db.QuerySingleAsync<TournamentModel>(getQuery, new { tournamentId });
            }
        }

        public async Task<IEnumerable<TournamentModel>> GetTournaments()
        {
            using (IDbConnection db = CreateConnection())
            {
                const string query = "SELECT TournamentId, TournamentName, Active FROM al.Tournament";
                var results = await db.QueryAsync<TournamentModel>(query);
                return results;
            }
        }

        public async Task<TournamentModel?> GetAciveTournament()
        {
            using (IDbConnection db = CreateConnection())
            {
                const string query = "SELECT TournamentId, TournamentName, Active FROM al.Tournament where Active=1";
                try
                {
                    var result = await db.QuerySingleAsync<TournamentModel>(query);
                    return result;
                }
                catch (InvalidOperationException)
                {
                    // Handle case where no active tournament is found or more than one record is returned
                    return null; // or default action
                }
            }
        }

        public async Task<TournamentModel> UpsertTournament(TournamentModel model)
        {
            var sql = "EXEC al.UpsertTournament @TournamentId, @TournamentName, @Active";
            using (var connection = CreateConnection())
            {
                var result = await connection.QuerySingleAsync<TournamentModel>(sql, new
                {
                    model.TournamentId,
                    model.TournamentName,
                    model.Active
                });
                return result;
            }
        }
        public async Task<int> SetActiveTournament(int id)
        {
            var sql = "update al.Tournament set Active=0";
            using (var connection = CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql);
                sql = "update al.Tournament set Active=1 where TournamentId = @id";
                result = await connection.ExecuteAsync(sql, new { id });
                return result;
            }
        }


        public async Task<int> DeleteTournament(int tournamentId)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string deleteQuery = "DELETE al.Tournament WHERE TournamentId = @tournamentId";
                var rowsAffected = await db.ExecuteScalarAsync<int>(deleteQuery, new { tournamentId });
                return rowsAffected;
            }
        }
        #endregion Tournament

        #region Result
        public async Task<ResultModel> GetResult(int id)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string getQuery = "SELECT ResultId,PlayerId,MatchId,Hcp,Score,Birdies FROM al.Result WHERE ResultId = @id";
                return await db.QuerySingleAsync<ResultModel>(getQuery, new { id });
            }
        }
        
        public async Task<ResultModel> UpsertResult(ResultModel model)
        {
            var sql = "EXEC al.UpsertResult @PlayerId, @MatchId, @Hcp, @Score, @Birdies, @Par3";
            using (var connection = CreateConnection())
            {
                var result = await connection.QuerySingleAsync<ResultModel>(sql, new
                {
                    model.PlayerId,
                    model.MatchId,
                    model.Hcp,
                    model.Score,
                    model.Birdies,
                    model.Par3
                });
                return result;
            }
        }

        public async Task<IEnumerable<ResultViewModel>> GetResults()
        {
            var sql = @"SELECT 
                TournamentId, TournamentName, Active, MatchId, MatchDate, 
                CourseId, CourseName, CourseRating, Slope, Hcp, Score, 
                Birdies, PlayerId, PlayerName, HcpIndex
            FROM al.vResult order by PlayerName";

            using (IDbConnection db = CreateConnection())
            {
                return await db.QueryAsync<ResultViewModel>(sql);
            }
        }

        public async Task<IEnumerable<ResultViewModel>> GetResults(int matchId)
        {
            var sql = @"SELECT 
                ResultId, TournamentId, TournamentName, Active, MatchId, MatchDate, 
                CourseId, CourseName, CourseRating, Slope, Hcp, Score, 
                Birdies, PlayerId, PlayerName, HcpIndex, Par3,
                Rank() over (partition by TournamentId order by MatchDate) as matchRank
                FROM al.vResult
                WHERE MatchId = @matchId
                ORDER BY PlayerName"
            ;
            using (IDbConnection db = CreateConnection())
            {
                return await db.QueryAsync<ResultViewModel>(sql, new { matchId });
            }
        }

        public async Task<IEnumerable<MatchResultViewModel>> TournamentResult(int tournamentId)
        {
            var sql = @" SELECT TournamentId, CourseName, MatchId, MatchDate, PlayerName, PlayerId,
                Hcp, Score, Score-Hcp as Netto, Birdies, HcpIndex, Par3
                FROM al.vResult WHERE TournamentId = @tournamentId
                union
                SELECT TournamentId, 'Total', -1, null, PlayerName, PlayerId, 
                null, sum(Score), sum(Score-Hcp) as Netto,  sum(Birdies), null, sum(Par3)
                FROM al.vResult 
				group by TournamentId, PlayerName, PlayerId 
				HAVING TournamentId = @tournamentId";

            using (IDbConnection db = CreateConnection())
            {
                return await db.QueryAsync<MatchResultViewModel>(sql, new { tournamentId });
            }
        }

        public async Task<IEnumerable<MatchResultViewModel>> MatchResult(int matchId)
        {
            var sql = @" SELECT CourseName, MatchId, MatchDate, PlayerName, PlayerId,
                Hcp, Score, Score-Hcp as Netto, Birdies, HcpIndex, Par3
                FROM al.vResult WHERE MatchId = @matchId
                union
                SELECT 'Total', -1, null, PlayerName, PlayerId, 
                null, sum(Score), sum(Score-Hcp) as Netto,  sum(Birdies), null, sum(Par3)
                FROM al.vResult 
				group by MatchId, PlayerName, PlayerId 
				HAVING MatchId = @matchId";

            using (IDbConnection db = CreateConnection())
            {
                return await db.QueryAsync<MatchResultViewModel>(sql, new { matchId });
            }
        }

        public async Task<int> DeleteResult(int resultId)
        {
            using (IDbConnection db = CreateConnection())
            {
                const string deleteQuery = "DELETE al.Result WHERE ResultId = @resultId";
                var rowsAffected = await db.ExecuteScalarAsync<int>(deleteQuery, new { resultId });
                return rowsAffected;
            }
        }
        #endregion
    }
}
