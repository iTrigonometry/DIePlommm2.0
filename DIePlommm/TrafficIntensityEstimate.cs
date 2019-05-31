using System;
using System.Diagnostics;

namespace DIePlommm
{
    class TrafficIntensityEstimate
    {

        private PerformanceCounter inputTrafficCounter;
        private PerformanceCounterCategory[] categoriesOfCounters;
        private int indexOfNetworkInterface;
        
        /**
         * Конструктор
         */
        public TrafficIntensityEstimate()
        {
            categoriesOfCounters = PerformanceCounterCategory.GetCategories();
            indexOfNetworkInterface = findNetworkInterface();

            string[] instanceNames = getNetworkInterfaces();
            inputTrafficCounter = new PerformanceCounter(categoriesOfCounters[indexOfNetworkInterface].CategoryName, "Bytes Received/sec", instanceNames[0]);
        }


        /**
         * Ищет категорию сетевых интерфейсов среди всех интерфейсов
         */
        private int findNetworkInterface()
        {
            for (int i = 0; i < categoriesOfCounters.Length; i++)
                if (categoriesOfCounters[i].CategoryName.Equals("Network Interface"))
                    return i;
            return -1;
        }

        
        /**
         * Возвращает все сетевые интерфейсы которые есть на компьютере
         */
        public string[] getNetworkInterfaces()
        {
            string[] instanceNames = categoriesOfCounters[indexOfNetworkInterface].GetInstanceNames();
            return instanceNames;
        }


        /**
         * Опредеялет сетевой интерфейс для Counter
         */
        public void setNetworkInterface(String networkInterface)
        {
            inputTrafficCounter = new PerformanceCounter(categoriesOfCounters[indexOfNetworkInterface].CategoryName, "Bytes Received/sec", networkInterface);
        }

        /**
         * Возвращает значение Counter
         */
        public float getInputTraffic()
        {
            return inputTrafficCounter.NextValue();
        }


        /**
         *На основе того идет ли трафик будет определяться есть подключение или нет
         */
        public bool isNetworkConnection()
        {
            if(getInputTraffic() > 0)
            {
                return true;
            }
            return false;
        }

        public string getStringMBits()
        {
            double data = Math.Round(inputTrafficCounter.NextValue(), 3) / 125000;
            return data.ToString() + "Mbits";
        }

        public string getStringKBits()
        {
            double data = Math.Round(inputTrafficCounter.NextValue(), 3) / 125;
            return data.ToString() + "Mbits";
        }

        
    }
}
