using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RL.Backend.Commands;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ProceduresController : ControllerBase
{
    private readonly ILogger<ProceduresController> _logger;
    private readonly RLContext _context;
    private readonly IMediator _mediator;

    public ProceduresController(ILogger<ProceduresController> logger, RLContext context,IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    [EnableQuery]
    public IEnumerable<Procedure> Get()
    {
        return _context.Procedures;
    }


    [HttpPost("addUserToProcedure")]
    public async Task<IActionResult> AddUserToProcedure(AddUserToProcedureCommand command, CancellationToken token)
    {
        // Add the user to procedure
        var response = await _mediator.Send(command, token);
        return response.ToActionResult();
    }

    [HttpDelete("deleteUsersFromProcedure")]
    public async Task<IActionResult> DeleteUsersFromProcedure(DeleteUsersFromProcedureCommand command, CancellationToken token)
    {
        // Delete all the users from the procedure
        var response = await _mediator.Send(command, token);
        return response.ToActionResult();
    }

    [HttpDelete("unassignUsersFromProcedure")]
    public async Task<IActionResult> UnAssignUsersFromProcedure(UnAssignUserFromProcedureCommand command, CancellationToken token)
    {
        // Unassign the users from the procedure
        var response = await _mediator.Send(command, token);
        return response.ToActionResult();
    }
}
