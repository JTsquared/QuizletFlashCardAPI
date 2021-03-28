using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FlashCardAPI
{
    public interface IHtmlParser
    {
        Task<IDocument> ParseDocument(string source);
        List<FlashCard> GetFlashCards(IDocument document);
        string GetFlashCardUrl(IDocument document);
    }
}
