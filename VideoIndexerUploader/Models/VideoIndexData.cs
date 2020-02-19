using System;
using System.Collections.Generic;
using System.Text;

namespace VideoIndexerUploader.Models
{
    public class VideoIndexData
    {
        public object partition { get; set; }
        public object description { get; set; }
        public string privacyMode { get; set; }
        public string state { get; set; }
        public string accountId { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string userName { get; set; }
        public DateTime created { get; set; }
        public bool isOwned { get; set; }
        public bool isEditable { get; set; }
        public bool isBase { get; set; }
        public int durationInSeconds { get; set; }
        public SummarizedInsights summarizedInsights { get; set; }
        public List<Video> videos { get; set; }
        public List<VideosRange> videosRanges { get; set; }
    }
    public class Duration
    {
        public string time { get; set; }
        public double seconds { get; set; }
    }

    public class Appearance
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
        public double startSeconds { get; set; }
        public double endSeconds { get; set; }
    }

    public class Face
    {
        public string videoId { get; set; }
        public int confidence { get; set; }
        public object description { get; set; }
        public object title { get; set; }
        public string thumbnailId { get; set; }
        public double seenDuration { get; set; }
        public double seenDurationRatio { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public List<Appearance> appearances { get; set; }
    }

    public class Keyword
    {
        public bool isTranscript { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public List<Appearance> appearances { get; set; }
    }

    public class Sentiment
    {
        public string sentimentKey { get; set; }
        public double seenDurationRatio { get; set; }
        public List<Appearance> appearances { get; set; }
    }

    public class Label
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Appearance> appearances { get; set; }
    }

    public class SpeakerTalkToListenRatio
    {
        public double __invalid_name__1 { get; set; }
        public double __invalid_name__2 { get; set; }
        public double __invalid_name__3 { get; set; }
    }

    public class SpeakerLongestMonolog
    {
        public int __invalid_name__1 { get; set; }
        public int __invalid_name__2 { get; set; }
        public int __invalid_name__3 { get; set; }
    }

    public class SpeakerNumberOfFragments
    {
        public int __invalid_name__1 { get; set; }
        public int __invalid_name__2 { get; set; }
        public int __invalid_name__3 { get; set; }
    }

    public class SpeakerWordCount
    {
        public int __invalid_name__1 { get; set; }
        public int __invalid_name__2 { get; set; }
        public int __invalid_name__3 { get; set; }
    }

    public class Statistics
    {
        public int correspondenceCount { get; set; }
        public SpeakerTalkToListenRatio speakerTalkToListenRatio { get; set; }
        public SpeakerLongestMonolog speakerLongestMonolog { get; set; }
        public SpeakerNumberOfFragments speakerNumberOfFragments { get; set; }
        public SpeakerWordCount speakerWordCount { get; set; }
    }

    public class SummarizedInsights
    {
        public string name { get; set; }
        public string id { get; set; }
        public string privacyMode { get; set; }
        public Duration duration { get; set; }
        public string thumbnailVideoId { get; set; }
        public string thumbnailId { get; set; }
        public List<Face> faces { get; set; }
        public List<Keyword> keywords { get; set; }
        public List<Sentiment> sentiments { get; set; }
        public List<object> emotions { get; set; }
        public List<object> audioEffects { get; set; }
        public List<Label> labels { get; set; }
        public List<object> framePatterns { get; set; }
        public List<object> brands { get; set; }
        public List<object> namedLocations { get; set; }
        public List<object> namedPeople { get; set; }
        public Statistics statistics { get; set; }
        public List<object> topics { get; set; }
    }

    public class Instance
    {
        public string adjustedStart { get; set; }
        public string adjustedEnd { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }

    public class Transcript
    {
        public int id { get; set; }
        public string text { get; set; }
        public double confidence { get; set; }
        public int speakerId { get; set; }
        public string language { get; set; }
        public List<Instance> instances { get; set; }
    }

    public class Ocr
    {
        public int id { get; set; }
        public string text { get; set; }
        public double confidence { get; set; }
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string language { get; set; }
        public List<Instance> instances { get; set; }
    }

    public class Thumbnail
    {
        public string id { get; set; }
        public string fileName { get; set; }
        public List<Instance> instances { get; set; }
    }

    public class Scene
    {
        public int id { get; set; }
        public List<Instance> instances { get; set; }
    }

    public class KeyFrame
    {
        public int id { get; set; }
        public List<Instance> instances { get; set; }
    }

    public class Shot
    {
        public int id { get; set; }
        public List<string> tags { get; set; }
        public List<KeyFrame> keyFrames { get; set; }
        public List<Instance> instances { get; set; }
    }

    public class Block
    {
        public int id { get; set; }
        public List<Instance> instances { get; set; }
    }

    public class Speaker
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Instance> instances { get; set; }
    }

    public class TextualContentModeration
    {
        public int id { get; set; }
        public int bannedWordsCount { get; set; }
        public int bannedWordsRatio { get; set; }
        public List<object> instances { get; set; }
    }

    public class Insights
    {
        public string version { get; set; }
        public string duration { get; set; }
        public string sourceLanguage { get; set; }
        public List<string> sourceLanguages { get; set; }
        public string language { get; set; }
        public List<string> languages { get; set; }
        public List<Transcript> transcript { get; set; }
        public List<Ocr> ocr { get; set; }
        public List<Keyword> keywords { get; set; }
        public List<Face> faces { get; set; }
        public List<Label> labels { get; set; }
        public List<Scene> scenes { get; set; }
        public List<Shot> shots { get; set; }
        public List<Sentiment> sentiments { get; set; }
        public List<Block> blocks { get; set; }
        public List<Speaker> speakers { get; set; }
        public TextualContentModeration textualContentModeration { get; set; }
        public Statistics statistics { get; set; }
    }

    public class Video
    {
        public string accountId { get; set; }
        public string id { get; set; }
        public string state { get; set; }
        public string moderationState { get; set; }
        public string reviewState { get; set; }
        public string privacyMode { get; set; }
        public string processingProgress { get; set; }
        public string failureCode { get; set; }
        public string failureMessage { get; set; }
        public object externalId { get; set; }
        public object externalUrl { get; set; }
        public object metadata { get; set; }
        public Insights insights { get; set; }
        public string thumbnailId { get; set; }
        public bool detectSourceLanguage { get; set; }
        public string languageAutoDetectMode { get; set; }
        public string sourceLanguage { get; set; }
        public List<string> sourceLanguages { get; set; }
        public string language { get; set; }
        public List<string> languages { get; set; }
        public string indexingPreset { get; set; }
        public string linguisticModelId { get; set; }
        public string personModelId { get; set; }
        public bool isAdult { get; set; }
        public string publishedUrl { get; set; }
        public object publishedProxyUrl { get; set; }
        public string viewToken { get; set; }
    }

    public class Range
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class VideosRange
    {
        public string videoId { get; set; }
        public Range range { get; set; }
    }
}
