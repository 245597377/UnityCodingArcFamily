using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STFEngine.Module
{
    public interface ISTF_NetClient
    {
        void connect(string host, int port);

        void disconnect();

        /// <summary>
        /// 开始接听
        /// </summary>
        void StartListenRec();
    }
}
