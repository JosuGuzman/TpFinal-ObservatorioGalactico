namespace Observatorio.Core.Services;

public class DiscoveryService : IDiscoveryService
{
    private readonly IDiscoveryRepository _discoveryRepository;
    private readonly IDiscoveryVoteRepository _voteRepository;
    private readonly ILoggingService _loggingService;

    public DiscoveryService(
        IDiscoveryRepository discoveryRepository,
        IDiscoveryVoteRepository voteRepository,
        ILoggingService loggingService)
    {
        _discoveryRepository = discoveryRepository;
        _voteRepository = voteRepository;
        _loggingService = loggingService;
    }

    public async Task<Discovery> CreateDiscoveryAsync(int reporterUserId, string objectType, 
        string suggestedName, double ra, double dec, string description, string attachments = null)
    {
        try
        {
            if (!AstronomicalCalculations.IsValidCoordinates(ra, dec))
                throw new ValidationException(ErrorMessages.INVALID_COORDINATES);

            var discovery = new Discovery
            {
                ReporterUserID = reporterUserId,
                ObjectType = Enum.Parse<ObjectType>(objectType),
                SuggestedName = suggestedName,
                RA = ra,
                Dec = dec,
                Description = description,
                Attachments = attachments,
                State = DiscoveryState.Pendiente,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdDiscovery = await _discoveryRepository.AddAsync(discovery);
            
            await _loggingService.LogInfoAsync("DiscoveryCreated", 
                $"New discovery reported: {suggestedName} by user {reporterUserId}", reporterUserId);

            return createdDiscovery;
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("DiscoveryCreation", 
                $"Error creating discovery by user {reporterUserId}", reporterUserId, null, ex);
            throw;
        }
    }

    public async Task<Discovery> GetDiscoveryByIdAsync(int id)
    {
        var discovery = await _discoveryRepository.GetByIdAsync(id);
        if (discovery == null)
            throw new NotFoundException("Discovery", id);

        return discovery;
    }

    public async Task<IEnumerable<Discovery>> GetAllDiscoveriesAsync()
    {
        return await _discoveryRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Discovery>> GetDiscoveriesByStateAsync(string state)
    {
        return await _discoveryRepository.GetByStateAsync(state);
    }

    public async Task<IEnumerable<Discovery>> GetUserDiscoveriesAsync(int userId)
    {
        return await _discoveryRepository.GetByReporterAsync(userId);
    }

    public async Task<Discovery> UpdateDiscoveryStateAsync(int discoveryId, string state)
    {
        var discovery = await GetDiscoveryByIdAsync(discoveryId);
        discovery.State = Enum.Parse<DiscoveryState>(state);
        discovery.UpdatedAt = DateTime.UtcNow;

        await _discoveryRepository.UpdateAsync(discovery);
        
        await _loggingService.LogInfoAsync("DiscoveryStateUpdated", 
            $"Discovery {discoveryId} state updated to {state}", null);

        return discovery;
    }

    public async Task DeleteDiscoveryAsync(int discoveryId)
    {
        var discovery = await GetDiscoveryByIdAsync(discoveryId);
        await _discoveryRepository.DeleteAsync(discoveryId);
        
        await _loggingService.LogInfoAsync("DiscoveryDeleted", 
            $"Discovery {discoveryId} deleted", discovery.ReporterUserID);
    }

    // Votaciones
    public async Task VoteDiscoveryAsync(int discoveryId, int voterUserId, bool vote, string comment = null)
    {
        var discovery = await GetDiscoveryByIdAsync(discoveryId);

        if (await _voteRepository.HasVotedAsync(voterUserId, discoveryId))
            throw new ValidationException(ErrorMessages.ALREADY_VOTED);

        var discoveryVote = new DiscoveryVote
        {
            DiscoveryID = discoveryId,
            VoterUserID = voterUserId,
            Vote = vote,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };

        await _voteRepository.AddAsync(discoveryVote);
        
        await _loggingService.LogInfoAsync("DiscoveryVoted", 
            $"User {voterUserId} voted on discovery {discoveryId} (vote: {vote})", voterUserId);
    }

    public async Task RemoveVoteAsync(int discoveryId, int voterUserId)
    {
        var vote = await _voteRepository.GetByUserAndDiscoveryAsync(voterUserId, discoveryId);
        if (vote == null)
            throw new NotFoundException("Vote", $"{voterUserId}-{discoveryId}");

        await _voteRepository.DeleteAsync(vote.VoteID);
        
        await _loggingService.LogInfoAsync("VoteRemoved", 
            $"User {voterUserId} removed vote from discovery {discoveryId}", voterUserId);
    }

    public async Task<bool> HasUserVotedAsync(int discoveryId, int userId)
    {
        return await _voteRepository.HasVotedAsync(userId, discoveryId);
    }

    public async Task<int> GetUpvotesCountAsync(int discoveryId)
    {
        return await _voteRepository.GetUpvotesCountAsync(discoveryId);
    }

    public async Task<int> GetDownvotesCountAsync(int discoveryId)
    {
        return await _voteRepository.GetDownvotesCountAsync(discoveryId);
    }

    public async Task<double> GetApprovalRateAsync(int discoveryId)
    {
        var upvotes = await GetUpvotesCountAsync(discoveryId);
        var downvotes = await GetDownvotesCountAsync(discoveryId);
        var totalVotes = upvotes + downvotes;

        return totalVotes > 0 ? (double)upvotes / totalVotes : 0;
    }

    // Validaciones
    public async Task<bool> ValidateDiscoveryAsync(int discoveryId, int astronomerId)
    {
        var discovery = await GetDiscoveryByIdAsync(discoveryId);
        
        // Solo se puede validar si está en revisión
        if (discovery.State != DiscoveryState.RevisadoAstronomo)
            throw new ValidationException("Discovery must be under astronomer review");

        discovery.State = DiscoveryState.Aprobado;
        discovery.UpdatedAt = DateTime.UtcNow;

        await _discoveryRepository.UpdateAsync(discovery);
        
        await _loggingService.LogInfoAsync("DiscoveryValidated", 
            $"Discovery {discoveryId} validated by astronomer {astronomerId}", astronomerId);

        return true;
    }

    public async Task<bool> RejectDiscoveryAsync(int discoveryId, int astronomerId, string reason)
    {
        var discovery = await GetDiscoveryByIdAsync(discoveryId);
        
        discovery.State = DiscoveryState.Rechazado;
        discovery.Description += $"\n\nRejected by astronomer {astronomerId}. Reason: {reason}";
        discovery.UpdatedAt = DateTime.UtcNow;

        await _discoveryRepository.UpdateAsync(discovery);
        
        await _loggingService.LogInfoAsync("DiscoveryRejected", 
            $"Discovery {discoveryId} rejected by astronomer {astronomerId}. Reason: {reason}", astronomerId);

        return true;
    }
}