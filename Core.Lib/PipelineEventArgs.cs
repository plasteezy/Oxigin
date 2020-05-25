using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using Global.Models;

namespace Core.Lib
{
    public class PipelineEventArgs : CancelEventArgs
    {
        public PipelineEvents Events;
        public HttpClient Client;
        public Job Job;
        public List<string> ModuleCollection;


        public PipelineEventArgs(Job job, PipelineEvents events, List<string> moduleCollection, HttpClient client)
        {
            Events = events;
            Job = job;
            Client = client;
            ModuleCollection = moduleCollection;
        }
    }
}