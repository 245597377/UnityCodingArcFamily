using STFEngine.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace STFEngine.Module
{
    
    public class STF_NetControl : STF_Singleton<STF_NetControl>
    {

        private ISTF_NetClient sockClient;

        public T RegiestClient<T>() where T: ISTF_NetClient, new()
        {
            return new T();
        }
    }
}
