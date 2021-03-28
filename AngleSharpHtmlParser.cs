using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace FlashCardAPI
{
    public class AngleSharpHtmlParser : IHtmlParser
    {
        private ILogger _logger;
        private FlashCardDOMQueries _flashCardDOMQueries;
        private string _mockBrowserUserAgent;

        public AngleSharpHtmlParser(IConfiguration configuration, ILogger<AngleSharpHtmlParser> logger)
        {
            _logger = logger;
            _flashCardDOMQueries = new FlashCardDOMQueries();
            configuration.GetSection("FlashCardDOMQueries").Bind(_flashCardDOMQueries);
            _mockBrowserUserAgent = configuration["MockBrowserUserAgent"];
        }

        public List<FlashCard> GetFlashCards(IDocument document)
        {
            IEnumerable<IElement> flashCardElements;
            List<FlashCard> flashCards = new List<FlashCard>();

            try
            {
                //quizElements = document.All.Where(x => x.ClassName == "SetPageTerms-term");
                flashCardElements = document.QuerySelectorAll(_flashCardDOMQueries.FlashCardElementsQuery);

                if (flashCardElements.Any())
                {
                    foreach (var flashCard in flashCardElements)
                    {
                        flashCards.Add(new FlashCard()
                        {
                            Question = flashCard.QuerySelector(_flashCardDOMQueries.FlashCardQuestionQuery).Text(),
                            Answer = flashCard.QuerySelector(_flashCardDOMQueries.FlashCardAnswerQuery).Text()
                        });

                    }
                }
                else
                {
                    _logger.LogInformation("unable to find any flash card results");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred while attempting to fetch flash cards, Exception: {e.Message}");
            }

            return flashCards;
        }

        public string GetFlashCardUrl(IDocument document)
        {
            string quizUrl = document.QuerySelector(_flashCardDOMQueries.QuizLinkQuery).GetAttribute("href");
            return quizUrl;
        }

        public async Task<IDocument> ParseDocument(string source)
        {
            var requester = new DefaultHttpRequester();
            requester.Headers["User-Agent"] = _mockBrowserUserAgent;
            var config = Configuration.Default.With(requester).WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(source);
            return document;
        }
    }

    public class FlashCardDOMQueries
    {
        public string QuizLinkQuery { get; set; }
        public string FlashCardElementsQuery { get; set; }
        public string FlashCardQuestionQuery { get; set; }
        public string FlashCardAnswerQuery { get; set; }
    }
}
