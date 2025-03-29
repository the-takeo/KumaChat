using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KumaAI;

namespace KumaChat.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            KumaMessage = "ƒNƒ}‚¾‚ª?";
        }

        [BindProperty]
        public string MyMessage { get; set; } = "";

        [BindProperty]
        public string KumaMessage { get; set; } = "";

        [BindProperty]
        public Kuma.Emotion KumaEmotion { get; set; } = Kuma.Emotion.Other;

        private Kuma kuma = new Kuma();

        [BindProperty]
        public string KumaImageFile { get; set; } = "images/Kuma99.png";

        public async Task OnPost()
        {
            KumaMessage = await kuma.GetKumaMessage(MyMessage);

            KumaEmotion = await kuma.GetKumaEmotion(KumaMessage);
            KumaImageFile = "images/Kuma" + (int)KumaEmotion + ".png";

            MyMessage = string.Empty;
        }
    }
}
