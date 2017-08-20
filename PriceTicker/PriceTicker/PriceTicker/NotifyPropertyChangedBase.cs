using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace PriceTicker
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        private static readonly Dictionary<string, PropertyChangedEventArgs> eventArgCache;

        /// <summary>
        /// Initializes static members of the <see cref="NotifyPropertyChangedBase"/> class.
        /// </summary>
        static NotifyPropertyChangedBase()
        {
            eventArgCache = new Dictionary<string, PropertyChangedEventArgs>();
        }

        /// <summary>
        /// Event raised to notify of changed property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            this.VerifyPropertyExists(propertyName);

            var handler = this.PropertyChanged;
            if (null != handler)
            {
                // Get the cached event args.
                PropertyChangedEventArgs args = GetPropertyChangedEventArgs(propertyName);

                handler(this, args);
            }

            this.HandlePropertyChanged(propertyName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void HandlePropertyChanged(string propertyName)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <param name="forceNotify"></param>
        /// <returns></returns>
        protected bool SetAndNotify<T>(ref T field, T value, string propertyName, bool forceNotify = false)
        {
            bool shouldNotify = forceNotify;

            if (!Equals(field, value))
            {
                field = value;
                shouldNotify = true;
            }

            if (shouldNotify)
            {
                this.NotifyPropertyChanged(propertyName);
            }

            return shouldNotify;
        }

        private static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName cannot be null or empty");
            }

            PropertyChangedEventArgs args;

            lock (eventArgCache)
            {
                if (!eventArgCache.TryGetValue(propertyName, out args))
                {
                    eventArgCache.Add(propertyName, new PropertyChangedEventArgs(propertyName));
                }

                args = eventArgCache[propertyName];
            }

            return args;
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyExists(string propertyName)
        {
            Type type = this.GetType();

            PropertyInfo propertyInfo = type.GetProperty(propertyName);

            if (null == propertyInfo)
            {
                string message = string.Format("{0} is not a public property of {1}", propertyName, type.Name);

                Debug.WriteLine(message);
            }
        }
    }
}