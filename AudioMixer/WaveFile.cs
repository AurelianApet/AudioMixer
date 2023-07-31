using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using NAudio.Wave;
using System.Diagnostics;
using NAudio.Dsp;
using NAudio.Wave.SampleProviders;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System.Threading;

namespace AudioMixer
{
    /// <summary>
    /// A sample provider mixer, allowing inputs to be added and removed
    /// </summary>
    public class MyMixingSampleProvider : ISampleProvider
    {
        private readonly List<ISampleProvider> sources;
        private float[] sourceBuffer;
        private const int MaxInputs = 1024; // protect ourselves against doing something silly
        private EQProperty eq;
        private readonly BiQuadFilter[,] filters;
        private readonly int channels;
        private readonly int bandCount;

        public int Tempo
        {
            get
            {
                if (Pulse.it!=null)
                {
                    return Pulse.it.Ppm;
                }
                return 120;
            }
        }

        /// <summary>
        /// Creates a new MixingSampleProvider, with no inputs, but a specified WaveFormat
        /// </summary>
        /// <param name="waveFormat">The WaveFormat of this mixer. All inputs must be in this format</param>
        public MyMixingSampleProvider(WaveFormat waveFormat, EQProperty eq)
        {
            if (waveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Mixer wave format must be IEEE float");
            }
            sources = new List<ISampleProvider>();
            WaveFormat = waveFormat;
            this.eq = eq;
            channels = 2;
            bandCount = 6;
            filters = new BiQuadFilter[channels, bandCount];
            CreateFilters();
        }

        private void CreateFilters()
        {
            for (int bandIndex = 0; bandIndex < 4; bandIndex++)
            {
                var band = eq.handleItem[bandIndex];
                for (int n = 0; n < channels; n++)
                {
                    if (filters[n, bandIndex] == null)
                    {
                        if (bandIndex == 0)
                        {
                            filters[n, bandIndex] = BiQuadFilter.LowShelf(WaveFormat.SampleRate, band.frequency, band.GetFactor(), band.db);
                        }
                        else if (bandIndex == 3)
                        {
                            filters[n, bandIndex] = BiQuadFilter.HighShelf(WaveFormat.SampleRate, band.frequency, band.GetFactor(), band.db);
                        }
                        else
                        {
                            filters[n, bandIndex] = BiQuadFilter.PeakingEQ(WaveFormat.SampleRate, band.frequency, (band.factor + 0.25f), band.db);
                        }
                    }
                    else
                    {
                        if (bandIndex == 0)
                        {
                            filters[n, bandIndex] = BiQuadFilter.LowShelf(WaveFormat.SampleRate, band.frequency, (band.factor + 0.25f), band.db);
                        }
                        else if (bandIndex == 3)
                        {
                            filters[n, bandIndex] = BiQuadFilter.HighShelf(WaveFormat.SampleRate, band.frequency, (band.factor + 0.25f), band.db);
                        }
                        else
                        {
                            filters[n, bandIndex].SetPeakingEq(WaveFormat.SampleRate, band.frequency, (band.factor + 0.25f), band.db);
                        }
                    }
                }
            }
            for (int n = 0; n < channels; n++)
            {
                if (filters[n, 4] == null)
                    filters[n, 4] = BiQuadFilter.LowPassFilter(WaveFormat.SampleRate, eq.highcut, 0.8f);
                else
                    filters[n, 4].SetLowPassFilter(WaveFormat.SampleRate, eq.highcut, 0.8f);
                if (filters[n, 5] == null)
                    filters[n, 5] = BiQuadFilter.HighPassFilter(WaveFormat.SampleRate, eq.lowcut, 0.8f);
                else
                    filters[n, 5].SetHighPassFilter(WaveFormat.SampleRate, eq.lowcut, 0.8f);
            }
        }

        /// <summary>
        /// Creates a new MixingSampleProvider, based on the given inputs
        /// </summary>
        /// <param name="sources">Mixer inputs - must all have the same waveformat, and must
        /// all be of the same WaveFormat. There must be at least one input</param>
        public MyMixingSampleProvider(IEnumerable<ISampleProvider> sources)
        {
            this.sources = new List<ISampleProvider>();
            foreach (var source in sources)
            {
                AddMixerInput(source);
            }
            if (this.sources.Count == 0)
            {
                throw new ArgumentException("Must provide at least one input in this constructor");
            }
        }

        /// <summary>
        /// Returns the mixer inputs (read-only - use AddMixerInput to add an input
        /// </summary>
        public IEnumerable<ISampleProvider> MixerInputs => sources;

        /// <summary>
        /// When set to true, the Read method always returns the number
        /// of samples requested, even if there are no inputs, or if the
        /// current inputs reach their end. Setting this to true effectively
        /// makes this a never-ending sample provider, so take care if you plan
        /// to write it out to a file.
        /// </summary>
        public bool ReadFully { get; set; }

        /// <summary>
        /// Adds a WaveProvider as a Mixer input.
        /// Must be PCM or IEEE float already
        /// </summary>
        /// <param name="mixerInput">IWaveProvider mixer input</param>
        public void AddMixerInput(IWaveProvider mixerInput)
        {
            AddMixerInput(mixerInput.ToSampleProvider());
        }

