using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.Procedure
{
    public class UnassignUsersFromProcedureCommandHandler : IRequestHandler<UnAssignUserFromProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;
        public UnassignUsersFromProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<Unit>> Handle(UnAssignUserFromProcedureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate request early to reduce nesting
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));

                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

                // Combine queries to minimize the number of database hits
                var planWithProcedure = await _context.Plans
                    .Include(p => p.PlanProceduresUsers)
                    .FirstOrDefaultAsync(p => p.PlanId == request.PlanId);

                if (planWithProcedure is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));

                // Check for the procedure directly and include related users
                var procedureWithUsers = await _context.Procedures
                    .Include(p => p.PlanProcedureUsers)
                    .FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId);

                if (procedureWithUsers is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));

                // Filter and remove users in a single query
                var removedUsers = procedureWithUsers.PlanProcedureUsers
                    .Where(ppu => ppu.PlanId == request.PlanId && ppu.UserId == request.UserId)
                    .ToList();

                if (removedUsers.Any())
                {
                    _context.PlanProceduresUsers.RemoveRange(removedUsers);
                    await _context.SaveChangesAsync(); 
                }

                return ApiResponse<Unit>.Succeed(new Unit());
            }
            catch (Exception e)
            {
                return ApiResponse<Unit>.Fail(e);
            }

        }

    }
}
