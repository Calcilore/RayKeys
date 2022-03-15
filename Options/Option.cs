using System;

namespace RayKeys.Options {
    public class Option {
        public OptionType OptionType { get; }
        public object CurrentValue;
        public Action<string, object> ChangedEvent;
        public string DisplayName;
        public string Id;
        
        public Option(string id, string displayName, OptionType optionType, object defaultValue, Action<string, object> changedFunc) {
            Id = id;
            OptionType = optionType;
            CurrentValue = defaultValue;
            ChangedEvent = changedFunc;
            DisplayName = displayName;
        }
    }
}