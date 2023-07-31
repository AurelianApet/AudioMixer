using System;
using System.Collections.Generic;
using System.Text;

namespace AudioMixer
{
    public class HistoryManager
    {
        static Stack<Operation> history = new Stack<Operation>();
        static Stack<Operation> forwardHistory = new Stack<Operation>();
        public enum OperationType
        {
            Add, Delete, Change
        }
        public class Operation
        {
            private Object obj, obj1;
            private OperationType tp;

            public Operation(Object obj, OperationType tp)
            {
                this.obj = obj;
                this.tp = tp;
            }

            public void Undo()
            {
                if (tp == OperationType.Delete && obj.GetType() == typeof(List<AudioTrack>))
                {
                    List<AudioTrack> cur = (List<AudioTrack>)obj;
                    List<int> index = (List<int>)obj1;
                    for (int i=cur.Count; i-->0; ) 
                    {
                        AudioTrack a = cur[i];
                        AudioTrack b = a.GetParentCtrl();
                        b.AddChildCtrl(a, index[i]);
                        foreach (AudioTrack at in a.GetAllChild(true))
                        {
                            foreach (WaveForm wf in at.waves)
                            {
                                TimeLineContent.GetInstance().WaveFormAdd(wf);
                                TimeLineContent.GetInstance().samples.Add(wf.m_sprovider);
                            }
                        }
                    }
                    foreach(AudioTrack a in TrackView.GetInstance().selectedControls) a.UnSelected();
                    TrackView.GetInstance().selectedControls.Clear();
                    foreach (AudioTrack a in cur)
                    {
                        a.Selected();
                        TrackView.GetInstance().selectedControls.Add(a);
                    }
                    TrackView.GetInstance().RefreshAll();
                    TrackView.GetInstance().Focus();
                }
                if (tp == OperationType.Delete && obj.GetType() == typeof(List<WaveForm>))
                {
                    List<WaveForm> cur = (List<WaveForm>)obj;
                    foreach (WaveForm a in cur)
                    {
                        a.trackCtrl.AddWaveForm(a);
                        TimeLineContent.GetInstance().WaveFormAdd(a);
                        TimeLineContent.GetInstance().samples.Add(a.m_sprovider);
                    }
                    foreach (WaveForm wf in TimeLineContent.GetInstance().selectedWaveForms) wf.UnSelected();
                    TimeLineContent.GetInstance().selectedWaveForms.Clear();
                    foreach (WaveForm wf in cur) TimeLineContent.GetInstance().selectedWaveForms.Add(wf);
                }
            }
            public void Do()
            {
                if (tp == OperationType.Delete && obj.GetType() == typeof(List<AudioTrack>))
                {
                    List<AudioTrack> cur = (List<AudioTrack>)obj;
                    List<int> index = new List<int>();
                    foreach(AudioTrack a in cur)
                    {
                        AudioTrack b = a.GetParentCtrl();
                        index.Add(b.RemoveChild(a));
                        foreach (AudioTrack at in a.GetAllChild(true))
                        {
                            foreach (WaveForm wf in at.waves)
                            {
                                TimeLineContent.GetInstance().WaveFormRemove(wf);
                                wf.Stop();
                                TimeLineContent.GetInstance().mixer?.RemoveMixerInput(wf.m_sprovider);
                                TimeLineContent.GetInstance().samples.Remove(wf.m_sprovider);
                            }
                        }
                    }
                    obj1 = index;
                    TrackView.GetInstance().RefreshAll();
                    TrackView.GetInstance().selectedControls.Clear();
                    TrackView.GetInstance().Focus();
                }
                if (tp == OperationType.Delete && obj.GetType() == typeof(List<WaveForm>))
                {
                    List<WaveForm> cur = (List<WaveForm>)obj;
                    foreach(WaveForm a in cur)
                    {
                        TimeLineContent.GetInstance().WaveFormRemove(a);
                        TimeLineContent.GetInstance().mixer?.RemoveMixerInput(a.m_sprovider);
                        TimeLineContent.GetInstance().samples.Remove(a.m_sprovider);
                        a.trackCtrl.RemoveWaveForm(a);
                        a.Stop();
                    }
                    TimeLineContent.GetInstance().selectedWaveForms.Clear();
                }
            }
        }

        public static void Do(Operation op)
        {
            op.Do();
            forwardHistory.Clear();
            history.Push(op);
        }

        public static void Undo()
        {
            if (history.Count == 0) return;
            Operation op = history.Pop();
            op.Undo();
            forwardHistory.Push(op);
        }

        public static void Redo()
        {
            if (forwardHistory.Count == 0) return;
            Operation op = forwardHistory.Pop();
            op.Do();
            history.Push(op);
        }

        public static void Clear()
        {
            history.Clear();
            forwardHistory.Clear();
        }
    }
}
