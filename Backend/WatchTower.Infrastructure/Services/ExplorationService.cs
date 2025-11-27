namespace WatchTower.Infrastructure.Services;

public class ExplorationService : IExplorationService
{
    private readonly IExplorationRepository _explorationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICelestialBodyRepository _celestialBodyRepository;

    public ExplorationService(
        IExplorationRepository explorationRepository,
        IUserRepository userRepository,
        ICelestialBodyRepository celestialBodyRepository)
    {
        _explorationRepository = explorationRepository;
        _userRepository = userRepository;
        _celestialBodyRepository = celestialBodyRepository;
    }

    public async Task<IEnumerable<ExplorationHistory>> GetUserHistoryAsync(int userId)
    {
        return await _explorationRepository.GetUserHistoryAsync(userId);
    }

    public async Task<bool> AddToHistoryAsync(ExplorationHistory history)
    {
        // Validar que el usuario existe
        var user = await _userRepository.GetByIdAsync(history.UserId);
        if (user == null) throw new NotFoundException("User", history.UserId);

        // Validar que el cuerpo celeste existe
        var body = await _celestialBodyRepository.GetByIdAsync(history.CelestialBodyId);
        if (body == null) throw new NotFoundException("Celestial body", history.CelestialBodyId);

        return await _explorationRepository.AddToHistoryAsync(history);
    }

    public async Task<bool> UpdateTimeSpentAsync(int historyId, int timeSpent, int userId)
    {
        var history = await _explorationRepository.GetUserHistoryAsync(userId);
        var userHistory = history.FirstOrDefault(h => h.HistoryId == historyId);
        if (userHistory == null)
            throw new NotFoundException("Exploration history", historyId);

        return await _explorationRepository.UpdateTimeSpentAsync(historyId, timeSpent);
    }

    public async Task<int> GetTotalExplorationTimeAsync(int userId)
    {
        return await _explorationRepository.GetTotalExplorationTimeAsync(userId);
    }

    public async Task<int> GetUniqueBodiesExploredCountAsync(int userId)
    {
        return await _explorationRepository.GetUniqueBodiesExploredCountAsync(userId);
    }

    public async Task<bool> RecordExplorationAsync(int userId, int celestialBodyId, int timeSpentSeconds = 60)
    {
        var history = new ExplorationHistory
        {
            UserId = userId,
            CelestialBodyId = celestialBodyId,
            VisitedAt = DateTime.UtcNow,
            TimeSpent = timeSpentSeconds
        };

        return await _explorationRepository.AddToHistoryAsync(history);
    }
}