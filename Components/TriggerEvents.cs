namespace Chloride.RA2Scripts.Components
{
    public class TriggerEvents
    {
        //triggerID = eventsCount, (eventID,tag,p1), (eventID,tag,p1,[optional p2]), ...
        private string[] raw;
        private int seek;
        private int cur;
        public int Length => int.Parse(raw[0]);
        public TriggerEvents(string[] tEvents)
        {
            raw = tEvents;
            ResetSeek();
        }
        public void ResetSeek()
        {
            seek = 1;
            cur = 0;
        }
        public bool Seekable => cur < Length;
        public void Next()
        {
            if (!Seekable)
                return;
            cur++;
            seek += CurrentParamTag switch
            {
                2 => 4,
                _ => 3
            };
        }
        public string CurrentID => raw[seek];
        public int CurrentParamTag => int.Parse(raw[seek + 1]);  // real p1
        public string GetCurrentParamX(int paramOrder) => raw[seek + paramOrder + 1];  // actually p2, or p3
        public void SetCurrentParamX(int paramOrder, string val) => raw[seek + paramOrder + 1] = val;
        public override string ToString() => string.Join(',', raw);
    }
}
