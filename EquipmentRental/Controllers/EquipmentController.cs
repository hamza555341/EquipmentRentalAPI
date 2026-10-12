using EquipmentRental.Api.Controllers.Common;
using EquipmentRental.Application.Features.Equipment.Commands.CreateEquipment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentRental.Api.Controllers
{

   
    public class EquipmentController : ApiBaseController
    {
        private readonly ISender _sender;

        public EquipmentController(ISender sender)
        {
            _sender = sender;
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentCommand command, CancellationToken cancellationToken)

        { 
          var result = await _sender.Send(command, cancellationToken);
            return HandleResult(result);
        }




    }
}
