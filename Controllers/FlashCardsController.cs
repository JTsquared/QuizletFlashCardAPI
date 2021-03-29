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
    public class FlashCardsController : ControllerBase
    {
        private readonly ILogger<FlashCardsController> _logger;
        private IConfiguration _configuration;
        private IHtmlParser _htmlParser;

        public FlashCardsController(ILogger<FlashCardsController> logger, IConfiguration configuration, IHtmlParser htmlParser) {
            _logger = logger;
            _configuration = configuration;
            _htmlParser = htmlParser;
        }

        [HttpGet]
        public IEnumerable<FlashCard> Get(string flashCardTopic)
        {
            try
            {
                WebScraper webScraper = new WebScraper(_configuration, _htmlParser);
                string topic = WebUtility.UrlEncode(flashCardTopic);
                var quizUrl = webScraper.GetUrlFromTopic(topic);
                List<FlashCard> flashCards = webScraper.Scrape(quizUrl);
                return flashCards;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Unable to find data - Error: " + e.Message);
            }

            return new List<FlashCard>();
        }
    }
}
