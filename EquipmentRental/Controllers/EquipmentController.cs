using EquipmentRental.Api.Controllers.Common;
using EquipmentRental.Application.Features.Equipment.Commands.CreateEquipment;
using EquipmentRental.Application.Features.Equipment.Commands.UpdateEquipmentStatus;
using EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentById;
using EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentById.Responses;
using EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentList;
using EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentList.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentRental.Api.Controllers
{

    [Authorize]
    public class EquipmentController : ApiBaseController
    {
        private readonly ISender _sender;

        public EquipmentController(ISender sender)
        {
            _sender = sender;
        }


        // GET api/equipment
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<EquipmentListItemResponse>>> GetList(CancellationToken ct)
            => HandleResult(await _sender.Send(new GetEquipmentListQuery(), ct));

        // GET api/equipment/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EquipmentResponse>> GetById(int id, CancellationToken ct)
            => HandleResult(await _sender.Send(new GetEquipmentByIdQuery(id), ct));

        // POST api/equipment  (Admin فقط)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateEquipmentCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);
            return result.IsSuccess ? StatusCode(StatusCodes.Status201Created) : HandleResult(result);
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(
               int id,
               [FromBody] UpdateEquipmentStatusCommand command,
                                       CancellationToken cancellationToken)
        {
            if (id != command.EquipmentId)
            {
                return BadRequest("Route ID must match EquipmentId.");
            }

            var result = await _sender.Send(command, cancellationToken);

            return HandleResult(result);
        }


    }
}
