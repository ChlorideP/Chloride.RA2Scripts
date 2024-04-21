namespace Chloride.RA2Scripts.Components
{
    public class TriggerActions
    {
        //triggerID = actionsCount, (actionID,p1,p2,p3,p4,p5,p6,p7), ...
        private string[] raw;
        private int seek;
        private int cur;
        public int Length => int.Parse(raw[0]);
        public TriggerActions(string[] tActions)
        {
            raw = tActions;
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
            seek += 8;
        }
        public string CurrentID => raw[seek];
        public string GetCurrentParamX(int paramOrder) => raw[seek + paramOrder];
        public void SetCurrentParamX(int paramOrder, string val) => raw[seek + paramOrder] = val;
        public override string ToString() => string.Join(',', raw);
    }
}
