using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WanderlustJournal.Pages.Journal
{
    [Authorize]
    public abstract class AuthorizedPageModel : PageModel
    {
        // Base class for journal pages that require authorization
    }
}