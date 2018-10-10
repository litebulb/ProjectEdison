using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Common.Chat.Models
{
    public class QnAMakerResponse
    {
        public IList<Answer> Answers { get; set; }
    }

    public class Metadata
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Answer
    {
        public IList<string> Questions { get; set; }
        public string answer { get; set; }
        public double Score { get; set; }
        public int Id { get; set; }
        public string Source { get; set; }
        public IList<object> Keywords { get; set; }
        public IList<Metadata> Metadata { get; set; }
    }
}