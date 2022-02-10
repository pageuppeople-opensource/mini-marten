using Microsoft.AspNetCore.Mvc;

namespace MartenApi.Controllers;

/// <summary>
/// Dev-only controller for hashing/unhashing ids
/// This could also be set up as a npm script as the js version will produce identical ids.
/// There is also a bash/powershell version.
/// </summary>
[Route("[controller]")]
[ApiController]
public class HashidController : ControllerBase
{
    [HttpGet("encode/{entityName}/{id:long}")]
    public string Encode(string entityName, long id)
    {
        // Create a dedicated hasher. Prevents hasher dictionary getting flooded with hashers for junk entity names.
        var hasher = HashIdManager.CreateHasher(entityName);
        return hasher.EncodeLong(id);
    }

    [HttpGet("decode/{entityName}/{id}")]
    public ActionResult<long> Encode(string entityName, string id)
    {
        // Create a dedicated hasher. Prevents hasher dictionary getting flooded with hashers for junk entity names.
        var hasher = HashIdManager.CreateHasher(entityName);

        try
        {
            var unhashedId = hasher.DecodeLong(id)[0];
            return Ok(unhashedId);
        }
        catch (Exception)
        {
            return BadRequest("Entity and/or hashed id is invalid");
        }
    }
}