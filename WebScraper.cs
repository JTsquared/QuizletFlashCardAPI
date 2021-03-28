using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading;

namespace FlashCardAPI
{
    public class WebScraper
    {
        private IHtmlParser _htmlParser;
        private string _baseUrl;

        public WebScraper(IConfiguration configuration, IHtmlParser htmlParser)
        {
            _baseUrl = configuration["FlashCardDOMQueries:BaseUrl"];
            _htmlParser = htmlParser;
        }

        internal string GetUrlFromTopic(string quizTopic)
        {
            var quizUrl = "";
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            var document = _htmlParser.ParseDocument(_baseUrl + quizTopic).Result;
            cancellationToken.Token.ThrowIfCancellationRequested();
            quizUrl = _htmlParser.GetFlashCardUrl(document);

            return quizUrl;
        }

        internal List<FlashCard> Scrape(string quizUrl)
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            var document = _htmlParser.ParseDocument(quizUrl).Result;
            cancellationToken.Token.ThrowIfCancellationRequested();
            List<FlashCard> flashCards = _htmlParser.GetFlashCards(document);

            return flashCards;
        }
    }
}
