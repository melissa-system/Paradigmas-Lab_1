using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.WebAPI.Controllers
{
    public partial class LibrariesController
    {
        [HttpGet("{libraryId}")]
        public async Task<IActionResult> Get(int libraryId)
        {
            var library = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
            if (library == null)
                return NotFound();
            return Ok(library);
        }
    }
}