        /// <summary>
        /// Adds a new mixer input
        /// </summary>
        /// <param name="mixerInput">Mixer input</param>
        public void AddMixerInput(ISampleProvider mixerInput)
        {
            // we'll just call the lock around add since we are protecting against an AddMixerInput at
            // the same time as a Read, rather than two AddMixerInput calls at the same time
            lock (sources)
            {
                if (sources.Count >= MaxInputs)
                {
                    throw new InvalidOperationException("Too many mixer inputs");
                }
                sources.Add(mixerInput);
            }
            if (WaveFormat == null)
            {
                WaveFormat = mixerInput.WaveFormat;
            }
            else
            {
                //if (WaveFormat.SampleRate != mixerInput.WaveFormat.SampleRate ||
                //    WaveFormat.Channels != mixerInput.WaveFormat.Channels)
                //{
                //    throw new ArgumentException("All mixer inputs must have the same WaveFormat");
                //}
            }
        }

        /// <summary>
        /// Raised when a mixer input has been removed because it has ended
        /// </summary>
        public event EventHandler<SampleProviderEventArgs> MixerInputEnded;

        /// <summary>
        /// Removes a mixer input
        /// </summary>
        /// <param name="mixerInput">Mixer input to remove</param>
        public void RemoveMixerInput(ISampleProvider mixerInput)
        {
            lock (sources)
            {
                if (sources.Contains(mixerInput))
                {
                    ((MySampleProvider)mixerInput).wf.trackCtrl.ClearDb();
                    sources.Remove(mixerInput);
                }
            }
        }

        private long curTime = 0;
        public long CurTime
        {
            get { return curTime; }
            set
            {
                curTime = value;
            }
        }

        /// <summary>
        /// Removes all mixer inputs
        /// </summary>
        public void RemoveAllMixerInputs()
        {
            lock (sources)
            {
                sources.Clear();
            }
        }

        /// <summary>
        /// The output WaveFormat of this sample provider
        /// </summary>
        private WaveFormat wformat;
        public WaveFormat WaveFormat { get { return wformat; } private set { wformat = value; } }

        /// <summary>
        /// Reads samples from this sample provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="count">Number of samples required</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            lock (this)
            {
                int outputSamples = 0;
                CreateFilters();
                bool ok = sources.Count > 0;
                lock (sources)
                {
                    int mx = count;
                    bool ff = false;
                    for (int i = sources.Count; i-- > 0;)
                    {
                        var source = sources[i] as MySampleProvider;
                        int cc = (int)(count * ((double)Tempo / source.Tempo) + 1e-4);
                        if (Tempo - source.Tempo < 1e-3 && Tempo - source.Tempo > -1e-3 || ff) cc = count;
                        mx = Math.Max(cc, mx);
                    }
                    sourceBuffer = NAudio.Utils.BufferHelpers.Ensure(sourceBuffer, mx);
                    int index = sources.Count - 1;
                    while (index >= 0)
                    {
                        var source = sources[index] as MySampleProvider;
                        var flag = source.WaveFormat.Channels == 1;
                        int cc = (flag ? count / 2 : count);
                        int cp = (int)(cc * ((double)Tempo / source.Tempo) + 1e-4);
                        if (Tempo - source.Tempo < 1e-3 && Tempo - source.Tempo > -1e-3 || ff) cp = cc;
                        int samplesRead = source.Read(sourceBuffer, 0, cp);
                        int outIndex = offset;
                        bool all = samplesRead < cp;
                        if (flag)
                        {
                            samplesRead *= 2;
                            for (int i = samplesRead; i-- > 0;)
                            {
                                sourceBuffer[i] = sourceBuffer[i / 2];
                            }
                        }
                        if (samplesRead < count)
                        {
                            for (int i = samplesRead; i < count; i++) sourceBuffer[i] = sourceBuffer[i - samplesRead];
                            samplesRead = count;
                        }
                        samplesRead = Math.Min(samplesRead, count);
                        for (int n = 0; n < samplesRead; n++)
                        {
                            int id = n;
                            if (id >= outputSamples)
                            {
                                buffer[outIndex++] = GetVal(sourceBuffer[n], id & 1);
                            }
                            else
                            {
                                buffer[outIndex++] += GetVal(sourceBuffer[n], id & 1);
                            }
                        }
                        outputSamples = Math.Max(samplesRead, outputSamples);
                        if (all)
                        {
                            MixerInputEnded?.Invoke(this, new SampleProviderEventArgs(source));
                            sources.RemoveAt(index);
                        }
                        index--;
                    }
                }
                // optionally ensure we return a full buffer
                if (ReadFully && outputSamples < count)
                {
                    int outputIndex = offset + outputSamples;
                    while (outputIndex < offset + count)
                    {
                        buffer[outputIndex++] = 0;
                    }
                    outputSamples = count;
                }
                try
                {
                    for (int i = 0; i < eq.dllitem.Count; i++)
                        eq.UpdateFromDll(i, buffer, offset, outputSamples);
                } catch (Exception e)
                {
                    string f = e.Message;
                }
                int cnt = count / 1;
                if (ok)
                    for (int i = 0; i < 1; i++)
                    {
                        float f1 = float.MinValue, f2 = float.MinValue;
                        for (int j = 0; j < cnt && i * cnt + j + 1 < count; j += 2)
                        {
                            f1 = Math.Max(f1, Math.Abs(buffer[i * cnt + j]));
                            f2 = Math.Max(f2, Math.Abs(buffer[i * cnt + j + 1]));
                        }
                        MasterTrack.GetInstance()?.AddVal(f1, f2);
                        TrackView.GetInstance()?.root.AddDb(f1, f2);
                    }
                return outputSamples;
            }
        }

