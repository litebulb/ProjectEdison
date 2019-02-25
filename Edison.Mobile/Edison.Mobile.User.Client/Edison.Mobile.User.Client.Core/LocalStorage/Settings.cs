using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Plugin.Settings;
using Plugin.Settings.Abstractions;

using Newtonsoft.Json;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using System.Collections.ObjectModel;
using Edison.Core.Common.Models;

namespace Edison.Mobile.User.Client.Core.LocalStorage
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters.
    /// It uses the Settings plugin from James Montemagno
    /// https://github.com/jamesmontemagno/SettingsPlugin
    /// https://jamesmontemagno.github.io/SettingsPlugin/
    /// </summary>
    public static class Settings
    {

        private static ISettings AppSettings => CrossSettings.Current;


        public static List<Guid> CurrentResponseIds
        {
            get
            {
                var json = AppSettings.GetValueOrDefault(nameof(CurrentResponseIds), string.Empty);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<Guid>(0);
                return JsonConvert.DeserializeObject<List<Guid>>(json);
            }
            set
            {
                string json = JsonConvert.SerializeObject(value);
                AppSettings.AddOrUpdateValue(nameof(CurrentResponseIds), json);
            }
        }

        public static void ClearCurrentResponseIds()
        {
            CurrentResponseIds = new List<Guid>(0);
        }

        public static void AddCurrentResponseId(Guid id)
        {
            var responses = CurrentResponseIds;
            responses.Add(id);
            CurrentResponseIds = responses;
        }
        public static void AddCurrentResponseIds(List<Guid> ids)
        {
            var responses = CurrentResponseIds;
            responses.AddRange(ids);
            CurrentResponseIds = responses;
        }
        public static void AddCurrentResponseIds(ObservableRangeCollection<ResponseCollectionItemViewModel> items)
        {
            var ids = items.Select(i => i.Response.ResponseId).ToList();
            AddCurrentResponseIds(ids);
        }





        public static List<ResponseLightModel> CurrentResponseSummaries
        {
            get
            {
                var json = AppSettings.GetValueOrDefault(nameof(CurrentResponseSummaries), string.Empty);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<ResponseLightModel>(0);
                return JsonConvert.DeserializeObject<List<ResponseLightModel>>(json);
            }
            set
            {
                string json = JsonConvert.SerializeObject(value);
                AppSettings.AddOrUpdateValue(nameof(CurrentResponseSummaries), json);
            }
        }

        public static void ClearCurrentResponseSummaries()
        {
            CurrentResponseSummaries = new List<ResponseLightModel>(0);
        }

        public static void AddCurrentResponseSummaries(ResponseLightModel summary)
        {
            var responses = CurrentResponseSummaries;
            responses.Add(summary);
            CurrentResponseSummaries = responses;
        }
        public static void AddCurrentResponseSummaries(List<ResponseLightModel> summaries)
        {
            var responses = CurrentResponseSummaries;
            responses.AddRange(summaries);
            CurrentResponseSummaries = responses;
        }
        public static void AddCurrentResponseSummaries(ObservableRangeCollection<ResponseCollectionItemViewModel> items)
        {
            var summaries = items.Select(i => i.ToLightModel()).ToList();
            AddCurrentResponseSummaries(summaries);
        }



    }
}
