using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Nuget;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using Component = Nuget.Component;

namespace WPF1
{
    public class ViewModel : INotifyPropertyChanged
    {
        private bool operationStarted = false;
        private Component component;
        private double currentProgress;
        private float[,] distances;
        private List<Image<Rgb24>> images;
        private string[] imagesPaths;
        private float[,] similarities;
        private int totalProgress;
        private bool completed;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        public ViewModel()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            images = new List<Image<Rgb24>>();
        }

        public bool Completed
        {
            get { return completed; }
            set
            {
                completed = value;
                OnPropertyChanged();
            }
        }

        public double CurrentProgress
        {
            get => currentProgress;
            set
            {
                currentProgress = value;
                OnPropertyChanged();
            }
        }

        public float[,] Distances
        {
            get => distances;
            set
            {
                distances = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get => path;
            set
            {
                path = value;
                OnPropertyChanged();

                if (path != null)
                {
                    ImagesPaths = Directory.GetFiles(Path, "*.png");
                }
            }
        }

        private string path;

        public string[] ImagesPaths
        {
            get => imagesPaths;
            set
            {
                imagesPaths = value;
                OnPropertyChanged();
            }
        }

        public float[,] Similarities
        {
            get => similarities;
            set
            {
                similarities = value;
                OnPropertyChanged();
            }
        }

        public int TotalProgress
        {
            get => totalProgress;
            set
            {
                totalProgress = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Clear()
        {
            Path = null;
            images.Clear();
            ImagesPaths = null;
            Distances = null;
            Similarities = null;
            CurrentProgress = 0;
            Completed= false;
        }

        private void GetImages()
        {
            images.Clear();
            if (ImagesPaths == null)
            {
                return;
            }

            foreach (string filename in ImagesPaths)
            {
                images.Add(Image.Load<Rgb24>(filename));
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task StartButtonClicked()
        {
            if (operationStarted) return;

            try
            {
                operationStarted = true;
                GetImages();
                cancellationTokenSource.TryReset();
                using (component = new Component())
                {
                    var distances = new float[images.Count, images.Count];
                    var similarities = new float[images.Count, images.Count];
                    var totalProgress = (double)(images.Count * images.Count);

                    for (int i = 0; i < images.Count; i++)
                    {
                        for (int j = 0; j < images.Count; j++)
                        {
                            var emb1 = await component.GetEmbeddings(images[i], cancellationToken);
                            var emb2 = await component.GetEmbeddings(images[j], cancellationToken);

                            var distance = Component.GetDistance(emb1, emb2);
                            var similarity = Component.GetSimilarity(emb1, emb2);

                            distances[i, j] = distance;
                            similarities[i, j] = similarity;

                            CurrentProgress = ((currentProgress + 1) / totalProgress) * 100;
                        }
                    }

                    Distances = distances;
                    Similarities = similarities;
                    operationStarted = false;
                    Completed = true;
                }
            }
            catch (OperationCanceledException)
            {
                Completed = false;
                Trace.WriteLine("Operation was cancelled");
            }
        }

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
