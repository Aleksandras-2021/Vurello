using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK.Controllers;
using PSK.Server.Data.Entities;
using PSK.Server.Misc;
using PSK.Server.Services;

namespace PSK.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/label")]
    public class LabelController : GenericController<Label, LabelCreate, LabelUpdate>
    {
        private readonly ILabelService _labelService;
        private readonly IUserContext _userContext;
        public LabelController(ILabelService service, IUserContext userContext) : base(service)
        {
            _labelService = service;
            _userContext = userContext;
        }

    }
}
