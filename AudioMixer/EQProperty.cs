using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using Vst;

namespace AudioMixer
{
    public class EQProperty : IDisposable
    {
        public class BandHandle
        {
            public bool enabled;
            public int type = 0;
            public float db = 0;
            public float frequency = 0;
            public float factor = 0;
            public BandHandle(int t)
            {
                type = t;
                db = 0;
                factor = 1.0f;
                enabled = false;
                frequency = new float[] { 100, 800, 2000, 12000 }[t];
            }
            public void Load(System.IO.BinaryReader bin)
            {
                type = bin.ReadInt32();
                db = bin.ReadSingle();
                factor = bin.ReadSingle();
                enabled = bin.ReadBoolean();
                frequency = bin.ReadSingle();
            }
            public void Save(System.IO.BinaryWriter bin)
            {
                bin.Write(type);
                bin.Write(db);
                bin.Write(factor);
                bin.Write(enabled);
                bin.Write(frequency);
            }
            public float GetFactor()
            {
                return 1 / (factor + 1);
            }
            public void SetDefault()
            {
                db = 0;
                factor = 1.0f;
                enabled = false;
                frequency = new float[] { 100, 800, 2000, 12000 }[type];
            }
        }
        public BandHandle[] handleItem;
        public bool isHC, isLC;
        public float highcut;
        public float lowcut;
        public float Volume;
        public bool Mute;
        public List<string> eqDll;
        public List<bool> dllActive;
        public int dllCount;
        public List<object> dllitem;
        public List<PluginEditorForm> dllEditor;
        public EQProperty()
        {
            handleItem = new BandHandle[4];
            isHC = false; isLC = false;
            highcut = 20000; lowcut = 20;
            eqDll = new List<string>();
            dllActive = new List<bool>();
            dllitem = new List<object>();
            dllEditor = new List<PluginEditorForm>();
            dllCount = 13;
            for (int i = 0; i < dllCount; i++) eqDll.Add("");
            for (int i = 0; i < dllCount; i++) dllActive.Add(false);
            for (int i = 0; i < dllCount; i++) dllitem.Add(null);
            for (int i = 0; i < dllCount; i++) dllEditor.Add(null);
            Mute = false; Volume = 1.0f;
            for (int i = 0; i < 4; i++)
            {
                handleItem[i] = new BandHandle(i);
                handleItem[i].SetDefault();
            }
        }
        public bool IsSet
        {
            get
            {
                if (isHC || isLC) return true;
                for (int i = 0; i < 4; i++) if (handleItem[i].enabled) return true;
                return false;
            }
        }
        public void ReLoad(int id, dllButton cbtn)
        {
            try
            {
                if (dllitem[id] != null)
                {
                    (dllitem[id] as PluginItem).Dispose();
                    dllitem[id] = null;
                }
                dllitem[id] = new PluginItem(eqDll[id]);
                if (!(dllitem[id] as PluginItem).isValid())
                {
                    (dllitem[id] as PluginItem).Dispose();
                    dllitem[id] = null;
                    return;
                }
                ShowDllGui(id, cbtn);
            }
            catch
            {

            }
        }
        public void ShowDllGui(int id, dllButton cbtn)
        {
            try
            {
                if (dllitem[id] == null)
                {
                    dllitem[id] = new PluginItem(eqDll[id]);
                }

                if (!(dllitem[id] as PluginItem).isValid()) return;
                if (dllEditor[id] == null)
                {
                    dllEditor[id] = new PluginEditorForm();
                    dllEditor[id].FormClosed += (sender, e) =>
                    {
                        cbtn.IsEq = false;
                    };
                }
                Rectangle wndRect = new Rectangle();
                if ((dllitem[id] as PluginItem).command.EditorGetRect(out wndRect))
                {
                    dllEditor[id].ClientSize = wndRect.Size;
                }

                (dllitem[id] as PluginItem).command.EditorOpen(dllEditor[id].Handle);

                dllEditor[id].Show();
            } catch
            {

            }
        }

        public void HideDllGui(int id)
        {
            try
            {
                if (dllitem[id] == null || !(dllitem[id] as PluginItem).isValid()) return;
                if (dllEditor[id] != null)
                {
                    (dllitem[id] as PluginItem).command.EditorClose();
                    dllEditor[id].Dispose();
                }
            } catch (Exception e)
            {
            }
            dllEditor[id] = null;
        }
        public void Close()
        {
            try
            {
                for (int i = 0; i < eqDll.Count; i++)
                {
                    Close(i);
                }
            } catch
            {

            }
        }
        public void Close(int i)
        {
            try
            {
                HideDllGui(i);
                if (dllitem[i] != null) (dllitem[i] as PluginItem).Dispose();
            } catch
            {

            }
        }
        public void UpdateFromDll(int id, float[] buffer, int offset, int count)
        {
            try
            {
                if (!dllActive[id]) return;
                if (string.IsNullOrEmpty(eqDll[id])) return;
                if (dllitem[id] == null)
                {
                    dllitem[id] = new PluginItem(eqDll[id]);
                }

                if (!(dllitem[id] as PluginItem).isValid()) return;

                (dllitem[id] as PluginItem).process(buffer, offset, count, 2, 44100.0f);
            } catch
            {

            }
        }

        public void Load(System.IO.BinaryReader bin)
        {
            try
            {
                isHC = bin.ReadBoolean();
                isLC = bin.ReadBoolean();
                highcut = bin.ReadSingle();
                lowcut = bin.ReadSingle();
                for (int i = 0; i < dllCount; i++)
                {
                    eqDll[i] = AudioTrack.ReadString(bin);
                    if (eqDll[i].Length > 0)
                        eqDll[i] = MainForm.CurProjectPath + "\\" + eqDll[i];
                }
                for (int i = 0; i < dllCount; i++)
                {
                    dllActive[i] = bin.ReadBoolean();
                }
                Mute = bin.ReadBoolean();
                Volume = bin.ReadSingle();
                for (int i = 0; i < 4; i++) handleItem[i].Load(bin);
            } catch
            {

            }
        }

        public void Save(System.IO.BinaryWriter bin)
        {
            try
            {
                bin.Write(isHC);
                bin.Write(isLC);
                bin.Write(highcut);
                bin.Write(lowcut);
                for (int i = 0; i < dllCount; i++)
                {
                    AudioTrack.WriteString(bin, dllButton.GetFileName(eqDll[i]));
                    if (eqDll[i] != "")
                    {
                        try
                        {
                            System.IO.File.Copy(eqDll[i], MainForm.CurProjectPath + "\\" + dllButton.GetFileName(eqDll[i]));
                        } catch
                        {

                        }
                        string str = System.IO.Path.GetDirectoryName(eqDll[i]);
                        string name = System.IO.Path.GetFileName(eqDll[i]);
                        name = name.Substring(0, Math.Min(name.Length, 2));
                        foreach(string g in System.IO.Directory.GetFiles(str))
                        {
                            if (g.StartsWith(str+"\\"+name))
                            {
                                try
                                {
                                    System.IO.File.Copy(g, MainForm.CurProjectPath + "\\" + dllButton.GetFileName(g));
                                } catch
                                {

                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < dllCount; i++)
                {
                    bin.Write(dllActive[i]);
                }
                bin.Write(Mute);
                bin.Write(Volume);
                for (int i = 0; i < 4; i++)
                {
                    handleItem[i].Save(bin);
                }
            } catch
            {

            }
        }
        ~EQProperty()
        {
            Close();
        }
        void IDisposable.Dispose()
        {
            Close();
        }
    }
}