        float GetVal(float val, int ch)
        {
            if (eq.Mute) return 0;
            for (int band = 0; band < 4; band++)
            {
                if (eq.handleItem[band].enabled) val = filters[ch, band].Transform(val);
            }

            if (eq.isLC) val = filters[ch, 5].Transform(val);
            if (eq.isHC) val = filters[ch, 4].Transform(val);
            if (eq.Volume < 1.0f - 1e-3) val *= eq.Volume;

            return val;
        }
    }

    /// <summary>
    /// SampleProvider event args
    /// </summary>
    public class SampleProviderEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new SampleProviderEventArgs
        /// </summary>
        public SampleProviderEventArgs(ISampleProvider sampleProvider)
        {
            SampleProvider = sampleProvider;
        }

        /// <summary>
        /// The Sample Provider
        /// </summary>
        public ISampleProvider SampleProvider { get; private set; }
    }

    public class MySampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;

        public EQProperty eq;
        private readonly BiQuadFilter[,] filters;
        private readonly int channels;
        private readonly int bandCount;
        private bool updated;
        public WaveForm wf;
        public MyWaveStream ws;
        public int Tempo
        {
            get
            {
                return wf.m_Wavefile.Tempo;
            }
        }
        /// <summary>
        /// Initializes a new instance of VolumeSampleProvider
        /// </summary>
        /// <param name="source">Source Sample Provider</param>
        public MySampleProvider(ISampleProvider source, EQProperty eq, WaveForm wf, MyWaveStream s)
        {
            this.source = source; this.wf = wf; this.ws = s;
            s.Position = 0;
            this.eq = eq;
            channels = source.WaveFormat.Channels;
            bandCount = 6;
            filters = new BiQuadFilter[channels, bandCount];
            CreateFilters();
        }
        public void SetEq(EQProperty eq)
        {
            this.eq = eq;
        }
        private void CreateFilters()
        {
            for (int bandIndex = 0; bandIndex < 4; bandIndex++)
            {
                var band = eq.handleItem[bandIndex];
                for (int n = 0; n < channels; n++)
                {
                    if (filters[n, bandIndex] == null)
                    {
                        if (bandIndex == 0)
                        {
                            filters[n, bandIndex] = BiQuadFilter.LowShelf(source.WaveFormat.SampleRate, band.frequency, band.GetFactor(), band.db);
                        }
                        else if (bandIndex == 3)
                        {
                            filters[n, bandIndex] = BiQuadFilter.HighShelf(source.WaveFormat.SampleRate, band.frequency, band.GetFactor(), band.db);
                        }
                        else
                        {
                            filters[n, bandIndex] = BiQuadFilter.PeakingEQ(source.WaveFormat.SampleRate, band.frequency, (band.factor + 0.25f), band.db);
                        }
                    }
                    else
                    {
                        if (bandIndex == 0)
                        {
                            filters[n, bandIndex] = BiQuadFilter.LowShelf(source.WaveFormat.SampleRate, band.frequency, band.GetFactor(), band.db);
                        }
                        else if (bandIndex == 3)
                        {
                            filters[n, bandIndex] = BiQuadFilter.HighShelf(source.WaveFormat.SampleRate, band.frequency, band.GetFactor(), band.db);
                        }
                        else
                        {
                            filters[n, bandIndex].SetPeakingEq(source.WaveFormat.SampleRate, band.frequency, (band.factor + 0.25f), band.db);
                        }
                    }
                }
            }
            for (int n = 0; n < channels; n++)
            {
                if (filters[n, 4] == null)
                    filters[n, 4] = BiQuadFilter.LowPassFilter(source.WaveFormat.SampleRate, eq.highcut, 0.8f);
                else
                    filters[n, 4].SetLowPassFilter(source.WaveFormat.SampleRate, eq.highcut, 0.8f);
                if (filters[n, 5] == null)
                    filters[n, 5] = BiQuadFilter.HighPassFilter(source.WaveFormat.SampleRate, eq.lowcut, 0.8f);
                else
                    filters[n, 5].SetHighPassFilter(source.WaveFormat.SampleRate, eq.lowcut, 0.8f);
            }
        }

        public void Update()
        {
            updated = true;
            CreateFilters();
        }

        /// <summary>
        /// WaveFormat
        /// </summary>
        public WaveFormat WaveFormat => source.WaveFormat;

        DateTime pre = DateTime.Now;
        /// <summary>
        /// Reads samples from this sample provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="sampleCount">Number of samples desired</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = source.Read(buffer, offset, sampleCount);
            DateTime cur = DateTime.Now;
            //MainForm.GetInstance().Text = cur.Subtract(pre).TotalMilliseconds + "";
            pre = cur;
            if (updated || true)
            {
                CreateFilters();
                updated = false;
            }
            for (int n = 0; n < sampleCount; n++)
            {
                int ch = n % channels;
                if (eq.Mute)
                {
                    buffer[offset + n] = 0;
                    continue;
                }

                for (int band = 0; band < 4; band++)
                {
                    if (eq.handleItem[band].enabled) buffer[offset + n] = filters[ch, band].Transform(buffer[offset + n]);
                }

                if (eq.isLC) buffer[offset + n] = filters[ch, 5].Transform(buffer[offset + n]);
                if (eq.isHC) buffer[offset + n] = filters[ch, 4].Transform(buffer[offset + n]);
                buffer[offset + n] *= eq.Volume;
            }
            for (int i = 0; i < eq.dllitem.Count; i++)
                eq.UpdateFromDll(i, buffer, offset, samplesRead);
            if (wf != null)
            {
                int cnt = sampleCount / 1;
                for (int i = 0; i < 1; i++)
                {
                    float f1 = float.MinValue, f2 = float.MinValue;
                    for (int j = 0; j < cnt && i * cnt + j + 1 < sampleCount; j += 2)
                    {
                        f1 = Math.Max(f1, Math.Abs(buffer[i * cnt + j]));
                        f2 = Math.Max(f2, Math.Abs(buffer[i * cnt + j + 1]));
                    }
                    wf.trackCtrl.AddDb(f1, f2);
                }
            }
            return samplesRead;
        }
    }
    public class MyWaveOut : WaveOut
    {
        public MyWaveOut()
        {
            tmpVol = 1.0f;
            isMute = false;
        }
        float tmpVol = 1.0f;
        bool isMute;
        public bool Mute
        {
            get { return isMute; }
            set
            {
                isMute = value;
                if (isMute)
                {
                    Volume = 1.0f;
                }
                else
                {
                    Volume = tmpVol;
                }
            }
        }
    }
    public class MyWaveStream : WaveStream
    {
        private WaveStream sourceStream;
        private long audioStartPosition;
        private long sourceOffsetBytes;
        private long sourceLengthBytes;
        private long length;
        private readonly int bytesPerSample; // includes all channels
        private long position;
        private TimeSpan startTime;
        private TimeSpan sourceOffset;
        private TimeSpan sourceLength;
        private readonly object lockObject = new object();

        /// <summary>
        /// Creates a new WaveOffsetStream
        /// </summary>
        /// <param name="sourceStream">the source stream</param>
        /// <param name="startTime">the time at which we should start reading from the source stream</param>
        /// <param name="sourceOffset">amount to trim off the front of the source stream</param>
        /// <param name="sourceLength">length of time to play from source stream</param>
        public MyWaveStream(WaveStream sourceStream, TimeSpan startTime, TimeSpan sourceOffset, TimeSpan sourceLength)
        {
            if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
                throw new ArgumentException("Only PCM supported");
            // TODO: add support for IEEE float + perhaps some others -
            // anything with a fixed bytes per sample

            this.sourceStream = sourceStream;
            position = 0;
            bytesPerSample = (sourceStream.WaveFormat.BitsPerSample / 8) * sourceStream.WaveFormat.Channels;
            StartTime = startTime;
            SourceOffset = sourceOffset;
            SourceLength = sourceLength;
        }

        /// <summary>
        /// Creates a WaveOffsetStream with default settings (no offset or pre-delay,
        /// and whole length of source stream)
        /// </summary>
        /// <param name="sourceStream">The source stream</param>
        public MyWaveStream(WaveStream sourceStream)
            : this(sourceStream, TimeSpan.Zero, TimeSpan.Zero, sourceStream.TotalTime)
        {
        }

        /// <summary>
        /// The length of time before which no audio will be played
        /// </summary>
        public TimeSpan StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                lock (lockObject)
                {
                    startTime = value;
                    audioStartPosition = (long)(startTime.TotalSeconds * sourceStream.WaveFormat.SampleRate) * bytesPerSample;
                    // fix up our length and position
                    length = audioStartPosition + sourceLengthBytes;
                    Position = Position;
                }
            }
        }

        /// <summary>
        /// An offset into the source stream from which to start playing
        /// </summary>
        public TimeSpan SourceOffset
        {
            get
            {
                return sourceOffset;
            }
            set
            {
                lock (lockObject)
                {
                    sourceOffset = value;
                    sourceOffsetBytes = (long)(sourceOffset.TotalSeconds * sourceStream.WaveFormat.SampleRate) * bytesPerSample;
                    // fix up our position
                    Position = Position;
                }
            }
        }

        /// <summary>
        /// Length of time to read from the source stream
        /// </summary>
        public TimeSpan SourceLength
        {
            get
            {
                return sourceLength;
            }
            set
            {
                lock (lockObject)
                {
                    sourceLength = value;
                    sourceLengthBytes = (long)(sourceLength.TotalSeconds * sourceStream.WaveFormat.SampleRate) * bytesPerSample;
                    // fix up our length and position
                    length = audioStartPosition + sourceLengthBytes;
                    Position = Position;
                }
            }

        }

        /// <summary>
        /// Gets the block alignment for this WaveStream
        /// </summary>
        public override int BlockAlign => sourceStream.BlockAlign;

        /// <summary>
        /// Returns the stream length
        /// </summary>
        public override long Length => length;

        /// <summary>
        /// Gets or sets the current position in the stream
        /// </summary>
        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                lock (lockObject)
                {
                    // make sure we don't get out of sync
                    value -= (value % BlockAlign);
                    if (value < audioStartPosition)
                        sourceStream.Position = sourceOffsetBytes;
                    else
                        sourceStream.Position = sourceOffsetBytes + (value - audioStartPosition);
                    position = value;
                }
            }
        }

        public void SetPosition(long t, long duration, double tottime)
        {
            Position = (long)((double)t / duration * tottime * WaveFormat.SampleRate * bytesPerSample);
        }

        /// <summary>
        /// Reads bytes from this wave stream
        /// </summary>
        /// <param name="destBuffer">The destination buffer</param>
        /// <param name="offset">Offset into the destination buffer</param>
        /// <param name="numBytes">Number of bytes read</param>
        /// <returns>Number of bytes read.</returns>
        public override int Read(byte[] destBuffer, int offset, int numBytes)
        {
            lock (lockObject)
            {
                int bytesWritten = 0;
                // 1. fill with silence
                if (position < audioStartPosition)
                {
                    bytesWritten = (int)Math.Min(numBytes, audioStartPosition - position);
                    for (int n = 0; n < bytesWritten; n++)
                        destBuffer[n + offset] = 0;
                }
                if (bytesWritten < numBytes)
                {
                    // don't read too far into source stream
                    int sourceBytesRequired = (int)Math.Min(
                        numBytes - bytesWritten,
                        sourceLengthBytes + sourceOffsetBytes - sourceStream.Position);
                    int read = sourceStream.Read(destBuffer, bytesWritten + offset, sourceBytesRequired);
                    bytesWritten += read;
                }
                // 3. Fill out with zeroes
                for (int n = bytesWritten; n < numBytes; n++)
                    destBuffer[offset + n] = 0;
                position += numBytes;
                return numBytes;
            }
        }

        /// <summary>
        /// <see cref="WaveStream.WaveFormat"/>
        /// </summary>
        public override WaveFormat WaveFormat => sourceStream.WaveFormat;

        /// <summary>
        /// Determines whether this channel has any data to play
        /// to allow optimisation to not read, but bump position forward
        /// </summary>
        public override bool HasData(int count)
        {
            if (position + count < audioStartPosition)
                return false;
            if (position >= length)
                return false;
            // Check whether the source stream has data.
            // source stream should be in the right poisition
            return sourceStream.HasData(count);
        }

        /// <summary>
        /// Disposes this WaveStream
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (sourceStream != null)
                {
                    sourceStream.Dispose();
                    sourceStream = null;
                }
            }
            else
            {
                //                //System.Diagnostics.Debug.Assert(false, "WaveOffsetStream was not Disposed");
            }
            base.Dispose(disposing);
        }
    }
    public class MyWaveFile
    {
        private short[,] Mi, Ma;
        public int tot;
        public int totalBeats;
        public bool isPlayable = false;
        public string filename;
        public MyWaveFile(string filename)
        {
            this.filename = filename;
            isPlayable = false;
        }

        public static string GetType(string filename)
        {
            string str = "";
            int t = filename.LastIndexOf(".");
            for (int i = t + 1; i < filename.Length; i++) str += filename[i];
            return str;
        }
        public long Duration
        {
            get { return (long)(os.TotalTime.TotalMilliseconds * 1000); }
        }
        public int BitsPerSample
        {
            get { return os.WaveFormat.BitsPerSample; }
        }
        public int Channels
        {
            get { return os.WaveFormat.Channels; }
        }
        public int SampleRate
        {
            get { return os.WaveFormat.SampleRate; }
        }
        public void GetDb(double pos, double volume, out double maxVal, out double avgVal)
        {
            int t = (int)(pos * (tot - 1));
            int k = 4;
            while (t + (1 << k) > tot) k--;
//            double v1 = Math.Max(0.0001, (double)Math.Max(Math.Abs(Ma[t, k, 0]), Math.Abs(Mi[t, k, 0])) / 32768);
            double v2 = Math.Max(0.0001, (double)Math.Abs(find_max(t, t+(1<<k), 0)) / 32768);
//            maxVal = 20 * Math.Log10(v1) + volume;
            avgVal = 20 * Math.Log10(v2) + volume;
            maxVal = avgVal;
        }

        public MyWaveStream os;
        public int Tempo = 120;
        public short max_pitch = 0;
        public bool Read()
        {
            string type = GetType(filename).ToLower();
            if (type != "mp3" && type != "wav") return isPlayable = false;
            string outfilename = filename + ".wf";
            if (!File.Exists(outfilename))
            {
                if (type == "mp3")
                {
                    try
                    {
                        using (Mp3FileReader mp3 = new Mp3FileReader(filename))
                        {
                            using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                            {
                                WaveFileWriter.CreateWaveFile(outfilename, pcm);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MyMessageBox.Show(e.Message);
                        return isPlayable = false;
                    }
                }
                else
                {
                    try
                    {
                        using (WaveFileReader ws = new WaveFileReader(filename))
                        {
                            if (ws.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                            {
                                var pcm = new WaveFloatTo16Provider(ws);
                                var p = new WaveFormatConversionProvider(new WaveFormat(), pcm);
                                WaveFileWriter.CreateWaveFile(outfilename, p);
                            }
                            else
                            {
                                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(ws))
                                {
                                    if (pcm.WaveFormat.BitsPerSample == 24)
                                    {
                                        var p = new Pcm24BitToSampleProvider(pcm);
                                        var p2 = new WaveFormatConversionProvider(new WaveFormat(), p.ToWaveProvider16());
                                        WaveFileWriter.CreateWaveFile(outfilename, p2);
                                    }
                                    else if (pcm.WaveFormat.BitsPerSample == 8)
                                    {
                                        var p = new Pcm8BitToSampleProvider(pcm);
                                        var p2 = new WaveFormatConversionProvider(new WaveFormat(), p.ToWaveProvider16());
                                        WaveFileWriter.CreateWaveFile(outfilename, p2);
                                    }
                                    else if (pcm.WaveFormat.BitsPerSample == 32)
                                    {
                                        var p = new Pcm32BitToSampleProvider(pcm);
                                        var p2 = new WaveFormatConversionProvider(new WaveFormat(), p.ToWaveProvider16());
                                        WaveFileWriter.CreateWaveFile(outfilename, p2);
                                    }
                                    else
                                    {
                                        var p = new WaveFormatConversionProvider(new WaveFormat(), ws);
                                        WaveFileWriter.CreateWaveFile(outfilename, p);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MyMessageBox.Show(e.Message);
                        return isPlayable = false;
                    }
                }
            }
            try
            {
                WaveFileReader ws1 = new WaveFileReader(outfilename);
                os = new MyWaveStream(ws1);
                int bytesPerSample = (os.WaveFormat.BitsPerSample / 8) * os.WaveFormat.Channels;
                int sz = (int)os.Length / bytesPerSample;
                if (sz == 0) return isPlayable = false;
                //short[,] m_Data = new short[sz, os.WaveFormat.Channels];
                int len = sz * os.WaveFormat.Channels;
                int length = sz * os.WaveFormat.Channels;
                ISampleProvider sampleReader = os.ToSampleProvider();
                Thread.Sleep(0);
                float[] samples = new float[length];
                Thread.Sleep(0);
                sampleReader.Read(samples, 0, length);
                Thread.Sleep(0);
                max_pitch = 0;
                for (int ch = 0; ch < Channels; ch++)
                {
                    BiQuadFilter lowpass = BiQuadFilter.LowPassFilter(SampleRate, 150, 1);
                    BiQuadFilter highpass = BiQuadFilter.HighPassFilter(SampleRate, 100, 1);
                    for (int i = ch; i < length; i += Channels)
                    {
                        samples[i] = highpass.Transform(lowpass.Transform(samples[i]));
                        short t = (short)(samples[i] * 32767);
                        if (t < 0) t = (short)-t;
                        if (t > max_pitch) max_pitch = t;
                    }
                    Thread.Sleep(0);
                }
                //for (int i = 0; i + 1 < len; i += 2)
                //{
                //    m_Data[i>>1, 0] = (short)(samples[i]*32767);
                //    m_Data[i>>1, 1] = (short)(samples[i+1]*32767);
                //}
                Peak[] peaks = getPeaks(samples);
                Thread.Sleep(0);

                BPMGroup[] allGroups = getIntervals(peaks);
                Thread.Sleep(0);

                Array.Sort(allGroups, (x, y) => y.Count.CompareTo(x.Count));
                Thread.Sleep(0);

                if (allGroups.Length > 5)
                {
                    Array.Resize(ref allGroups, 5);
                }

                this.group = allGroups;
                if (group.Length == 0)
                {
                    this.Tempo = 120;
                } else
                {
                    this.Tempo = (int)Math.Round(group[0].all / group[0].Count);
                }
                if (Pulse.it != null)
                {
                    Pulse.it.SetBPM(this.Tempo);
                    this.Tempo = Pulse.it.firstBPM;
                }
                Thread.Sleep(0);

                int k = (sz + 50000 - 1) / 50000;
                tot = (sz + k - 1) / k;
                int cnt = 0;
                while ((1 << cnt) < tot + 2) cnt++;
                MM = 1 << cnt;
                Mi = new short[MM<<1, os.WaveFormat.Channels];
                Ma = new short[MM<<1, os.WaveFormat.Channels];
                Thread.Sleep(0);
                for (int i = 0; i < sz; i += k)
                {
                    for (int r = 0; r < os.WaveFormat.Channels; r++)
                    {
                        short mi = short.MaxValue, ma = short.MinValue;
                        for (int j = i; j < i + k && j < sz; j++)
                        {
                            short t = (short)(samples[j * os.WaveFormat.Channels + r] * 32767);
                            mi = Math.Min(mi, t);
                            ma = Math.Max(ma, t);
                        }
                        Mi[i / k + MM + 1, r] = mi;
                        Ma[i / k + MM + 1, r] = ma;
                        Thread.Sleep(0);
                    }
                }
                for (int i = MM; i-- > 0;)
                {
                    for (int j = 0; j < os.WaveFormat.Channels; j++)
                    {
                        Mi[i, j] = Math.Min(Mi[i << 1, j], Mi[(i << 1) | 1, j]);
                        Ma[i, j] = Math.Max(Ma[i << 1, j], Ma[(i << 1) | 1, j]);
                    }
                    Thread.Sleep(0);
                }
                //for (int j = 1; j < cnt; j++)
                //{
                //    for (int a = 0; (a + (1 << j)) <= tot; a++)
                //    {
                //        for (int r = 0; r < os.WaveFormat.Channels; r++)
                //        {
                //            Mi[a, j, r] = Math.Min(Mi[a, j - 1, r], Mi[a + (1 << j - 1), j - 1, r]);
                //            Ma[a, j, r] = Math.Max(Ma[a, j - 1, r], Ma[a + (1 << j - 1), j - 1, r]);
                //        }
                //    }
                //}
                //m_Data = null;
                samples = null;
                return isPlayable = true;
            }
            catch (Exception e)
            {
                MyMessageBox.Show(e.Message);
                return isPlayable = false;
            }
        }

        public bool Read1() {
            try {
                filename = MainForm.CurProjectPath + "\\" + filename;
                return Read();
                //WaveFileReader ws1 = new WaveFileReader(ms);
                //os = new MyWaveStream(ws1);
                //int bytesPerSample = (os.WaveFormat.BitsPerSample / 8) * os.WaveFormat.Channels;
                //int sz = (int)os.Length / bytesPerSample;
                //if (sz == 0) return isPlayable = false;
                ////short[,] m_Data = new short[sz, os.WaveFormat.Channels];
                //int len = sz * os.WaveFormat.Channels;
                //int length = sz * os.WaveFormat.Channels;
                //ISampleProvider sampleReader = os.ToSampleProvider();
                //float[] samples = new float[length];
                //Thread.Sleep(0);
                //sampleReader.Read(samples, 0, length);
                //Thread.Sleep(0);
                //max_pitch = 0;
                //for (int ch = 0; ch < Channels; ch++)
                //{
                //    BiQuadFilter lowpass = BiQuadFilter.LowPassFilter(SampleRate, 150, 1);
                //    BiQuadFilter highpass = BiQuadFilter.HighPassFilter(SampleRate, 100, 1);
                //    for (int i = ch; i < length; i += Channels)
                //    {
                //        samples[i] = highpass.Transform(lowpass.Transform(samples[i]));
                //        short t = (short)(samples[i] * 32767);
                //        if (t < 0) t = (short)-t;
                //        if (t > max_pitch) max_pitch = t;
                //    }
                //    Thread.Sleep(0);
                //}
                ////for (int i = 0; i + 1 < len; i += 2)
                ////{
                ////    m_Data[i >> 1, 0] = (short)(samples[i] * 32767);
                ////    m_Data[i >> 1, 1] = (short)(samples[i + 1] * 32767);
                ////}
                //Peak[] peaks = getPeaks(samples);
                //Thread.Sleep(0);

                //BPMGroup[] allGroups = getIntervals(peaks);
                //Thread.Sleep(0);

                //Array.Sort(allGroups, (x, y) => y.Count.CompareTo(x.Count));
                //Thread.Sleep(0);

                //if (allGroups.Length > 5)
                //{
                //    Array.Resize(ref allGroups, 5);
                //}

                //this.group = allGroups;
                //if (this.group.Length == 0)
                //{
                //    this.Tempo = 120;
                //} else
                //{
                //    this.Tempo = (int)Math.Round(group[0].all / group[0].Count);
                //}
                //if (Pulse.it!= null)
                //{
                //    Pulse.it.SetBPM(this.Tempo);
                //    this.Tempo = Pulse.it.firstBPM;
                //}
                //Thread.Sleep(0);
                ////                MainForm.GetInstance()?.Text = Tempo.ToString();

                //int k = (sz + 100000 - 1) / 100000;
                //tot = (sz + k - 1) / k;
                //int cnt = 0;
                //while ((1 << cnt) < tot + 2) cnt++;
                //MM = 1 << cnt;
                //Mi = new short[MM<<1, os.WaveFormat.Channels];
                //Ma = new short[MM<<1, os.WaveFormat.Channels];
                //Thread.Sleep(0);
                //for (int i = 0; i < sz; i += k)
                //{
                //    for (int r = 0; r < os.WaveFormat.Channels; r++)
                //    {
                //        short mi = short.MaxValue, ma = short.MinValue;
                //        for (int j = i; j < i + k && j < sz; j++)
                //        {
                //            short t = (short)(samples[j * os.WaveFormat.Channels + r] * 32767);
                //            mi = Math.Min(mi, t);
                //            ma = Math.Max(ma, t);
                //        }
                //        Mi[i / k + MM + 1, r] = mi;
                //        Ma[i / k + MM + 1, r] = ma;
                //    }
                //    Thread.Sleep(0);
                //}
                //for (int i = MM; i-->0; )
                //{
                //    for (int j= 0; j < os.WaveFormat.Channels; j++)
                //    {
                //        Mi[i, j] = Math.Min(Mi[i << 1, j], Mi[(i << 1) | 1, j]);
                //        Ma[i, j] = Math.Max(Ma[i << 1, j], Ma[(i << 1) | 1, j]);
                //    }
                //    Thread.Sleep(0);
                //}
                ////for (int j = 1; j < cnt; j++)
                ////{
                ////    for (int a = 0; (a + (1 << j)) <= tot; a++)
                ////    {
                ////        for (int r = 0; r < os.WaveFormat.Channels; r++)
                ////        {
                ////            Mi[a, j, r] = Math.Min(Mi[a, j - 1, r], Mi[a + (1 << j - 1), j - 1, r]);
                ////            Ma[a, j, r] = Math.Max(Ma[a, j - 1, r], Ma[a + (1 << j - 1), j - 1, r]);
                ////        }
                ////    }
                ////}
                ////m_Data = null;
                //samples = null;
                //return isPlayable = true;
            }
            catch (Exception e)
            {
                MyMessageBox.Show(e.Message);
                return isPlayable = false;
            }
        }
        short ConvVal(byte[] buffer, int offset, int bits)
        {
            float val = 0;
            while (bits != 16) ;
            if (bits == 32) val = BitConverter.ToSingle(buffer, offset);
            else
            {
                int tmp = 0;
                for (int i = 0; i < bits / 8; i++) tmp = tmp | (((int)buffer[i + offset]) << i * 8);
                int mx = (1 << bits - 1);
                val = (float)tmp / (1 << bits - 1);
                if ((tmp & mx) == mx) val -= 2;
            }
            return (short)(val * 32767);
        }

        public int MM = 0;
        short find_min(int st, int ed, int r)
        {
            int s = MM + st;
            int e = MM + ed + 1;
            short x = short.MaxValue;
            while ((s ^ e) > 1)
            {
                if ((s & 1) == 0 && x > Mi[s^1, r]) x = Mi[s^1, r];
                if ((e & 1) == 1 && x > Mi[e ^ 1, r]) x = Mi[e ^ 1, r];
                e >>= 1;
                s >>= 1;
            }
            return x;
        }
        short find_max(int st, int ed, int r)
        {
            int s = MM + st;
            int e = MM + ed + 1;
            short x = short.MinValue;
            while ((s ^ e) > 1)
            {
                if ((s & 1) == 0 && x < Ma[s ^ 1, r]) x = Ma[s ^ 1, r];
                if ((e & 1) == 1 && x < Ma[e ^ 1, r]) x = Ma[e ^ 1, r];
                e >>= 1;
                s >>= 1;
            }
            return x;
        }

        public short GetVal(int st, int ed, int r, int m)
        {
            try
            {
                if (m == 0) return find_min(st, ed, r);
                return find_max(st, ed, r);
                //int k = 0;
                //while ((1 << k) <= ed - st) k++;
                //k--;
                //if (m == 0) return Math.Min(Mi[st, k, r], Mi[ed - (1 << k), k, r]);
                //return Math.Max(Ma[st, k, r], Ma[ed - (1 << k), k, r]);
            }
            catch
            {
                return 0;
            }
        }
        private struct Peak
        {
            public int Position;
            public float Volume;
        }
        struct BPMGroup
        {
            public int Count;
            public short Tempo;
            public double all;
        }
        private Peak[] getPeaks(float[] data)
        {
            // What we're going to do here, is to divide up our audio into parts.

            // We will then identify, for each part, what the loudest sample is in that
            // part.

            // It's implied that that sample would represent the most likely 'beat'
            // within that part.

            // Each part is 0.5 seconds long

            // This will give us 60 'beats' - we will only take the loudest half of
            // those.

            // This will allow us to ignore breaks, and allow us to address tracks with
            // a BPM below 120.

            int sampleRate = os.WaveFormat.SampleRate;
            int channels = os.WaveFormat.Channels;
            int partSize = sampleRate / 2;
            int parts = data.Length / channels / partSize;
            Peak[] peaks = new Peak[parts];

            for (int i = 0; i < parts; ++i)
            {
                Peak max = new Peak
                {
                    Position = -1,
                    Volume = 0.0F
                };
                for (int j = 0; j < partSize; ++j)
                {
                    float vol = 0.0F;
                    for (int k = 0; k < channels; ++k)
                    {
                        float v = data[i * channels * partSize + j * channels + k];
                        if (vol < v)
                        {
                            vol = v;
                        }
                    }
                    if (max.Position == -1 || max.Volume < vol)
                    {
                        max.Position = i * partSize + j;
                        max.Volume = vol;
                    }
                }
                peaks[i] = max;
            }

            // We then sort the peaks according to volume...

            Array.Sort(peaks, (x, y) => y.Volume.CompareTo(x.Volume));

            // ...take the loundest half of those...

            Array.Resize(ref peaks, peaks.Length / 2);

            // ...and re-sort it back based on position.

            Array.Sort(peaks, (x, y) => x.Position.CompareTo(y.Position));

            return peaks;
        }

        private BPMGroup[] getIntervals(Peak[] peaks)
        {
            // What we now do is get all of our peaks, and then measure the distance to
            // other peaks, to create intervals.  Then based on the distance between
            // those peaks (the distance of the intervals) we can calculate the BPM of
            // that particular interval.

            // The interval that is seen the most should have the BPM that corresponds
            // to the track itself.

            List<BPMGroup> groups = new List<BPMGroup>();

            int sampleRate = os.WaveFormat.SampleRate;
            for (int index = 0; index < peaks.Length; ++index)
            {
                Peak peak = peaks[index];
                for (int i = 1; index + i < peaks.Length && i < 10; ++i)
                {
                    float tempo = 60.0F * sampleRate / (peaks[index + i].Position - peak.Position);
                    while (tempo < 80.0F)
                    {
                        tempo *= 2.0F;
                    }
                    while (tempo > 180.0F)
                    {
                        tempo /= 2.0F;
                    }
                    BPMGroup group = new BPMGroup
                    {
                        Count = 1,
                        Tempo = (short)Math.Round(tempo),
                        all = tempo
                    };
                    int j;
                    for (j = 0; j < groups.Count && groups[j].Tempo != group.Tempo; ++j) { }
                    if (j < groups.Count)
                    {
                        group.Count = groups[j].Count + 1;
                        group.all += groups[j].all;
                        groups[j] = group;
                    }
                    else
                    {
                        groups.Add(group);
                    }
                }
            }
            return groups.ToArray();
        }

        BPMGroup[] group;
    }
}