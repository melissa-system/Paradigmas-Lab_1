using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryService.WebAPI.Data;

namespace LibraryService.WebAPI.Controllers
{
    public partial class LibrariesController
    {
        [HttpPost]
        public async Task<IActionResult> Add(Library l)
        {
            await _librariesService.Add(l);
            return Ok(l);
        }
    }
}
