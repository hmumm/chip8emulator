using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace HJM.Chip8.MonoGameUI.Sound
{
    public class SoundInstance
    {
        private const int SAMPLE_RATE = 44100;
        private const int CHANNELS_COUNT = 2;
        private const int SAMPLES_PER_BUFFER = 3000;
        private const int BYTES_PER_SAMPLE = 2;

        private readonly byte[] xnaBuffer = new byte[CHANNELS_COUNT * SAMPLES_PER_BUFFER * BYTES_PER_SAMPLE];
        private readonly DynamicSoundEffectInstance dynamicSoundEffectInstance = new DynamicSoundEffectInstance(SAMPLE_RATE, (CHANNELS_COUNT == 2) ? AudioChannels.Stereo : AudioChannels.Mono);
        private readonly float[,] workingBuffer = new float[CHANNELS_COUNT, SAMPLES_PER_BUFFER];

        private bool playingSound = false;

        private double time = 0.0;

        public void Update()
        {
            // On Update
            while (dynamicSoundEffectInstance.PendingBufferCount < 3 && playingSound)
            {
                SubmitBuffer();
            }
        }

        public void PlaySound()
        {
            playingSound = true;
            dynamicSoundEffectInstance.Play();
        }

        public void StopPlayingSound()
        {
            playingSound = false;
            dynamicSoundEffectInstance.Stop();
        }

        private static void ConvertBuffer(float[,] from, byte[] to)
        {
            for (int i = 0; i < SAMPLES_PER_BUFFER; i++)
            {
                for (int c = 0; c < CHANNELS_COUNT; c++)
                {
                    // First clamp the value to the [-1.0..1.0] range
                    float floatSample = MathHelper.Clamp(from[c, i], -1.0f, 1.0f);

                    // Convert it to the 16 bit [short.MinValue..short.MaxValue] range
                    short shortSample = (short)(floatSample >= 0.0f ? floatSample * short.MaxValue : floatSample * short.MinValue * -1);

                    // Calculate the right index based on the PCM format of interleaved samples per channel [L-R-L-R]
                    int index = (i * CHANNELS_COUNT * BYTES_PER_SAMPLE) + (c * BYTES_PER_SAMPLE);

                    // Store the 16 bit sample as two consecutive 8 bit values in the buffer with regard to endian-ness
                    if (!BitConverter.IsLittleEndian)
                    {
                        to[index] = (byte)(shortSample >> 8);
                        to[index + 1] = (byte)shortSample;
                    }
                    else
                    {
                        to[index] = (byte)shortSample;
                        to[index + 1] = (byte)(shortSample >> 8);
                    }
                }
            }
        }

        private static double SineWave(double time, double frequency)
        {
            return Math.Sin(time * 2 * Math.PI * frequency);
        }

        private void FillWorkingBuffer()
        {
            for (int i = 0; i < SAMPLES_PER_BUFFER; i++)
            {
                // Here is where you sample your wave function
                workingBuffer[0, i] = (float)SineWave(time, 440); // Left Channel
                workingBuffer[1, i] = (float)SineWave(time, 220); // Right Channel

                // Advance time passed since beginning
                // Since the amount of samples in a second equals the chosen SampleRate
                // Then each sample should advance the time by 1 / SampleRate
                time += 1.0 / SAMPLE_RATE;
            }
        }

        private void SubmitBuffer()
        {
            FillWorkingBuffer();
            ConvertBuffer(workingBuffer, xnaBuffer);
            dynamicSoundEffectInstance.SubmitBuffer(xnaBuffer);
        }
    }
}
