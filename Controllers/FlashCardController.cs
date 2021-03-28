using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlashCardAPI.Controllers
{
    [Route("api/[controller]/{flashCardTopic}")]
    [ApiController]
    public class FlashCardController : ControllerBase
    {
        private readonly ILogger<FlashCardController> _logger;
        private IConfiguration _configuration;
        private IHtmlParser _htmlParser;

        public FlashCardController(ILogger<FlashCardController> logger, IConfiguration configuration, IHtmlParser htmlParser) {
            _logger = logger;
            _configuration = configuration;
            _htmlParser = htmlParser;
        }

        [HttpGet]
        public IEnumerable<FlashCard> Get(string flashCardTopic)
        {
            WebScraper webScraper = new WebScraper(_configuration, _htmlParser);
            string topic = WebUtility.UrlEncode(flashCardTopic);
            var quizUrl = webScraper.GetUrlFromTopic(topic);
            List<FlashCard> flashCards = webScraper.Scrape(quizUrl);
            return flashCards;
        }
    }
}
