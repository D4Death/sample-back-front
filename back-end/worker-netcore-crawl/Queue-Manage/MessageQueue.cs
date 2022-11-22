using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace worker_netcore_crawl
{
    public class MessageQueue
    {
        private const int RANGE = 500;
        private static readonly Lazy<MessageQueue> lazy = new Lazy<MessageQueue>(() => new MessageQueue(), true);
        private static string queue_id = Guid.NewGuid().ToString();
        public static MessageQueue Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private MessageQueue() { }

        #region Binance realtime messages
        private ConcurrentQueue<string> m_bnbTradeQueue = new ConcurrentQueue<string>();

        public void BinanceEnqueue(string msg)
        {
            m_bnbTradeQueue.Enqueue(msg);
            //Console.WriteLine($"m_bnbTradeQueue ID {queue_id} count: {m_bnbTradeQueue.Count}");
        }

        public List<string> BinanceDequeueAll()
        {
            var listMsg = new List<string>();
            int count = 0;

            while (count < RANGE)
            {
                if (m_bnbTradeQueue.TryDequeue(out string msg))
                {
                    // Móc dữ liệu ra khỏi Queue
                    listMsg.Add(msg);
                }

                count++;
            }

            Console.WriteLine($"m_bnbTradeQueue ID {queue_id} remaining count: {m_bnbTradeQueue.Count}");

            return listMsg;
        }

        public void BinanceRollbackMsg(List<string> rollbackMsg)
        {
            foreach (var msg in rollbackMsg)
            {
                m_bnbTradeQueue.Enqueue(msg);
            }
        }

        public void BinanceClearQueue()
        {
            m_bnbTradeQueue.Clear();
        }
        #endregion

        #region VN Stock Queue
        private ConcurrentQueue<string> m_vnStockTradeQueue = new ConcurrentQueue<string>();

        public void VNStockEnqueue(string msg)
        {
            m_vnStockTradeQueue.Enqueue(msg);
            Console.WriteLine($"m_vnStockTradeQueue ID {queue_id} count: {m_vnStockTradeQueue.Count}");
        }

        public List<string> VNStockDequeueAll()
        {
            var listMsg = new List<string>();
            int count = 0;

            if (m_vnStockTradeQueue.Count > 0)
            {
                string msg;
                while (count < RANGE)
                {
                    if (m_vnStockTradeQueue.TryDequeue(out msg))
                    {
                        // Móc dữ liệu ra khỏi Queue
                        listMsg.Add(msg);
                    }

                    count++;
                }

                Console.WriteLine($"m_vnStockTradeQueue ID {queue_id} remaining count: {m_vnStockTradeQueue.Count}");
            }
            
            return listMsg;
        }

        public void VNStockRollbackMsg(List<string> rollbackMsg)
        {
            foreach (var msg in rollbackMsg)
            {
                m_vnStockTradeQueue.Enqueue(msg);
            }
        }
        #endregion
    }
}
