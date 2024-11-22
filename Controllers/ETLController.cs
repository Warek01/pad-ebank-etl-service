using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PadEbankETLService.Data.DataWarehouse;
using PadEbankETLService.Services;

namespace PadEbankETLService.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("Api/V{v:apiVersion}/[controller]")]
public class ETLController(DataWarehouseDbContext dataWarehouseDb) : ControllerBase {
   [HttpGet]
   public async Task<ActionResult> Test() {
      List<FactCardTransaction> result = await dataWarehouseDb.FactCardTransactions
         .ToListAsync();
      return Ok(result);
   }

   [HttpPost]
   public async Task<ActionResult> Perform() {
      try {
         TimeSpan duration = await ETLService.Perform();
         return Ok(new { Success = true, Duration = duration });
      }
      catch (Exception ex) {
         Console.WriteLine(ex);
         Response.StatusCode = StatusCodes.Status500InternalServerError;
         return new ObjectResult(new { Error = ex.ToString() });
      }
   }
}
