using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    [HttpGet("{image}")]
    public ActionResult<byte[]> GetImage(string image)
    {
        try
        {
            var bytes = System.IO.File.ReadAllBytes("Images/" + image.Replace("%2F", "/"));
            return File(bytes, "image/jpeg");
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [Authorize]
    [HttpPost("UploadImage")]
    public async Task<ActionResult<string>> UploadImage([FromForm] IFormFile file, string path)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не выбран");
        if (file.ContentType != "image/jpeg")
            return BadRequest("Неверный формат данных");
        string fullPath = path + "/" + Path.GetRandomFileName() + ".jpg";
        try
        {
            using (var stream = new FileStream(Path.Combine("Images/", fullPath), FileMode.Create))
                await file.CopyToAsync(stream);
            return Ok(fullPath);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}