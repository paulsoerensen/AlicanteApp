using AlicanteWeb.Models;

namespace AlicanteWeb.Data;


public interface IRepository
{
    Task<CourseModel> GetCourse(int courseId);
    Task<IEnumerable<CourseModel>> GetAllCourses();
    Task<CourseModel> UpsertCourse(CourseModel model);
    Task<int> DeleteCourse(int courseId);

    Task<MatchViewModel> GetMatch(int matchId);
    Task<IEnumerable<MatchViewModel>> GetAllMatches();
    Task<MatchModel> UpsertMatch(MatchModel match);
    Task<int> DeleteMatch(int matchId);

    Task<PlayerModel> GetPlayer(int playerId);
    Task<IEnumerable<PlayerModel>> GetAllPlayers();
    Task<IEnumerable<PlayerViewModel>> GetPlayersForMatch(int matchId);
    Task<PlayerModel> UpsertPlayer(PlayerModel player);
    Task<int> DeletePlayer(int playerId);

    //Task<IEnumerable<ResultModel>> GetPlayerMatches();
    //Task<int> DeletePlayerMatch(int playerId);

    Task<TournamentModel> GetTournament(int tournamentId);
    Task<IEnumerable<TournamentModel>> GetTournaments();
    Task<TournamentModel> UpsertTournament(TournamentModel model);
    Task<int> SetActiveTournament(int id);
    Task<TournamentModel?> GetAciveTournament();
    Task<int> DeleteTournament(int tournamentId);

    Task<ResultModel> GetResult(int id);
    Task<ResultModel> UpsertResult(ResultModel model);

    Task<IEnumerable<ResultViewModel>> GetResults();
    Task<IEnumerable<ResultViewModel>> GetResults(int matchId);
    Task<IEnumerable<MatchResultViewModel>> TournamentResult(int tournamentId);
    Task<IEnumerable<MatchResultViewModel>> MatchResult(int matchId);
    Task<int> DeleteResult(int resultId);

}