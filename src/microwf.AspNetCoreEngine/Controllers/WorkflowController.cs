using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tomware.Microwf.Engine
{
  [Authorize]
  [Route("api/workflow")]
  public class WorkflowController : Controller
  {
    private readonly IWorkflowService _service;

    public WorkflowController(IWorkflowService service)
    {
      _service = service;
    }

    [HttpGet()]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(PaginatedList<WorkflowViewModel>), 200)]
    public async Task<ActionResult<PaginatedList<WorkflowViewModel>>> GetWorkflows(
      [FromQuery] WorkflowSearchPagingParameters pagingParameters
    )
    {
      PaginatedList<WorkflowViewModel> result
        = await _service.GetWorkflowsAsync(pagingParameters);

      AddXPagination(pagingParameters, result);

      return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(WorkflowViewModel), 200)]
    public async Task<ActionResult<WorkflowViewModel>> Get(int id)
    {
      var result = await _service.GetAsync(id);

      return Ok(result);
    }

    [HttpGet("{id}/history")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(IEnumerable<WorkflowHistory>), 200)]
    public async Task<ActionResult<IEnumerable<WorkflowHistory>>> GetHistory(int id)
    {
      var result = await _service.GetHistoryAsync(id);

      return Ok(result);
    }

    [HttpGet("{id}/variables")]
    [Authorize(Constants.MANAGE_WORKFLOWS_POLICY)]
    [ProducesResponseType(typeof(IEnumerable<WorkflowVariable>), 200)]
    public async Task<ActionResult<IEnumerable<WorkflowVariable>>> GetVariables(int id)
    {
      var result = await _service.GetVariablesAsync(id);

      return Ok(result);
    }

    [HttpGet("instance/{type}/{correlationId}")]
    [ProducesResponseType(typeof(WorkflowViewModel), 200)]
    public async Task<ActionResult<WorkflowViewModel>> GetInstance(string type, int correlationId)
    {
      WorkflowViewModel result = await _service.GetInstanceAsync(type, correlationId);

      return Ok(result);
    }

    [HttpGet("definitions")]
    [ProducesResponseType(typeof(IEnumerable<WorkflowDefinitionViewModel>), 200)]
    public ActionResult<IEnumerable<WorkflowDefinitionViewModel>> GetWorkflowDefinitions()
    {
      var result = _service.GetWorkflowDefinitions();

      return Ok(result);
    }

    [HttpGet("dot/{type}")]
    [ProducesResponseType(typeof(string), 200)]
    public ActionResult<string> Dot(string type)
    {
      var result = _service.Dot(type);

      return Ok(result);
    }

    [HttpGet("dotwithhistory/{type}/{correlationId}")]
    [ProducesResponseType(typeof(WorkflowViewModel), 200)]
    public async Task<ActionResult<WorkflowViewModel>> DotWithHistory(string type, int correlationId)
    {
      var result = await _service.DotWithHistoryAsync(type, correlationId);

      return Ok(result);
    }

    private void AddXPagination(
      PagingParameters pagingParameters,
      PaginatedList<WorkflowViewModel> result
    )
    {
      var paginationMetadata = new
      {
        totalCount = result.AllItemsCount,
        pageSize = pagingParameters.PageSize,
        pageIndex = pagingParameters.PageIndex,
        totalPages = result.TotalPages
      };

      Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
    }
  }
}
